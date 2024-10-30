using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SfxList : MonoBehaviour
{
    public AudioSource m_audioSource;
    public List<AudioClip> m_audioClips;

    public void Start()
    {
        if (m_audioSource == null) m_audioSource = GetComponent<AudioSource>();
    }

    public void PlayIdxFromList_WillLoop(int idx = 0, bool willLoop = false)
    {
        m_audioSource.clip = m_audioClips[idx];
        m_audioSource.loop = willLoop;
        m_audioSource.Play();
    }
}
