# ClamAV API

This project provides an API for running ClamAV virus scans on a specified directory. The API is built using ASP.NET Core and runs inside a Docker container. The ClamAV command-line tool is used to perform the virus scans.

## Project Structure

- **Program.cs**: Defines the API endpoints and logic for running ClamAV scans.
- **Dockerfile**: Builds the Docker image for the API, including the installation of ClamAV.

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
    docker run -d -p 8080:8080 -v /path/to/scan/on/host:/data --name clamav-api clamav-api
    ```
    Replace /path/to/scan/on/host with the actual directory on your host machine that you want to scan.


## API Endpoints
### Scan Directory
- URL: /scan

- Method: POST

- Content-Type: application/json

- Request Body for Directory:
    ```json
    {
        "Path": "/data",
        "IsDirectory": true
    }
    ```
- Request Body for file
    ```json
    {
        "Path": "/data/file1.txt",
        "IsDirectory": false
    }
    ```
    The Path field specifies the path to the directory or file to be scanned. This path should be within the /data directory, which is mapped from the host machine. The IsDirectory field specifies whether the path is a directory or a single file.


### Example Request
To test the endpoint, you can use `curl`:

#### For Directory:
```
curl -X POST "http://localhost:8080/scan" \
     -H "Content-Type: application/json" \
     -d "{\"Path\":\"/data\",  \"IsDirectory\":true}"
```
#### For Single File:
```
curl -X POST "http://localhost:8080/scan" \
     -H "Content-Type: application/json" \
     -d "{\"Path\":\"/data/file1.txt\", \"IsDirectory\":false}"
```

## Expected Response
The response includes both the plain text output of the ClamAV scan and a structured JSON object.

- **Response Format**:

```
{
    "output": "/data/6.1-MB-scaled-1.jpg:Zone.Identifier: OK\n/data/file_example_JPG_10.1MB-scaled-1.jpg: OK\n/data/file_example_JPG_10.1MB-scaled-1.jpg:Zone.Identifier: OK\n/data/SampleDOCFile_5MB.doc: OK\n/data/6.1-MB-scaled-1.jpg: OK\n/data/SampleDOCFile_5MB.doc:Zone.Identifier: OK\n/data/file2.txt: OK\n/data/file1.txt: OK\n\n----------- SCAN SUMMARY -----------\nKnown viruses: 8694977\nEngine version: 1.0.3\nScanned directories: 1\nScanned files: 8\nInfected files: 0\nData scanned: 17.36 MB\nData read: 6.67 MB (ratio 2.60:1)\nTime: 12.885 sec (0 m 12 s)\nStart Date: 2024:06:19 14:03:39\nEnd Date:   2024:06:19 14:03:51\n",
    "errors": "",
    "exitCode": 0
}
```
- **Fields**:
    - **Output:** The raw output from the ClamAV scan, including any updates and scan results.
    - **Errors:** Any errors encountered during the scan.
    - **ExitCode:** The exit code from the ClamAV command. 
        - Returns 1, if virus is detected. 
        - Returns 0, if virus is not detected. 

Response Examples:
- No Virus Detected:
```
    {
        "Outputs": {
            "Result Scan": {
                "/data/6.1-MB-scaled-1.jpg:Zone.Identifier": "OK",
                "/data/file_example_JPG_10.1MB-scaled-1.jpg": "OK",
                "/data/file_example_JPG_10.1MB-scaled-1.jpg:Zone.Identifier": "OK",
                "/data/SampleDOCFile_5MB.doc": "OK",
                "/data/6.1-MB-scaled-1.jpg": "OK",
                "/data/SampleDOCFile_5MB.doc:Zone.Identifier": "OK",
                "/data/file2.txt": "OK",
                "/data/file1.txt": "OK"
            }
            "Known viruses": "8694977",
            "Engine version": "1.0.3",
            "Scanned directories": "1",
            "Scanned files": 8,
            "Infected files": 0,
            "Data scanned": "17.36 MB",
            "Data read": "6.67 MB (ratio 2.60:1)",
            "Time": "12.885 sec (0 m 12 s)",
            "Start Date": "2024:06:19 14:03:39",
            "End Date": "2024:06:19 14:03:51",
        },
        "Errors": "",
        "ExitCode": 0
    }
```


- Virus Detected:


```
    {
        "Outputs": {
            "Result Scan": {
                "/data/6.1-MB-scaled-1.jpg:Zone.Identifier": "OK",
                "/data/file_example_JPG_10.1MB-scaled-1.jpg": "OK",
                "/data/file_example_JPG_10.1MB-scaled-1.jpg:Zone.Identifier": "OK",
                "/data/SampleDOCFile_5MB.doc": "OK",
                "/data/6.1-MB-scaled-1.jpg": "OK",
                "/data/SampleDOCFile_5MB.doc:Zone.Identifier": "Eicar-Test-Signature FOUND",
                "/data/file2.txt": "OK",
                "/data/file1.txt": "OK"
            }
            "Known viruses": "8694977",
            "Engine version": "1.0.3",
            "Scanned directories": "1",
            "Scanned files": 8,
            "Infected files": 0,
            "Data scanned": "17.36 MB",
            "Data read": "6.67 MB (ratio 2.60:1)",
            "Time": "12.885 sec (0 m 12 s)",
            "Start Date": "2024:06:19 14:03:39",
            "End Date": "2024:06:19 14:03:51",
        },
        "Errors": "",
        "ExitCode": 0
    }
```

## Development and Testing
### Building the Project

To build the project locally using the .NET SDK:

```sh
dotnet build
```

### Running the Application
If you prefer to run the application without Docker:

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

If you are running the application locally without Docker, use the following commands instead:

#### For Directory:
```
curl -X POST "http://localhost:5093/scan" \
     -H "Content-Type: application/json" \
     -d "{\"Path\":\"/data\",  \"IsDirectory\":true}"
```
#### For Single File:
```
curl -X POST "http://localhost:5093/scan" \
     -H "Content-Type: application/json" \
     -d "{\"Path\":\"/data/file1.txt\", \"IsDirectory\":false}"
```