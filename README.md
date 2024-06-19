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
- URL: /scan-directory

- Method: POST

- Content-Type: application/json

- Request Body:
    ```json
    {
        "directory": "/data"
    }
    ```
    The directory field specifies the path to the directory to be scanned. This path should be within the /data directory, which is mapped from the host machine.


### Example Request
To test the endpoint, you can use `curl`:
```
curl -X POST "http://localhost:8080/scan-directory" \
     -H "Content-Type: application/json" \
     -d "{\"directory\":\"/data\"}"
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

## Development and Testing
### Building the Project

To build the project locally using the .NET SDK:

```sh
dotnet build
```

### Running the Application
To run the application locally without Docker:
```sh
dotnet run
```
