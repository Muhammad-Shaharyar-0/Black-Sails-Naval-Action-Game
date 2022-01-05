using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;

    AudioSource Music;
    [SerializeField] GameObject [] Sounds;
    public static SoundManager Instance
    {
        get
        {
            if (SoundManager.instance == null)
            {
                SoundManager.instance = FindObjectOfType<SoundManager>();
            }
            return SoundManager.instance;
        }
    }

    private void Start()
    {
        Music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        Sounds = GameObject.FindGameObjectsWithTag("Sound");
        Music.volume = PlayerPrefs.GetFloat("MusicVolume");
        for (int i = 0; i < Sounds.Length; i++)
        {
            Sounds[i].GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MasterVolume"); ;
        }



    }
    public void changeMusicVolume(float changevalue)
    {

        Music.volume += changevalue;
        PlayerPrefs.SetFloat("MusicVolume", Music.volume);
    }

    public void changeSoundVolume(float changevalue)
    {
        float volume=0.333f;
        for(int i=0;i<Sounds.Length;i++)
        {
            AudioSource sound = Sounds[i].GetComponent<AudioSource>();
            sound.volume+= changevalue;
            volume = sound.volume;
        }
        PlayerPrefs.SetFloat("MasterVolume", volume);
    

    }

    public void SetSoundVolume(float v)
    {
        float volume = 0.333f;
        for (int i = 0; i < Sounds.Length; i++)
        {
            AudioSource sound = Sounds[i].GetComponent<AudioSource>();
            sound.volume = v;
        }
        PlayerPrefs.SetFloat("MasterVolume", volume);

    }

    public void SetMusicVolume(float v)
    {
        Music.volume = v;
        PlayerPrefs.SetFloat("MusicVolume", Music.volume);
    }

}
