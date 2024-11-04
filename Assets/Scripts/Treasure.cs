using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public bool m_isActive = true;
    public GameObject m_twinkleHandler;
    public float m_respawnTime = 10;
    public AudioSource m_treasureAcquiredSfx;
    public float m_deactivateDelay = 3f;
    private IEnumerator m_respawnCoroutine;

    public void OnEnable()
    {
        Respawn();
        m_treasureAcquiredSfx = GetComponent<AudioSource>();
    }

    public void DeactivateHandler(bool playAudio = true)
    {
        if (gameObject.activeSelf && transform.parent.gameObject.activeSelf)
        {
            m_respawnCoroutine = DeactivateHandlerCoroutine();
            StartCoroutine(m_respawnCoroutine);
        }
    }

    public IEnumerator DeactivateHandlerCoroutine()
    {
        m_twinkleHandler.SetActive(false);
        GetComponent<SpriteRenderer>().enabled = false;
        m_treasureAcquiredSfx?.Play();
        float timePassed = 0f;
        while(timePassed < m_deactivateDelay)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }
        m_twinkleHandler.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = true;
        StopCoroutine(m_respawnCoroutine);
        transform.parent.gameObject.SetActive(false);
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
        if((collision.CompareTag("Father") || collision.CompareTag("Shield")) && gameObject.activeSelf && transform.parent.gameObject.activeSelf)
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
