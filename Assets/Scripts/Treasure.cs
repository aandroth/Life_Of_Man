using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public bool m_isActive = true;
    public GameObject m_twinkleHandler;
    public float m_respawnTime = 10;
    public void DeactivateHandler()
    {
        transform.parent.gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Father"))
        {
            m_isActive = false;
            m_twinkleHandler.SetActive(false);
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Father"))
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
    }
}
