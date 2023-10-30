using System.Diagnostics;
using UnityEngine;

public class initserver : MonoBehaviour 
{
    public GameObject prefab;
    public GameObject instancia;
    public gameagain game;
    public bool servercomplete;
    public float intcontac;
    public float clearconsole;
    public bool instaserver;
    public bool lagserver;
    public bool initialserver;
    public effnetclient script;
    private void OnEnable() 
    {
        Process.Start("cmd.exe", "/K cd /D E:\\Repositorios\\Simulation_aquatic\\Recogedor_de_basura_acuatico\\Robot_acuatico_autonomo\\server && python .\\effnetserver.py");
        servercomplete = true;
        intcontac = Time.time +10f;
        clearconsole = Time.time + 25f;
        
    }
    private void Update() {
        if(servercomplete || !lagserver)
        {
            
            if(Time.time>=intcontac)
            {
                if(!instaserver)
                {
                    UnityEngine.Debug.Log("Hola pew");
                    instancia = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
                    script = instancia.GetComponent<effnetclient>();
                    instaserver = true;
                     initialserver = true;
                    game.Again();
                }
                
            }
        }
        else
        {
            Destroy(instancia);
            intcontac = Time.time +35f;
            servercomplete = true;
            lagserver = false;
             initialserver = false;
            instaserver = false;
        }
        if( initialserver)
        {
            if(Time.time >= clearconsole)
            {
                if(script.contant <= script.cont )
                {
                    script.contant = script.cont;
                    UnityEngine.Debug.Log("Cliente conectado");
                }
                else
                {
                    UnityEngine.Debug.Log("Servidor Caido Reconectando");
                    lagserver = true;
                    Clear();
                }
                clearconsole = Time.time +25f;
            }
        }
    }
    public static void Clear()
    {
        // Limpia la consola usando el comando "ClearLog"
        System.Type type = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        System.Reflection.MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

}
