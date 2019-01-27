using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAudioManager
{
    void PlayProjectileAudio();
}


[RequireComponent(typeof(AudioSource))]
public class CatAudioManager : MonoBehaviour, IPlayerAudioManager
{
    AudioSource AudioSource;
    [SerializeField] AudioClip[] audioClips;

    public void PlayProjectileAudio()
    {
        AudioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.clip = audioClips[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
