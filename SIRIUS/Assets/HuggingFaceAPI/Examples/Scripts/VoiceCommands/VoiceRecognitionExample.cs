using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using HuggingFace.API;
using System.Threading.Tasks;
using System;


public class SpeechScript : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;

    private AudioClip clip;
    private bool recording;
    string selectedDevice;

    private const int RECORDINGLENGTH = 10;

    private void Start()
    {
        startButton.onClick.AddListener(startRecording);
        stopButton.onClick.AddListener(stopRecording);
    }

    private void startRecording()
    {
        // Get all available microphone devices
        string[] devices = Microphone.devices;

        // Print them to the console (this is in case you have multiple microphones)
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log("Device " + i + ": " + devices[i]);
        }

        // Choose a specific device by its name
        string selectedDevice = devices[0]; // Replace with the desired device index, always 0 if only 1 microphone
        clip = Microphone.Start(selectedDevice, false, RECORDINGLENGTH, 44100);
        recording = true;
    }
    private void Update()
    {
        if (recording && Microphone.GetPosition(selectedDevice) >= clip.samples)
        {
            stopRecording();
        }
    }

    private async void stopRecording()
    {
        string selectedDevice = Microphone.devices[0]; // Match the device you started recording with
        int position = Microphone.GetPosition(selectedDevice);
        Microphone.End(selectedDevice);

        float[] samples = new float[position * clip.channels];
        clip.GetData(samples, 0);

        byte[] audioBytes = EncodeAsWav(samples, clip.frequency, clip.channels);
        recording = false;

        //  get the response from SendRecording()
        string response = await SendRecording(audioBytes);

        // Handle the response outside of SendRecording()
        if (response != null)
        {
            text.text = response;
        }
        else
        {
            text.text = "Error occurred.";
        }
    }

    private async Task<string> SendRecording(byte[] audioBytes)
    {
        var taskCompletionSource = new TaskCompletionSource<string>();

        HuggingFaceAPI.AutomaticSpeechRecognition(audioBytes, response =>
        {
            taskCompletionSource.SetResult(response);
        }, error =>
        {
            taskCompletionSource.SetException(new Exception(error));
        });

        try
        {
            return await taskCompletionSource.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error: " + ex.Message);
            return null;
        }
    }


    private byte[] EncodeAsWav(float[] samples, int frequency, int channels)
    {
        int sampleCount = samples.Length;
        int byteCount = sampleCount * sizeof(short);

        using (MemoryStream memoryStream = new MemoryStream(44 + byteCount))
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                // RIFF Header
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + byteCount); // File size minus first 8 bytes of RIFF header
                writer.Write("WAVE".ToCharArray());

                // Format chunk
                writer.Write("fmt ".ToCharArray());
                writer.Write(16); // Subchunk size for PCM
                writer.Write((ushort)1); // Audio format (1 = PCM)
                writer.Write((ushort)channels); // Number of channels
                writer.Write(frequency); // Sample rate
                writer.Write(frequency * channels * sizeof(short)); // Byte rate
                writer.Write((ushort)(channels * sizeof(short))); // Block align
                writer.Write((ushort)(16)); // Bits per sample

                // Data chunk
                writer.Write("data".ToCharArray());
                writer.Write(byteCount); // Data size in bytes

                // Convert the samples to 16-bit PCM
                foreach (var sample in samples)
                {
                    var intSample = (short)(Mathf.Clamp(sample, -1f, 1f) * short.MaxValue);
                    writer.Write(intSample);
                }
            }

            return memoryStream.ToArray();
        }
    }

}