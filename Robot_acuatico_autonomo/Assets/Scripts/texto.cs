using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class texto : MonoBehaviour
{
    bool contador=false;
    Text textfield;
    int number;
    void Start()
    {
        textfield=GetComponent<Text>();
        textfield.text="Grabar";
    }

    // Update is called once per frame
    public void changetext()
    {
        if(contador==true)
        {
            contador=false;
            textfield.text="Grabar";
        }
        else
        {
            contador=true;
            textfield.text="Parar";
        }
    }
}
