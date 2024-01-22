using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingHeart : MonoBehaviour
{
    public float m_healingRadius = 12;
    public Vector3 m_centerOffset = new Vector3(0,0,0);

    public void HealAllHurtables()
    {
        Collider2D[] cArr = Physics2D.OverlapCircleAll(m_centerOffset, m_healingRadius);
        foreach (Collider2D c in cArr)
            if (c.gameObject.GetComponent<I_Hurtable>())
                c.gameObject.GetComponent<I_Hurtable>().Heal();
    }
}
