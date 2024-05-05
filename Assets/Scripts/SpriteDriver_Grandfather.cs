using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDriver_Grandfather : SpriteDriver_Abstract
{
    public GameObject m_sprite;
    public new SpriteController_Grandfather m_spriteController;
    public enum GRANDFATHER_STATE { IDLE, HEALING };
    public GRANDFATHER_STATE m_state = GRANDFATHER_STATE.IDLE;
    public CollisionReporter m_collisionReporter = null;
    public GameObject m_sonDetectorPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //CreateSonDetector();
    }

    //public void SonDetected(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Son")
    //    {
    //        m_spriteController.Action();
    //    }
    //}

    //public void CreateSonDetector()
    //{
    //    GameObject go = Instantiate(m_sonDetectorPrefab, transform);
    //    go.name = "SonDetector";
    //    m_collisionReporter = go.GetComponent<CollisionReporter>();
    //    m_collisionReporter.m_reportCollision = SonDetected;
    //}

}
