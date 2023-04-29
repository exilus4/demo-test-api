#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app

COPY ./bin/Release/net7.0/ /app

EXPOSE 80

ENTRYPOINT ["dotnet", "demo-test-api.dll"]