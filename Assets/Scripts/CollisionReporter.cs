using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    public delegate void ReportCollision(Collider2D collision);
    public ReportCollision m_reportCollision;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_reportCollision?.Invoke(collision);
    }
}
