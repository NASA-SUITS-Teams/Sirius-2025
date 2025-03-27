# SUITS Telemetry & Rover Control System

This project integrates a Unity‑based DUST simulation (TSS Server) with a custom ASP.NET Core backend and a React front‑end to provide a complete control and telemetry monitoring solution for the SUITS pressurized rover. The system supports both manual (car‑like) control and autonomous navigation via UDP commands sent to the TSS server, while displaying real‑time telemetry data in a web interface.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Folder Structure](#folder-structure)
- [Server-Side Components](#server-side-components)
  - [TelemetryClient](#telemetryclient)
  - [ManualController](#manualcontroller)
  - [TelemetryController](#telemetrycontroller)
  - [AutonomousNavigationService (Optional)](#autonomousnavigationservice-optional)
  - [Program.cs & Launch Settings](#programcs--launch-settings)
- [Client-Side (React) Components](#client-side-react-components)
  - [App.js](#appjs)
  - [App.css](#appcss)
- [Setup Instructions](#setup-instructions)
  - [Pre-requisites](#pre-requisites)
  - [Backend Setup & Running](#backend-setup--running)
  - [Front-End Setup & Running](#front-end-setup--running)
  - [TSS / DUST Simulation Setup](#tss--dust-simulation-setup)
- [Control Instructions](#control-instructions)
  - [Manual Control](#manual-control)
  - [Autonomous Navigation](#autonomous-navigation)
  - [Telemetry Monitoring](#telemetry-monitoring)
- [Important Code Locations & How to Edit](#important-code-locations--how-to-edit)
- [Next Steps & Future Enhancements](#next-steps--future-enhancements)
- [Troubleshooting](#troubleshooting)


---

## Project Overview

This project combines three major components:

1. **TSS/DUST Simulation:**  
   A Unity-based simulation that runs the pressurized rover and provides telemetry data via a UDP socket and an HTTP server. The TSS server (built in C/C++) listens on port 14141 and exposes telemetry JSON files (e.g. `/teams/0/ROVER_TELEMETRY.json`).

2. **ASP.NET Core Backend (ConsoleControlWeb):**  
   - **Command Proxy:** Receives manual and autonomous control commands from the React UI and sends them via UDP to the TSS server.  
     - Command numbers:
       - **1107:** Brakes (0 or 1)
       - **1109:** Throttle (float: -100 to 100)
       - **1110:** Steering (float: -1.0 to 1.0)
   - **Telemetry Proxy:** Fetches telemetry JSON from the TSS server’s public HTTP endpoint and provides it to the front end.

3. **React Front-End (ClientApp):**  
   A web UI that displays telemetry data and allows manual control (using sliders, a joystick, and WASD keyboard support) as well as autonomous navigation.

---

## Folder Structure

C:\Users\brand\Documents\SUITS\
├── DUST_V1                // Contains the DUST simulation executable and its assets.
│   └── Windows
│       ├── Engine
│       └── SUITS_DUST
├── suits
│   ├── ConsoleControlWeb  // ASP.NET Core backend + React front-end for Console Control.
│   │   ├── ConsoleControlWeb   // ASP.NET Core project folder
│   │   │   ├── Controllers
│   │   │   │   ├── ManualController.cs
│   │   │   │   └── TelemetryController.cs
│   │   │   ├── Services
│   │   │   │   └── AutonomousNavigationService.cs (if applicable)
│   │   │   ├── TelemetryClient.cs
│   │   │   ├── Program.cs
│   │   │   └── Properties
│   │   │       └── launchSettings.json
│   │   └── clientapp       // React project folder
│   │       ├── package.json
│   │       ├── public
│   │       └── src
│   │           ├── App.js
│   │           └── App.css
│   └── VRControl           // Other code for VR-based UI (kept separate)
├── TSS



---

## Server-Side Components

### TelemetryClient

- **Location:** `ConsoleControlWeb/TelemetryClient.cs`
- **Purpose:**  
  Sends UDP commands to the TSS server and provides helper methods:
  - `SendCommandAsync(uint commandNumber, float inputData)`
  - `RequestTelemetryValueAsync(uint commandNumber)`
  - `RequestLidarDataAsync()`
- **Important:**  
  Uses big-endian encoding for the UDP packet (12 bytes: timestamp, command number, float value).

### ManualController

- **Location:** `ConsoleControlWeb/Controllers/ManualController.cs`
- **Purpose:**  
  Accepts POST requests with manual control commands from the React UI and uses `TelemetryClient` to send:
  - **1107:** Brakes
  - **1109:** Throttle
  - **1110:** Steering

### TelemetryController

- **Location:** `ConsoleControlWeb/Controllers/TelemetryController.cs`
- **Purpose:**  
  Acts as a proxy to fetch telemetry JSON from the TSS server.  
- **Key URL:**  
  `http://127.0.0.1:14141/json_data/teams/0/ROVER_TELEMETRY.json`  
  (Adjust the team number or URL as needed.)

### AutonomousNavigationService (Optional)

- **Location:** `ConsoleControlWeb/Services/AutonomousNavigationService.cs`
- **Purpose:**  
  Contains logic for autonomous navigation (reading telemetry and sending commands continuously).  
- **Important:**  
  Ensure it calls the TelemetryClient methods properly.

### Program.cs & Launch Settings

- **Location:** `ConsoleControlWeb/Program.cs` and `ConsoleControlWeb/Properties/launchSettings.json`
- **Purpose:**  
  Configures the ASP.NET Core backend, registers HttpClient (used by TelemetryController), and sets the listening URL/port.
- **Important:**  
  Verify that the `applicationUrl` in launchSettings.json matches the proxy URL in the React project.

---

## Client-Side (React) Components

### App.js

- **Location:** `ConsoleControlWeb/clientapp/src/App.js`
- **Purpose:**  
  Main UI for:
  - Displaying telemetry data from the backend.
  - Sending manual control commands (via sliders, a brake checkbox, on‑screen joystick, and WASD support).
  - Switching between manual and autonomous modes.
- **Important Functions/States:**  
  - `sendManualCommand`: Sends a POST to `/api/manual/command`.
  - Telemetry polling: Fetches data from `/api/telemetry` every second.
  - Continuous command sending: Sends repeated commands (every 500ms) for on‑screen joystick buttons.

### App.css

- **Location:** `ConsoleControlWeb/clientapp/src/App.css`
- **Purpose:**  
  Contains styling for UI components (telemetry cards, sliders, joystick, etc.).

---

## Setup Instructions

### Pre-requisites

- [.NET 6 SDK or newer](https://dotnet.microsoft.com/download)
- [Node.js and npm](https://nodejs.org/)
- TSS/DUST simulation executable running (with UDP server active)
- Basic command-line and code editor familiarity

### Backend Setup & Running

1. Open a terminal and navigate to:  
   `C:\Users\brand\Documents\SUITS\suits\ConsoleControlWeb\ConsoleControlWeb`
2. Run:
   ```bash
   dotnet restore
   dotnet build
   dotnet run
3. Confirm that the console shows a message like:
    Now listening on: http://localhost:5271
4. Test the telemetry endpoint by visiting:
    http://localhost:5271/api/telemetry
    You should see the telemetry JSON containing "pr_telemetry".

### Front-End Setup & Running

1. Open a terminal and navigate to:
    C:\Users\brand\Documents\SUITS\suits\ConsoleControlWeb\clientapp
2. Verify that package.json has a proxy setting:
    "proxy": "http://localhost:5271",
3. run:
    npm install
    npm start
4. Your browser should open at http://localhost:3000, showing the UI.

### TSS / DUST Simulation Setup

1. In a terminal, navigate to your TSS folder, for example:
    C:\Users\brand\Documents\SUITS\TSS-2025
2. Run the TSS server:
    ./server.exe --local
3. Verify that the TSS server logs show it is listening on port 14141.
4. Open your browser and check:
    http://127.0.0.1:14141/json_data/teams/0/ROVER_TELEMETRY.json
    This should display the telemetry JSON. Confirm that when the rover moves, the values (like current_pos_x and current_pos_y) update.

### Control Instructions

**Manual Control**

    Sliders & Brake Toggle:
    Adjust the throttle slider (range –100 to 100) and steering slider (range –1.0 to 1.0). Use the brake checkbox to engage braking when needed.
    Send Command:
    Click the "Send Command Now" button to send a one‑shot command.
    Continuous Mode & Joystick:
    Enable Continuous Mode to send commands repeatedly. Use the on‑screen joystick buttons (↑, ←, ↓, →) for directional control.
    WASD Keyboard:
    Enable WASD Keyboard Control to drive using keys: W (forward), S (brake), A (left), D (right), and Space (brake).

**Autonomous Navigation** - Work in progress

    Switch the control mode to "Autonomous."
    Enter destination X and Y coordinates.
    Use the Start/Stop Autonomous Navigation buttons to engage or cancel autonomous control.

**Telemetry Monitoring**

    The telemetry panel displays:
        Position: X, Y, and Altitude.
        LIDAR: An array of 13 sensor readings.
        Other Data: Speed, Battery Level, Oxygen Levels, etc.
    Data is fetched from the TSS server’s JSON file via the backend TelemetryController.

### Important Code Locations & How to Edit

    TelemetryClient.cs:
        Location: ConsoleControlWeb/TelemetryClient.cs
        Purpose: Sends UDP commands to the TSS server.
        Key Methods: SendCommandAsync, RequestTelemetryValueAsync, RequestLidarDataAsync
        Editing Tip: Update command numbers or data formatting if the TSS spec changes.

    ManualController.cs:
        Location: ConsoleControlWeb/Controllers/ManualController.cs
        Purpose: Receives manual control commands from the React UI and sends them via TelemetryClient.
        Editing Tip: Verify that command numbers (1107, 1109, 1110) match the TSS documentation.

    TelemetryController.cs:
        Location: ConsoleControlWeb/Controllers/TelemetryController.cs
        Purpose: Fetches telemetry JSON from the TSS server.
        Editing Tip: Adjust the telemetryUrl if the file location or team number changes.

    Program.cs & launchSettings.json:
        Location: ConsoleControlWeb/ConsoleControlWeb/Program.cs and ConsoleControlWeb/ConsoleControlWeb/Properties/launchSettings.json
        Purpose: Configures the ASP.NET Core backend.
        Editing Tip: Ensure the listening URL and port match the proxy in the React app.

    React Front-End (App.js):
        Location: ConsoleControlWeb/clientapp/src/App.js
        Purpose: Main UI for telemetry display and rover control.
        Editing Tip: Modify slider ranges, continuous command intervals, or telemetry key names as needed.

    App.css:
        Location: ConsoleControlWeb/clientapp/src/App.css
        Purpose: Provides styling for the UI components.

### Next Steps & Future Enhancements

    Refine Command Timing:
    Adjust the continuous command interval if the TSS server rejects commands due to frequency.

    Enhance Telemetry Display:
    Add additional telemetry panels (e.g., gauges, graphs) to visualize trends like speed, battery drain, etc.

    Improve Error Handling:
    Enhance logging and error handling on both backend and frontend to facilitate troubleshooting.

    Consider WebGL Build (Long-Term):
    If possible, convert the Unity project to a WebGL build to embed the simulation directly in the browser. If not, investigate remote streaming solutions.

    Expand Autonomous Navigation:
    Further develop the autonomous navigation logic based on sensor feedback and mission objectives.

    Team Collaboration:
    Write tests for both backend and frontend, and document any additional changes in version control.

### Troubleshooting

    404 or ECONNREFUSED Errors:
    Verify that the ASP.NET Core backend is running on the correct URL/port and that your React proxy (in package.json) points to it.

    Telemetry Not Updating:
    Ensure that the TSS server’s JSON endpoint returns updated data when the rover moves. Test by visiting the endpoint directly in your browser.

    Invalid Command Messages:
    Check that the command numbers and value formats sent by TelemetryClient match the TSS specification exactly.

    Build Errors:
    Rebuild both the backend and frontend after making changes. Check console logs for any error messages and address them as needed.

  