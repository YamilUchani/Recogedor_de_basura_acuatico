using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class Capture_image : MonoBehaviour
{
    public Camera camara;
    private string carpetaAssets = "Assets/Images";
    private string carpetaFecha;

    private void Awake()
    {
        // Obtener la fecha actual en el formato deseado (año_mes_día)
        string fechaActual = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");

        // Crear la ruta completa para la carpeta de la fecha
        carpetaFecha = Path.Combine(carpetaAssets, fechaActual);

        // Verificar si la carpeta de la fecha no existe y crearla si es necesario
        if (!Directory.Exists(carpetaFecha))
        {
            Directory.CreateDirectory(carpetaFecha);
        }
    }

    public void CapturarYGuardar()
    {
        // Obtener la hora actual en el formato deseado (hora_minuto_segundo)
        string horaActual = DateTime.Now.ToString("HH_mm_ss");

        // Crear el nombre del archivo con la hora actual y la extensión ".png"
        string nombreArchivo = horaActual + ".png";

        // Capturar la imagen de la cámara y guardarla en la carpeta de la fecha
        string rutaCompleta = Path.Combine(carpetaFecha, nombreArchivo);
        ScreenCapture.CaptureScreenshot(rutaCompleta);
    }
}
