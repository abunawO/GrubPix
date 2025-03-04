# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first to take advantage of Docker layer caching
COPY ["GrubPix.API/GrubPix.API.csproj", "GrubPix.API/"]
COPY ["GrubPix.Application/GrubPix.Application.csproj", "GrubPix.Application/"]
COPY ["GrubPix.Domain/GrubPix.Domain.csproj", "GrubPix.Domain/"]
COPY ["GrubPix.Infrastructure/GrubPix.Infrastructure.csproj", "GrubPix.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "GrubPix.API/GrubPix.API.csproj" --force

# Copy remaining source files
COPY . .

# Ensure NuGet packages are fully restored
RUN dotnet restore "GrubPix.API/GrubPix.API.csproj"

# Build and publish the application
WORKDIR "/src/GrubPix.API"
RUN dotnet publish "GrubPix.API.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose ports
EXPOSE 8080
EXPOSE 443

# Start the API
ENTRYPOINT ["dotnet", "GrubPix.API.dll"]
