# cd MyEF2.WebApp
# need to run manually:
# dotnet publish -c Release

# docker build -t ghcr.io/russtydog/myef2/myef2:latest .

# Auth to packages
# docker login ghcr.io -u russtydog -p TOKEN

# push the image
# docker push ghcr.io/russtydog/myef2/myef2:latest

# Find the package:
# https://github.com/users/russtydog/packages/container/package/myef2

# Use the official ASP.NET Core v8 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory inside the container
WORKDIR /app

# Copy the published output of your ASP.NET Core WebApp to the container
COPY ./bin/Release/net8.0/publish .

# Expose the port that your ASP.NET Core WebApp listens on
EXPOSE 80
EXPOSE 443

# Set the entry point for the container
ENTRYPOINT ["dotnet", "MyEF2.WebApp.dll"]