using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public GameObject cam;
    public float aceleracion=500f;
    public float aceleracionangular=5f;
    public float aceleracionactual=0f;
    public float aceleracionangularactual=0f;
    public float giro=0f;
    public Rigidbody rb;
    private void Start()
    {
        rb=GetComponent<Rigidbody>();   
    }
    private void FixedUpdate()
    {
        aceleracionactual=aceleracion*Input.GetAxis("Vertical");
        aceleracionangularactual=aceleracionangular*Input.GetAxis("Horizontal");
        rb.AddForce((transform.position-cam.transform.position)*aceleracionactual);
        rb.AddTorque(Vector3.up*aceleracionangularactual*Time.deltaTime);
    }
}
