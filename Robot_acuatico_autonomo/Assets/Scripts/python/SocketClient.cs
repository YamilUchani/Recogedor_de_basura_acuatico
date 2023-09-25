using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;

public class SocketClient : MonoBehaviour
{
    public Camera cam;
    private float captureInterval = 5.0f;
    private float nextCaptureTime;
    public int quality = 25;  // Quality level of the image, from 0 (worst) to 100 (best)

    void Update()
    {
        if (Time.time > nextCaptureTime)
        {
            nextCaptureTime = Time.time + captureInterval;
            CaptureAndSendImage();
        }
    }

        void CaptureAndSendImage()
    {
        RenderTexture rt = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(cam.pixelWidth, cam.pixelHeight, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, cam.pixelWidth, cam.pixelHeight), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null; 
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToJPG(100 - quality);  // Modificar la calidad aquí

        // Send the image data to Python over a socket
        TcpClient client = new TcpClient("localhost", 5000);
        NetworkStream nwStream = client.GetStream();
        nwStream.Write(bytes, 0, bytes.Length);
        client.Close();
    }

}
