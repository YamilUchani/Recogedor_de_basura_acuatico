using UnityEngine;
using System;
using System.IO;

public class Capture_image : MonoBehaviour
{
    public Camera Camino;
    public RenderTexture renderTexture;
    private string assetsFolder = "Assets/Images";
    private string dateFolder;
    private int counter = 0;
    private int renderTextureWidth = 1920; // Nueva resolución de ancho
    private int renderTextureHeight = 1080; // Nueva resolución de alto

    private void Awake()
    {
        renderTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 24);
        renderTexture.Create();

        // Asignar el RenderTexture a la cámara
        Camino.targetTexture = renderTexture;
        string currentDate = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
        dateFolder = Path.Combine(assetsFolder, currentDate);

        if (!Directory.Exists(dateFolder))
        {
            Directory.CreateDirectory(dateFolder);
        }

        
    }

    public void CaptureAndSave()
    {
        // Configure the camera to use the current lighting conditions
        Camino.Render();

        string fileName = "empty" + counter.ToString() + ".png";
        string fullPath = Path.Combine(dateFolder, fileName);
        Texture2D screenShot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        RenderTexture.active = null;
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(fullPath, bytes);
        Destroy(screenShot);
        counter++;

        
        Camino.Render();
        
        
        
        
        
    }
}
