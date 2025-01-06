using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundController : MonoBehaviour
{
    public void SFXPlay(AudioClip clip)
    {
        SoundManager.instance.SFXPlay(clip);
    }
}
