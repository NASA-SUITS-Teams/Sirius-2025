import React, { useState, useEffect, useRef, useCallback } from 'react';
import './App.css';

function App() {
  // Telemetry state: we expect data.pr_telemetry
  const [telemetry, setTelemetry] = useState(null);
  const [telemetryError, setTelemetryError] = useState(null);
  
  // Control mode: "manual" or "autonomous"
  const [controlMode, setControlMode] = useState("manual");
  
  // Manual control states (car-like controls)
  const [throttle, setThrottle] = useState(0);   // -100 to 100
  const [steering, setSteering] = useState(0);     // -1.0 to 1.0
  const [brake, setBrake] = useState(0);           // 0 (off) or 1 (on)
  const [continuous, setContinuous] = useState(false);
  
  // WASD keyboard control
  const [wasdEnabled, setWasdEnabled] = useState(false);
  
  // Autonomous destination inputs
  const [destinationX, setDestinationX] = useState("");
  const [destinationY, setDestinationY] = useState("");
  
  // Ref for continuous command interval
  const continuousInterval = useRef(null);

  // Poll telemetry data every second from backend API.
  useEffect(() => {
    const interval = setInterval(() => {
      fetch('/api/telemetry')
        .then(res => {
          if (!res.ok) throw new Error("Telemetry fetch error");
          return res.json();
        })
        .then(data => {
          setTelemetry(data);
          setTelemetryError(null);
        })
        .catch(err => {
          console.error(err);
          setTelemetryError("Error fetching telemetry data.");
        });
    }, 1000);
    return () => clearInterval(interval);
  }, []);

  // Function to send manual command to backend API.
  const sendManualCommand = async (throttleVal, steeringVal, brakeVal) => {
    try {
      const res = await fetch('/api/manual/command', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ throttle: throttleVal, steering: steeringVal, brakes: brakeVal })
      });
      const data = await res.json();
      console.log("Command sent:", data);
    } catch (error) {
      console.error("Error sending command:", error);
    }
  };

  // One-shot command handler.
  const handleSendCommand = () => {
    sendManualCommand(throttle, steering, brake);
  };

  // Continuous command sending for on-screen joystick.
  const startContinuousCommands = (action) => {
    if (continuousInterval.current) return;
    continuousInterval.current = setInterval(() => {
      switch (action) {
        case 'forward':
          sendManualCommand(100, 0, 0);
          break;
        case 'reverse':
          sendManualCommand(-100, 0, 0);
          break;
        case 'left':
          sendManualCommand(100, -1, 0);
          break;
        case 'right':
          sendManualCommand(100, 1, 0);
          break;
        default:
          break;
      }
    }, 500);
  };

  const stopContinuousCommands = () => {
    if (continuousInterval.current) {
      clearInterval(continuousInterval.current);
      continuousInterval.current = null;
    }
    // When released, send a stop command.
    sendManualCommand(0, 0, 1);
  };

  // WASD keyboard control handler.
  const handleKeyDown = useCallback((event) => {
    if (!wasdEnabled) return;
    switch (event.key.toLowerCase()) {
      case 'w':
        sendManualCommand(100, 0, 0);
        break;
      case 's':
        sendManualCommand(0, 0, 1);
        break;
      case 'a':
        sendManualCommand(100, -1, 0);
        break;
      case 'd':
        sendManualCommand(100, 1, 0);
        break;
      case ' ':
        sendManualCommand(0, 0, 1);
        break;
      default:
        break;
    }
  }, [wasdEnabled]);

  useEffect(() => {
    if (wasdEnabled) {
      window.addEventListener('keydown', handleKeyDown);
    } else {
      window.removeEventListener('keydown', handleKeyDown);
    }
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [wasdEnabled, handleKeyDown]);

  // Autonomous commands.
  const startAutonomous = async () => {
    try {
      const res = await fetch('/api/autonomous/start', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ destinationX: parseFloat(destinationX), destinationY: parseFloat(destinationY) })
      });
      const data = await res.json();
      console.log("Autonomous navigation started", data);
    } catch (error) {
      console.error("Error starting autonomous navigation", error);
    }
  };

  const stopAutonomous = async () => {
    try {
      const res = await fetch('/api/autonomous/stop', { method: 'POST' });
      const data = await res.json();
      console.log("Autonomous navigation stopped", data);
    } catch (error) {
      console.error("Error stopping autonomous navigation", error);
    }
  };

  return (
    <div className="App">
      <h1>Rover Control Dashboard</h1>
      
      <section className="telemetry-section">
        <h2>Telemetry Data</h2>
        {telemetryError && <p className="error">{telemetryError}</p>}
        {telemetry && telemetry.pr_telemetry ? (
          <div className="telemetry-cards">
            <div className="card">
              <h3>Position</h3>
              <p>X: {telemetry.pr_telemetry.current_pos_x !== undefined ? telemetry.pr_telemetry.current_pos_x.toFixed(2) : "N/A"}</p>
              <p>Y: {telemetry.pr_telemetry.current_pos_y !== undefined ? telemetry.pr_telemetry.current_pos_y.toFixed(2) : "N/A"}</p>
              <p>Altitude: {telemetry.pr_telemetry.current_pos_alt !== undefined ? telemetry.pr_telemetry.current_pos_alt.toFixed(2) : "N/A"}</p>
            </div>
            <div className="card">
              <h3>LIDAR</h3>
              <p>{telemetry.pr_telemetry.lidar ? telemetry.pr_telemetry.lidar.join(" ") : "N/A"}</p>
            </div>
            <div className="card">
              <h3>Other Data</h3>
              <p>Speed: {telemetry.pr_telemetry.speed !== undefined ? telemetry.pr_telemetry.speed.toFixed(2) : "N/A"}</p>
              <p>Battery: {telemetry.pr_telemetry.battery_level !== undefined ? telemetry.pr_telemetry.battery_level.toFixed(2) + "%" : "N/A"}</p>
              <p>Oxygen: {telemetry.pr_telemetry.oxygen_levels !== undefined ? telemetry.pr_telemetry.oxygen_levels.toFixed(2) : "N/A"}</p>
            </div>
          </div>
        ) : (
          <p>Loading telemetry...</p>
        )}
      </section>
      
      <section className="control-section">
        <h2>Control Mode</h2>
        <div className="mode-select">
          <label>
            <input 
              type="radio" 
              value="manual" 
              checked={controlMode === "manual"} 
              onChange={() => setControlMode("manual")} 
            />
            Manual
          </label>
          <label>
            <input 
              type="radio" 
              value="autonomous" 
              checked={controlMode === "autonomous"} 
              onChange={() => setControlMode("autonomous")} 
            />
            Autonomous
          </label>
        </div>
      </section>
      
      {controlMode === "manual" && (
        <section className="manual-controls">
          <h2>Manual Controls</h2>
          <div className="control-group">
            <label>
              Throttle: {throttle}
              <input 
                type="range" 
                min="-100" 
                max="100" 
                value={throttle} 
                onChange={e => setThrottle(parseInt(e.target.value))}
              />
            </label>
          </div>
          <div className="control-group">
            <label>
              Steering: {steering}
              <input 
                type="range" 
                min="-1" 
                max="1" 
                step="0.01" 
                value={steering} 
                onChange={e => setSteering(parseFloat(e.target.value))}
              />
            </label>
          </div>
          <div className="control-group">
            <label>
              Brake: 
              <input 
                type="checkbox" 
                checked={brake === 1} 
                onChange={e => setBrake(e.target.checked ? 1 : 0)}
              />
            </label>
          </div>
          <div className="control-group">
            <button onClick={handleSendCommand}>Send Command Now</button>
          </div>
          <div className="control-group">
            <label>
              Continuous Mode:
              <input 
                type="checkbox" 
                checked={continuous} 
                onChange={e => setContinuous(e.target.checked)}
              />
            </label>
          </div>
          <div className="control-group">
            <h3>On-Screen Joystick</h3>
            <div className="joystick">
              <button 
                className="joystick-button" 
                onMouseDown={() => startContinuousCommands('forward')}
                onMouseUp={stopContinuousCommands}
                onTouchStart={() => startContinuousCommands('forward')}
                onTouchEnd={stopContinuousCommands}
              >↑</button>
              <div className="horizontal-controls">
                <button 
                  className="joystick-button" 
                  onMouseDown={() => startContinuousCommands('left')}
                  onMouseUp={stopContinuousCommands}
                  onTouchStart={() => startContinuousCommands('left')}
                  onTouchEnd={stopContinuousCommands}
                >←</button>
                <button 
                  className="joystick-button" 
                  onMouseDown={() => startContinuousCommands('reverse')}
                  onMouseUp={stopContinuousCommands}
                  onTouchStart={() => startContinuousCommands('reverse')}
                  onTouchEnd={stopContinuousCommands}
                >↓</button>
                <button 
                  className="joystick-button" 
                  onMouseDown={() => startContinuousCommands('right')}
                  onMouseUp={stopContinuousCommands}
                  onTouchStart={() => startContinuousCommands('right')}
                  onTouchEnd={stopContinuousCommands}
                >→</button>
              </div>
            </div>
          </div>
          <div className="control-group">
            <label>
              Enable WASD Keyboard Control:
              <input 
                type="checkbox" 
                checked={wasdEnabled} 
                onChange={e => setWasdEnabled(e.target.checked)}
              />
            </label>
            {wasdEnabled && <p>W: Forward, S: Brake, A: Left, D: Right, Space: Brake</p>}
          </div>
        </section>
      )}
      
      {controlMode === "autonomous" && (
        <section className="autonomous-controls">
          <h2>Autonomous Controls</h2>
          <div className="destination-inputs">
            <label>
              Destination X:
              <input 
                type="number" 
                value={destinationX} 
                onChange={e => setDestinationX(e.target.value)} 
              />
            </label>
            <label>
              Destination Y:
              <input 
                type="number" 
                value={destinationY} 
                onChange={e => setDestinationY(e.target.value)} 
              />
            </label>
          </div>
          <div className="auto-buttons">
            <button onClick={startAutonomous}>Start Autonomous Navigation</button>
            <button onClick={stopAutonomous}>Stop Autonomous Navigation</button>
          </div>
        </section>
      )}
    </div>
  );
}

export default App;
