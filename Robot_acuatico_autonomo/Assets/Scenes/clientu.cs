using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
public class clientu : MonoBehaviour
{
    private Texture2D texture;
    private GUIStyle style;
    public Camera cam;
    public float captureInterval = 10.0f;  // Intervalo de captura de 10 segundos
    public int quality = 25;  // Calidad de la imagen, de 0 (peor) a 100 (mejor)
    private int screenWidth;
    private int screenHeight;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] data;
    string message;
    public Movimiento_autonomo mov_auto;
    public Camera Camino;

    //Variables para dibujar los bbox respectivos

    public Color boxColor = Color.red; // Color del cuadro
    public float boxThickness = 2f; // Grosor del cuadro
    private List<Box> boxes = new List<Box>();
    private bool draw;
    private int lineCount = 0;
    
    void Start()
    {
        
        InvokeRepeating("SendMessage", 0.0f, captureInterval);
        texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.red);
        texture.Apply();
        client = new TcpClient("localhost", 12345);
        stream = client.GetStream();
        style = new GUIStyle();
        style.normal.background = texture;
    }

    void SetupTCP(byte[] data)
    {   
        stream.Write(data, 0, data.Length);
        data = new byte[2048];
        int bytes = stream.Read(data, 0, data.Length);
        message = Encoding.ASCII.GetString(data, 0, bytes);
        boxes.Clear();
        draw = false;
        string[] lines = message.Split('\n');
        lineCount = 0;
        int[] rangos = new int[5]; // Un arreglo para llevar un registro de la cantidad de puntos en cada rango
        foreach (string line in lines)
        {
            // Saltar la primera línea
            if (lineCount == 0)
            {
                lineCount++;
                continue;
            }
            try
            {
                string[] values = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                Box box = new Box();
                box.xmin = float.Parse(values[1]);
                box.ymin = float.Parse(values[2]);
                box.xmax = float.Parse(values[3]);
                box.ymax = float.Parse(values[4]);
                float puntoMedioX = (box.xmin + box.xmax) / 2f;
                // Determina en qué rango se encuentra el punto medio
                if (puntoMedioX >= 0 && puntoMedioX <= 384)
                {
                    rangos[0]++;
                }
                else if (puntoMedioX >= 385 && puntoMedioX <= 768)
                {
                    rangos[1]++;
                }
                else if (puntoMedioX >= 769 && puntoMedioX <= 1152)
                {
                    rangos[2]++;
                }
                else if (puntoMedioX >= 1153 && puntoMedioX <= 1536)
                {
                    rangos[3]++;
                }
                else if (puntoMedioX >= 1537 && puntoMedioX <= 1920)
                {
                    rangos[4]++;
                }
                box.confidence = float.Parse(values[5]);
                box.classId = int.Parse(values[6]);
                box.name = values[7];
                boxes.Add(box);

                lineCount++;
            }
            catch
            {
                Debug.Log("No se detectan lentejas");
                continue;
            }
            
        }
        int rangoConMasPuntos = Array.IndexOf(rangos, rangos.Max());
        switch (rangoConMasPuntos)
        {
            case 0:
                mov_auto.GirarHaciaAnguloAutonoma(-30);
                break;
            case 1:
                mov_auto.GirarHaciaAnguloAutonoma(-15);
                break;
            case 2:
                mov_auto.GirarHaciaAnguloAutonoma(0);
                break;
            case 3:
                mov_auto.GirarHaciaAnguloAutonoma(15);
                break;
            case 4:
                mov_auto.GirarHaciaAnguloAutonoma(30);
                break;
            default:
                Debug.Log("Ningún rango tiene puntos medios.");
                break;
        }

        draw=true;
        
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
        
        Thread sendThread = new Thread(() =>
        {
            SetupTCP(bytes);
        });
        sendThread.Start();
       
    }



void OnGUI()
{
    if(draw && Camino.enabled)
    {
        var boxesCopy = new List<Box>(boxes); // Crear una copia de la lista
        foreach (var box in boxesCopy)
        {
            // Dibujar el cuadro
            Rect rect = new Rect(box.xmin, box.ymin, box.xmax - box.xmin, box.ymax - box.ymin);
            DrawRectangle(rect, boxThickness, boxColor);

            GUI.Box(new Rect(box.xmin, box.ymin - 25, 180, 30), GUIContent.none, style);
            // Dibujar el nombre y la confianza
            string label = $"{box.name} {string.Format("{0:0}", box.confidence * 100)}%";
            GUI.backgroundColor = boxColor;
            GUI.skin.label.fontSize = 20; // Cambia esto al tamaño que prefieras
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.Label(new Rect(box.xmin, box.ymin-25 , 180, 60), label); // Ajusta la posición y como prefieras
        }
    }
}

void DrawRectangle(Rect area, float thickness, Color color)
{
    GUI.color = color;
    // Top
    GUI.DrawTexture(new Rect(area.xMin, area.yMin, area.width, thickness), Texture2D.whiteTexture);
    // Left
    GUI.DrawTexture(new Rect(area.xMin, area.yMin, thickness, area.height), Texture2D.whiteTexture);
    // Right
    GUI.DrawTexture(new Rect(area.xMax - thickness, area.yMin, thickness, area.height), Texture2D.whiteTexture);
    // Bottom
    GUI.DrawTexture(new Rect(area.xMin, area.yMax - thickness, area.width, thickness), Texture2D.whiteTexture);

    GUI.color = Color.white;
}
    private void OnDestroy()
    {
        if (client != null)
        {
            stream.Close();
            client.Close();
        }
    }

}


    

    


public class Box
{
    public float xmin;
    public float ymin;
    public float xmax;
    public float ymax;
    public float confidence;
    public int classId;
    public string name;

}









    
