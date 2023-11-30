using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject m_player, m_father, m_grandfather;
    public Vector3 m_playerStartPosition, m_fatherStartPosition, m_grandfatherStartPosition;
    public float m_worldStartRotation;
    public ParticleSystem m_playerStartParticleBurst;
    public GameObject m_target;
    public float m_lookAhead = 0f;
    //public 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_target.transform.position.x, m_target.transform.position.y, transform.position.z);
        transform.rotation = m_target.transform.rotation;
    }

    public void BeginGame()
    {

    }
}
