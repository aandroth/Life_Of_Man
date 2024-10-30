using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public bool m_isActive = true;
    public GameObject m_twinkleHandler;
    public float m_respawnTime = 10;
    public AudioSource m_treasureAcquiredSfx;

    public void OnEnable()
    {
        Respawn();
        m_treasureAcquiredSfx = GetComponent<AudioSource>();
    }

    public void DeactivateHandler_AndPlayAudio(bool playAudio = false)
    {
        transform.parent.gameObject.SetActive(false);
        m_treasureAcquiredSfx?.Play();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.CompareTag("Father") || collision.CompareTag("Shield")) && gameObject.activeSelf)
        {
            m_isActive = false;
            m_twinkleHandler.SetActive(false);
            GetComponent<SpriteRenderer>().enabled = false;
            gameObject.tag = "Untagged";
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if((collision.CompareTag("Father") || collision.CompareTag("Shield")) && gameObject.activeSelf)
        {
            StartCoroutine(Respawn());
        }
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(m_respawnTime);
        m_isActive = true;
        m_twinkleHandler.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = true;
        gameObject.tag = "Treasure";
    }
}
