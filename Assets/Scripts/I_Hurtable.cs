using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_Hurtable : MonoBehaviour
{
    public int m_health = 3;
    public delegate void ReportTookDamage(int i);
    public ReportTookDamage m_reportHealthChangedAndIsNow;

    public void TakeDamage()
    {
        --m_health;

        if(m_health < 0)
        {
            m_health = 0;
        }
        m_reportHealthChangedAndIsNow.Invoke(m_health);
    }
    public void Heal()
    {
        m_health = 3;

        m_reportHealthChangedAndIsNow(3);
    }
}
