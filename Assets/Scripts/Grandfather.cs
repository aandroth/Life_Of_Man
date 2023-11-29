using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandfather : MonoBehaviour
{
    public GameObject m_moveTarget;
    public float m_moveSpeed = 0.01f;
    public float m_minDistance = 0.001f;
    public Vector3 m_offset;
    private IEnumerator coroutine;
    public delegate void ReportGrowOldMoveToTargetDone();
    public ReportGrowOldMoveToTargetDone m_reportGrowOldMoveToTargetDone;

    public void ActivateMoveToTarget(GameObject _target = null)
    {
        m_moveTarget = _target;
        Debug.Log("Activating MoveToTarget");
        transform.SetParent(m_moveTarget.transform);
        coroutine = MoveToTarget();
        StartCoroutine(coroutine);
    }

    public IEnumerator MoveToTarget()
    {
        // Get the position of the 

        Vector3 vectorToTarget = ((m_offset*transform.parent.localScale.x) + transform.parent.position) - transform.position;
        while (true)
        {
            if(Vector3.Distance(transform.localPosition, (m_offset)) < m_minDistance)
            {
                StopCoroutine(coroutine);
                transform.localPosition = m_offset;
                m_reportGrowOldMoveToTargetDone.Invoke();
            }
            
            transform.Translate(Vector3.Normalize(vectorToTarget) * m_moveSpeed);
            yield return null;
        }
    }

    public void EnactHealing()
    {
        GetComponent<Animator>().Play("GrandfatherHeal");
    }
}
