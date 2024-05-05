using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingHeart : MonoBehaviour
{
    public float m_healingRadius = 12;
    public Vector3 m_centerOffset = Vector3.zero;
    public Animator m_animator;
    public string m_heartHealAnimName = "Heart_Healing";

    public void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void HealAllHurtables()
    {
        Debug.Log($"Heart Heal");
        m_animator.Play(m_heartHealAnimName);
        Collider2D[] cArr = Physics2D.OverlapCircleAll(m_centerOffset, m_healingRadius);
        foreach (Collider2D c in cArr)
            if (c.gameObject.GetComponent<I_Hurtable>())
                c.gameObject.GetComponent<I_Hurtable>().Heal();
    }
}
