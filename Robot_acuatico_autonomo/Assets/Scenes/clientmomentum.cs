using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
public class clientmomentum : MonoBehaviour
{
public int contador;
    private Texture2D texture;
    public Camera cam;
    public float captureInterval = 10.0f;  // Intervalo de captura de 10 segundos
    public int quality = 25;  // Calidad de la imagen, de 0 (peor) a 100 (mejor)
    private int screenWidth;
    private int screenHeight;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] data;
    public string message;
    public Movimiento_autonomo mov_auto;
    public Camera Camino;
    private int lineCount = 0;
    private string valorFinal;
    void OnEnable()
    {
        
        InvokeRepeating("SendMessage", 0.0f, captureInterval);
        texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.red);
        texture.Apply();
        client = new TcpClient("localhost", 12345);
        stream = client.GetStream();
    }

    void SetupTCP(byte[] data)
    {   
        stream.Write(data, 0, data.Length);
        data = new byte[2048];
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.ASCII.GetString(data, 0, bytes);
        mov_auto.GirarHaciaAnguloAutonoma(float.Parse(message));
        data=null;
    }

    void SendMessage()
    {
        screenWidth = cam.pixelWidth;
        screenHeight = cam.pixelHeight;
        RenderTexture rt = new RenderTexture(screenWidth, screenHeight, 24); // Usar las variables
        Texture2D screenShot = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false); // Usar las variables
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0); // Usar las variables
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToJPG(100 - quality);  // Modificar la calidad aquí
        //string path = "Assets/Imagenes/muestra"+contador.ToString()+".png"; // Ruta del archivo de imagen en Assets
        //System.IO.File.WriteAllBytes(path, bytes);
        Destroy(screenShot);
        Destroy(rt);
        Thread sendThread = new Thread(() =>
        {
            SetupTCP(bytes);
        });
        sendThread.Start();
    }
}
