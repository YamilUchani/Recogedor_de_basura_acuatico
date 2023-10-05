using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class contador : MonoBehaviour
{
    public TMP_Text textoContador;
    public int conta = 0;
    public string texto;

    // Asegúrate de que solo se llame a count cuando sea necesario
    public void count(int conts)
    {
        // Comprobar si textoContador es nulo antes de actualizarlo
        if (textoContador != null)
        {
            texto = "Picked duckweed: " + conts.ToString();
            textoContador.text = texto;
        }
        else
        {
            Debug.LogError("textoContador es nulo. Asegúrate de asignarlo en el Inspector.");
        }
    }
}