using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource effectSource;
    public AudioSource effectSource2;
    public AudioSource effectSource3;
    public AudioSource bgmSource;
    public AudioSource loopEffectsource;
    public static SoundManager instance = null;

    public float lowPichRange = .95f;
    public float highPitchRange = 1.05f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine("StartBgmVolume");
    }
    IEnumerator StartBgmVolume()
    {
        bgmSource.volume = 0;
        while(bgmSource.volume<1)
        {
            bgmSource.volume += 0.01f;
            yield return new WaitForSeconds(0.05f);
        }
        bgmSource.volume = 1;
        yield return null;
    }
    IEnumerator StopBgmVolume()
    {
        bgmSource.volume = 1;
        while (bgmSource.volume > 0)
        {
            bgmSource.volume -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        bgmSource.volume = 0;
        yield return null;
    }

    public void PlaySingle(AudioClip clip)
    {
        if (clip != null)
        {
            EffectSourcePlay(clip);
        }
    }
    public void EffectSourcePlay(AudioClip aClip, float pitch=1)
    {
        if (effectSource.isPlaying)
        {
            effectSource2.clip = aClip;
            effectSource2.pitch = pitch;
            effectSource2.Play();
        }
        else if (effectSource2.isPlaying)
        {
            effectSource3.clip = aClip;
            effectSource3.pitch = pitch;
            effectSource3.Play();
        }
        else if (effectSource3.isPlaying)
        {
            effectSource.clip = aClip;
            effectSource.pitch = pitch;
            effectSource.Play();
        }
        else
        {
            effectSource.clip = aClip;
            effectSource.pitch = pitch;
            effectSource.Play();
        }
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPichRange, highPitchRange);

        if (clips[randomIndex] != null)
        {
            EffectSourcePlay(clips[randomIndex], randomPitch);
        }

    }

    public void PlaySingleLoop(AudioClip clip)
    {
        if (clip != null)
        {
            loopEffectsource.clip = clip;
            loopEffectsource.Play();
        }
    }

    public void StopSingleLoop()
    {
        loopEffectsource.clip = null;
        loopEffectsource.Stop();
    }
}
