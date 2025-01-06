using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [Header("사운드 믹서")]
    public AudioMixer mixer;

    [Header ("배경음 리스트")]
    public AudioClip[] bglist;

    [Header ("배경 사운드 플레이어")]
    public AudioSource bgSound;

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene arg0,LoadSceneMode arg1)
    {
        for(int i=0; i < bglist.Length; i++)
        {
            if (arg0.name == bglist[i].name)
                BgSoundPlay(bglist[i]);
        }
    }

    #region 볼륨 조절
    public void BGSoundVolume(float val)
    {
        mixer.SetFloat("BGVolume", Mathf.Log10(val) * 20); // 믹스의 볼륨은 로그 스케일을 사용하므로 로그로 변환
    }

    public void SFXVolume(float val)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(val) * 20); // 믹스의 볼륨은 로그 스케일을 사용하므로 로그로 변환
    }
    #endregion

    #region 배경
    public void BgSoundPlay(AudioClip clip)
    {
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.Play();
    }
    #endregion

    #region 효과음
    
    //UI 효과음
    public void SFXPlay(AudioClip clip)
    {
        GameObject go = new GameObject("UISound");
        AudioSource audiosource = go.AddComponent<AudioSource>();
        audiosource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audiosource.clip = clip;
        audiosource.Play();

        Destroy(go, clip.length);
    }

    // 그외 효과음 //방향감X
    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audiosource = go.AddComponent<AudioSource>();
        audiosource.outputAudioMixerGroup = mixer.FindMatchingGroups("BGSound")[0];
        audiosource.clip = clip;
        audiosource.Play();

        Destroy(go, clip.length);
    }

    // 그외 효과음 //방향감O
    public void SFXPlay3D(GameObject startPosition,string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");
        go.transform.position = startPosition.transform.position;
        AudioSource audiosource = go.AddComponent<AudioSource>();
        audiosource.clip = clip;
        audiosource.Play();

        Destroy(go, clip.length);
    }
    #endregion
}
