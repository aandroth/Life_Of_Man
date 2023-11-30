using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject m_theWorld;
    public I_SpriteController m_spriteController;
    public GameObject m_sprite;
    public GameObject m_spriteHandler;
    public GameObject m_childSprite;
    public GameObject m_childSpriteHandler;
    public GameObject m_fatherSprite;
    public GameObject m_fatherSpriteHandler;
    public GameObject m_fatherHead;
    public GameObject m_fatherVision;
    public GameObject m_grandfatherSprite;
    public GameObject m_grandfatherSpriteHandler;
    public Camera m_mainCamera;
    public Rigidbody2D m_rb;
    public float m_speed = 10;
    public float m_jumpForce = 10;
    public bool m_canJump;
    public bool m_inputControlsAreOn = true;

    public float m_growthRate = 0.1f;
    public float m_speedRate = 0.9f;
    public float m_fatherSpeedRate = 0.5f;
    public float m_jumpRate = 0.1f;
    public float m_cameraZoomOutRate = 0.1f;
    public float m_cameraPanDownRate = 0.5f;

    public float[] m_cameraPanValues;
    public int m_cameraPanValuesIndex = 0;

    public float m_cameraSizeValueAdult = 13;
    public Vector2 m_cameraPanValueAdult = new Vector2(0.3f, -0.12f);
    public float m_adultSpeed = 0.1f;

    public int m_growthCount;
    public float m_fatherHeadRotateSpeed = 1;

    public enum PLAYER_STATE {CHILD, FATHER, GRANDFATHER}
    public PLAYER_STATE m_state = PLAYER_STATE.CHILD;

    public delegate void GameControllerReportYoungAdult();
    public GameControllerReportYoungAdult m_gameControllerReportYoungAdult;
    public delegate void GameControllerReportFullAdult();
    public GameControllerReportFullAdult m_gameControllerReportFullAdult;
    public delegate void GameControllerReportPlayerAtPyramid(bool b);
    public GameControllerReportPlayerAtPyramid m_gameControllerReportPlayerAtPyramid;
    public delegate void GameControllerReportPlayerAtStartingArea(bool b);
    public GameControllerReportPlayerAtStartingArea m_gameControllerReportPlayerAtStartingArea;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_mainCamera = Camera.main;
        m_sprite = m_childSprite;
        m_spriteController = m_sprite.GetComponent<SpriteController_Son>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_inputControlsAreOn)
        {
            switch (m_state)
            {
                case PLAYER_STATE.CHILD:
                    if (Input.GetKey(KeyCode.D))
                    {
                        m_spriteController.PushForward();
                    }
                    else if (Input.GetKey(KeyCode.A))
                    {
                        m_spriteController.PushBackward();
                    }
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        m_spriteController.Action();
                    }
                    if (Input.GetKeyUp(KeyCode.E) && m_growthCount < 4)
                    {
                        GetOlder();
                    }

                    if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                    {
                        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                            m_spriteController.Idle();
                    }

                    break;
                case PLAYER_STATE.FATHER:
                    m_theWorld.transform.Rotate(new Vector3(0, 0, 1), m_fatherSpeedRate * Time.deltaTime);
                    if (Input.GetKey(KeyCode.D))
                    {
                        m_fatherHead.transform.Rotate(transform.forward, -m_fatherHeadRotateSpeed);
                    }
                    else if (Input.GetKey(KeyCode.A))
                    {
                        m_fatherHead.transform.Rotate(transform.forward, m_fatherHeadRotateSpeed);
                    }
                    break;
                case PLAYER_STATE.GRANDFATHER:
                    break;
            }
        }

    }

    public void GetOlder()
    {
        float newScale = m_spriteHandler.transform.localScale.x + m_growthRate;
        m_spriteHandler.transform.localScale = new Vector3(newScale, newScale, newScale);

        m_speed *= m_speedRate;
        m_jumpForce += m_jumpRate;

        ++m_growthCount;

        if (m_growthCount == 6)
        {
            m_fatherSprite.SetActive(false);
            m_grandfatherSprite.SetActive(true);
            m_spriteHandler = m_grandfatherSpriteHandler;
            m_sprite = m_grandfatherSprite;
            gameObject.tag = "Grandfather";
            m_canJump = false;
        }
        else if (m_growthCount == 5)
        {

            m_gameControllerReportFullAdult.Invoke();
            gameObject.tag = "Father";
            m_jumpForce = 0;
            m_mainCamera.orthographicSize = m_cameraSizeValueAdult;
            m_mainCamera.transform.localPosition = new Vector3(m_cameraPanValueAdult.x, m_cameraPanValueAdult.y, m_mainCamera.transform.localPosition.z);
        }
        else // if (m_growthCount < 4)
        {
            m_mainCamera.transform.localPosition = new Vector3(0, m_cameraPanValues[m_cameraPanValuesIndex], m_mainCamera.transform.localPosition.z);
            if (m_cameraPanValuesIndex < m_cameraPanValues.Length) { ++m_cameraPanValuesIndex; }
            m_mainCamera.orthographicSize = m_mainCamera.orthographicSize + m_cameraZoomOutRate; 
            
            if (m_growthCount == 2)
            {
                m_childSprite.SetActive(false);
                m_fatherSprite.SetActive(true);
                m_sprite = m_fatherSprite;
                m_spriteHandler = m_fatherSpriteHandler;
                GetComponent<I_Hurtable>().enabled = false;
                m_gameControllerReportYoungAdult.Invoke();
            }
        }
        
    }

    public void ReportYoungAdult()
    {
        m_gameControllerReportYoungAdult.Invoke();
    }

    public void ReportFullAdult()
    {
        m_gameControllerReportFullAdult.Invoke();
    }

    public void BecomeFather()
    {
        transform.localScale = new Vector3(50, 50, 1);
        m_fatherVision.SetActive(true);
        m_state = PLAYER_STATE.FATHER;
        m_fatherSprite.tag = "Father";
    }

    public void BuryFather()
    {
        Debug.Log("Called BuryFather");
        m_sprite.GetComponent<Animator>().Play("FatherPlacesDeadGrandfather");
    }
    public void BuryFatherDone()
    {
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && m_growthCount < 4)
        {
            m_canJump = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Treasure" && m_growthCount < 4)
        {
            GetOlder();
            collision.gameObject.GetComponent<Treasure>().DestroyHandler();
        }
        if (collision.tag == "Pyramid")
        {
            m_gameControllerReportPlayerAtPyramid.Invoke(true);
        }
        else if(collision.tag == "StartingArea")
        {
            if (m_growthCount == 4)
                m_gameControllerReportPlayerAtStartingArea.Invoke(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Pyramid")
        {
            m_gameControllerReportPlayerAtPyramid.Invoke(false);
        }
        else if(collision.tag == "StartingArea")
        {
            m_gameControllerReportPlayerAtStartingArea.Invoke(false);
        }
    }

    public void DisablePlayerControls()
    {
        m_inputControlsAreOn = false;
        m_sprite.GetComponent<Animator>().Play("Idle");
    }

    public void EnablePlayerControls()
    {
        m_inputControlsAreOn = true;
    }
}
