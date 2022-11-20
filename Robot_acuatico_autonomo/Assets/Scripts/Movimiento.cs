using UnityEngine;
using System.Net;
using System.Text;
public class Movimiento : MonoBehaviour
{
    public GameObject cam;
    public float aceleracion=500f;
    public float aceleracionangular=5f;
    public float aceleracionactual=0f;
    public float aceleracionangularactual=0f;
    public Rigidbody rb;
    public float movHorizontal=0;
    public float movVertical=0;

    private void Start()
    {
        rb=GetComponent<Rigidbody>();   
    }
    private void FixedUpdate()
    {
        movVertical = Input.GetAxis("Vertical");
        movHorizontal = Input.GetAxis("Horizontal");
        aceleracionactual=aceleracion*movVertical;
        aceleracionangularactual=aceleracionangular*movHorizontal;
        rb.AddForce((transform.position-cam.transform.position)*aceleracionactual);
        rb.AddTorque(Vector3.up*aceleracionangularactual*Time.deltaTime);
    }          
}