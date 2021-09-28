#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Test.ApiGateway.Web/Test.ApiGateway.Web.csproj", "Test.ApiGateway.Web/"]
RUN dotnet restore "Test.ApiGateway.Web/Test.ApiGateway.Web.csproj"
COPY . .
WORKDIR "/src/Test.ApiGateway.Web"
RUN dotnet build "Test.ApiGateway.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Test.ApiGateway.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Test.ApiGateway.Web.dll"]