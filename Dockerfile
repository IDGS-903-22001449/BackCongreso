# Etapa 1: Construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el código fuente y compilar
COPY . .
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Exponer el puerto que usará la aplicación
EXPOSE 8080

# Configurar la URL del servidor ASP.NET
ENV ASPNETCORE_URLS=http://+:8080

# Iniciar la aplicación
ENTRYPOINT ["dotnet", "app-congreso.dll"]
