#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DependencyCrawler.BlazorClient/DependencyCrawler.BlazorClient.csproj", "DependencyCrawler.BlazorClient/"]
RUN dotnet restore "DependencyCrawler.BlazorClient/DependencyCrawler.BlazorClient.csproj"
COPY . .
WORKDIR "/src/DependencyCrawler.BlazorClient"
RUN dotnet build "DependencyCrawler.BlazorClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DependencyCrawler.BlazorClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DependencyCrawler.BlazorClient.dll"]