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
    public enum TEENAGER_STATE { IDLE, RUNNING, ATTACKING, KNOCKBACK };
    public TEENAGER_STATE m_state = TEENAGER_STATE.IDLE;
    public float m_speedRate = 0.9f;
    public float m_jumpRate = 0.1f;
    public float m_growthRate = 0.1f;
    public float m_growthOffset = 0.05f;
    public float m_growthCount = 0;
    public float m_speed = 1f;
    public float m_attackRecoveryTime = 1.5f;
    public GameObject m_upperEnemyDestroyerCollider, m_lowerEnemyDestroyerCollider;

    public delegate void ReportAtPyramid(bool b);
    public ReportAtPyramid m_reportAtPyramid;
    public delegate void ReportAtStart(bool b);
    public ReportAtStart m_reportAtStart;
    public delegate void ReportGotOlder(SpriteController_Teenager t);
    public ReportGotOlder m_reportGotOlder;
    public delegate void ReportGotTreasure();
    public ReportGotTreasure m_reportGotTreasure;
    public int m_health = 3;
    public float m_keepRunningTimer = 0.25f, m_keepRunningTimerMax = 0.25f;
    public float m_knockbackTimer = 0.25f, m_knockbackTimerMax = 0.25f;
    public int m_knockbackDirection = 1;

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
            if (m_keepRunningTimer > 0)
            {
                m_spriteHandler.transform.Rotate(0, 0, Mathf.Sign(m_innerHandler.transform.localScale.x) * -m_speed * Time.deltaTime);
                m_keepRunningTimer -= Time.deltaTime;
            }
            else
            {
                Idle();
            }
        }
        else if(m_state == TEENAGER_STATE.KNOCKBACK)
        {

        }
    }
    public void PushForward()
    {
        if(m_state != TEENAGER_STATE.KNOCKBACK && m_state != TEENAGER_STATE.ATTACKING)
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
    }
    public void PushBackward()
    {
        if (m_state != TEENAGER_STATE.KNOCKBACK && m_state != TEENAGER_STATE.ATTACKING)
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
    }

    public void Action()
    {
        if (m_state != TEENAGER_STATE.ATTACKING)
        {
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
            if (collision.GetComponent<Treasure>().m_isActive)
            {
                GetOlder();
                collision.gameObject.GetComponent<Treasure>().DestroyHandler();
            }
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
        if (m_growthCount < 3)
        {
            float newScale = Mathf.Abs(m_innerHandler.transform.localScale.x) + m_growthRate;
            float newPos = m_innerHandler.transform.localPosition.y + m_growthRate;
            m_innerHandler.transform.localScale = new Vector3(newScale*Mathf.Sign(m_innerHandler.transform.localScale.x), Mathf.Abs(newScale), newScale);
            m_innerHandler.transform.localPosition = m_innerHandler.transform.localPosition + (m_innerHandler.transform.up * m_growthOffset);

            m_speed *= m_speedRate;

            ++m_growthCount;

            m_reportGotOlder.Invoke(GetComponent<SpriteController_Teenager>());
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
