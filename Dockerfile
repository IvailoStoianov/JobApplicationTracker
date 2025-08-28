# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files first for better layer caching
COPY JobApplicationTracker.API/JobApplicationTracker.API.csproj JobApplicationTracker.API/
COPY JobApplicationTracker.API.Infrastructure/JobApplicationTracker.API.Infrastructure.csproj JobApplicationTracker.API.Infrastructure/
COPY JobApplicationTracker.API.Models/JobApplicationTracker.API.Models.csproj JobApplicationTracker.API.Models/
COPY JobApplicationTracker.Common/JobApplicationTracker.Common.csproj JobApplicationTracker.Common/
COPY JobApplicationTracker.Data/JobApplicationTracker.Data.csproj JobApplicationTracker.Data/
COPY JobApplicationTracker.Data.Models/JobApplicationTracker.Data.Models.csproj JobApplicationTracker.Data.Models/
COPY JobApplicationTracker.Services/JobApplicationTracker.Services.csproj JobApplicationTracker.Services/
COPY JobApplicationTracker.Servies.Mapping/JobApplicationTracker.Servies.Mapping.csproj JobApplicationTracker.Servies.Mapping/

# Restore dependencies for the API (brings in transitive project refs)
RUN dotnet restore JobApplicationTracker.API/JobApplicationTracker.API.csproj

# Copy the rest of the source code
COPY . .

# Publish the API
RUN dotnet publish JobApplicationTracker.API/JobApplicationTracker.API.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out ./

# Bind to the PORT Render provides
ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["dotnet", "JobApplicationTracker.API.dll"]


