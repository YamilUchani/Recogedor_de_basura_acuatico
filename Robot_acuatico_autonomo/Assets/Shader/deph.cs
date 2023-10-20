using UnityEngine;
using System;
using System.Net.Sockets;
using System.Collections.Generic;

public class deph : MonoBehaviour
{
    private RenderTexture[] renderTextures = new RenderTexture[3];
    private Texture2D[] textures = new Texture2D[3];
    public Camera[] cameras = new Camera[3]; // Asigna tus cámaras en el inspector
    private float lastSendTime; // Para rastrear el tiempo de envío
    private int currentIndex = 0;
    private Queue<byte[]> imageQueue = new Queue<byte[]>();
    private int i;

    private void OnEnable()
    {
        lastSendTime = Time.time; // Inicializa el tiempo de envío
    }

    private void Update()
    {
        // Comprueba si ha pasado al menos 10 segundos desde el último envío
        if (Time.time - lastSendTime >= 1)
        {
            RenderTexture rt = new RenderTexture(960,540, 24);
            Texture2D screenShot = new Texture2D(960, 540, TextureFormat.RGB24, false);
            cameras[i].targetTexture = rt;
            cameras[i].Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, 960, 540), 0, 0);
            cameras[i].targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            byte[] data = screenShot.EncodeToPNG();

            // Encolar la imagen para el envío
            imageQueue.Enqueue(data);

            // Pasar al siguiente índice
            currentIndex = (currentIndex + 1) % 3;

            lastSendTime = Time.time;
            i++;
            if(i>=2+1)
            {
                i=0;
            }
        }

        // Comprueba si hay imágenes en la cola y envía una a la vez
        if (imageQueue.Count > 0)
        {
            byte[] imageData = imageQueue.Dequeue();
            SendToServer(imageData);
        }
        
    }

    private void SendToServer(byte[] data)
{
    try
    {
        // Establece una conexión con el servidor Python
        TcpClient client = new TcpClient("127.0.0.1", 12345); // Ajusta la dirección IP y el puerto

        NetworkStream stream = client.GetStream();

        // Agrega la marca "END_OF_IMAGE" al final de los datos
        byte[] dataWithMarker = new byte[data.Length + 12]; // 12 bytes para "END_OF_IMAGE"
        data.CopyTo(dataWithMarker, 0);
        byte[] marker = System.Text.Encoding.ASCII.GetBytes("END_OF_IMAGE");
        marker.CopyTo(dataWithMarker, data.Length);

        // Envía los datos
        stream.Write(dataWithMarker, 0, dataWithMarker.Length);

        // Cierra la conexión
        client.Close();
    }
    catch (Exception e)
    {
        Debug.LogError("Error al enviar datos: " + e.Message);
    }
}
}