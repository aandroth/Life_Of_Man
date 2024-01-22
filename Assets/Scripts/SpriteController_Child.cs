using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController_Child : MonoBehaviour, I_SpriteController
{
    public enum STATE {RUNNING, IDLE}
    public STATE m_state = STATE.IDLE;
    public GameObject m_sprite;
    public GameObject m_spriteHandler;
    public float m_speed = 1f;
    public float m_jumpForce = 450f;
    public GameObject m_innerHandler;

    private Rigidbody2D m_innerHandler_rb;
    public bool m_canJump = false;
    private Animator m_spriteAnimator;
    public float m_speedRate = 0.9f;
    public float m_jumpRate = 0.1f;
    public float m_growthRate = 0.1f;
    public float m_growthCount = 0;

    public delegate void ReportAtPyramid(bool b);
    public ReportAtPyramid m_reportAtPyramid;
    public delegate void ReportAtStart(bool b);
    public ReportAtStart m_reportAtStart;
    public delegate void ReportTookDamage(int i);
    public ReportTookDamage m_reportTookDamage;    
    public delegate void ReportAgedOut(SpriteController_Child c);
    public ReportAgedOut m_reportAgedOut;
    public int m_health = 3;

    public GameObject m_nextForm = null;
    public GameObject m_groundDetector;
    public LayerMask m_groundLayer;
    public float m_jumpDelayCountdown = 1, m_jumpDelayCountdownMax = 1;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteAnimator = m_sprite.GetComponent<Animator>();
        m_innerHandler_rb = m_innerHandler.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_state == STATE.RUNNING)
            m_spriteHandler.transform.Rotate(0, 0, Mathf.Sign(m_sprite.transform.localScale.x) * -m_speed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (m_jumpDelayCountdown < 0)
            m_canJump = Physics2D.OverlapCircle(m_groundDetector.transform.position, 0.03f, m_groundLayer);
        else
            m_jumpDelayCountdown -= Time.deltaTime;
    }

    public void PushForward() 
    {
        // set sprite to forward
        if (m_sprite.transform.localScale.x < 0) 
        { 
            m_sprite.transform.localScale = new Vector3(-m_sprite.transform.localScale.x,
                                                         m_sprite.transform.localScale.y, 
                                                         m_sprite.transform.localScale.z); 
        }
        // play run animation
        if (!m_spriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            m_spriteAnimator.Play("Walk");
        if (m_state != STATE.RUNNING)
            m_state = STATE.RUNNING;
    }
    public void PushBackward() 
    {
        // set sprite to backward
        if (m_sprite.transform.localScale.x > 0) 
        { 
            m_sprite.transform.localScale = new Vector3(-m_sprite.transform.localScale.x, 
                                                         m_sprite.transform.localScale.y, 
                                                         m_sprite.transform.localScale.z); 
        }
        // play run animation
        if (!m_spriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            m_spriteAnimator.Play("Walk");
        // move sprite using handler
        if (m_state != STATE.RUNNING)
            m_state = STATE.RUNNING;
    }
    public void Action() 
    {
        if (m_canJump)
        {
            m_innerHandler_rb.AddForce(m_innerHandler.transform.up * m_jumpForce);
            m_canJump = false;
            m_jumpDelayCountdown = m_jumpDelayCountdownMax;
        }
    }

    public void Idle()
    {
        if (!m_spriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            m_spriteAnimator.Play("Idle");
        if (m_state != STATE.IDLE)
            m_state = STATE.IDLE;
    }

    public void GroundCollision(Collider2D collision)
    {
        if (!m_canJump && collision.tag == "Ground")
            m_canJump = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Treasure")
        {
            GetOlder();
        }
    }


    public void GetOlder()
    {
        if(m_growthCount < 3)
        {
            float newScale = transform.localScale.x + m_growthRate;
            transform.localPosition = new Vector3(newScale, newScale, newScale);
            m_innerHandler.transform.localPosition += m_innerHandler.transform.up * newScale;

            m_speed *= m_speedRate;
            m_jumpForce += m_jumpRate;

            ++m_growthCount;

            if (m_growthCount == 3)
            {
                m_reportAgedOut.Invoke(GetComponent<SpriteController_Child>());
            }
        }
    }


    public void DestroySelf()
    {
        Destroy(m_spriteHandler);
    }
}
