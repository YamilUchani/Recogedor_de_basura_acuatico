using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flotacion : MonoBehaviour
{
    public GameObject Objeto;
    public GameObject agua;
    public float nivelagua=0.0f;
    public float umbralagua=2.0f;
    public float densidadagua=0.125f;
    public float fuerzaabajo=4.0f;

    float fuerzafactor;
    Vector3 fuerzafloat;
    private void Start()
    {
        nivelagua=agua.transform.position.y;
    }
    private void FixedUpdate()
    {
        fuerzafactor=1.0f-((transform.position.y-nivelagua)/umbralagua);
        if(fuerzafactor>0.0f)
        {
            fuerzafloat=-Physics.gravity*(fuerzafactor-Objeto.GetComponent<Rigidbody>().velocity.y*densidadagua);
            fuerzafloat+=new Vector3(0.0f,-fuerzaabajo,0.0f);
            Objeto.GetComponent<Rigidbody>().AddForceAtPosition(fuerzafloat,transform.position);
        }
    }
}
