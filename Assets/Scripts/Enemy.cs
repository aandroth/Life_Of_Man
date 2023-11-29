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
    public bool m_isActive = true;

    // Update is called once per frame
    void Start()
    {
        m_target = GameObject.FindGameObjectWithTag("Son");
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isChasing) 
            transform.Translate(Vector3.Normalize(m_target.transform.position - transform.position) *m_moveSpeed*Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger detected: "+collision.tag);
        if(collision.tag == "Father")
        {
            if (!m_isChasing)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                m_twinkle.SetActive(false);
                m_isActive = false;
            }
            else
               Die();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Father")
        {
            if (!m_isChasing)
            {
                GetComponent<SpriteRenderer>().enabled = true;
                m_twinkle.SetActive(true);
                m_isActive = true;
            }
            else
               Die();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected");
        if (collision.gameObject.tag == "Son" && m_isActive)
        {
            CauseDamage(collision);
        }
    }

    public void CauseDamage(Collision2D collision)
    {
        collision.gameObject.GetComponent<I_Hurtable>().TakeDamage();
    }

    public void Die()
    {
        Debug.Log("Die");
        Destroy(m_handler);
    }

    public void HurtTarget()
    {
        //m_target
    }
}
