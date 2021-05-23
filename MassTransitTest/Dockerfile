#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MassTransitTest/MassTransitTest.csproj", "MassTransitTest/"]
RUN dotnet restore "MassTransitTest/MassTransitTest.csproj"
COPY . .
WORKDIR "/src/MassTransitTest"
RUN dotnet build "MassTransitTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MassTransitTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MassTransitTest.dll"]