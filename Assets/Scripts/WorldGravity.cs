using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGravity : MonoBehaviour
{
    public float m_gravity = 8.5f;
    public Rigidbody2D m_rb = null;

    public void Start()
    {
        m_rb = gameObject.GetComponent<Rigidbody2D>();
        if(m_rb == null)
        {
            Debug.LogError($"WorldGravity requires a RigidBody2D to function!");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_rb.AddForce(-gameObject.transform.up * m_gravity);
    }
}
