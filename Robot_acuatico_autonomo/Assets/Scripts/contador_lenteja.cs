using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class contador_lenteja : MonoBehaviour
{
    public gameagain game;
    public int conta;
    public bool finishtime;
    public Text textoRestante;
    public Text textoContador;
    public bool cnt;
    public string texto;
    public int duckweed=0;
    string basePath;
    string momentumFolder;
    public initserver inits;
    public float intervalconnnect;
    public Movimiento_autonomo movauto;
    public initserver intser;
    /* Este script está dentro del sistema de partículas que genera las lentejas de agua.
       Solo se activa cuando hay una colisión con una de las partículas.
       Al detectarlo, se manda una orden al script del bote, que aumenta en 1
       el contador de las lentejas destruidas.
    */
    public ParticleSystem sistemaDeParticulas;  // Asigna el sistema de partículas en el Inspector
    public int cantidadDeParticulas;
    private void Start() {
        basePath = Application.dataPath + "/../server/";
        // Directorio de la carpeta "momentum"
        momentumFolder = basePath + "momentum/";
    }
    private void Update()
    {
        sistemaDeParticulas = GetComponent<ParticleSystem>();
        cantidadDeParticulas = sistemaDeParticulas.particleCount;
        texto = "Existing duckweed: " + cantidadDeParticulas.ToString();
        textoRestante.text = texto;
        texto = "Picked duckweed: " +  (sistemaDeParticulas.main.maxParticles-cantidadDeParticulas).ToString();
        textoContador.text = texto;
        if(finishtime)
        {
            string rutaArchivo = momentumFolder + "/lenteja"+conta.ToString()+".txt";
            File.WriteAllText(rutaArchivo, texto);
            Debug.Log("Valor guardado en el archivo: " + texto);
            finishtime=false;
            intser.servercomplete = false;
            game.Again();
            conta++;
        }
        else if(cnt)
        {
            string rutaArchivo = momentumFolder + "/lenteja"+conta.ToString()+".txt";
            File.WriteAllText(rutaArchivo, texto);
            Debug.Log("Valor guardado en el archivo: " + texto);
            cnt=false;
            intser.servercomplete = false;
            game.Again();
            conta++;
        }
    }
}
