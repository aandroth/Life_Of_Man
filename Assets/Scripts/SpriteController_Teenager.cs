using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController_Teenager : MonoBehaviour, I_SpriteController
{
    public GameObject m_sprite;
    public GameObject m_spriteHandler;
    public float m_walkSpeed = 1f;
    public GameObject m_innerHandler;
    private Rigidbody2D m_innerHandler_rb;
    private Animator m_spriteAnimator;

    public Animator m_animator;
    public string m_attackAnimName = "";
    public enum TEENAGER_STATE { IDLE, RUNNING, ATTACKING };
    public TEENAGER_STATE m_state = TEENAGER_STATE.IDLE;
    public float m_speedRate = 0.9f;
    public float m_jumpRate = 0.1f;
    public float m_growthRate = 0.1f;
    public float m_growthCount = 0;
    public float m_speed = 1f;
    public float m_attackRecoveryTime = 1.5f;
    public GameObject m_upperEnemyDestroyerCollider, m_lowerEnemyDestroyerCollider;

    public delegate void ReportAtPyramid(bool b);
    public ReportAtPyramid m_reportAtPyramid;
    public delegate void ReportAtStart(bool b);
    public ReportAtStart m_reportAtStart;
    public delegate void ReportTookDamage(int i);
    public ReportTookDamage m_reportTookDamage;
    public delegate void ReportAgedOut(SpriteController_Teenager t);
    public ReportAgedOut m_reportAgedOut;
    public int m_health = 3;
    public float m_keepRunningTimer = 0.25f, m_keepRunningTimerMax = 0.25f;

    public GameObject m_nextForm;

    // Start is called before the first frame update
    void Awake()
    {
        m_spriteAnimator = m_sprite.GetComponent<Animator>();
        m_innerHandler_rb = m_innerHandler.GetComponent<Rigidbody2D>();
        Physics2D.SyncTransforms();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_state == TEENAGER_STATE.RUNNING)
        {
            m_keepRunningTimer -= Time.deltaTime;
            m_spriteHandler.transform.Rotate(0, 0, Mathf.Sign(m_innerHandler.transform.localScale.x) * -m_speed * Time.deltaTime);

            if (m_keepRunningTimer <= 0)
                m_state = TEENAGER_STATE.IDLE;
        }
    }
    public void PushForward()
    {
        m_keepRunningTimer = m_keepRunningTimerMax;
        // set sprite to forward
        if (m_innerHandler.transform.localScale.x < 0) 
        {
            m_innerHandler.transform.localScale = new Vector3(-m_innerHandler.transform.localScale.x,
                                                               m_innerHandler.transform.localScale.y,
                                                               m_innerHandler.transform.localScale.z);
        }
        //Debug.Log($"m_innerHandler.transform.localScale.x: {m_innerHandler.transform.localScale.x}");
        // play run animation
        if (!m_spriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            m_spriteAnimator.Play("Walk");
        if (m_state != TEENAGER_STATE.RUNNING)
            m_state = TEENAGER_STATE.RUNNING;
    }
    public void PushBackward()
    {
        m_keepRunningTimer = m_keepRunningTimerMax;
        // set sprite to backward
        if (m_innerHandler.transform.localScale.x > 0) 
        {
            m_innerHandler.transform.localScale = new Vector3(-m_innerHandler.transform.localScale.x,
                                                               m_innerHandler.transform.localScale.y,
                                                               m_innerHandler.transform.localScale.z);
        }
        //Debug.Log($"m_innerHandler.transform.localScale.x: {m_innerHandler.transform.localScale.x}");
        // play run animation
        if (!m_spriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            m_spriteAnimator.Play("Walk");
        // move sprite using handler
        if (m_state != TEENAGER_STATE.RUNNING)
            m_state = TEENAGER_STATE.RUNNING;
    }

    public void Action()
    {
        if (m_state != TEENAGER_STATE.ATTACKING)
        {
            //Debug.Log($"Teen attack called");
            m_state = TEENAGER_STATE.ATTACKING;
            m_animator.Play(m_attackAnimName);
            StartCoroutine("Attacking");
        }
    }
    public void Idle()
    {
        m_state = TEENAGER_STATE.IDLE;
        m_spriteAnimator.Play("Idle");
    }
    public virtual Transform ReturnSpecialTransform() { return null; }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Treasure")
        {
            GetOlder();
            GetOlder();
            GetOlder();
            collision.gameObject.GetComponent<Treasure>().DestroyHandler();
        }
        else if (collision.tag == "Pyramid")
        {
            m_reportAtPyramid?.Invoke(true);
        }
        else if (collision.tag == "StartingArea")
        {
            m_reportAtStart?.Invoke(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Trigger undetected: " + collision.tag);
        if (collision.tag == "Pyramid")
        {
            m_reportAtPyramid?.Invoke(false);
        }
        else if (collision.tag == "StartingArea")
        {
            m_reportAtStart?.Invoke(false);
        }
    }
    public void GetOlder()
    {
        if(m_growthCount < 3)
        {
            float newScale = m_spriteHandler.transform.localScale.x + m_growthRate;
            //m_spriteHandler.transform.localScale = new Vector3(newScale, newScale, newScale);

            m_speed *= m_speedRate;

            ++m_growthCount;

            if (m_growthCount == 3)
            {
                m_reportAgedOut.Invoke(GetComponent<SpriteController_Teenager>());
            }
        }
    }

    public IEnumerator Attacking()
    {
        //Debug.Log($"Teen attacking");
        m_upperEnemyDestroyerCollider.SetActive(true);
        m_lowerEnemyDestroyerCollider.SetActive(true);
        yield return new WaitForSeconds(1);
        while (m_animator.GetCurrentAnimatorStateInfo(0).IsName(m_attackAnimName))
        {
            //Debug.Log($"Teen attack continues");
            yield return null;
        }
        //Debug.Log($"Teen attacking finished");
        m_upperEnemyDestroyerCollider.SetActive(false);
        m_lowerEnemyDestroyerCollider.SetActive(false);
        yield return new WaitForSeconds(m_attackRecoveryTime);
        m_state = TEENAGER_STATE.IDLE;
    }

    public void DestroySelf()
    {
        Destroy(m_spriteHandler);
    }
}
