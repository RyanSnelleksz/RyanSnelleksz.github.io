using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    Material material = null;

    bool charged = false;

    float maxEmission = 100.0f;
    float minEmission = -100.0f;
    float emissionValue = 0.0f;

    float chargeSpeed = 100.0f;

    public Gem()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (charged)
        {
            if (emissionValue < maxEmission)
            {
                emissionValue += chargeSpeed * Time.deltaTime;
            }
            else
            {
                emissionValue = maxEmission;
            }
            material.SetColor("_EmissiveColor", Color.white * emissionValue);
        }
        else if (!charged)
        {
            if (emissionValue > minEmission)
            {
                emissionValue -= chargeSpeed * Time.deltaTime;
            }
            else
            {
                emissionValue = minEmission;
            }
            material.SetColor("_EmissiveColor", Color.white * emissionValue);
        }
    }

    public void SwapState()
    {
        charged = !charged;
    }

    public void SetMaterial(Material mat)
    {
        material = mat;
    }
}
