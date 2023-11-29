using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenWorldRotate : MonoBehaviour
{
    public float m_rotateSpeed = 1f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, m_rotateSpeed * Time.deltaTime);
    }
}
