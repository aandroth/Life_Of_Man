using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController_Father : MonoBehaviour, I_SpriteController
{
    public GameObject m_sprite;
    public GameObject m_spriteHandler;
    public GameObject m_visionToWorldHitPoint = null;
    public Shield m_shield = null;
    public Collider2D m_collider;
    public float m_headSpeed = 1f;
    public float m_walkSpeed = 1f;
    public float m_currSpeed = 1f;
    public GameObject m_innerHandler;
    private Rigidbody2D m_innerHandler_rb;
    public GameObject m_fatherHead;
    private Animator m_spriteAnimator;
    public float m_headUpLimit = 355, m_headDownLimit = 280;
    public bool m_hasHeart = false;
    public GameObject m_heart, m_mind;
    public float m_timePassed = 0, m_timeMax = 3;

    // Sprite Collisions
    public delegate void ReportFatherHasSonInArea(bool b);
    public ReportFatherHasSonInArea m_reportFatherHasSonInArea;
    public delegate void ReportFatherReachesGrowOldArea(bool b); 
    public ReportFatherReachesGrowOldArea m_reportFatherReachesGrowOldArea;
    public delegate void ReportFatherReachedPyramid(bool b);
    public ReportFatherReachedPyramid m_reportFatherReachedPyramid;
    public delegate void ReportFatherReachedStartArea(bool b);
    public ReportFatherReachedStartArea m_reportFatherReachedStartArea;

    // Animation Finished
    public delegate void ReportPlaceGrandfatherdAnimDone();
    public ReportPlaceGrandfatherdAnimDone m_reportPlacedGrandfatherdAnimDone;
    public delegate void ReportPyramidAnimDone();
    public ReportPyramidAnimDone m_reportPyramidAnimDone;
    public delegate void ReportGrowOldAnimDone();
    public ReportGrowOldAnimDone m_reportGrowOldAnimDone;

    private static readonly int m_walkStateNameHash = Animator.StringToHash("Walk");

    // Start is called before the first frame update
    void Awake()
    {
        m_spriteAnimator = m_sprite.GetComponent<Animator>();
        m_innerHandler_rb = m_innerHandler.GetComponent<Rigidbody2D>();
        m_currSpeed = m_walkSpeed;
    }

    public void Update()
    {
        if (m_currSpeed > 0)
        {
            m_spriteHandler.transform.Rotate(new Vector3(0, 0, -m_currSpeed*Time.deltaTime));
        }
    }

    public void PushForward()
    {
        //Debug.Log($"PushForward, m_fatherHead.transform.eulerAngles.z: {m_fatherHead.transform.localEulerAngles.z}");
        if(m_fatherHead.transform.localEulerAngles.z > m_headDownLimit)
            m_fatherHead.transform.Rotate(0, 0, -m_headSpeed*Time.deltaTime);
    }
    public void PushBackward()
    {
        //Debug.Log($"PushBackward, m_fatherHead.transform.eulerAngles.z: {m_fatherHead.transform.localEulerAngles.z}");
        if(m_fatherHead.transform.localEulerAngles.z < m_headUpLimit)
            m_fatherHead.transform.Rotate(0, 0, m_headSpeed * Time.deltaTime);
    }
    public void Action()
    {
        // Heal son
        if (m_hasHeart && m_timePassed == 0)
            StartCoroutine(Heal());
    }


    public IEnumerator Heal()
    {
        // Play healing animation
        m_heart.GetComponent<HealingHeart>().HealAllHurtables();
        while (m_timePassed < m_timeMax)
        {
            m_timePassed += Time.deltaTime;
            yield return null;
        }
        m_timePassed = 0;
    }

    public void Idle()
    {
        m_currSpeed = 0;
        m_spriteAnimator.Play("Idle");
    }

    public void ContinueWalking()
    {
        m_currSpeed = m_walkSpeed;
        m_spriteAnimator.Play(m_walkStateNameHash);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger detected: " + collision.tag);
        if (collision.CompareTag("Pyramid"))
        {
            m_reportFatherReachedPyramid?.Invoke(true);
        }
        else if (collision.CompareTag("StartingArea"))
        {
            m_reportFatherReachedStartArea?.Invoke(true);
        }
        else if (collision.CompareTag("Son"))
        {
            m_reportFatherHasSonInArea?.Invoke(true);
            if (!m_shield.gameObject.activeSelf && m_shield.m_state == Shield.STATE.BRONZE)
            {
                m_shield.gameObject.SetActive(true);
            }
        }
        else if (collision.CompareTag("GrowOldArea"))
        {
                m_reportFatherReachesGrowOldArea?.Invoke(true);
        }
    }    
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Trigger undetected: " + collision.tag);
        if (collision.CompareTag("Pyramid"))
        {
            m_reportFatherReachedPyramid?.Invoke(false);
        }
        else if (collision.CompareTag("StartingArea"))
        {
            m_reportFatherReachedStartArea?.Invoke(false);
        }
        else if (collision.CompareTag("Son"))
        {
            m_reportFatherHasSonInArea?.Invoke(false);
        }
        else if (collision.CompareTag("GrowOldArea"))
        {
            m_reportFatherReachesGrowOldArea?.Invoke(false);
        }
    }

    public void ReportAnimationFinished(string animName)
    {
        if(animName == "PlacedGrandfather")
        {
            //Debug.Log("ReportAnimationFinished: PlacedGrandfatherFinished");
            m_reportPlacedGrandfatherdAnimDone.Invoke();
        }
        else if(animName == "Pyramid")
        {
            m_reportPyramidAnimDone.Invoke();
        }
        else if(animName == "GrowOld")
        {
            m_reportGrowOldAnimDone.Invoke();
        }
    }

    public Transform ReturnSpecialTransform()
    {
        //Debug.DrawLine(m_fatherHead.transform.position, m_fatherHead.transform.position + m_fatherHead.transform.right, new Color(256, 0, 0));
        return m_fatherHead.transform;
    }

    public void FatherGainsHeart()
    {
        m_hasHeart = true;
        m_heart.SetActive(true);
    }

    public void FatherGainsMind()
    {
        m_mind.SetActive(true);
    }

    public void FatherGainsSilverShield()
    {
        if (m_shield != null)
        {
            m_shield.gameObject.SetActive(true);
            m_shield.UpgradeToSilver();
        }
        else
            Debug.LogError($"Tried to upgrade shield on empty shield!");
    }

    public void FatherGainsGoldHeart()
    {
        Debug.Log("Golden Heart");
        if (m_heart != null)
        {
            m_heart.GetComponent<HealingHeart>().UpgradeHeartToGold();
        }
        else
            Debug.LogError($"Tried to upgrade heart on empty heart!");
    }

    public void DestroySelf()
    {
        Destroy(m_spriteHandler);
    }
}
