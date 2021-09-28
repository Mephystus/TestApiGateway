#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Test.ApiGateway.UI/Test.ApiGateway.UI.csproj", "Test.ApiGateway.UI/"]
RUN dotnet restore "Test.ApiGateway.UI/Test.ApiGateway.UI.csproj"
COPY . .
WORKDIR "/src/Test.ApiGateway.UI"
RUN dotnet build "Test.ApiGateway.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Test.ApiGateway.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Test.ApiGateway.UI.dll"]