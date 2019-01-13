FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY src/ShipRegistryAPI/*.csproj ./src/ShippingRegistryAPI/
COPY src/ShipRegistryApplication/*.csproj ./src/ShipRegistryApplication/
COPY src/ShipRegistryPorts/*.csproj ./src/ShipRegistryPorts/
RUN dotnet restore

# copy everything else and build app
COPY src/. ./src/
WORKDIR /app/src
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/src/out ./
ENTRYPOINT ["dotnet", "ShipRegistryAPI.dll"]