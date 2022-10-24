using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
//Esta clase del namespace Net se encarga de el envio y recibo de datos
using System.Net.Sockets;
//Esta clase se encarga de la creacion de subprocesos, los cuales permitiran el envio y recibo de datos en mas de un subproceso.
using System.Threading;
using System.Text;
public class Movimiento : MonoBehaviour
{
    Thread mThread;
    public string connectionIP= "127.0.0.3";
    public int connectionPort=2100;
    IPAddress localadd;
    TcpClient cleint;
    TcpListener listener;
    public GameObject cam;
    public float aceleracion=500f;
    public float aceleracionangular=5f;
    public float aceleracionactual=0f;
    public float aceleracionangularactual=0f;
    private float tiemporespuesta;
    public float giro=0f;
    public Rigidbody rb;
    public int movHorizontal=0;
    public int movVertical=0;
    private bool running;
    private void Start()
    {
        rb=GetComponent<Rigidbody>();   
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread=new Thread(ts);
        mThread.Start(); 
    }
    void GetInfo()
    {
        localadd= IPAddress.Parse(connectionIP);
        listener= new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        cleint=listener.AcceptTcpClient();
        running=true;
        while(running)
        {
            recibodedatos();
        }
        listener.Stop();
    }
    private void recibodedatos()
    {
        NetworkStream nwStream=cleint.GetStream();
        byte[] buffer=new byte[cleint.ReceiveBufferSize];
        int bytesRead=nwStream.Read(buffer,0,cleint.ReceiveBufferSize);
        string Dato=Encoding.UTF8.GetString(buffer,0,bytesRead);
        if(Dato!=null)
        {
            switch (Dato)
            {
                case "derecha":
                    movHorizontal=1;
                    break;
                case "izquierda":
                     movHorizontal=-1;
                    break;
                case "arriba":
                    movVertical=1;
                    break;
                case "abajo":
                     movVertical=-1;
                    break;    
            }
        }
    }
    private void FixedUpdate()
    {
        
        aceleracionactual=aceleracion*movVertical;
        aceleracionangularactual=aceleracionangular*movHorizontal;
        rb.AddForce((transform.position-cam.transform.position)*aceleracionactual);
        rb.AddTorque(Vector3.up*aceleracionangularactual*Time.deltaTime);
        movHorizontal=0;
        movVertical=0;
    }   
        
}
