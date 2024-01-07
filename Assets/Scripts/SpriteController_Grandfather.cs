using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController_Grandfather : MonoBehaviour, I_SpriteController
{
    public GameObject m_sprite;
    public GameObject m_spriteHandler;
    public GameObject m_innerHandler;
    public GameObject m_heart;
    public float m_timeMax = 3;
    public float m_timePassed = 0;

    public void PushForward()
    {
    }
    public void PushBackward()
    {
    }
    public void Action()
    {
        // Heal son
        if (m_timePassed == 0)
            Heal();
    }

    public IEnumerator Heal()
    {
        // Play healing animation
        m_heart.GetComponent<Animator>().Play("Heart_Healing");
        // Heal all Hurtable objects in area

        while(m_timePassed < m_timeMax)
        {
            m_timePassed += Time.deltaTime;
            yield return null;
        }
        m_timePassed = 0;
    }

    public void Idle()
    {
    }

    public void ReportSpriteEnterCollision(string tag)
    {

    }
    public Transform ReturnSpecialTransform()
    {
        return null;
    }

    public void GetOlder()
    {

    }

    public void RestoreHandlerSpeed()
    {

    }

    public void DestroySelf()
    {
        Destroy(m_spriteHandler);
    }
}
