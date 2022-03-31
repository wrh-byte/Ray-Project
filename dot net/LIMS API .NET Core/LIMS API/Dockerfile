# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY LIMS-API/*.csproj ./LIMS-API/
RUN dotnet restore

# copy everything else and build app
COPY LIMS-API/. ./LIMS-API/
WORKDIR /source/LIMS-API/
RUN dotnet publish -c Release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS prod
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "limsapi.dll"]

# the dotnet in the container will use port 80, expose it
EXPOSE 80