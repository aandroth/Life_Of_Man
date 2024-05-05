using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    public delegate void ReportCollision(Collider2D collision);
    public ReportCollision m_reportCollision;
    public delegate void ReportCollisionStopped();
    public ReportCollisionStopped m_reportCollisionStopped;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_reportCollision?.Invoke(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        m_reportCollisionStopped?.Invoke();
    }
}
