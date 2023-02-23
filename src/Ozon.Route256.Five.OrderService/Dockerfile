#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80;http://+:50001;http://+:50002
EXPOSE 80
EXPOSE 50001
EXPOSE 50002

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Ozon.Route256.Five.OrderService/Ozon.Route256.Five.OrderService.csproj", "src/Ozon.Route256.Five.OrderService/"]
RUN dotnet restore "src/Ozon.Route256.Five.OrderService/Ozon.Route256.Five.OrderService.csproj"
COPY . .
WORKDIR "/src/src/Ozon.Route256.Five.OrderService"
RUN dotnet build "Ozon.Route256.Five.OrderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ozon.Route256.Five.OrderService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ozon.Route256.Five.OrderService.dll"]