# Stage 1: Base runtime environment (lightweight, runs the app)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
# .NET 8+ defaults to port 8080 inside the container
EXPOSE 10000

# Stage 2: Build environment (heavy SDK, used to compile the code)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Restore NuGet packages first (helps cache this layer to speed up future builds)
COPY ["AIResumeMatcher.csproj", "./"]
RUN dotnet restore "AIResumeMatcher.csproj"

# Copy the rest of the source code and build
COPY . .
RUN dotnet build "AIResumeMatcher.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish the app (optimizes the output)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AIResumeMatcher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final Image (combines base runtime + published files)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIResumeMatcher.dll"]