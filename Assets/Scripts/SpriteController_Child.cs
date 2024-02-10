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
    public ReportAgedOut m_reportGotOlder;
    public int m_health = 3;

    public GameObject m_nextForm = null;
    public GameObject m_groundDetector;
    public LayerMask m_groundLayer;
    public float m_jumpDelayCountdown = 1, m_jumpDelayCountdownMax = 1;
    public float m_keepRunningTimer = 0.25f, m_keepRunningTimerMax = 0.25f;

    // Start is called before the first frame update
    void Awake()
    {
        m_spriteAnimator = m_sprite.GetComponent<Animator>();
        m_innerHandler_rb = m_innerHandler.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_state == STATE.RUNNING)
        {
            m_spriteHandler.transform.Rotate(0, 0, Mathf.Sign(m_sprite.transform.localScale.x) * -m_speed * Time.deltaTime);
            m_keepRunningTimer -= Time.deltaTime;
            if (m_keepRunningTimer <= 0)
                m_state = STATE.IDLE;
        }
    }

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, new Vector3(m_innerHandler.transform.up.x, m_innerHandler.transform.up.y, 0));
        if (m_jumpDelayCountdown < 0)
            m_canJump = Physics2D.OverlapCircle(m_groundDetector.transform.position, 0.03f, m_groundLayer);
        else
            m_jumpDelayCountdown -= Time.deltaTime;
    }

    public void PushForward()
    {
        m_keepRunningTimer = m_keepRunningTimerMax;
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
        m_keepRunningTimer = m_keepRunningTimerMax;
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
            m_innerHandler_rb.AddForce(new Vector2(transform.up.x, transform.up.y) * m_jumpForce);
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
            collision.gameObject.GetComponent<Treasure>().DestroyHandler();
        }
    }


    public void GetOlder()
    {
        if(m_growthCount < 3)
        {
            float newScale = transform.localScale.x + m_growthRate;
            Debug.Log($"newScale: {newScale}");
            transform.localScale = new Vector3(newScale, newScale, newScale);
            Debug.Log($"localScale: {transform.localScale}");
            m_innerHandler.transform.localPosition += m_spriteHandler.transform.up * newScale * 2;

            m_speed *= m_speedRate;
            m_jumpForce += m_jumpRate;

            ++m_growthCount;

            m_reportGotOlder.Invoke(GetComponent<SpriteController_Child>());
        }
    }


    public void DestroySelf()
    {
        Destroy(m_spriteHandler);
    }
}
