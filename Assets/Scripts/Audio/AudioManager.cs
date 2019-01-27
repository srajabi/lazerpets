using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource LoadingScreenScore;

    // Start is called before the first frame update
    void Start()
    {
        LoadingScreenScore = GetComponent<AudioSource>();
        LoadingScreenScore.rolloffMode = AudioRolloffMode.Logarithmic;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator FadeOut()
    {
        float startVolume = LoadingScreenScore.volume;

        while (LoadingScreenScore.volume > 0)
        {
            LoadingScreenScore.volume -= startVolume * Time.deltaTime / 4;

            yield return null;
        }

        LoadingScreenScore.Stop();
        LoadingScreenScore.volume = startVolume;
    }
}
