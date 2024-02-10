using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject m_playerTarget, m_cinematicTarget;
    public float m_lookAhead = 0.5f;
    public enum CAMERA_STATE { FOLLOW_TARGET_RIGHT, FOLLOW_TARGET_LEFT, CINEMATIC }
    public CAMERA_STATE m_state = CAMERA_STATE.FOLLOW_TARGET_RIGHT;
    public float m_speed = 1f;
    public bool m_playerIsAdult = false;
    public float m_cinematicZoomDistance = 5;
    public int m_zoomDistancesIndex;
    public float[] m_zoomDistances;

    // Update is called once per frame
    void Update()
    {
        if(!m_playerIsAdult)
        {
            if (Input.GetKeyDown(KeyCode.A) && m_state == CAMERA_STATE.FOLLOW_TARGET_RIGHT) m_state = CAMERA_STATE.FOLLOW_TARGET_LEFT;
            if (Input.GetKeyDown(KeyCode.D) && m_state == CAMERA_STATE.FOLLOW_TARGET_LEFT) m_state = CAMERA_STATE.FOLLOW_TARGET_RIGHT;
        }

        float interp = m_speed * Time.deltaTime;
        Vector3 pos = this.transform.position;
        if (m_state == CAMERA_STATE.CINEMATIC)
        {
            transform.rotation = m_cinematicTarget.transform.rotation;
            pos.x = Mathf.Lerp(this.transform.position.x, m_cinematicTarget.transform.position.x, interp);
            pos.y = Mathf.Lerp(this.transform.position.y, m_cinematicTarget.transform.position.y, interp);
        }
        else if(m_playerTarget != null)
        {
            transform.rotation = m_playerTarget.transform.rotation;
            Vector3 lookAheadVector = (((m_state == CAMERA_STATE.FOLLOW_TARGET_RIGHT) ? transform.right : -transform.right) * m_lookAhead);
            lookAheadVector += transform.up * m_lookAhead * 0.35f;
            pos.x = Mathf.Lerp(this.transform.position.x, m_playerTarget.transform.position.x + lookAheadVector.x, interp);
            pos.y = Mathf.Lerp(this.transform.position.y, m_playerTarget.transform.position.y + lookAheadVector.y, interp);
        }
        this.transform.position = pos;
    }

    public void ZoomOut()
    {
        if(m_zoomDistancesIndex < m_zoomDistances.Length-1)
        {
            ++m_zoomDistancesIndex;
            this.GetComponent<Camera>().orthographicSize = m_zoomDistances[m_zoomDistancesIndex];
            //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y,  m_zoomDistances[m_zoomDistancesIndex]);
        }
    }

    public void SetLookAhead_Right(bool lookRight)
    {
        m_lookAhead = (lookRight) ? Mathf.Abs(m_lookAhead) : -1 * Mathf.Abs(m_lookAhead);
    }

    public void EnterCinematicMode()
    {
        m_state = CAMERA_STATE.CINEMATIC;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, m_cinematicZoomDistance);
    }

    public void ExitCinematicMode()
    {
        m_state = CAMERA_STATE.FOLLOW_TARGET_RIGHT;
    }

    public void PlayerBecomesAdult()
    {
        m_playerIsAdult = true;
        m_state = CAMERA_STATE.FOLLOW_TARGET_RIGHT;
    }
}
