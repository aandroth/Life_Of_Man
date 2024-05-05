using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Father : MonoBehaviour
{
    public GameObject m_head;
    public GameObject m_handler;
    public float m_currTime, m_timeMax;


    public delegate void ReportCollision(string collisionTag);
    public ReportCollision m_reportCollision;
    public delegate void ReportAnimFinished(string animName);
    public ReportAnimFinished m_reportAnimFinished;

    public void LookDown()
    {
        m_head.transform.localEulerAngles = new Vector3(0, 0, -45);
        m_currTime = m_timeMax;
        StartCoroutine(CountdownToLookAhead());
    }
   public void LookAhead()
    {
        m_head.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public IEnumerator CountdownToLookAhead()
    {
        m_currTime -= Time.deltaTime;
        if (m_currTime <= 0)
        {
            LookAhead();
            StopCoroutine(CountdownToLookAhead());
        }
        yield return null;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        m_reportCollision(collision.tag);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trigger exited: " + collision.tag);
        m_reportCollision(collision.tag);
    }

    public void StartFatherPyramidAnim()
    {
        GetComponent<Animator>().Play("FatherPlacesDeadGrandfather");
    }

    public void ReportAnimFinishedInvoke(string animName)
    {
        m_reportAnimFinished?.Invoke(animName);
    }

    public void RestoreHandlerSpeed()
    {
        m_handler.GetComponent<Animator>().speed = 1;
    }
}
