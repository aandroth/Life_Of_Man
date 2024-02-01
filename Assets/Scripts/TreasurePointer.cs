using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasurePointer : MonoBehaviour
{
    public GameObject m_target = null;
    public GameObject m_pointer;

    public void FixedUpdate()
    {
        if(m_target != null)
        {
            m_pointer.transform.right = Vector3.Normalize(m_target.transform.position - m_pointer.transform.position);
        }
    }

    public void SetTarget(GameObject target)
    {
        m_target = target;
    }

    
    public void ClearTarget()
    {
        m_target = null;
    }
}
