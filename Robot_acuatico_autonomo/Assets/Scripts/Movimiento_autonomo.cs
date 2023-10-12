using UnityEngine;
using System.Threading.Tasks;
using System;
public class Movimiento_autonomo : MonoBehaviour
{
    private Rigidbody rb;
    private bool detenido;
    public float anguloObjetivo;
    private bool girando;
    private bool calculando;
    public int elec;
    public float velocidadMovimiento = 5f;
    public float velocidadAngular = 5f;
    public float rangoAnguloMinimo = 100f;
    public float rangoAnguloMaximo = 180f;
    public float toleranciaAngular = 10f;
    public bool autonomous;
    private float angulodeseado;
    private float angulorango;
    private DateTime tiempoUltimaLlamada = DateTime.MinValue;
    private TimeSpan tiempoEspera = TimeSpan.FromSeconds(5); // Cambia el tiempo de espera según tus necesidades
    private float tiempoEspe = 1.0f; // Cambia este valor a la cantidad de segundos que desee
    private float tiempoUltimaDeteccion = 0.0f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        detenido = false;
        girando = false;
    }

    private void FixedUpdate()
    {
        angulodeseado = transform.eulerAngles.y;
        if (!detenido)
        {
            switch (elec) 
            {
                case 0:
                    MoverEnDireccionControlada();
                    autonomous = false;
                    break;
                case 1:
                    MoverEnDireccionAleatoria();
                    autonomous = false;
                    break;
                case 2:
                    MoverEnDireccionAleatoria();
                    autonomous = true;
                    if (Mathf.Abs(angulodeseado- angulorango) <= toleranciaAngular)
                    {
                        calculando = false;
                    }
                    else
                    {
                        float direccionGiro = Mathf.Sign(angulodeseado- angulorango) * -1;
                        rb.angularVelocity = transform.up * direccionGiro * velocidadAngular;
                    }
                    break;
                default:
                    MoverEnDireccionControlada();
                    break;
            }
        }
        else
        {
            GirarHaciaAnguloAleatorio();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        rb.velocity = transform.forward*-1 * velocidadMovimiento;
        rb.angularVelocity = transform.up * 45 * velocidadAngular;
        if (!detenido)
        {
            if (other.gameObject.CompareTag("limit"))
            {
                // Verificar si ha pasado suficiente tiempo desde la última detección
                if (Time.time - tiempoUltimaDeteccion >= tiempoEspe)
                {
                    detenido = true;
                    tiempoUltimaDeteccion = Time.time;
                }
            }
        }
        else
        {
            // Si el tiempo de espera ha pasado, reactiva la detección
            if (Time.time - tiempoUltimaDeteccion >= tiempoEspe)
            {
                detenido = false;
                tiempoUltimaDeteccion = Time.time;
            }
        }
    }

    private void MoverEnDireccionControlada()
    {
        // Obtener las entradas del eje vertical y horizontal
        float movimientoVertical = Input.GetAxis("Vertical");
        float movimientoHorizontal = Input.GetAxis("Horizontal");

        // Calcular la dirección de movimiento y rotación 
        Vector3 direccionMovimiento = transform.forward * movimientoVertical;
        Vector3 direccionRotacion = transform.up * movimientoHorizontal;

        // Asignar la velocidad de movimiento y rotación al Rigidbody
        rb.velocity = direccionMovimiento * velocidadMovimiento;
        rb.angularVelocity = direccionRotacion * velocidadAngular;
    }
    private void MoverEnDireccionAleatoria()
    {
        rb.velocity = transform.forward * velocidadMovimiento;
    }
    private void GirarHaciaAnguloAleatorio()
    {
        float anguloActual = transform.eulerAngles.y;
        if (!girando)
        {
            anguloObjetivo = UnityEngine.Random.Range(rangoAnguloMinimo, rangoAnguloMaximo);
            anguloObjetivo += anguloActual;
            if(anguloObjetivo>360)
            {
                anguloObjetivo -=360;
            }
            girando = true;
        }

        

        if (Mathf.Abs(anguloActual - anguloObjetivo) <= toleranciaAngular)
        {
            girando = false;
            detenido  = false;
        }
        else
        {
            float direccionGiro = Mathf.Sign(anguloActual) * -1;
            rb.angularVelocity = transform.up * direccionGiro * velocidadAngular;
        }
    }
    public void model_change(int numberlist)
    {
        elec = numberlist;
    }
    public async Task GirarHaciaAnguloAutonoma(int anguloconduccion)
    {
        if (autonomous)
        {
            // Verificar si ha pasado suficiente tiempo desde la última llamada
            DateTime ahora = DateTime.Now;
            if ((ahora - tiempoUltimaLlamada) < tiempoEspera)
            {
                Console.WriteLine("Espera antes de la próxima llamada...");
                return; // Salir de la función sin realizar nada
            }

            // Actualizar el tiempo de la última llamada
            tiempoUltimaLlamada = ahora;

            // Realizar el cálculo si no se está calculando actualmente
            if (!calculando)
            {
                angulorango = anguloconduccion;

                angulorango += angulodeseado;
                if (angulorango > 360)
                {
                    angulorango -= 360;
                }
                calculando = true;

                // Simular trabajo en segundo plano
                await Task.Delay(1000); // Esperar 1 segundo (ajusta según tus necesidades)
                
                calculando = false;
            }
        }
    }

}

