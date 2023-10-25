
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
public class depthclient : MonoBehaviour
{
    
    public Movimiento_autonomo mov_auto;
    private RenderTexture[] renderTextures = new RenderTexture[3];
    private Texture2D[] textures = new Texture2D[3];
    public Camera[] cameras = new Camera[3]; // Asigna tus cámaras en el inspector
    private float lastSendTime; // Para rastrear el tiempo de envío
    private int currentIndex = 0;
    private Queue<byte[]> imageQueue = new Queue<byte[]>();
    Texture2D[] screenShots = new Texture2D[2];
    private int i;
    private string angle;
    private TcpClient client; // Ajusta la dirección IP y el puerto
    private NetworkStream stream;
    public float captureinterval;
    public float clearinter;
    private int bytes;
    public string message;
    public bool binary;
    private float nextContTime;
    private int contant;
    private int cont;
    public rest reseting;
    void OnEnable()
    {
        GameObject objWithMovimientoAutonomo = GameObject.FindGameObjectWithTag("MovimientoAutonomo");
        mov_auto = objWithMovimientoAutonomo.GetComponent<Movimiento_autonomo>();
        cameras[0] = GameObject.FindWithTag("stereo1").GetComponent<Camera>();
        cameras[1] = GameObject.FindWithTag("stereo2").GetComponent<Camera>();
        cameras[2] = GameObject.FindWithTag("camprin").GetComponent<Camera>();
        client = new TcpClient("localhost", 12345);
        stream = client.GetStream();
        lastSendTime = Time.time; 
        contant =  cont;
        nextContTime = Time.time + 15f;
        clearinter = Time.time +100;
        binary = false;
    }

    void SetupTCP(byte[] data)
    {
        byte[] dataWithMarker = new byte[data.Length + 12]; // 12 bytes para "END_OF_IMAGE"
        data.CopyTo(dataWithMarker, 0);
        byte[] marker = System.Text.Encoding.ASCII.GetBytes("END_OF_IMAGE");
        marker.CopyTo(dataWithMarker, data.Length);
        stream.Write(dataWithMarker, 0, dataWithMarker.Length);
        if(i==3)
        {
            data = new byte[2048];
            bytes = stream.Read(data, 0, data.Length);
            message = Encoding.ASCII.GetString(data, 0, bytes);
            mov_auto.GirarHaciaAnguloAutonoma(float.Parse(message));
            data = null;
            i=0;
        }
        cont++;
    }
    public void ResetDepthClient()
    {
        // Cierra la conexión TCP si está abierta
        if (client != null && client.Connected)
        {
            stream.Close();
            client.Close();
        }

        // Reinicia las variables y objetos necesarios
        lastSendTime = 0;
        currentIndex = 0;
        imageQueue.Clear();
        i = 0;
        angle = "";
        message = "";
        nextContTime = 0;
        contant = 0;
        cont = 0;
        
        // Reinicia las texturas y RenderTextures si es necesario
        for (int j = 0; j < renderTextures.Length; j++)
        {
            if (renderTextures[j] != null)
                Destroy(renderTextures[j]);
            renderTextures[j] = new RenderTexture(480, 270, 24);
            textures[j] = new Texture2D(480, 270, TextureFormat.RGB24, false);
            cameras[j].targetTexture = renderTextures[j];
        }

        // También puedes agregar cualquier otro reinicio necesario aquí

        // Llama a la función OnEnable para restablecer la conexión TCP
        OnEnable();
    }


    void Update()
    {
        if(Time.time -lastSendTime >=captureinterval)
        {
            lastSendTime = Time.time;
            SendMessage();

        }
        if(binary)
        {
            stream.Close();
            client.Close();
        }
    }

    void SendMessage()
    {
        if(i<=2)
        {
            RenderTexture rt = new RenderTexture(192,108, 24);
            Texture2D screenShot = new Texture2D(192,108, TextureFormat.RGB24, false);
            cameras[i].targetTexture = rt;
            cameras[i].Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, 192,108), 0, 0);
            byte[] data = screenShot.EncodeToPNG();
            imageQueue.Enqueue(data);
            cameras[i].targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            i++;
            
        }
        // Comprueba si hay imágenes en la cola y envía una a la vez
        if (imageQueue.Count > 0)
        {
            byte[] imageData = imageQueue.Dequeue();
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback((state) =>
            {
                SetupTCP(imageData);
            }));
        }
        
    }
}