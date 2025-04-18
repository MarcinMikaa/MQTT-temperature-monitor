# MQTT Temperature Monitor

A C# (.NET 8) application for monitoring CPU temperature using MQTT protocol.

## Features
- Client mode: Publishes CPU temperature periodically (every 10 seconds) and on demand.
- Server mode: Receives temperature data and sends measurement requests.
- Uses HiveMQ public broker, MQTTnet 5.x.x, and LibreHardwareMonitor libraries.

## Setup
1. Install .NET 8 SDK and Visual Studio 2022.
2. Install NuGet packages: MQTTnet (5.x.x), LibreHardwareMonitorLib.
3. Run as administrator (required for LibreHardwareMonitor to read CPU temperature).
4. Choose mode (client or server) at startup.

## Usage
- Run two instances: one in client mode, one in server mode.
- Client sends temperature every 10 seconds or on server request.
- Server displays received data and sends requests via Enter key.
- Press Ctrl+C to gracefully stop the application.
