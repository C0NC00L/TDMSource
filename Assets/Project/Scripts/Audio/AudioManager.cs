using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialSound;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, tempName => tempName.name == name);
        if(s != null)
            s.source.Play();
    }
}
