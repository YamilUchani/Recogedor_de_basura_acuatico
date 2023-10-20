using UnityEngine;
using System;
using System.Net.Sockets;

public class deph : MonoBehaviour
{
    private RenderTexture[] renderTextures = new RenderTexture[3];
    private Texture2D[] textures = new Texture2D[3];
    public Camera[] cameras = new Camera[3]; // Asigna tus cámaras en el inspector
    private float lastSendTime; // Para rastrear el tiempo de envío
    private int i;
    private void OnEnable()
    {
        // Inicializa los RenderTextures
        for (int i = 0; i < 3; i++)
        {
            renderTextures[i] = new RenderTexture(600, 250, 24);
            renderTextures[i].Create();
            cameras[i].targetTexture = renderTextures[i];
            textures[i] = new Texture2D(renderTextures[i].width, renderTextures[i].height);
        }

        lastSendTime = Time.time; // Inicializa el tiempo de envío
    }

    private void Update()
    {
        // Comprueba si ha pasado al menos 10 segundos desde el último envío
        if (Time.time - lastSendTime >= 0.7)
        {
            RenderTexture.active = renderTextures[i];
            textures[i].ReadPixels(new Rect(0, 0, renderTextures[i].width, renderTextures[i].height), 0, 0);
            textures[i].Apply();
            byte[] data0 = textures[i].EncodeToPNG();
            SendToServer(data0);
            i++;
            lastSendTime = Time.time;
        }
        if(i>2)
        {
            i=0;
        }
    }

    private void SendToServer(byte[] data0)
    {
        try
        {
            // Establece una conexión con el servidor Python
            TcpClient client = new TcpClient("127.0.0.1", 12345); // Ajusta la dirección IP y el puerto

            NetworkStream stream = client.GetStream();

            // Envía los datos
            stream.Write(data0, 0, data0.Length);

            // Cierra la conexión
            client.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error al enviar datos: " + e.Message);
        }
    }
}
