using System.Diagnostics;
using UnityEngine;

public class initserver : MonoBehaviour 
{
    public GameObject prefab;
    public GameObject instancia;
    public gameagain game;
    public bool servercomplete;
    public float intcontac;
    public bool instaserver;
    public bool startenabled;
    public Process serverProcess;
    private void OnEnable() 
    {
        serverProcess = Process.Start("cmd.exe", "/K cd /D E:\\Repositorios\\Simulation_aquatic\\Recogedor_de_basura_acuatico\\Robot_acuatico_autonomo\\server && python .\\depthserver.py");
        servercomplete = true;
        intcontac = Time.time +10f;

    }
    private void Update() {
        if(servercomplete)
        {
            if(Time.time>=intcontac)
            {
                if(!instaserver)
                {
                    
                    instancia = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
                    instaserver = true;
                    game.Again();
                    startenabled=true;
                }
                
            }
        }
        else
        {
            Destroy(instancia);
            intcontac = Time.time +240f;
            servercomplete = true;
            instaserver = false;
        }

    }
}