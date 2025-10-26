# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY AssignmentPortal.sln ./
COPY AssignmentPortal/ ./AssignmentPortal/

# Restore dependencies and publish
RUN dotnet restore AssignmentPortal/AssignmentPortal.csproj
RUN dotnet publish AssignmentPortal/AssignmentPortal.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AssignmentPortal.dll"]
