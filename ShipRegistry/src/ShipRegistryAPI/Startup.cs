using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Paramore.Brighter;
using Paramore.Brighter.MessageStore.MySql;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.MessagingGateway.RMQ.MessagingGatewayConfiguration;
using Paramore.Darker;
using Paramore.Darker.Builder;
using Paramore.Darker.SimpleInjector;
using Paramore.Darker.Policies;
using Paramore.Darker.Logging;
using Paramore.Darker.QueryLogging;
using Polly;
using ShipRegistryAPI.Factories;
using ShipRegistryPorts.BrighterFactories;
using ShipRegistryPorts.Db;
using ShipRegistryPorts.Handlers;
using ShipRegistryPorts.Mappers;
using ShipRegistryPorts.Repositories;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace ShipRegistryAPI
{
    public class Startup
    {
        private Container _container = new Container();
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _container.Options.ConstructorResolutionBehavior = new MostResolvableParametersConstructorResolutionBehavior(_container);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(_container));

            services.EnableSimpleInjectorCrossWiring(_container);
            services.UseSimpleInjectorAspNetRequestScoping(_container);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeContainer(app);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            
            app.UseHttpsRedirection();
            app.UseMvc();
            
            _container.Verify();
            
            CheckDbIsUp(Configuration["Database:ShipRegistryDb"]);

            EnsureDatabaseCreated();

            CreateMessageTable(Configuration["Database:MessageStore"], Configuration["Database:MessageTableName"]);
 
       }

        private void InitializeContainer(IApplicationBuilder app)
        {
            _container.Register<DbContextOptions<ShipRegistryDbContext>>( 
                () => new DbContextOptionsBuilder<ShipRegistryDbContext>()
                    .UseMySql(Configuration["Database:ShipRegistry"])
                    .Options, 
                Lifestyle.Singleton);
            
            _container.Register<IShipRegistryContextFactory>(
                () => new ShipRegistryContextFactory(
                    new DbContextOptionsBuilder<ShipRegistryDbContext>()
                        .UseMySql(Configuration["Database:ShipRegistry"])
                        .Options
                    ),
                Lifestyle.Singleton
                );
            
            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            RegisterQueryProcessor();
            RegisterCommandProcessor();
            
            
            _container.AutoCrossWireAspNetComponents(app);
        }
        
        private void RegisterQueryProcessor()
        {
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromMilliseconds(500));
            var policyRegistry = new Paramore.Darker.Policies.PolicyRegistry
            {
                {Paramore.Darker.Policies.Constants.RetryPolicyName, retryPolicy}, 
                {Paramore.Darker.Policies.Constants.CircuitBreakerPolicyName, circuitBreakerPolicy}
            };

            Func<Type, object> simpleFactory = type =>  _container.GetInstance(type);

            IQueryProcessor queryProcessor = QueryProcessorBuilder.With()
                .SimpleInjectorHandlers(_container, opts =>
                    opts.WithQueriesAndHandlersFromAssembly(typeof(NewShipRegistrationHandlerAsync).Assembly))
                .InMemoryQueryContextFactory()
                .Policies(policyRegistry)
                .JsonQueryLogging()
                .Build();
            
            _container.RegisterInstance<IQueryProcessor>(queryProcessor);

        }


        private void RegisterCommandProcessor()
        {
            //create handler 
            var subscriberRegistry = new SubscriberRegistry();
            RegisterBrighterHandlersFromAssembly(
                typeof(IHandleRequestsAsync<>), 
                new Assembly[] {typeof(NewShipRegistrationHandlerAsync).Assembly},
                typeof(IHandleRequestsAsync<>).GetTypeInfo().Assembly,
                subscriberRegistry);

            //create policies
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreaker(1, TimeSpan.FromMilliseconds(500));
            var retryPolicyAsync = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicyAsync = Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromMilliseconds(500));
            var policyRegistry = new Paramore.Brighter.PolicyRegistry()
            {
                { CommandProcessor.RETRYPOLICY, retryPolicy },
                { CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy },
                { CommandProcessor.RETRYPOLICYASYNC, retryPolicyAsync },
                { CommandProcessor.CIRCUITBREAKERASYNC, circuitBreakerPolicyAsync }
            };

            var servicesHandlerFactory = new ServicesHandlerFactoryAsync(_container);

            var messagingGatewayConfiguration = RmqGatewayBuilder.With.Uri(new Uri(Configuration["Broker:Uri"]))
                .Exchange(Configuration["Broker:Exchange"])
                .DefaultQueues();

            var gateway = new RmqMessageProducer(messagingGatewayConfiguration);
            var sqlMessageStore = new MySqlMessageStore(new MySqlMessageStoreConfiguration(Configuration["Database:MessageStore"], Configuration["Database:MessageTableName"]));

            var messageMapperFactory = new MessageMapperFactory(_container);

            var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory);
            
            RegisterMessageMappersFromAssembly(
                new Assembly[] {typeof(LineNameUpdatedEventMessageMapper).Assembly}, 
                typeof(IAmAMessageMapper<>).GetTypeInfo().Assembly, 
                messageMapperRegistry);

            var messagingConfiguration = new MessagingConfiguration(
                messageStore: sqlMessageStore,
                messageProducer: gateway,
                messageMapperRegistry: messageMapperRegistry);

            var commandProcessor = CommandProcessorBuilder.With()
                .Handlers(new Paramore.Brighter.HandlerConfiguration(subscriberRegistry, servicesHandlerFactory))
                .Policies(policyRegistry)
                .TaskQueues(messagingConfiguration)
                .RequestContextFactory(new Paramore.Brighter.InMemoryRequestContextFactory())
                .Build();

            _container.RegisterInstance<IAmACommandProcessor>(commandProcessor);
        }
        
        private void RegisterBrighterHandlersFromAssembly(Type interfaceType, IEnumerable<Assembly> assemblies, Assembly assembly, SubscriberRegistry subscriberRegistry)
        {
            assemblies = assemblies.Concat(new [] {assembly});
            var subscribers =
                from ti in assemblies.SelectMany(a => a.DefinedTypes).Distinct()
                where ti.IsClass && !ti.IsAbstract && !ti.IsInterface
                from i in ti.ImplementedInterfaces
                where i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == interfaceType
                select new
                {
                    RequestType = i.GenericTypeArguments.First(),
                    HandlerType = ti.AsType()
                };

            foreach (var subscriber in subscribers)
            {
                _container.Register(subscriber.HandlerType, subscriber.HandlerType, Lifestyle.Scoped);
                subscriberRegistry.Add(subscriber.RequestType, subscriber.HandlerType);
            }
        }

        private void RegisterMessageMappersFromAssembly(
            IEnumerable<Assembly> assemblies, 
            Assembly assembly,
            MessageMapperRegistry mapperRegistry)
        {
                        assemblies = assemblies.Concat(new [] {assembly});
            var mappers =
                from ti in assemblies.SelectMany(a => a.DefinedTypes).Distinct()
                where ti.IsClass && !ti.IsAbstract && !ti.IsInterface
                from i in ti.ImplementedInterfaces
                where i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IAmAMessageMapper<>)
                select new
                {
                    RequestType = i.GenericTypeArguments.First(),
                    MapperType = ti.AsType()
                };

            foreach (var mapper in mappers)
            {
               _container.Register(mapper.MapperType, mapper.MapperType, Lifestyle.Scoped); 
               mapperRegistry.Add(mapper.RequestType, mapper.MapperType); 
            }

        }

        private static void CheckDbIsUp(string connectionString)
        {
            var policy = Policy.Handle<MySqlException>().WaitAndRetryForever(
                retryAttempt => TimeSpan.FromSeconds(2),
                (exception, timespan) =>
                {
                    Console.WriteLine($"Healthcheck: Waiting for the database {connectionString} to come online - {exception.Message}");
                });

            policy.Execute(() =>
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                }
            });
        }
        private void EnsureDatabaseCreated()
        {
            var contextOptions = _container.GetInstance<DbContextOptions<ShipRegistryDbContext>>();
            using (var context = new ShipRegistryDbContext(contextOptions))
            {
                context.Database.EnsureCreated();
            }
        }


        private static void CreateMessageTable(string dataSourceTestDb, string tableNameMessages)
        {
            try
            {
                using (var sqlConnection = new MySqlConnection(dataSourceTestDb))
                {
                    sqlConnection.Open();
                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = MySqlMessageStoreBuilder.GetDDL(tableNameMessages);
                        command.ExecuteScalar();
                    }
                }
                
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Issue with creating MessageStore table, {e.Message}");
            }
        }
    }
}