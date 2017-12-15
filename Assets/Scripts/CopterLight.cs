using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopterLight : MonoBehaviour {

    public Light BlueLight;
    public Color currentColor;
    public int Number = 1;
    // Use this for initialization
    void Start()
    {

       // BlueLight.color = currentColor;
        Number = 1;
        BlueLight.intensity = 50f;
    }


    void Update()
    {
        if (Number == 1)
        {
            BlueLight.intensity = 0;
            StartCoroutine(waitforred());
        }
        if (Number == 2)
        {
            BlueLight.intensity = 1.5f;
            StartCoroutine(waitforblue());
        }
    }
    IEnumerator waitforred()
    {
        yield return new WaitForSeconds(0.2f);
        Number = 2;
    }
    IEnumerator waitforblue()
    {
        yield return new WaitForSeconds(0.2f);
        Number = 1;
    }

    public void getColor(Color color)
    {
        BlueLight.color = color;
    }
}
