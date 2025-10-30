# Stage 1: Build the application
# Use the official .NET 8 SDK image which contains the tools to build your app.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies. This is done in a separate step
# to take advantage of Docker's layer caching. If the project file hasn't changed,
# Docker will reuse the cached layer from a previous build, speeding things up.
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application's source code.
COPY . ./

# Publish the application, creating a release build.
RUN dotnet publish -c Release -o out

# Stage 2: Create the final runtime image
# Use the smaller ASP.NET runtime image which is optimized for running the app.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Set the environment variable for the URLs the app will listen on.
ENV ASPNETCORE_URLS=http://+:8000
# Expose the port the app will run on.
EXPOSE 8000

# Define the entry point for the container. This command starts your application.
ENTRYPOINT ["dotnet", "ExpenseTracker.dll"]