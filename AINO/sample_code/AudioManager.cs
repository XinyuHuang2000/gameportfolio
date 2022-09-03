using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Sound[] sounds;
    Sound currentBgm;
    public static AudioManager instance;
    //Sound footstep;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        foreach(Sound s in sounds)
        {
            //if (s.name.Equals("walk"))
            //{
            //    footstep = s;
            //}
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    public Sound Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Cannot find sound with name: " + name);
            return null;
        }
        s.source.Play();
        return s;
    }

    public void PlayBgm(string name)
    {
        if (currentBgm != null && currentBgm.name.Equals(name)) return;
        StopBgm();
        if (name.Equals("none")) return;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Cannot find sound with name: " + name);
            return;
        }
        s.source.Play();
        currentBgm = s;
    }

    public void StopBgm()
    {
        if (currentBgm != null)
        {
            currentBgm.source.Stop();
            currentBgm = null;
        }
    }

}
