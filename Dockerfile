# Builds the MusicRUs API as a container image.
# Used by the demo (demo/docker-compose.yml) and can also be used when hosting.

# Build stage: restore and publish with the full .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first, so the NuGet packages are cached until the project file changes
COPY MusicRUs.API/MusicRUs.API.csproj MusicRUs.API/
RUN dotnet restore MusicRUs.API/MusicRUs.API.csproj

COPY MusicRUs.API/ MusicRUs.API/
RUN dotnet publish MusicRUs.API/MusicRUs.API.csproj -c Release -o /app/publish --no-restore

# Runtime stage: only the ASP.NET Core runtime and the published files
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# The API listens on port 8080 inside the container (default for .NET images)
EXPOSE 8080

# Run as the non-root user that the .NET images provide
USER $APP_UID
ENTRYPOINT ["dotnet", "MusicRUs.API.dll"]
