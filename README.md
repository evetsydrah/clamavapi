# ClamAV .NET API with clamd Integration

## Overview

This project provides an API for running ClamAV virus scans on a specified directory or file. The API is built using ASP.NET Core and runs inside a Docker container. The ClamAV command-line tool `clamdscan` is used to perform the virus scans, leveraging the ClamD daemon for efficient and faster scanning.

## Prerequisites

- Docker installed on your machine
- .NET SDK installed on your machine

## Setup

### Clone the Project

```sh
git clone git@github.com:kevinjcwu/clamavapi.git
```

## Project Structure

- **Program.cs**: Defines the API endpoints and logic for running ClamAV scans.
- **Dockerfile**: Builds the Docker image for the API, including the installation of ClamAV and clamd.
- **ClamAVBenchmark.cs**: Contains benchmark tests comparing directory scanning versus individual file scanning.
- **entrypoint.sh**: Entry point script for the Docker container to start the ClamAV daemon and run benchmarks.
- **clamd.conf**: Configuration file for ClamAV daemon.
- **freshclam.conf**: Configuration file for FreshClam, the automatic database update tool for ClamAV.

## Prerequisites

- Docker installed on your machine
- .NET SDK installed on your machine

## Building and Running the Docker Container

To build and run the Docker container, follow these steps:

1. **Build the Docker image:**

   ```sh
    docker build -t clamav-api .
    ```
2. **Build the Docker container**
    ```sh
    docker run -d -p 8080:5000 -v /path/to/scan/on/host:/data --name clamav-container clamav-api
    ```
    Replace **/path/to/scan/on/host** with the actual directory on your host machine that you want to scan.

### Note: 
When you run the docker container, you will need to **wait a minute** for the initialisation of the daemon before running the `curl` commands to test the endpoint.

To verify if your /path/to/scan/on/host is mounted correctly in the container:

```sh
docker exec -it clamav-container sh
```

## API Endpoints
### Scan Directory or File
- URL: /scan
- Method: GET
#### Query Parameter
filePath: The path to the file or directory to be scanned within the `/data` directory. Here the `/data` directory is the directory inside the container with the mounted files. 

### Example Request
**To test the endpoint**, you can use `curl`:

#### For Directory:
```sh
curl "http://localhost:8080/scan?filePath=/data"
```
#### For Single File:
```sh
curl "http://localhost:8080/scan?filePath=/data/file1.txt"
```

## Expected Response
The original output of the `clamdscan <file/directory>` is:

```
/data/eicar.com: Eicar-Test-Signature FOUND

----------- SCAN SUMMARY -----------
Infected files: 1
Time: 0.001 sec (0 m 0 s)
Start Date: 2024:06:22 21:05:55
End Date:   2024:06:22 21:05:55
```
```json
{"/data/eicar.com: Eicar-Signature FOUND\n\n----------- SCAN SUMMARY -----------\nInfected files: 1\nTime: 0.001 sec (0 m 0 s)\nStart Date: 2024:06:22 21:05:55\nEnd Date:   2024:06:22 21:05:55\n}
```

The `clamdscan` command originally outputs only a plain text output whch shows the files detected with a  virus in the format `"filepath": "<virus> FOUND"`. 
If no virus is detected while scanning the entire directory, it will output  `"filepath": "OK"`.

The output JSON object is reformated with custom fields such as `Engine Version`, `Database Version`, `Infected Files`, `Total Files Scanned` to provide more information about the av scan.

The response includes both the plain text output of the ClamAV scan and a structured JSON object.

### **Response Format**:
- **Fields**:
    - **EngineVersion**: The version of the ClamAV engine.
    - **DatabaseVersion**: The version of the ClamAV database.
    - **ScanResult**: A dictionary containing the scan results for each file.
    - **InfectedFiles**: The number of infected files found.
    - **Time**: The time taken for the scan.
    - **StartDate**: The start date and time of the scan.
    - **EndDate**: The end date and time of the scan.


####  **Response Examples**:
- Virus Detected:
```sh
curl "http://localhost:8080/scan?filePath=/data"
```
```
{
    "engineVersion":"ClamAV 1.3.1/27315/Sun Jun 23 08:23:58 2024",
    "databaseVersion":"62",
    "scanResult":{
        "/data/files/eicar.com":"Eicar-Signature FOUND",
        "/data/eicar.com":"Eicar-Signature FOUND",
        "/data/files2/eicar.com":"Eicar-Signature FOUND"
    },
    "totalScannedFiles":42,
    "infectedFiles":3,
    "time":"1.188 sec (0 m 1 s)",
    "startDate":"2024:06:24 01:49:49",
    "endDate":"2024:06:24 01:49:50"
}
```

- No Virus Detected:


```sh
curl "http://localhost:8080/scan?filePath=/data/files/file1.txt"
```

```
{
    "engineVersion":"ClamAV 1.3.1/27315/Sun Jun 23 08:23:58 2024",
    "databaseVersion":"62",
    "scanResult":{
        "/data/files/file1.txt":"OK"
    },
    "totalScannedFiles":1,
    "infectedFiles":0,
    "time":"0.003 sec (0 m 0 s)",
    "startDate":"2024:06:24 01:53:53",
    "endDate":"2024:06:24 01:53:53"
}
```

## Development and Testing
### Building the Project

To build the project locally using the .NET SDK:

```sh
dotnet build
```

### Running the Application
If you prefer to run the application **without Docker**:

1. Ensure ClamAV is installed and running.

For Ubuntu/Debian:
```sh
sudo apt-get update
sudo apt-get install clamav clamav-daemon
sudo freshclam
```

Start the ClamAV Daemon
```sh
sudo systemctl start clamav-daemon
```

2. Build the project.

```sh
dotnet build
```
3. Run the application.

```sh
dotnet run
```


# Benchmark
## Running Benchmarks
To run benchmarks that compare scanning an entire directory versus scanning each file individually, follow these steps:

Ensure to set the `var DirectoryPath` in `ClamAVBenchmar.cs` before running this step. 

```
dotnet build -c Release
docker exec clamav-container /entrypoint.sh --benchmark
```

```scss
mountFiles/
├── cleanFile.txt (clean file)
├── eicar.com (virus file)
├── Car Photos (contains 6 files - no virus)
├── files/ (contains 7 files - 1 virus)
└── files2/ (contains 7 files - 1 virus)

```
```sh
dotnet build -c Release
dotnet exec ./bin/Release/net8.0/ClamAVApi.dll --benchmark
```

The benchmark compares scanning an entire directory (**ScanEntireDirectory**) versus scanning each file individually (**ScanEachFileIndividually**)

| Method                   | Mean      | Error     | StdDev    |
|------------------------- |----------:|----------:|----------:|
| ScanEntireDirectory      |  38.31 ms |  0.752 ms |  1.357 ms |
| ScanEachFileIndividually | 335.77 ms | 14.282 ms | 41.661 ms |
