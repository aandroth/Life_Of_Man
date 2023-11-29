using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Father : MonoBehaviour
{
    public GameObject m_head;
    public GameObject m_handler;
    public float m_currTime, m_timeMax;
    public delegate void ReportFatherHasPlayerInArea();
    public ReportFatherHasPlayerInArea m_reportFatherHasPlayerInArea;
    public delegate void ReportFatherReachedPyramid();
    public ReportFatherReachedPyramid m_reportFatherReachedPyramid;
    public delegate void ReportFatherReachedStartArea();
    public ReportFatherReachedStartArea m_reportFatherReachedStartArea;
    public delegate void ReportPyramidAnimDone();
    public ReportPyramidAnimDone m_reportPyramidAnimDone;
    public delegate void ReportGrowOldAnimDone();
    public ReportGrowOldAnimDone m_reportGrowOldAnimDone;


    public void LookDown()
    {
        m_head.transform.localEulerAngles = new Vector3(0, 0, -45);
        m_currTime = m_timeMax;
        StartCoroutine("CountdownToLookAhead");
    }
   public void LookAhead()
    {
        m_head.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public IEnumerable CountdownToLookAhead()
    {
        m_currTime -= Time.deltaTime;
        if (m_currTime <= 0)
        {
            LookAhead();
            StopCoroutine("CountdownToLookAhead");
        }
        yield return null;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger detected: " + collision.tag);
        if (collision.tag == "Pyramid")
        {
            GetComponent<Animator>().Play("Idle");
            m_handler.GetComponent<Animator>().speed = 0;
            m_reportFatherReachedPyramid?.Invoke();
        }
        else if (collision.tag == "StartingArea")
        {
            GetComponent<Animator>().Play("Idle");
            m_handler.GetComponent<Animator>().speed = 0;
            m_reportFatherReachedStartArea?.Invoke();
        }
        else if (collision.tag == "Son")
        {
            //m_reportFatherHasPlayerInArea?.Invoke(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trigger detected: " + collision.tag);
        if (collision.tag == "Pyramid")
        {
            GetComponent<Animator>().Play("Idle");
            m_handler.GetComponent<Animator>().speed = 0;
            m_reportFatherReachedPyramid?.Invoke();
        }
        else if (collision.tag == "StartingArea")
        {
            //GetComponent<Animator>().Play("Idle");
            m_handler.GetComponent<Animator>().speed = 0;
            m_reportFatherReachedStartArea?.Invoke();
        }
        else if (collision.tag == "Son")
        {
            //m_reportFatherHasPlayerInArea.Invoke();
        }
    }

    public void StartFatherPyramidAnim()
    {
        GetComponent<Animator>().Play("FatherPlacesDeadGrandfather");
    }

    public void ReportPyramidAnimDoneInvoke()
    {
        m_reportPyramidAnimDone?.Invoke();
    }

    public void ReportGrowOldAnimDoneInvoke()
    {
        m_reportGrowOldAnimDone?.Invoke();
    }

    public void RestoreHandlerSpeed()
    {
        m_handler.GetComponent<Animator>().speed = 1;
    }
}
