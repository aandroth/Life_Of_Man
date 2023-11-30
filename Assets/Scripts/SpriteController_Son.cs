using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController_Son : MonoBehaviour, I_SpriteController
{
    public enum STATE {RUNNING, IDLE}
    public STATE m_state = STATE.IDLE;
    public GameObject m_sprite;
    public GameObject m_spriteHandler;
    public float m_speed = 1f;
    public float m_jumpForce = 450f;
    public delegate void ReportDamage();
    public ReportDamage m_reportDamage;
    public GameObject m_innerHandler;

    private Rigidbody2D m_innerHandler_rb;
    private bool m_canJump = false;
    private Animator m_spriteAnimator;

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

    public void PushForward() 
    {
        // set sprite to forward
        if (m_sprite.transform.localScale.x < 0) { m_sprite.transform.localScale = new Vector3(-m_sprite.transform.localScale.x, m_sprite.transform.localScale.y, m_sprite.transform.localScale.z); }
        // play run animation
        if (!m_spriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            m_spriteAnimator.Play("Walk");
        if (m_state != STATE.RUNNING)
            m_state = STATE.RUNNING;
    }
    public void PushBackward() 
    {
        // set sprite to backward
        if (m_sprite.transform.localScale.x > 0) { m_sprite.transform.localScale = new Vector3(-m_sprite.transform.localScale.x, m_sprite.transform.localScale.y, m_sprite.transform.localScale.z); }
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
        }
    }

    public void Idle()
    {
        if (!m_spriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            m_spriteAnimator.Play("Idle");
        if (m_state != STATE.IDLE)
            m_state = STATE.IDLE;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enenmy")
            m_reportDamage.Invoke();
        if (collision.tag == "Ground" && !m_canJump)
            m_canJump = true;
    }
}
