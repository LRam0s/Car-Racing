using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeLigth : MonoBehaviour
{
    public Material brakeMaterial;
    public Color brakingColor;
    public float brakeColorIntense;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BrakeLigthOn(float brakeInput)
    {
        if (brakeMaterial)
        {

            if (brakeInput > 0)
            {
                brakeMaterial.EnableKeyword("_EMISSION");
                brakeMaterial.SetColor("_EmissionColor", brakingColor);
            }
            else
            {
                brakeMaterial.DisableKeyword("_EMISSION");
                brakeMaterial.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}
