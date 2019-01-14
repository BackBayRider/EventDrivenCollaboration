FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ./src/ShipRegistryAPI/ShipRegistryAPI.csproj ./src/ShipRegistryAPI/
COPY ./src/ShipRegistryApplication/ShipRegistryApplication.csproj ./src/ShipRegistryApplication/
COPY ./src/ShipRegistryPorts/ShipRegistryPorts.csproj ./src/ShipRegistryPorts/
COPY ./tests/ShipRegistryTests/ShipRegistryTests.csproj ./tests/ShipRegistryTests/
RUN dotnet restore

# copy everything else and build app
COPY src/. ./src/
COPY tests/. ./tests
WORKDIR /app
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/src/ShipRegistryAPI/out ./
ENTRYPOINT ["dotnet", "ShipRegistryAPI.dll"]