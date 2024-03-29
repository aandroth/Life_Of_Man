using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController_Grandfather : MonoBehaviour, I_SpriteController
{
    public GameObject m_spriteHandler;
    private Animator m_spriteAnimator;
    public GameObject m_moveTarget;
    public float m_moveSpeed = 0.01f;
    public float m_minDistance = 0.001f;
    private IEnumerator coroutine;
    public Vector3 m_offset;
    public GameObject m_sprite;
    public GameObject m_heart;
    public float m_timeMax = 3;
    public float m_timePassed = 0;
    public Vector3 m_carriedByFatherOffset = new Vector3(-1.55f, 2.2f, 0f);
    public GameObject m_targetIndicator;

    public delegate void ReportGrowOldMoveToTargetDone();
    public ReportGrowOldMoveToTargetDone m_reportGrowOldMoveToTargetDone;
    public delegate void ReportRevealPyramid();
    public ReportRevealPyramid m_reportRevealPyramid;    
    public delegate void ReportRevealPyramidDone();
    public ReportRevealPyramidDone m_reportRevealPyramidDone;

    public void Awake()
    {
        m_spriteAnimator = GetComponent<Animator>();
    }
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
            StartCoroutine(Heal());
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
        //transform.rotation = Quaternion.identity;
        m_targetIndicator.transform.SetParent(m_moveTarget.transform);
        m_targetIndicator.transform.localPosition = m_carriedByFatherOffset; //(m_carriedByFatherOffset + m_moveTarget.transform.position);
        GameObject tempGameObject = new GameObject();
        tempGameObject.transform.position = transform.position;
        while (true)
        {
            Vector3 vectorToTarget = m_targetIndicator.transform.position - transform.position;
            float currDistanceToTarget = Vector3.Distance(transform.localPosition, (m_carriedByFatherOffset));
            if (Vector3.Distance(transform.position, m_targetIndicator.transform.position) < m_minDistance)
            {
                transform.localPosition = m_carriedByFatherOffset;
                m_reportGrowOldMoveToTargetDone.Invoke();
                StopCoroutine(coroutine);
            }
            
            tempGameObject.transform.Translate(Vector3.Normalize(vectorToTarget) * m_moveSpeed * 0.1f);
            transform.position = tempGameObject.transform.position;
            Debug.DrawLine(transform.position, m_targetIndicator.transform.position);
            Debug.DrawRay(transform.position, vectorToTarget, Color.red);
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

    public void BeginRevealPyramidAnim()
    {
        m_spriteAnimator.Play("GrandfatherEye");
    }

    public void ReportRevealPyramidEvent()
    {
        m_reportRevealPyramid.Invoke();
    }

    public void ReportRevealPyramidDoneEvent()
    {
        m_reportRevealPyramidDone.Invoke();
    }

    public void DestroySelf()
    {
        Destroy(m_spriteHandler);
        Destroy(gameObject);
    }
}
