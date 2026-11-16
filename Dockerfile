# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/PonecasPedidosApi/PonecasPedidosApi.csproj ./PonecasPedidosApi/
RUN dotnet restore ./PonecasPedidosApi/PonecasPedidosApi.csproj
COPY src/PonecasPedidosApi/ ./PonecasPedidosApi/
RUN dotnet publish ./PonecasPedidosApi/PonecasPedidosApi.csproj \
    -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PonecasPedidosApi.dll"]

