using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using System;
public class time : MonoBehaviour
{
    public contador_lenteja cont;
    public int conta;
    public int conta1;
    public int conta2;
    public int conta3;
    public int conta4;
    public Text textoCronometro;
    public float tiempoTranscurrido = 0f;
    string basePath;
    string momentumFolder;
    public Movimiento_autonomo movauto;
    
    private void Start() {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Ruta a la carpeta "Documentos"
        string recyclingRushPath = System.IO.Path.Combine(documentsPath, "!Recycling Rush"); // Ruta a la carpeta "!Recycling Rush"
        basePath = System.IO.Path.Combine(recyclingRushPath, "servers"); // Ruta a la carpeta "servers"
        Time.timeScale = 1.0f;
    }
    public void aceleration (float acel)
    {
        Time.timeScale += acel ;
    }

    void Update()
    {
            switch (movauto.elec) 
            {
                case 1:
                    momentumFolder = System.IO.Path.Combine (basePath, "Modality1");
                    break;
                case 2:
                    momentumFolder = System.IO.Path.Combine (basePath, "Modality2");
                    break;
                case 3:
                    momentumFolder = System.IO.Path.Combine (basePath, "Modality3");
                    break;
                case 4:
                    momentumFolder = System.IO.Path.Combine (basePath, "Modality4");
                    break;
                default:
                    Debug.Log("Modo Manual");
                    break;
            }
            // Actualiza el tiempo transcurrido según la escala de tiempo actual.
            tiempoTranscurrido += Time.deltaTime * Time.timeScale;

            // Convierte el tiempo transcurrido a formato de tiempo (hh:mm:ss).
            int horas = Mathf.FloorToInt(tiempoTranscurrido / 3600);
            int minutos = Mathf.FloorToInt((tiempoTranscurrido % 3600) / 60);
            int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60);

            // Muestra el tiempo en formato de tiempo en el TextMeshPro.
            string texto = string.Format("Time: {0:D2}:{1:D2}:{2:D2}\nVelocity: {3:F2}x", horas, minutos, segundos, Time.timeScale);
            textoCronometro.text = texto;
            if(minutos ==20 && segundos>0)
            {
                switch (movauto.elec) 
                {
                    case 1:
                        conta=conta1;
                        break;
                    case 2:
                        conta=conta2;
                        break;
                    case 3:
                        conta=conta3;
                        break;
                    case 4:
                        conta=conta4;
                        break;
                    default:
                        Debug.Log("Modo Manual");
                        break;
                }
                string rutaArchivo =  momentumFolder+ "/time"+conta.ToString()+".txt";
                File.WriteAllText(rutaArchivo, texto);
                Debug.Log("Valor guardado en el archivo: " + texto);
                cont.finishtime=true;
                switch (movauto.elec) 
                {
                    case 1:
                        conta1++;
                        break;
                    case 2:
                        conta2++;
                        break;
                    case 3:
                        conta3++;
                        break;
                    case 4:
                        conta4++;
                        break;
                    default:
                        Debug.Log("Modo Manual");
                        break;
                }
                

            }
            if(cont.cantidadDeParticulas<30 && minutos >2)
            {
                switch (movauto.elec) 
                {
                    case 1:
                        conta=conta1;
                        break;
                    case 2:
                        conta=conta2;
                        break;
                    case 3:
                        conta=conta3;
                        break;
                    case 4:
                        conta=conta4;
                        break;
                    default:
                        Debug.Log("Modo Manual");
                        break;
                }
                string rutaArchivo =  momentumFolder + "/time"+conta.ToString()+".txt";
                File.WriteAllText(rutaArchivo, texto);
                Debug.Log("Valor guardado en el archivo: " + texto);
                cont.cnt=true;
                switch (movauto.elec) 
                {
                    case 1:
                        conta1++;
                        break;
                    case 2:
                        conta2++;
                        break;
                    case 3:
                        conta3++;
                        break;
                    case 4:
                        conta4++;
                        break;
                    default:
                        Debug.Log("Modo Manual");
                        break;
                }
                
            }

        
    }
}
