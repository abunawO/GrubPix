# Use official .NET runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

# Use SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GrubPix.API/GrubPix.API.csproj", "GrubPix.API/"]
COPY ["GrubPix.Application/GrubPix.Application.csproj", "GrubPix.Application/"]
COPY ["GrubPix.Domain/GrubPix.Domain.csproj", "GrubPix.Domain/"]
COPY ["GrubPix.Infrastructure/GrubPix.Infrastructure.csproj", "GrubPix.Infrastructure/"]

RUN dotnet restore "GrubPix.API/GrubPix.API.csproj"

# Copy source files and build the application
COPY . .
WORKDIR "/src/GrubPix.API"
RUN dotnet build "GrubPix.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "GrubPix.API.csproj" -c Release -o /app/publish

# Final stage: Run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrubPix.API.dll"]
