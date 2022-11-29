using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FadeSealAudio : MonoBehaviour
{
    public float volumeDecrease = 0.05f;
    float volumeMin = 0.0f;
    bool lowerVolume = false;

    Seal sealReference;
    // Update is called once per frame
    void Update()
    {
        if(lowerVolume == true)
        {
            sealReference.sealSurroundSource.volume = Mathf.Lerp(sealReference.sealSurroundSource.volume, volumeMin, volumeDecrease);
            if(sealReference.sealSurroundSource.volume <= 0.1f)
            {
                sealReference.sealSurroundSource.volume = volumeMin;
                sealReference.sealSurroundSource.Stop();
                lowerVolume = false;
            }
        }
    }

    public void DecreaseVolume(GameObject seal)
    {
        sealReference = seal.GetComponent<Seal>();
        sealReference.sealSurroundSource.volume = Mathf.Clamp(sealReference.sealSurroundSource.volume, volumeMin, sealReference.sealSurroundSource.volume);
        lowerVolume = true;
    }

}
