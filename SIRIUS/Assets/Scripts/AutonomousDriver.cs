using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AutonomousDriver : MonoBehaviour
{
    private bool isAutonomous = false;

    private void Start()
    {
        InvokeRepeating(nameof(Drive), 0f, 0.1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            isAutonomous = !isAutonomous;
        }
    }

    private (bool, float, float) AutonomousControl()
    {
        PrTelemetry prTelemetry = TSSClient.Instance.PRTelemetry;

        if (prTelemetry == null) {
            print($"No telemetry data!");
            return (false, 0, 0);
        }

        Vector2 currPos = new(prTelemetry.current_pos_x, prTelemetry.current_pos_y);
        Vector2 destPos = new(prTelemetry.dest_x, prTelemetry.dest_y);
        Vector2 diffPos = destPos - currPos;

        if (diffPos.magnitude < 5) {
            print($"At destination! Distance: {diffPos.magnitude} m");
            return (true, 0, 0);
        }

        List<float> lidarArray = prTelemetry.lidar;

        if (lidarArray[2] != -1) {
            print($"Obstacle directly ahead! Distance: {diffPos.magnitude} m");
            return (false, 50, 1);
        }

        if (lidarArray[1] != -1 || lidarArray[5] < 80 || lidarArray[6] > 200) {
            print($"Obstacle to the left. Distance: {diffPos.magnitude} m");
            return (false, 50, 1);
        }

        if (lidarArray[3] != -1 || lidarArray[6] < 80 || lidarArray[6] > 200) {
            print($"Obstacle to the right. Distance: {diffPos.magnitude} m");
            return (false, 50, -1);
        }

        // Convert DUST heading from degrees clockwise positive from south to radians counterclockwise positive from east
        float currHeading = -(prTelemetry.heading * MathF.PI / 180 - MathF.PI / 2) % (2 * MathF.PI);
        float targetHeading = MathF.Atan2(diffPos.y, -diffPos.x);

        float headingDiff = (targetHeading - currHeading) % (2 * MathF.PI);
        if (headingDiff > Mathf.PI)
        {
            headingDiff -= 2 * MathF.PI;
        }
        else if (headingDiff < -Mathf.PI)
        {
            headingDiff += 2 * MathF.PI;
        }

        float steering = MathF.Abs(headingDiff) < MathF.PI / 16 ? 0 : headingDiff > 0 ? -1 : 1;

        print($"Proceeding to destination. Distance: {diffPos.magnitude} m, Current Heading: {currHeading}, target Heading: {targetHeading}, Heading Diff: {headingDiff} rad");
        return (false, 80, steering);
    }

    private (bool, float, float) TeleopControl()
    {
        bool brakes = false;
        float throttle = 0;
        float steering = 0;

        if (Input.GetKey(KeyCode.Space))
        {
            brakes = true;
        }

        if (Input.GetKey(KeyCode.W))
        {
            throttle = 100;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            throttle = -100;
        }

        if (Input.GetKey(KeyCode.A))
        {
            steering = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            steering = 1;
        }

        PrTelemetry prTelemetry = TSSClient.Instance.PRTelemetry;

        Vector2 currPos = new(prTelemetry.current_pos_x, prTelemetry.current_pos_y);
        Vector2 destPos = new(prTelemetry.dest_x, prTelemetry.dest_y);
        Vector2 diffPos = destPos - currPos;

        // Convert DUST heading from degrees clockwise positive from south to radians counterclockwise positive from east
        float currHeading = -(prTelemetry.heading * MathF.PI / 180 - MathF.PI / 2) % (2 * MathF.PI);
        float targetHeading = MathF.Atan2(diffPos.y, -diffPos.x);

        float headingDiff = (targetHeading - currHeading) % (2 * MathF.PI);
        if (headingDiff > Mathf.PI)
        {
            headingDiff -= 2 * MathF.PI;
        }
        else if (headingDiff < -Mathf.PI)
        {
            headingDiff += 2 * MathF.PI;
        }

        print($"Teleoperationally driving. Distance: {diffPos.magnitude} m, Current Heading: {currHeading}, target Heading: {targetHeading}, Heading Diff: {headingDiff} rad");
        return (brakes, throttle, steering);
    }

    private async Task Drive()
    {
        (bool brakes, float throttle, float steering) = isAutonomous ? AutonomousControl() : TeleopControl();
        print($"Drive command: {brakes}, {throttle}, {steering}");

        await TSSClient.Instance.SendBrakes(brakes);
        await TSSClient.Instance.SendThrottle(throttle);
        await TSSClient.Instance.SendSteering(steering);
    }
}
