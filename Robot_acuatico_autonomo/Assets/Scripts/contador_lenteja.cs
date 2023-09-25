using UnityEngine;

public class contador_lenteja : MonoBehaviour
{
    /* Este script está dentro del sistema de partículas que genera las lentejas de agua.
       Solo se activa cuando hay una colisión con una de las partículas.
       Al detectarlo, se manda una orden al script del bote, que aumenta en 1
       el contador de las lentejas destruidas.
    */

    private void OnParticleCollision(GameObject jugador)
    {
        Debug.Log("contacto");

        if (jugador.CompareTag("bote"))
        {
            jugador.GetComponent<contador>().conta += 1;
        }
    }
}
