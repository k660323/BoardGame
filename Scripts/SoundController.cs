using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    // 개발자 편의 코드
    void Awake()
    {
        if (SoundManager.instance == null)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("SoundObject/SoundManager"));
            go.name = "SoundManager";
        }
    }

    // 다른 씬에서 SoundManager를 인스펙터에 버튼을 넣을수 없을 경우 사용

    public void BgSoundPlay(AudioClip clip)
    {
        SoundManager.instance.bgSound.clip = clip;
        SoundManager.instance.bgSound.loop = true;
        SoundManager.instance.bgSound.Play();
    }

    public void SFXPlay(AudioClip clip)
    {
        SoundManager.instance.SFXPlay(clip);
    }

    public void BGSoundVolume(float val)
    {
        SoundManager.instance.mixer.SetFloat("BGVolume", Mathf.Log10(val) * 20); // 믹스의 볼륨은 로그 스케일을 사용하므로 로그로 변환
    }

    public void SFXVolume(float val)
    {
        SoundManager.instance.mixer.SetFloat("SFXVolume", Mathf.Log10(val) * 20); // 믹스의 볼륨은 로그 스케일을 사용하므로 로그로 변환
    }

}
