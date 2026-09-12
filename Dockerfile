# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["AIResumeMatcher.csproj", "./"]
RUN dotnet restore "AIResumeMatcher.csproj"

COPY . .
RUN dotnet publish "AIResumeMatcher.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 2: Final Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Render routes to port 10000 by default
EXPOSE 10000

ENTRYPOINT ["dotnet", "AIResumeMatcher.dll"]