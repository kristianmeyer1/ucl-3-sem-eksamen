# Use .NET 8 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the code
COPY . ./

# Build and publish the project
RUN dotnet publish -c Release -o out

# Use .NET 8 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose the port your service uses
EXPOSE 5002

# Start the app
ENTRYPOINT ["dotnet", "NotificationService.dll"]
