using System.Diagnostics;
using UnityEngine;

public class initserver : MonoBehaviour
{
    private Process cmdProcess;
    public GameObject dpclient; // Asigna el script "depthclient" desde el editor
    public depthclient dpc;
    public bool act;
    private float startTime;

    private void OnEnable()
    {
        dpclient.SetActive(false);
        StartServerProcess();
        startTime = Time.time;
        
        
    }

    private void OnDisable()
    {
        StopServerProcess();
    }

    public void StartServerProcess()
    {
        if (cmdProcess != null && !cmdProcess.HasExited)
        {
            cmdProcess.Kill();
            cmdProcess = null;
        }

        string pythonFilePath = @"E:\Repositorios\Simulation_aquatic\Recogedor_de_basura_acuatico\Robot_acuatico_autonomo\server\depthserver.py";
        cmdProcess = new Process();
        cmdProcess.StartInfo.FileName = "python";
        cmdProcess.StartInfo.Arguments = pythonFilePath;
        cmdProcess.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(pythonFilePath);
        cmdProcess.Start();
        startTime = Time.time;
        // Registra el tiempo de inicio
        
    }

    public void StopServerProcess()
    { 
        dpc.enabled =false;
        dpclient.SetActive(false);
        if (cmdProcess != null && !cmdProcess.HasExited)
        {
            cmdProcess.Kill();
            cmdProcess = null;
        }

        // Deshabilita el script "depthclient"
        
        act=true;
        

    }

    private void Update()
    {
        if (Time.time - startTime >= 10 && !dpclient.activeSelf)
        {
            // Habilita el script "depthclient" después de 10 segundos
            dpc.enabled =true;
            dpclient.SetActive(true);
            
            dpc.ResetDepthClient();
            startTime = 10 + Time.time;

        }
    }
}
