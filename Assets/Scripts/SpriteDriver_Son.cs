using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDriver_Son : SpriteDriver_Abstract
{
    public GameObject m_sprite;
    public float m_timeMax = 3;
    public float m_timePassed = 0;
    public bool m_forward;
    public GameObject m_target;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteController = m_sprite.GetComponent<SpriteController_Son>();
    }

    // Update is called once per frame
    void Update()
    {
        float crossProductResult = transform.up.x * m_target.transform.localPosition.y - transform.up.y * m_target.transform.localPosition.x;
        //Debug.Log($"Getting {crossProductResult} from ({transform.up.x}, {transform.up.y} X ({m_target.transform.localPosition.y}, {m_target.transform.localPosition.x})");
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
