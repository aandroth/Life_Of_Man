using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController_Father : MonoBehaviour, I_SpriteController
{
    public GameObject m_sprite;
    public GameObject m_spriteHandler;
    public GameObject m_visionToWorldHitPoint = null;
    public float m_headSpeed = 1f;
    public float m_walkSpeed = 1f;
    public GameObject m_innerHandler;
    private Rigidbody2D m_innerHandler_rb;
    public GameObject m_fatherHead;
    private Animator m_spriteHandlerAnimator;
    private Animator m_spriteAnimator;
    public float m_currTime, m_timeMax;
    public float m_headUpLimit = 355, m_headDownLimit = 280;
    public bool m_hasHeart = false;
    public GameObject m_heart;

    // Sprite Collisions
    public delegate void ReportFatherHasSonInArea(bool b);
    public ReportFatherHasSonInArea m_reportFatherHasSonInArea;
    public delegate void ReportFatherHasGrandfatherInArea(bool b);
    public ReportFatherHasGrandfatherInArea m_reportFatherHasGrandfatherInArea;
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

    // Start is called before the first frame update
    void Start()
    {
        m_spriteHandlerAnimator = m_spriteHandler.GetComponent<Animator>();
        m_spriteAnimator = m_sprite.GetComponent<Animator>();
        m_innerHandler_rb = m_innerHandler.GetComponent<Rigidbody2D>();
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
        m_spriteHandlerAnimator.speed = m_walkSpeed;
    }

    public void Idle()
    {
        m_spriteHandlerAnimator.speed = 0;
        m_spriteAnimator.Play("Idle");
    }

    public void ContinueWalking()
    {
        m_spriteHandlerAnimator.speed = m_walkSpeed;
        m_spriteAnimator.Play("FatherWalk");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger detected: " + collision.tag);
        if (collision.tag == "Pyramid")
        {
            m_reportFatherReachedPyramid?.Invoke(true);
        }
        else if (collision.tag == "StartingArea")
        {
            m_reportFatherReachedStartArea?.Invoke(true);
        }
        else if (collision.tag == "Son")
        {
            m_reportFatherHasSonInArea?.Invoke(true);
        }
        else if (collision.tag == "Grandfather")
        {
            m_reportFatherHasSonInArea?.Invoke(true);
        }
    }    
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trigger undetected: " + collision.tag);
        if (collision.tag == "Pyramid")
        {
            m_reportFatherReachedPyramid?.Invoke(false);
        }
        else if (collision.tag == "StartingArea")
        {
            m_reportFatherReachedStartArea?.Invoke(false);
        }
        else if (collision.tag == "Son")
        {
            m_reportFatherHasSonInArea?.Invoke(false);
        }
    }

    public void ReportAnimationFinished(string animName)
    {
        if(animName == "PlacedGrandfather")
        {
            Debug.Log("ReportAnimationFinished: PlacedGrandfatherFinished");
            m_reportPlacedGrandfatherdAnimDone.Invoke();
        }
        if(animName == "Pyramid")
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
        Debug.DrawLine(m_fatherHead.transform.position, m_fatherHead.transform.position + m_fatherHead.transform.right, new Color(256, 0, 0));
        return m_fatherHead.transform;
    }

    public void FatherGainsHeart()
    {
        m_hasHeart = true;
        m_heart.SetActive(true);
    }

    public void GetOlder()
    {

    }

    public void RestoreHandlerSpeed()
    {
        m_spriteHandlerAnimator.speed = m_walkSpeed;
        m_spriteAnimator.Play("Walk");
    }

    public void DestroySelf()
    {
        Destroy(m_spriteHandler);
    }
}
