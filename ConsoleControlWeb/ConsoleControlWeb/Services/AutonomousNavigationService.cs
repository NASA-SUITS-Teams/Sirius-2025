using ConsoleControlWeb.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleControlWeb.Services
{
    public class AutonomousNavigationService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly string serverIP = "127.0.0.1";
        private readonly int serverPort = 14141;
        private bool _isAutonomous = false;
        private Vector2 _destination;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private Vector2 lastPosition;
        private bool hasLastPosition = false;

        public void StartAutonomous(Vector2 destination)
        {
            _destination = destination;
            _isAutonomous = true;
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            Task.Run(() => AutonomousLoop(_cts.Token));
        }

        public void StopAutonomous()
        {
            _isAutonomous = false;
            _cts.Cancel();
        }

        private async Task AutonomousLoop(CancellationToken token)
        {
            while (_isAutonomous && !token.IsCancellationRequested)
            {
                using (var client = new TelemetryClient(serverIP, serverPort))
                {
                    float? posX = await client.RequestTelemetryValueAsync(23);
                    float? posY = await client.RequestTelemetryValueAsync(24);
                    if (!posX.HasValue || !posY.HasValue)
                    {
                        await Task.Delay(500, token);
                        continue;
                    }
                    var currentPosition = new Vector2(posX.Value, posY.Value);
                    Vector2 toDestination = _destination - currentPosition;
                    float distanceToDestination = toDestination.Magnitude;
                    if (distanceToDestination < 100f)
                    {
                        await client.SendCommandAsync(1107, 1f);
                        await client.SendCommandAsync(1109, 0f);
                        await client.SendCommandAsync(1110, 0f);
                        _isAutonomous = false;
                        break;
                    }
                    float desiredHeading = (float)Math.Atan2(toDestination.Y, toDestination.X);
                    float currentHeading = 0f;
                    if (hasLastPosition)
                    {
                        Vector2 delta = currentPosition - lastPosition;
                        if (delta.Magnitude > 0.1f)
                            currentHeading = (float)Math.Atan2(delta.Y, delta.X);
                    }
                    else
                    {
                        currentHeading = desiredHeading;
                        hasLastPosition = true;
                    }
                    float headingError = NormalizeAngle(desiredHeading - currentHeading);
                    float destinationSteering = Math.Clamp(headingError * 1.0f, -1f, 1f);

                    float[] lidar = await client.RequestLidarDataAsync();
                    float obstacleSteering = 0f;
                    int count = 0;
                    float obstacleThreshold = 500f;
                    float criticalThreshold = 300f;
                    float[] sensorAngles = new float[] { -30f, -20f, 0f, 20f, 30f };
                    for (int i = 0; i < 5; i++)
                    {
                        float distance = lidar[i];
                        if (distance < 0)
                            continue;
                        if (distance < obstacleThreshold)
                        {
                            float repulsion = (obstacleThreshold - distance) / obstacleThreshold;
                            float steeringAdjustment = -Math.Sign(sensorAngles[i]) * repulsion;
                            obstacleSteering += steeringAdjustment;
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        obstacleSteering /= count;
                        obstacleSteering = Math.Clamp(obstacleSteering, -1f, 1f);
                    }
                    float finalSteering = Math.Clamp(destinationSteering + obstacleSteering, -1f, 1f);

                    bool obstacleTooClose = false;
                    for (int i = 0; i < 5; i++)
                    {
                        if (lidar[i] > 0 && lidar[i] < criticalThreshold)
                        {
                            obstacleTooClose = true;
                            break;
                        }
                    }
                    if (obstacleTooClose)
                    {
                        await client.SendCommandAsync(1107, 1f);
                        await client.SendCommandAsync(1109, 0f);
                    }
                    else
                    {
                        await client.SendCommandAsync(1107, 0f);
                        await client.SendCommandAsync(1109, 50f);
                    }
                    await client.SendCommandAsync(1110, finalSteering);
                    lastPosition = currentPosition;
                    await Task.Delay(200, token);
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private float NormalizeAngle(float angle)
        {
            while (angle > MathF.PI)
                angle -= 2 * MathF.PI;
            while (angle < -MathF.PI)
                angle += 2 * MathF.PI;
            return angle;
        }
    }
}
