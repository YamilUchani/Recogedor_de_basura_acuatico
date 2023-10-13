using UnityEngine;
using TMPro;
public class contador_lenteja : MonoBehaviour
{
    public TMP_Text textoRestante;
    public TMP_Text textoContador;
    public string texto;
    public int duckweed=0;
    /* Este script está dentro del sistema de partículas que genera las lentejas de agua.
       Solo se activa cuando hay una colisión con una de las partículas.
       Al detectarlo, se manda una orden al script del bote, que aumenta en 1
       el contador de las lentejas destruidas.
    */
    public ParticleSystem sistemaDeParticulas;  // Asigna el sistema de partículas en el Inspector
    public int cantidadDeParticulas;
    private void Update()
    {
        sistemaDeParticulas = GetComponent<ParticleSystem>();
        cantidadDeParticulas = sistemaDeParticulas.particleCount;
        texto = "Existing duckweed: " + cantidadDeParticulas.ToString();
        textoRestante.text = texto;
        texto = "Picked duckweed: " +  (sistemaDeParticulas.main.maxParticles-cantidadDeParticulas).ToString();
        textoContador.text = texto;
        
    }
}
