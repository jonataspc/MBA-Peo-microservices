# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used to build the Blazor WebAssembly project
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore dependencies
COPY ["src/Peo.Web.Spa/Peo.Web.Spa.csproj", "src/Peo.Web.Spa/"]
RUN dotnet restore "./src/Peo.Web.Spa/Peo.Web.Spa.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/src/Peo.Web.Spa"
RUN dotnet build "./Peo.Web.Spa.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the Blazor WebAssembly project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Peo.Web.Spa.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - use nginx to serve the static files
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Copy published Blazor WebAssembly files
COPY --from=publish /app/publish/wwwroot .

# Override appsettings.json for production
COPY src/Peo.Web.Spa/wwwroot/appsettings.Production.json ./appsettings.json

# Copy custom nginx configuration if needed
COPY src/Peo.Web.Spa/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]