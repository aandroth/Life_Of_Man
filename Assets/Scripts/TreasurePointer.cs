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
            Vector3 tPos = new Vector3(m_target.transform.position.x, m_target.transform.position.y, m_pointer.transform.position.z);
            m_pointer.transform.right = Vector3.Normalize(tPos - m_pointer.transform.position);
            //m_pointer.transform.eulerAngles = new Vector3(0, 0, m_pointer.transform.rotation.eulerAngles.z);
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
