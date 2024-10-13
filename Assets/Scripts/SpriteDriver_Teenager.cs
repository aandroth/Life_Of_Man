using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDriver_Teenager : SpriteDriver_Abstract
{
    public GameObject m_sprite;
    public new SpriteController_Teenager m_spriteController;

    public CollisionReporter m_collisionReporter = null;
    public bool m_forward, m_controlsEnabled = true;
    public GameObject m_target;
    public float m_targetDistanceMin = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteController = m_sprite.GetComponent<SpriteController_Teenager>();
        m_collisionReporter.m_reportCollision = ObjectDetected;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_controlsEnabled)
        {
            if (m_target != null)
            {
                float crossProductResult = transform.up.x * m_target.transform.position.y - transform.up.y * m_target.transform.position.x;
                //Debug.Log($"Teen crossProductResult: {crossProductResult}");

                if (Mathf.Abs(crossProductResult) > m_targetDistanceMin)
                {
                    if (crossProductResult < 0)
                    {
                        m_spriteController.PushForward();
                    }
                    else
                    {
                        m_spriteController.PushBackward();
                    }
                }
            }
        }
    }

    public void ObjectDetected(Collider2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Enemy") && collision.gameObject.GetComponent<Enemy>().m_isChasing)
        {
            m_spriteController.Action();
        }
    }

    public void DisableControls()
    {
        m_controlsEnabled = false;
        m_spriteController.Idle();
    }

    public void EnableControls()
    {
        m_controlsEnabled = true;
    }
}
