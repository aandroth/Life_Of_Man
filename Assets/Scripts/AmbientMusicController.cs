using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientMusicController : MonoBehaviour
{
    public List<AudioClip> m_ambientMusicStart;
    public List<AudioClip> m_ambientMusicLoop;
    public List<AudioClip> m_ambientMusicEnd;
    public List<AudioClip> m_ambientMusicMisc;
    public AudioSource m_ambientAudioSource;

    public void Start()
    {
        m_ambientAudioSource = GetComponent<AudioSource>();
    }

    public void StartAmbientMusic(int audioListIdx)
    {
        m_ambientAudioSource.clip = m_ambientMusicStart[audioListIdx];
        m_ambientAudioSource.loop = false;
        m_ambientAudioSource.Play();
        StartCoroutine(LoopAmbientMusicAfterStartMusic(audioListIdx));
    }
    private IEnumerator LoopAmbientMusicAfterStartMusic(int audioListIdx)
    {
        float time = m_ambientAudioSource.clip.length;
        while(time > 0)
        {
            yield return null;
            time -= Time.deltaTime;
        }
        m_ambientAudioSource.clip = m_ambientMusicLoop[audioListIdx];
        m_ambientAudioSource.loop = true;
        m_ambientAudioSource.Play();
    }

    public void StartAmbientMusicEnd_EndIsHappy(bool endIsHappy)
    {
        m_ambientAudioSource.clip = m_ambientMusicEnd[endIsHappy ? 0 : 1];
        m_ambientAudioSource.loop = false;
        m_ambientAudioSource.Play();
    }

    public void StartAmbientMusicMisc(int idx = 0)
    {
        m_ambientAudioSource.clip = m_ambientMusicMisc[idx];
        m_ambientAudioSource.loop = false;
        m_ambientAudioSource.Play();
    }

    public void StopAmbientMusic()
    {
        m_ambientAudioSource.Stop();
    }
}
