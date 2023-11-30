using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDriver_Son : SpriteDriver_Abstract
{
    public GameObject m_sprite;
    public float m_timeMax = 3;
    public float m_timePassed = 0;
    public bool m_forward;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteController = m_sprite.GetComponent<SpriteController_Son>();
    }

    // Update is called once per frame
    void Update()
    {
        m_timePassed += Time.deltaTime;
        if (m_timePassed > m_timeMax) {
            m_timePassed = 0;
            m_forward = !m_forward;
        }

        if (m_forward)
        {
            m_spriteController.PushForward();
        }
        else
        {
            m_spriteController.PushBackward();
        }
    }
}
