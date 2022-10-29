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
    public int connectionPort=2100;
    IPAddress localadd;
    TcpClient cleint;
    TcpListener listener;
    ThreadStart ts;
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
    private int bytesRead;
    private byte[] mensaje;
    private byte[] buffer;
    private bool activo=true;
    private void Start()
    {
        rb=GetComponent<Rigidbody>();   
        ts = new ThreadStart(GetInfo);
        mThread=new Thread(ts);
        mThread.Start(); 
        activo=true;
    }
    void GetInfo()
    {
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
        try
        {
            NetworkStream nwStream=cleint.GetStream();
            nwStream.Write(mensaje,0,mensaje.Length); 
            buffer=new byte[cleint.ReceiveBufferSize];
            bytesRead=nwStream.Read(buffer,0,cleint.ReceiveBufferSize);
        }
        catch
        {
            Debug.Log("El control de movimiento se ha desconectado");
            running=false;
            activo=false;
        }
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
        mensaje=Encoding.ASCII.GetBytes("Confirmacion"); 
        aceleracionactual=aceleracion*movVertical;
        aceleracionangularactual=aceleracionangular*movHorizontal;
        rb.AddForce((transform.position-cam.transform.position)*aceleracionactual);
        rb.AddTorque(Vector3.up*aceleracionangularactual*Time.deltaTime);
        movHorizontal=0;
        movVertical=0;
        if(!activo)
        {
            mThread.Abort();
            ts = new ThreadStart(GetInfo);
            mThread=new Thread(ts);
            mThread.Start();
            activo=true;
        }
    }          
}