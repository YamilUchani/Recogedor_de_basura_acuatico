using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
public class time : MonoBehaviour
{
    public contador_lenteja cont;
    public int conta;
    public Text textoCronometro;
    public float tiempoTranscurrido = 0f;
    string basePath;
    string momentumFolder;
    
    private void Start() {
        basePath = Application.dataPath + "/../server/";

        // Directorio de la carpeta "momentum"
        momentumFolder = basePath + "momentum/";
        Time.timeScale = 1.0f;
    }
    public void aceleration (float acel)
    {
        Time.timeScale += acel ;
    }

    void Update()
    {

            // Actualiza el tiempo transcurrido según la escala de tiempo actual.
            tiempoTranscurrido += Time.deltaTime * Time.timeScale;

            // Convierte el tiempo transcurrido a formato de tiempo (hh:mm:ss).
            int horas = Mathf.FloorToInt(tiempoTranscurrido / 3600);
            int minutos = Mathf.FloorToInt((tiempoTranscurrido % 3600) / 60);
            int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60);

            // Muestra el tiempo en formato de tiempo en el TextMeshPro.
            string texto = string.Format("Time: {0:D2}:{1:D2}:{2:D2}\nVelocity: {3:F2}x", horas, minutos, segundos, Time.timeScale);
            textoCronometro.text = texto;
            if(minutos ==2 && segundos>0)
            {
                string rutaArchivo =  momentumFolder+ "/time"+conta.ToString()+".txt";
                File.WriteAllText(rutaArchivo, texto);
                Debug.Log("Valor guardado en el archivo: " + texto);
                cont.finishtime=true;
                conta++;
                

            }
            if(cont.cantidadDeParticulas<30 && minutos >2)
            {
                string rutaArchivo =  momentumFolder + "/time"+conta.ToString()+".txt";
                File.WriteAllText(rutaArchivo, texto);
                Debug.Log("Valor guardado en el archivo: " + texto);
                cont.cnt=true;
                conta++;
                
            }

        
    }
}
