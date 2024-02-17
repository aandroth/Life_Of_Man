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

    // Update is called once per frame
    void Update()
    {
        if(m_isChasing) 
            transform.Translate(Vector3.Normalize(m_target.transform.position - transform.position) *m_moveSpeed*Time.deltaTime);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger detected: "+collision.tag);
        if(collision.tag == "Father")
        {
            if (!m_isChasing && m_twinkle != null)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                m_twinkle.GetComponent<Animator>().enabled = false;
                m_twinkleHandler.SetActive(false);
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
            if (!m_isChasing && m_twinkle != null)
            {
                StartCoroutine(StunRecovery(m_stunnedTime));
            }
            else
               Die();
        }
    }

    public IEnumerator StunRecovery(float duration)
    {
        yield return new WaitForSeconds(duration - 1);
        m_twinkleHandler?.SetActive(true);

        yield return new WaitForSeconds(1);
        m_isActive = true;
        GetComponent<SpriteRenderer>().enabled = true;
        m_twinkle.GetComponent<Animator>().enabled = true;
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
        Debug.Log($"player vel: {collision.gameObject.GetComponent<Rigidbody2D>().velocity}");
        Vector2 knockbackDirection = new Vector2(collision.transform.position.x - transform.position.x, collision.transform.position.y - transform.position.y);
        Vector3 v = -collision.gameObject.GetComponent<Rigidbody2D>().velocity + (knockbackDirection * m_knockbackForce);
        collision.gameObject.GetComponent<Rigidbody2D>().velocity = (v);
        Debug.Log($"player vel: {collision.gameObject.GetComponent<Rigidbody2D>().velocity}");
        m_twinkle.SetActive(false);
        GetComponent<SpriteRenderer>().maskInteraction = 0;
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
