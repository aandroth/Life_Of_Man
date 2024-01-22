using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDriver_Father : SpriteDriver_Abstract
{
    public GameObject m_sprite;
    [SerializeField]
    public new SpriteController_Father m_spriteController;
    public float m_timeMax = 3;
    public float m_timePassed = 0;
    public GameObject m_currTarget = null;
    public GameObject m_downTarget = null;
    public Vector3 m_downAndRightPos;
    public enum FATHER_STATE {LOOKING_DOWN, LOOKING_AT_TREASURE};
    public FATHER_STATE m_state = FATHER_STATE.LOOKING_DOWN;
    public bool m_hasHeart = false;
    public CollisionReporter m_collisionReporter = null;
    public GameObject m_hiddenObjectDetectorPrefab;
    public float m_fatherLookAtThreshold = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        CreateLookDownTarget();
        CreateHiddenObjectDetector();
        m_currTarget = m_downTarget;
    }

    // Update is called once per frame
    void Update()
    {
        Transform fatherHeadTransform = m_spriteController.ReturnSpecialTransform();
        Vector3 targetRelativeToFatherHead = m_currTarget.transform.position - fatherHeadTransform.position;

        float crossProductResult = fatherHeadTransform.right.x * targetRelativeToFatherHead.y - fatherHeadTransform.transform.right.y * targetRelativeToFatherHead.x;

        if(Mathf.Abs(crossProductResult) > m_fatherLookAtThreshold)
        if (crossProductResult < 0)
        {
            m_spriteController.PushForward();
        }
        else
        {
            m_spriteController.PushBackward();
        }

        if (m_hasHeart)
        {

        }
    }

    public void ObjectDetected(Collider2D collision)
    {
        if(collision.gameObject.tag == "Treasure")
        {
            //Debug.Log($"Treasure detected");
            m_state = FATHER_STATE.LOOKING_AT_TREASURE;
            m_currTarget = collision.gameObject;
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            EnterLookDownState();
        }
    }

    public void EnterLookDownState()
    {
        //Debug.Log($"Enemy detected");
        m_state = FATHER_STATE.LOOKING_DOWN;
        m_currTarget = m_downTarget;
    }

    public void CreateHiddenObjectDetector()
    {
        GameObject go = Instantiate(m_hiddenObjectDetectorPrefab, transform);
        m_collisionReporter = go.GetComponent<CollisionReporter>();
        m_collisionReporter.m_reportCollision = ObjectDetected;
    }

    public void CreateLookDownTarget()
    {
        m_downTarget = new GameObject();
        m_downTarget.name = "FatherLookDownTarget";
        m_downTarget.transform.SetParent(transform);
        m_downTarget.transform.localPosition = transform.localPosition + m_downAndRightPos;
    }
}
