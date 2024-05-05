using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDriver_Child : SpriteDriver_Abstract
{
    public GameObject m_sprite;
    public new SpriteController_Child m_spriteController;
    public float m_timeMax = 3;
    public float m_timePassed = 0;
    public bool m_forward, m_controlsEnabled = true;
    public GameObject m_target;
    public float m_jumpThreshold = 0.01f;

    void Start()
    {
        m_controlsEnabled = true;
        //m_spriteController.m_jumpForce *= 0.1f;
    }

    void Update()
    {
        if (m_controlsEnabled)
        {
            if (m_target != null)
            {
                float crossProductResult = transform.up.x * m_target.transform.position.y - transform.up.y * m_target.transform.position.x;

                if (Mathf.Abs(crossProductResult) < m_jumpThreshold)
                    m_spriteController.Action();
                else
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

    public void DisableControls()
    {
        m_controlsEnabled = false;
    }

    public void EnableControls()
    {
        m_controlsEnabled = true;
    }
}
