using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class time : MonoBehaviour
{
    public TMP_Text textoCronometro;
    public float tiempoTranscurrido = 0f;
    private void Start() {
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
    }
}
