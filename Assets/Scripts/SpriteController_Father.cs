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
    private Animator m_spriteAnimator;
    public float m_currTime, m_timeMax;
    public float m_headUpLimit = 355, m_headDownLimit = 280;

    // Sprite Collisions
    public delegate void ReportFatherHasPlayerInArea();
    public ReportFatherHasPlayerInArea m_reportFatherHasPlayerInArea;
    public delegate void ReportFatherReachedPyramid();
    public ReportFatherReachedPyramid m_reportFatherReachedPyramid;
    public delegate void ReportFatherReachedStartArea();
    public ReportFatherReachedStartArea m_reportFatherReachedStartArea;

    // Animation Finished
    public delegate void ReportPyramidAnimDone();
    public ReportPyramidAnimDone m_reportPyramidAnimDone;
    public delegate void ReportGrowOldAnimDone();
    public ReportGrowOldAnimDone m_reportGrowOldAnimDone;

    // Start is called before the first frame update
    void Start()
    {
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
        m_spriteAnimator.speed = m_walkSpeed;
    }

    public void Idle()
    {
        m_spriteHandler.GetComponent<Animator>().speed = 0;
    }

    public void ReportSpriteEnterCollision(string tag)
    {
        Debug.Log("Trigger detected: " + tag);
        if (tag == "Pyramid")
        {
            m_spriteAnimator.Play("Idle");
            m_spriteAnimator.speed = 0;
            m_reportFatherReachedPyramid?.Invoke();
        }
        else if (tag == "StartingArea")
        {
            GetComponent<Animator>().Play("Idle");
            m_spriteAnimator.speed = 0;
            m_reportFatherReachedStartArea?.Invoke();
        }
        else if (tag == "Son")
        {
            //m_reportFatherHasPlayerInArea?.Invoke(true);
        }
    }

    public void ReportSpriteExitCollision(string tag)
    {
        Debug.Log("Trigger detected: " + tag);
        if (tag == "Pyramid")
        {
            GetComponent<Animator>().Play("Idle");
            m_spriteHandler.GetComponent<Animator>().speed = 0;
            m_reportFatherReachedPyramid?.Invoke();
        }
        else if (tag == "StartingArea")
        {
            GetComponent<Animator>().Play("Idle");
            m_spriteHandler.GetComponent<Animator>().speed = 0;
            m_reportFatherReachedStartArea?.Invoke();
        }
        else if (tag == "Son")
        {
            //m_reportFatherHasPlayerInArea?.Invoke(true);
        }
    }

    public void ReportAnimationFinished()
    {

    }

    public Transform ReturnSpecialTransform()
    {
        Debug.DrawLine(m_fatherHead.transform.position, m_fatherHead.transform.position + m_fatherHead.transform.right, new Color(256, 0, 0));
        return m_fatherHead.transform;
    }

    public void GetOlder()
    {

    }

    public void RestoreHandlerSpeed()
    {

    }

    public void DestroySelf()
    {
        Destroy(m_spriteHandler);
    }
}
