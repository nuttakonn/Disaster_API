# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /app

# Copy csproj and restore
COPY ["Disaster_API.csproj", "./"]
RUN dotnet restore "Disaster_API.csproj"

# Copy all file then build
COPY . .
RUN dotnet build "Disaster_API.csproj" -c $configuration -o /app/build

# Stage 2: Publish
FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Disaster_API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8000
ENV ASPNETCORE_URLS=http://+:8000

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Disaster_API.dll"]
