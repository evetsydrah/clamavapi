FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ClamAVApi.csproj", "."]
RUN dotnet restore "./ClamAVApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "ClamAVApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClamAVApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update && apt-get install -y \
    clamav clamav-daemon clamav-freshclam \
    cmake make gcc g++ pkg-config \
    libssl-dev libpcre3-dev zlib1g-dev \
    libbz2-dev libcurl4-openssl-dev \
    libxml2-dev libjson-c-dev libyara-dev \
    python3 python3-dev python3-pip \
    valgrind git curl \
    check libpcre2-dev libncurses5-dev \
    libmilter-dev

# Install Rust
RUN curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh -s -- -y
ENV PATH="/root/.cargo/bin:${PATH}"

# Download, build, and install ClamAV from source
WORKDIR /clamav
RUN curl -L -o clamav.tar.gz https://www.clamav.net/downloads/production/clamav-1.3.1.tar.gz && \
    tar xzf clamav.tar.gz && \
    cd clamav-1.3.1 && \
    mkdir build && cd build && \
    cmake .. && make && make install

# Configure ClamAV
RUN cp /usr/local/etc/clamd.conf.sample /usr/local/etc/clamd.conf && \
    sed -i 's/^#LogFile \/tmp\/clamd.log/LogFile \/var\/log\/clamav\/clamd.log/' /usr/local/etc/clamd.conf && \
    sed -i 's/^#PidFile \/run\/clamav\/clamd.pid/PidFile \/var\/run\/clamav\/clamd.pid/' /usr/local/etc/clamd.conf && \
    sed -i 's/^#LocalSocket \/run\/clamav\/clamd.sock/LocalSocket \/var\/run\/clamav\/clamd.sock/' /usr/local/etc/clamd.conf && \
    sed -i 's/^#DatabaseDirectory \/var\/lib\/clamav/DatabaseDirectory \/var\/lib\/clamav/' /usr/local/etc/clamd.conf && \
    sed -i 's/^#TCPSocket 3310/TCPSocket 3310/' /usr/local/etc/clamd.conf && \
    sed -i 's/^#TCPAddr localhost/TCPAddr 127.0.0.1/' /usr/local/etc/clamd.conf && \
    sed -i '/^Example/d' /usr/local/etc/clamd.conf

RUN cp /usr/local/etc/freshclam.conf.sample /usr/local/etc/freshclam.conf && \
    sed -i 's/^#DatabaseDirectory \/var\/lib\/clamav/DatabaseDirectory \/var\/lib\/clamav/' /usr/local/etc/freshclam.conf && \
    sed -i 's/^#UpdateLogFile \/var\/log\/clamav\/freshclam.log/UpdateLogFile \/var\/log\/clamav\/freshclam.log/' /usr/local/etc/freshclam.conf && \
    sed -i 's/^#LogFileMaxSize 2M/LogFileMaxSize 2M/' /usr/local/etc/freshclam.conf && \
    sed -i 's/^#LogTime yes/LogTime yes/' /usr/local/etc/freshclam.conf && \
    sed -i 's/^#LogRotate yes/LogRotate yes/' /usr/local/etc/freshclam.conf && \
    sed -i 's/^#Foreground yes/Foreground yes/' /usr/local/etc/freshclam.conf && \
    sed -i 's/^#Debug yes/Debug yes/' /usr/local/etc/freshclam.conf && \
    sed -i '/^Example/d' /usr/local/etc/freshclam.conf


RUN mkdir -p /usr/local/share/clamav && \
    mkdir -p /var/lib/clamav /var/run/clamav /var/log/clamav && \
    chown -R clamav:clamav /usr/local/share/clamav /var/lib/clamav /var/run/clamav /var/log/clamav

# Update database before starting the service
RUN freshclam

RUN mkdir -p /var/lib/clamav /var/run/clamav /var/log/clamav && \
    chown -R clamav:clamav /var/lib/clamav /var/run/clamav /var/log/clamav

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
