using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherEyeRay : MonoBehaviour
{
    public float m_eyeRayLength = 100f;
    public LayerMask m_layerMask = 7;
    public GameObject m_worldHitPos;
    public float m_worldSurface = 80;

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), m_eyeRayLength, m_layerMask);
        if (hit)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right) * m_eyeRayLength);
            m_worldHitPos.transform.position = new Vector3(hit.point.x, hit.point.y, 0);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right) * m_eyeRayLength);
        }
    }
}
