using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject m_target;
    public float m_moveSpeed = 1f;
    public bool m_isChasing = true;
    public GameObject m_handler;
    public GameObject m_twinkle;
    public GameObject m_twinkleHandler;
    public bool m_isActive = true;
    public float m_stunnedTime = 3;
    public float m_knockbackForce = 10;

    // Update is called once per frame
    void Start()
    {
        m_target = GameObject.FindGameObjectWithTag("Son");
    }

    public void OnEnable()
    {
        if (m_twinkleHandler != null)
        {
            transform.position = m_twinkleHandler.transform.position;
            m_isChasing = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.up = transform.position;
        if (m_isChasing && m_target != null)
        {
            transform.Translate(m_moveSpeed * Time.deltaTime * Vector3.Normalize(m_target.transform.position - transform.position));
        }
    }

    public void SetToChasing()
    {
        m_isChasing = true;
        if (m_twinkle != null) { m_twinkle.SetActive(false); }
        GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shield"))
        {
            if (m_isChasing || m_twinkle == null)
               Die();
        }
        if(collision.CompareTag("Father"))
        {
            if (!m_isChasing && m_twinkle != null)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                m_twinkle.GetComponent<Animator>().enabled = false;
                m_twinkleHandler.SetActive(false);
                m_isActive = false;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Father"))
        {
            if (!m_isChasing && m_twinkle != null)
            {
                StartCoroutine(StunRecovery(m_stunnedTime));
            }
        }
    }

    public IEnumerator StunRecovery(float duration)
    {
        yield return new WaitForSeconds(duration - 1);
        if(m_twinkleHandler != null) m_twinkleHandler.SetActive(true);

        yield return new WaitForSeconds(1);
        m_isActive = true;
        GetComponent<SpriteRenderer>().enabled = true;
        m_twinkle.GetComponent<Animator>().enabled = true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collision detected");
        if (collision.gameObject.CompareTag("Son") && m_isActive)
        {
            if(m_twinkle != null) m_twinkle.SetActive(false);
            GetComponent<SpriteRenderer>().maskInteraction = 0;
            SetToChasing();
            collision.gameObject.GetComponent<I_SpriteController>().TakeDamage(gameObject, m_knockbackForce);
        }
    }

    public void Die()
    {
        if (m_handler != null)
            m_handler.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}
