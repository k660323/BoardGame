using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    AudioSource bgAudio, effectAudio;

    [SerializeField]
    Slider bgSlider, effectSlider;

    [SerializeField]
    AudioClip[] sound;

    // Start is called before the first frame update
    void Start()
    {
        bgAudio = Camera.main.GetComponent<AudioSource>(); // 카메라의 배경 음악
        effectAudio = GameObject.Find("AudioController").GetComponent<AudioSource>(); // 효과음

        if(PlayerInfo.playerInfo != null)
        {
             bgAudio.volume = PlayerInfo.playerInfo.getBgSound();
             effectAudio.volume = PlayerInfo.playerInfo.getEffectSound();

            if (bgSlider != null && effectSlider != null)
            {
                bgSlider.value = PlayerInfo.playerInfo.getBgSound();
                effectSlider.value = PlayerInfo.playerInfo.getEffectSound();
            }
        }
        else
        {
            bgAudio.volume = 0.5f;
            effectAudio.volume = 0.5f;

            if(bgSlider !=null && effectSlider != null)
            {
                bgSlider.value = 0.5f;
                effectSlider.value = 0.5f;
            }
        }
    }

    public void setBgVolume()
    {
        if (PlayerInfo.playerInfo != null) // 플레이어 데이터가 있으면
        {
            PlayerInfo.playerInfo.setBgSound(bgSlider.value);
            bgAudio.volume = PlayerInfo.playerInfo.getBgSound();
        }
        else
        {
            bgAudio.volume = bgSlider.value;
        }
    }

    public void setEffectVolume()
    {
        if (PlayerInfo.playerInfo != null)  // 플레이어 데이터가 있으면
        {
            PlayerInfo.playerInfo.setEffectSound(effectSlider.value);
            effectAudio.volume = PlayerInfo.playerInfo.getEffectSound();
        }
        else
        {
            effectAudio.volume = effectSlider.value;
        }
    }

}
