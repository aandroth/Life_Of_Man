using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    public delegate void ReportCollision(Collider2D collision);
    public ReportCollision m_reportCollision;
    public delegate void ReportCollisionStopped();
    public ReportCollisionStopped m_reportCollisionStopped;
    public GameObject m_marker, m_markerPrefab;
    public Vector3 m_markerOffset;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Treasure")
        {
            if(m_markerPrefab != null)
            {
                m_marker = Instantiate(m_markerPrefab, collision.transform);
                m_marker.transform.rotation = collision.transform.rotation;
                m_marker.transform.localPosition = m_markerOffset;
            }
            m_reportCollision?.Invoke(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Treasure")
        {
            m_reportCollisionStopped?.Invoke();
            if(m_marker != null)
                Destroy(m_marker);
        }
    }
}
