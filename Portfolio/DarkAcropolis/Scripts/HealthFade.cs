using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthFade : MonoBehaviour
{
    public Animator animtor;

    public void PlayerHit()
    {
        FadeInBlood();
    }

    public void FadeInBlood()
    {
        animtor.SetTrigger("FadeIn");
    }

    public void FadeOutBlood()
    {
        animtor.SetTrigger("FadeOut");
    }
}