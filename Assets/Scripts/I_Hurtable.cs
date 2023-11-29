using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Hurtable : MonoBehaviour
{
    public int m_health = 3;
    public GameObject[] m_heartSprites;

    public void TakeDamage()
    {
        --m_health;
        m_heartSprites[m_health].SetActive(false);

        if(m_health == 0)
        {
            // son dies
        }
    }
    public void Heal()
    {
        foreach(GameObject go in m_heartSprites)
        {
            go.SetActive(true);
        }
        m_health = 3;
    }
}
