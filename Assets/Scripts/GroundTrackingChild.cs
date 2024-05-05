using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrackingChild : MonoBehaviour
{
    public GameObject m_target;
    public float m_distance = 79.61f;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = m_target.transform.localPosition;
        pos.y = m_distance;
        transform.localPosition = pos;
    }
}
