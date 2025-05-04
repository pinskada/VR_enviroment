using UnityEngine;
using System.Threading;
using System.Collections.Generic; // For Queue<T>
using System;
using System.Diagnostics;
using StbImageSharp;

public class GuiRenderer : MonoBehaviour
{
    Queue<byte[]> decodeQueue = new Queue<byte[]>();
    Queue<(byte[] rawData, int width, int height, string eyeSide)> mainThreadQueue = new Queue<(byte[], int, int, string)>();
    AutoResetEvent decodeSignal = new AutoResetEvent(false);
    Thread decodeThread;

    [SerializeField] private UnityEngine.UI.RawImage leftEyeImage;
    [SerializeField] private UnityEngine.UI.RawImage rightEyeImage;

    [SerializeField] private UnityEngine.UI.AspectRatioFitter leftEyeAspectFitter;
    [SerializeField] private UnityEngine.UI.AspectRatioFitter rightEyeAspectFitter;


    private string newEyeSide = "none";
    private bool running = true;

    public static event Action OnGuiReady;

    void Awake()
    {
        OnGuiReady?.Invoke();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        decodeThread = new Thread(HandleImage);
        decodeThread.IsBackground = true;
        decodeThread.Start();
    }

    void OnApplicationQuit()
    {
        running = false;
        decodeSignal.Set(); // unblock if waiting
        decodeThread.Join();
    }

    public void UpdateEyeSide(string side)
    {
        if (side == "left_JPEG")
        {
            newEyeSide = "left";
        }
        else if (side == "right_JPEG")
        {
            newEyeSide = "right";
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid eye side: " + side);
        }
    }

    public void OnImageReceived(byte[] imageData)
    {
        try
        {
            lock (decodeQueue)
            {
                decodeQueue.Enqueue(imageData);
            }
            
            decodeSignal.Set(); // Signal decoder to wake up
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Error in OnImageReceived: " + ex.Message);
        }
    }

    public void HandleImage()
    {
        while (running)
        {
            decodeSignal.WaitOne(); // Blocks until signal is set

            byte[] imageData = null;

            lock (decodeQueue)
            {
                if (decodeQueue.Count > 0)
                    imageData = decodeQueue.Dequeue();
            }

            if (imageData != null)
            {
                // DECODE using StbImageSharp
                var result = ImageResult.FromMemory(imageData, ColorComponents.RedGreenBlueAlpha);
                
                // Copy raw RGBA bytes to new array (optional but safer)
                byte[] rawImageData = new byte[result.Data.Length];
                Array.Copy(result.Data, rawImageData, result.Data.Length);

                // Send to main thread (via ConcurrentQueue or dispatcher)
                lock (mainThreadQueue)
                {
                    mainThreadQueue.Enqueue((rawImageData, result.Width, result.Height, newEyeSide));
                }
            }
        }
    }

    void Update()
    {
        lock (mainThreadQueue)
        {
            while (mainThreadQueue.Count > 0)
            {
                // Create texture on MAIN THREAD
                var image = mainThreadQueue.Dequeue();
                Texture2D tex = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
                tex.LoadRawTextureData(image.rawData);
                tex.Apply();
                string eyeSide = image.eyeSide;

                if (eyeSide == "left" && leftEyeImage != null)
                {
                    leftEyeImage.texture = tex;
                    float aspect = (float)tex.width / tex.height;
                    leftEyeAspectFitter.aspectRatio = aspect;
                }   
                else if (eyeSide == "right" && rightEyeImage != null)
                {
                    rightEyeImage.texture = tex;
                    float aspect = (float)tex.width / tex.height;
                    rightEyeAspectFitter.aspectRatio = aspect;
                }
            }
        }
    }
}
