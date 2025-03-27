using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class TelemetryClient : IDisposable
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    public TelemetryClient(string serverIP, int serverPort)
    {
        udpClient = new UdpClient();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
    }

    // Converts a uint value to a 4-byte big-endian array.
    private byte[] ToBigEndianBytes(uint value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    // Converts a float value to a 4-byte big-endian array.
    private byte[] ToBigEndianBytes(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }

    /// <summary>
    /// Sends a 12-byte command packet (timestamp, command number, input data) via UDP.
    /// </summary>
    public async Task SendCommandAsync(uint commandNumber, float inputData)
    {
        uint timestamp = (uint)Environment.TickCount;
        byte[] packet = new byte[12];
        Array.Copy(ToBigEndianBytes(timestamp), 0, packet, 0, 4);
        Array.Copy(ToBigEndianBytes(commandNumber), 0, packet, 4, 4);
        Array.Copy(ToBigEndianBytes(inputData), 0, packet, 8, 4);

        Console.WriteLine($"[TelemetryClient] Sending command {commandNumber} with data {inputData}");
        await udpClient.SendAsync(packet, packet.Length, remoteEndPoint);
    }

    /// <summary>
    /// Sends a command and waits for a response (up to expectedResponseLength bytes) or until a timeout.
    /// </summary>
    public async Task<byte[]> SendCommandAndReceiveAsync(uint commandNumber, float inputData, int expectedResponseLength, int timeoutMillis = 1000)
    {
        uint timestamp = (uint)Environment.TickCount;
        byte[] packet = new byte[12];
        Array.Copy(ToBigEndianBytes(timestamp), 0, packet, 0, 4);
        Array.Copy(ToBigEndianBytes(commandNumber), 0, packet, 4, 4);
        Array.Copy(ToBigEndianBytes(inputData), 0, packet, 8, 4);

        await udpClient.SendAsync(packet, packet.Length, remoteEndPoint);

        var receiveTask = udpClient.ReceiveAsync();
        if (await Task.WhenAny(receiveTask, Task.Delay(timeoutMillis)) == receiveTask)
        {
            return receiveTask.Result.Buffer;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Sends a command (with input data 0) and expects a 4-byte float response.
    /// </summary>
    public async Task<float?> RequestTelemetryValueAsync(uint commandNumber)
    {
        byte[] response = await SendCommandAndReceiveAsync(commandNumber, 0f, 4);
        if (response == null || response.Length < 4)
            return null;
        if (BitConverter.IsLittleEndian)
            Array.Reverse(response, 0, 4);
        return BitConverter.ToSingle(response, 0);
    }

    /// <summary>
    /// Requests the LIDAR data using command 165. The response is expected to be 52 bytes (13 floats).
    /// </summary>
    public async Task<float[]> RequestLidarDataAsync()
    {
        byte[] response = await SendCommandAndReceiveAsync(165, 0f, 52);
        float[] lidarData = new float[13];
        if (response == null || response.Length < 52)
            return lidarData; // returns an array of zeros if no valid response
        for (int i = 0; i < 13; i++)
        {
            byte[] floatBytes = new byte[4];
            Array.Copy(response, i * 4, floatBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(floatBytes);
            lidarData[i] = BitConverter.ToSingle(floatBytes, 0);
        }
        return lidarData;
    }

    public void Dispose()
    {
        udpClient?.Close();
        udpClient?.Dispose();
    }
}
