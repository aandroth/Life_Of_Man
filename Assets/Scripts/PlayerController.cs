using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject m_sprite;
    public I_SpriteController m_spriteController;
    public Rigidbody2D m_rb;
    public bool m_inputControlsAreOn = true;
    public CameraController m_cameraController;

    public int m_growthCount = 0;

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
        m_spriteController = m_sprite.GetComponent<I_SpriteController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_inputControlsAreOn)
        {
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
                m_spriteController.GetOlder();
            }

            //if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            //{
            //    if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            //        m_spriteController.Idle();
            //}
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
        m_state = PLAYER_STATE.FATHER;
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
