using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController_Teenager : MonoBehaviour, I_SpriteController
{
    public GameObject m_sprite;
    public GameObject m_spriteHandler;
    public float m_walkSpeed = 1f;
    public GameObject m_innerHandler;
    //private Rigidbody2D m_innerHandler_rb;
    private Animator m_spriteAnimator;

    public Animator m_animator;
    public string m_attackAnimName = "";
    public enum TEENAGER_STATE { IDLE, RUNNING, ATTACKING, KNOCKBACK };
    public TEENAGER_STATE m_state = TEENAGER_STATE.IDLE;
    public float m_speedRate = 0.9f;
    public float m_jumpRate = 0.1f;
    public float m_growthRate = 0.1f;
    public float m_growthOffset = 0.05f;
    public int m_growthCount = 0;
    public float m_speed = 1f;
    public float m_attackRecoveryTime = 1.5f;
    public GameObject m_upperEnemyDestroyerCollider, m_lowerEnemyDestroyerCollider;

    public bool m_treasureCooldown;
    public bool m_isAtGrandfather = false;
    public delegate void ReportAtPyramid(bool b);
    public ReportAtPyramid m_reportAtPyramid;
    public delegate void ReportAtStart(bool b);
    public ReportAtStart m_reportAtStart;
    public delegate void ReportGotOlder(SpriteController_Teenager t);
    public ReportGotOlder m_reportGotOlder;
    public delegate void ReportGotTreasure(int i);
    public ReportGotTreasure m_reportGotTreasure;
    public delegate void ReportReachingGrandfather(bool b);
    public ReportReachingGrandfather m_reportReachingGrandfather;
    public delegate void ReportPickingUpGrandfather();
    public ReportPickingUpGrandfather m_reportPickingUpGrandfather;
    public delegate void ReportDies();
    public ReportDies m_reportDies;
    public int m_health = 3;
    public float m_keepRunningTimer = 0.25f, m_keepRunningTimerMax = 0.25f;
    public float m_knockbackTimer = 0.25f, m_knockbackTimerMax = 0.25f;
    public int m_knockbackDirection = 1;
    public Shield m_shield;

    public SfxList m_sfxList;

    void Awake()
    {
        m_spriteAnimator = m_sprite.GetComponent<Animator>();
        Physics2D.SyncTransforms();
    }

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
        if (Input.GetKeyUp(KeyCode.E) && m_growthCount < 4)
        {
            GetOlder();
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            TakeDamage(m_sprite, 3);
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
        if(m_isAtGrandfather && m_growthCount == 3)
        {
            m_animator.Play(m_attackAnimName);
            m_reportPickingUpGrandfather.Invoke();
            return;
        }
        if (m_state != TEENAGER_STATE.ATTACKING)
        {
            m_state = TEENAGER_STATE.ATTACKING;
            m_animator.Play(m_attackAnimName);
            StartCoroutine(Attacking());
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
        if (collision.gameObject.CompareTag("Treasure"))
        {
            if (collision.GetComponent<Treasure>().m_isActive)
            {
                StartCoroutine(CountDownTreasureCooldown());
                GetOlder();
                m_reportGotTreasure.Invoke(m_growthCount);
                collision.gameObject.GetComponent<Treasure>().DeactivateHandler_AndPlayAudio(true);
            }
        }
        else if (collision.CompareTag("Pyramid"))
        {
            m_reportAtPyramid?.Invoke(true);
        }
        else if (collision.CompareTag("StartingArea"))
        {
            m_reportAtStart?.Invoke(true);
        }
        else if (collision.CompareTag("Grandfather"))
        {
            m_isAtGrandfather = true;
            m_reportReachingGrandfather?.Invoke(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Trigger undetected: " + collision.tag);
        if (collision.CompareTag("Pyramid"))
        {
            m_reportAtPyramid?.Invoke(false);
        }
        else if (collision.CompareTag("StartingArea"))
        {
            m_reportAtStart?.Invoke(false);
        }
        else if (collision.CompareTag("Grandfather"))
        {
            m_isAtGrandfather = false;
        }
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

    public void GetOlder()
    {
        if (m_growthCount < 3)
        {
            float newScale = m_innerHandler.transform.localScale.y + m_growthRate;
            float newPos = m_innerHandler.transform.localPosition.y + m_growthRate;
            m_innerHandler.transform.localScale = new Vector3(newScale*Mathf.Sign(m_innerHandler.transform.localScale.x), newScale, newScale);
            m_innerHandler.transform.localPosition = m_innerHandler.transform.localPosition + (m_innerHandler.transform.up * m_growthOffset);

            m_speed *= m_speedRate;

            ++m_growthCount;

            m_reportGotOlder.Invoke(GetComponent<SpriteController_Teenager>());
            m_sfxList?.PlayIdxFromList_WillLoop(0, false);
        }
    }

    //public IEnumerator FadePulseAfterDamage()
    //{
    //    Color c;
    //    Debug.Log($"m_damageImmunityTimer is gained");
    //    m_damageImmunityTimer = m_damageImmunityTimeMax;
    //    m_controlLossTimer = m_controlLossTimeMax;
    //    while (m_damageImmunityTimer > 0)
    //    {
    //        c = m_spriteRenderer.color;
    //        c.a = Mathf.Max(Mathf.Sin(m_damageImmunityTimer * m_damageImmunityFadeSpeed), m_minFadeDuringDamageImmunity);
    //        m_spriteRenderer.color = c;
    //        m_damageImmunityTimer -= Time.deltaTime;
    //        if (m_controlLossTimer > 0)
    //        {
    //            m_controlLossTimer -= Time.deltaTime;
    //            if (m_controlLossTimer < 0)
    //            {
    //                Debug.Log($"Control is regained");
    //                if (m_spriteHandler.GetComponent<PlayerController>().enabled)
    //                    m_spriteHandler.GetComponent<PlayerController>().EnablePlayerControls();
    //                else
    //                    m_spriteHandler.GetComponent<SpriteDriver_Child>().EnableControls();
    //            }
    //        }
    //        yield return null;
    //    }
    //    Debug.Log($"m_damageImmunityTimer is lost");
    //    c = m_spriteRenderer.color;
    //    c.a = 1;
    //    m_spriteRenderer.color = c;
    //}

    public void TakeDamage(GameObject gO, float knockbackForce)
    {
        m_shield.gameObject.SetActive(true);
        GetComponent<I_Hurtable>().TakeDamage();

        if (GetComponent<I_Hurtable>().m_health <= 0)
        {
            m_reportDies.Invoke();
            m_sfxList?.PlayIdxFromList_WillLoop(2, false);
        }
        else
            m_sfxList?.PlayIdxFromList_WillLoop(1, false);
    }

    public IEnumerator Attacking()
    {
        m_upperEnemyDestroyerCollider.SetActive(true);
        m_lowerEnemyDestroyerCollider.SetActive(true);
        yield return new WaitForSeconds(1);
        while (m_animator.GetCurrentAnimatorStateInfo(0).IsName(m_attackAnimName))
        {
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
