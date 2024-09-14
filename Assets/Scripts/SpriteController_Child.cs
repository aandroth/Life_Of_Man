using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController_Child : MonoBehaviour, I_SpriteController
{
    public enum STATE {RUNNING, IDLE, KNOCKBACK}
    public STATE m_state = STATE.IDLE;
    public GameObject m_sprite;
    public GameObject m_spriteHandler;
    public float m_speed = 1f, m_speedMin = 1.0f, m_speedMax = 2.0f, m_speedCurr;
    public float m_jumpForce = 450f;
    //public GameObject m_innerHandler;
    public Rigidbody2D m_rb;
    public float m_groundCollisionRadius = 0.03f;

    public bool m_treasureCooldown = false;
    public bool m_canJump = false;
    private Animator m_spriteAnimator;
    public float m_speedRate = 0.9f;
    public float m_jumpRate = 0.1f;
    public float m_growthRate = 0.1f;
    public int m_growthCount = 0;
    public float m_speedIncreaseRate = 0.1f;

    public delegate void ReportAtPyramid(bool b);
    public ReportAtPyramid m_reportAtPyramid;
    public delegate void ReportAtStart(bool b);
    public ReportAtStart m_reportAtStart;
    public delegate void ReportTookDamage(int i);
    public ReportTookDamage m_reportTookDamage;    
    public delegate void ReportAgedOut(SpriteController_Child c);
    public ReportAgedOut m_reportGotOlder;
    public delegate void ReportGotTreasure(int i);
    public ReportGotTreasure m_reportGotTreasure;
    public delegate void ReportDies();
    public ReportDies m_reportDies;
    public float m_runSpeedLimit, m_runSpeedFriction_Air, m_runSpeedFriction_Ground;

    public GameObject m_nextForm = null;
    public GameObject m_groundDetector;
    public LayerMask m_groundLayer;
    public float m_jumpDelayCountdown = 1, m_jumpDelayCountdownMax = 1;
    public float m_keepRunningTimer = 0.25f, m_keepRunningTimerMax = 0.25f;
    public float m_damageImmunityTimer = -1f, m_damageImmunityTimeMax = 1.5f;
    public float m_minFadeDuringDamageImmunity = 0.01f, m_damageImmunityFadeSpeed = 5f;
    public float m_controlLossTimer = -1f, m_controlLossTimeMax = 0.5f;
    public float m_knockbackTimer = 0.25f, m_knockbackTimerMax = 0.25f, m_knockbackDistance = 1;
    public int m_knockbackDirection = 1;

    private static readonly int m_walkStateNameHash = Animator.StringToHash("Walk");
    private static readonly int m_idleStateNameHash = Animator.StringToHash("Idle");
    public float m_startingY = -1.634216f;

    private SpriteRenderer m_spriteRenderer;

    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteAnimator = m_sprite.GetComponent<Animator>();
        //m_startingY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_state == STATE.RUNNING)
        {
            if (m_keepRunningTimer > 0)
            {
                if (Mathf.Abs(m_speedCurr) < m_speedMax)
                    m_speedCurr += m_sprite.transform.localScale.x > 0 ? m_speed * Time.deltaTime : -m_speed * Time.deltaTime;
                else
                    m_speedCurr = m_sprite.transform.localScale.x > 0 ? m_speedMax : -m_speedMax;
                m_keepRunningTimer -= Time.deltaTime;
                m_spriteHandler.transform.Rotate(new(0, 0, -m_speedCurr * Time.deltaTime), Space.Self);
            }
            else
            {
                Idle();
            }
        }
        else if (m_state == STATE.KNOCKBACK)
        {
            m_knockbackTimer -= Time.deltaTime;
            m_spriteHandler.transform.Rotate(0, 0, ( m_sprite.transform.localScale.x > 0 ? m_speedMax : -m_speedMax ) * Time.deltaTime);
            if(m_knockbackTimer < 0)
            {
                Idle();
            }
        }
    }

    public IEnumerator CountDownDamageImmunityAndControlFreeze()
    {
        Color c;
        m_damageImmunityTimer = m_damageImmunityTimeMax;
        m_controlLossTimer = m_controlLossTimeMax;
        while (m_damageImmunityTimer > 0)
        {
            c = m_spriteRenderer.color;
            c.a = Mathf.Max(Mathf.Sin(m_damageImmunityTimer * m_damageImmunityFadeSpeed), m_minFadeDuringDamageImmunity);
            m_spriteRenderer.color = c;
            m_damageImmunityTimer -= Time.deltaTime;
            if (m_controlLossTimer > 0)
            {
                m_controlLossTimer -= Time.deltaTime;
                if (m_controlLossTimer < 0)
                {
                    if (m_spriteHandler.GetComponent<PlayerController>().enabled)
                        m_spriteHandler.GetComponent<PlayerController>().EnablePlayerControls();
                    else
                        m_spriteHandler.GetComponent<SpriteDriver_Child>().EnableControls();
                }
            }
            yield return null;
        }
        c = m_spriteRenderer.color;
        c.a = 1;
        m_spriteRenderer.color = c;
    }

    public IEnumerator CountDownTreasureCooldown(float duration = 3.0f)
    {
        m_treasureCooldown = true;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        m_treasureCooldown = false;
    }

    void FixedUpdate()
    {
        if (m_jumpDelayCountdown > 0)
        {
            m_jumpDelayCountdown -= Time.deltaTime;
            m_canJump = false;
        }
        else
        {
            m_canJump = Physics2D.OverlapCircle(m_groundDetector.transform.position, m_groundCollisionRadius, m_groundLayer);
        }
        if (transform.localPosition.y < m_startingY)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, m_startingY, transform.localPosition.z);
        }
    }

    public void PushForward()
    {
        if (m_state != STATE.KNOCKBACK)
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
            if (!(m_spriteAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == m_walkStateNameHash))
                m_spriteAnimator.Play(m_walkStateNameHash);
            if (m_state != STATE.RUNNING)
                m_state = STATE.RUNNING;
            if (m_speedCurr == m_speedMin)
                m_speedCurr = m_speedMin;
        }
    }
    public void PushBackward()
    {
        if (m_state != STATE.KNOCKBACK)
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
            if (!(m_spriteAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == m_walkStateNameHash))
                m_spriteAnimator.Play(m_walkStateNameHash);
            // move sprite using handler
            if (m_state != STATE.RUNNING)
                m_state = STATE.RUNNING;
            if (m_speedCurr == m_speedMin)
                m_speedCurr = -m_speedMin;
        }
    }
    public void Action() 
    {
        if (m_canJump)
        {
            m_rb.AddForce(new Vector2(transform.parent.transform.up.x, transform.parent.transform.up.y) * m_jumpForce);
            m_canJump = false;
            m_jumpDelayCountdown = m_jumpDelayCountdownMax;
            Debug.DrawRay(transform.position, transform.up, Color.blue, 100);
        }
    }

    public void Idle()
    {
        if (!(m_spriteAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == m_idleStateNameHash))
            m_spriteAnimator.Play(m_idleStateNameHash);
        if (m_state != STATE.IDLE)
            m_state = STATE.IDLE;
        m_speedCurr = m_speedMin;
    }

    public void GroundCollision(Collision2D collision)
    {
        if (!m_canJump && collision.gameObject.CompareTag("Ground"))
            m_canJump = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Treasure") && !m_treasureCooldown)
        {
            StartCoroutine(CountDownTreasureCooldown());
            //Debug.Log("Treasure");
            GetOlder();
            m_reportGotTreasure.Invoke(m_growthCount);
            collision.gameObject.GetComponent<Treasure>().DeactivateHandler();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GroundCollision(collision);
        }
    }


    public void GetOlder()
    {
        if(m_growthCount < 3)
        {
            float newScale = transform.localScale.y + m_growthRate;
            transform.localScale = new Vector3(newScale*Mathf.Sign(transform.localScale.x), newScale, newScale);
            transform.position += 2 * newScale * m_spriteHandler.transform.up;

            m_speed *= m_speedRate;
            m_jumpForce += m_jumpRate;

            ++m_growthCount;

            m_reportGotOlder.Invoke(GetComponent<SpriteController_Child>());
        }
    }

    public void TakeDamage(GameObject gO, float knockbackForce)
    {
        if (m_state != STATE.KNOCKBACK && m_damageImmunityTimer <= 0)
        {
            m_state = STATE.KNOCKBACK;
            m_knockbackTimer = m_knockbackTimerMax;
            GetComponent<I_Hurtable>().TakeDamage();

            if(GetComponent<I_Hurtable>().m_health <= 0)
            {
                m_reportDies.Invoke();
                return;
            }

            float enemyDirection = transform.up.x * gO.transform.position.y - transform.up.y * gO.transform.position.x;
            m_speedCurr = (m_sprite.transform.localScale.x > 0 ? m_speedMax : -m_speedMax);

            StartCoroutine(CountDownDamageImmunityAndControlFreeze());

            Debug.Log($"Control is lost");
            if (m_spriteHandler.GetComponent<PlayerController>().enabled)
                m_spriteHandler.GetComponent<PlayerController>().DisablePlayerControls();
            else
                m_spriteHandler.GetComponent<SpriteDriver_Child>().DisableControls();
        }
    }


    public void DestroySelf()
    {
        Destroy(m_spriteHandler);
    }
}
