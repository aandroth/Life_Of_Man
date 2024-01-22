using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController_Grandfather : MonoBehaviour, I_SpriteController
{
    public GameObject m_moveTarget;
    public float m_moveSpeed = 0.01f;
    public float m_minDistance = 0.001f;
    private IEnumerator coroutine;
    public Vector3 m_offset;
    public GameObject m_sprite;
    public GameObject m_heart;
    public float m_timeMax = 3;
    public float m_timePassed = 0;
    public Vector3 m_carriedByFatherOffset = new Vector3(-1.55f, 2.29f, 0f);

    public delegate void ReportGrowOldMoveToTargetDone();
    public ReportGrowOldMoveToTargetDone m_reportGrowOldMoveToTargetDone;


    public void PushForward()
    {
    }
    public void PushBackward()
    {
    }
    public void Action()
    {
        // Heal son
        if (m_timePassed == 0)
            Heal();
    }

    public IEnumerator Heal()
    {
        // Play healing animation
        m_heart.GetComponent<HealingHeart>().HealAllHurtables();
        while(m_timePassed < m_timeMax)
        {
            m_timePassed += Time.deltaTime;
            yield return null;
        }
        m_timePassed = 0;
    }

    public void Idle()
    {
    }

    public void BeginMoveToTarget(GameObject _target)
    {
        m_moveTarget = _target;
        Debug.Log("Activating MoveToTarget");
        transform.SetParent(m_moveTarget.transform);
        coroutine = MoveToTargetCoroutine();
        StartCoroutine(coroutine);
    }

    public IEnumerator MoveToTargetCoroutine()
    {
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
    public void ReportSpriteEnterCollision(string tag)
    {

    }
    public Transform ReturnSpecialTransform()
    {
        return null;
    }

    public void GetOlder()
    {

    }

    public void RestoreHandlerSpeed()
    {

    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
