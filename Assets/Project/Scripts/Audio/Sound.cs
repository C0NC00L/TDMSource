using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0, 1f)]
    public float volume;
    [Range(0, 1f)]
    public float pitch;
    [Range(0, 1f)]
    public float spatialSound;
    public bool loop;
    [HideInInspector]
    public AudioSource source;
}
