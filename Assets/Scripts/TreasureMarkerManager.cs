using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureMarkerManager : MonoBehaviour
{
    public GameObject m_marker;
    public Vector3 m_markerOffset;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Treasure") && m_marker != null)
        {
            m_marker.SetActive(true);
            m_marker.transform.parent = collision.transform;
            m_marker.transform.rotation = collision.transform.rotation;
            m_marker.transform.localPosition = m_markerOffset;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Treasure") && m_marker != null)
        {
            m_marker.SetActive(false);
        }
    }
}
