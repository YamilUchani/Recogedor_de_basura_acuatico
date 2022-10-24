using System.Collections;
using UnityEngine;
/*
Estas librerias son las utilizadas para la comunicacion entre Unity y Python. 
Tomando en cuenta el puerto y la IP asignada, la cual tiene que ser la misma en ambos archivos
*/
//Este namespace se encarga de habilitar los protocolos para la red
using System.Net;
//Esta clase del namespace Net se encarga de el envio y recibo de datos
using System.Net.Sockets;
//Esta clase se encarga de la creacion de subprocesos, los cuales permitiran el envio y recibo de datos en mas de un subproceso.
using System.Threading;
using System.Text;
public class Imagenes_detector : MonoBehaviour
{
        /*
    Este script se trata de un script que mandara una imagen como informacion 
    */
    //Esta variable es el subproceso en el cual se realizara el envio y recibo de datos
    Thread mThread;
    public string connectionIP= "127.0.0.1";
    public int connectionPort=2530;
    IPAddress localdd;
    TcpClient client;
    TcpListener listener;
    public bool envio;
    public Camera Detector;
    Texture2D image;
    byte[] bytes;

    
    private void Start()
    {
        //Inicio de creacion de un Detector para comunicacion para las dos camaras
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread=new Thread(ts);
        mThread.Start();   
    }
    void GetInfo()
    {
        localdd= IPAddress.Parse(connectionIP);
        listener= new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();        
        
        client=listener.AcceptTcpClient();
        envio=true;
        while(envio)
        {
            Enviodedatos();
        }
        listener.Stop();
    }
    void Enviodedatos()
    {   
        NetworkStream nwStream= client.GetStream();
        byte[] buffer=new byte[client.ReceiveBufferSize];
        //Confirmacion del cliente
        int confirmacion=nwStream.Read(buffer,0,client.ReceiveBufferSize);
        string dataconfirmacion=Encoding.UTF8.GetString(buffer,0,confirmacion);
        nwStream.Write(bytes,0,bytes.Length); 
    }  
    public void FixedUpdate()
    {
        //Inicio de conversion de imagen de camara encargada de la deteccion
        RenderTexture activeRenderTextureTwo = RenderTexture.active;
        RenderTexture.active = Detector.targetTexture;
        Detector.Render();
        image = new Texture2D(Detector.targetTexture.width, Detector.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Detector.targetTexture.width, Detector.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTextureTwo;
        bytes = image.EncodeToPNG();
        Destroy(image);
    }
    //Las funciones encargadas de enviar los datos de puerto e IP para la comunicacion de los dos Detectors Thread
   
}
