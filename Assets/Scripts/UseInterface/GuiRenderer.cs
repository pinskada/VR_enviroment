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

    [SerializeField] private EyeImageSelector leftEyeImageSelector;
    [SerializeField] private EyeImageSelector rightEyeImageSelector;
    private string newEyeSide = "none";
    private bool running = true;

    private string dataType = "JPEG";
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
        if (side == "Left" || side == "Right")
        {
            newEyeSide = side;
        }
        else
        {
            UnityEngine.Debug.LogError("Invalid eye side: " + side);
        }
    }

    public void OnImageReceived(byte[] imageData, string dataType)
    {
        this.dataType = dataType;
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
                
                byte[] rawImageData;

                if (dataType == "JPEG")
                {
                    leftEyeImageSelector.SetRotation(true);
                    rightEyeImageSelector.SetRotation(true);
                    // Rotate the image 90 degrees clockwise
                    rawImageData = Rotate90Right(result.Data, result.Width, result.Height);
                    // Swap width and height after rotation
                    int temp = result.Width;
                    result.Width = result.Height;
                    result.Height = temp;
                }
                else if (dataType == "PNG")
                {
                    // No rotation needed for PNG or JPEG
                    // Just copy the raw image data
                    
                    leftEyeImageSelector.SetRotation(false);
                    rightEyeImageSelector.SetRotation(false);
                    rawImageData = FlipVertical(result.Data, result.Width, result.Height);
                }
                else
                {
                    UnityEngine.Debug.LogError("Unsupported image type: " + dataType);
                    return;
                }

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

                if (eyeSide == "Left" && leftEyeImage != null)
                {
                    leftEyeImage.texture = tex;
                    float aspect = (float)tex.width / tex.height;
                    leftEyeAspectFitter.aspectRatio = aspect;
                }   
                else if (eyeSide == "Right" && rightEyeImage != null)
                {
                    rightEyeImage.texture = tex;
                    float aspect = (float)tex.width / tex.height;
                    rightEyeAspectFitter.aspectRatio = aspect;
                }
            }
        }
    }

    private byte[] Rotate90Right(byte[] rawData, int width, int height)
    {
        byte[] rotated = new byte[rawData.Length];
        int bytesPerPixel = 4; // RGBA

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int srcIndex = (y * width + x) * bytesPerPixel;

                // Correct clockwise rotation mapping
                int newX = y;
                int newY = width - x - 1;

                int destIndex = (newY * height + newX) * bytesPerPixel;

                Array.Copy(rawData, srcIndex, rotated, destIndex, bytesPerPixel);
            }
        }

        return rotated;
    }

    private byte[] FlipVertical(byte[] rawData, int width, int height)
    {
        byte[] flipped = new byte[rawData.Length];
        int bytesPerPixel = 4;

        for (int y = 0; y < height; y++)
        {
            int srcRow = y * width * bytesPerPixel;
            int destRow = (height - y - 1) * width * bytesPerPixel;

            Array.Copy(rawData, srcRow, flipped, destRow, width * bytesPerPixel);
        }

        return flipped;
    }

}
