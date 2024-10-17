using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenWorldRotate : MonoBehaviour
{
    public float m_rotateSpeed = 1f;
    public float slowdown = 0.001f;


    void Update()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot.z = Mathf.LerpAngle(transform.rotation.eulerAngles.z, transform.rotation.eulerAngles.z+5f, Time.deltaTime*m_rotateSpeed*slowdown);
        transform.eulerAngles = rot;
    }
}
