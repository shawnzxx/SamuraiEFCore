#https://docs.docker.com/engine/examples/dotnetcore/
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app
# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore
# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
ARG URL_PORT
ENV ASPNETCORE_ENVIRONMENT Development
ENV ASPNETCORE_URLS http://*:${URL_PORT}
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WebApi.dll"]