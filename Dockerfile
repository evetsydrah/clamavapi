# Use the official ASP.NET Core runtime as a base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the SDK for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ClamAVApi.csproj", "./"]
RUN dotnet restore "ClamAVApi.csproj"
COPY . .
RUN dotnet build "ClamAVApi.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ClamAVApi.csproj" -c Release -o /app/publish

# Final image setup
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN apt-get update && apt-get install -y clamav clamav-daemon
RUN freshclam
CMD ["dotnet", "ClamAVApi.dll"]
