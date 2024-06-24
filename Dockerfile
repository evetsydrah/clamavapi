# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ClamAVApi.csproj", "."]
RUN dotnet restore "./ClamAVApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "ClamAVApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClamAVApi.csproj" -c Release -o /app/publish

# Final stage
FROM clamav/clamav:latest AS final
WORKDIR /app

# Install .NET SDK dependencies
RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl3 libstdc++ zlib curl \
    && wget https://dotnetcli.azureedge.net/dotnet/Sdk/8.0.100/dotnet-sdk-8.0.100-linux-musl-x64.tar.gz \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet-sdk-8.0.100-linux-musl-x64.tar.gz -C /usr/share/dotnet \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
    && rm dotnet-sdk-8.0.100-linux-musl-x64.tar.gz

COPY --from=publish /app/publish .

# Ensure the correct permissions are set for the configuration files
RUN chmod 644 /etc/clamav/clamd.conf /etc/clamav/freshclam.conf

# Create necessary directories for ClamAV
RUN mkdir -p /var/lib/clamav /var/run/clamav /var/log/clamav && \
    chown -R clamav:clamav /var/lib/clamav /var/run/clamav /var/log/clamav

# Update database before starting the service
RUN freshclam

# Override the entrypoint to start ClamAV and then the .NET application
CMD ["sh", "-c", "freshclam && clamd & dotnet /app/ClamAVApi.dll"]
