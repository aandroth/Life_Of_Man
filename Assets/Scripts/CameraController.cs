using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject m_playerTarget, m_cinematicTarget;
    public float m_lookAhead = 0.5f;
    public enum CAMERA_STATE { FOLLOW_TARGET_RIGHT, FOLLOW_TARGET_LEFT, CINEMATIC, ENDING }
    public CAMERA_STATE m_state = CAMERA_STATE.FOLLOW_TARGET_RIGHT;
    public float m_speed = 1f, m_cinematicSpeed = 0.25f, m_cinematicRotationSpeed = 0.25f;
    public bool m_playerIsAdult = false;
    public float m_cinematicZoomDistance = 5, m_cinematicZoomDistanceEnding = 20;
    public int m_zoomDistancesIndex;
    public float[] m_zoomDistances;
    public float m_zoomOutSpeed = 1, m_zoomOutSpeedEnding = 0.25f;
    public float m_fadeInEndingCardTime = 3f, m_fadeInEndingCardDelay = 3f;
    public float m_cameraLerpValue = 0f;
    public GameObject m_endingCard;
    private SpriteRenderer m_endingCardSpriteRenderer;

    public void Start()
    {
        m_endingCardSpriteRenderer = m_endingCard.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_playerIsAdult)
        {
            if (Input.GetKeyDown(KeyCode.A) && m_state == CAMERA_STATE.FOLLOW_TARGET_RIGHT) m_state = CAMERA_STATE.FOLLOW_TARGET_LEFT;
            if (Input.GetKeyDown(KeyCode.D) && m_state == CAMERA_STATE.FOLLOW_TARGET_LEFT) m_state = CAMERA_STATE.FOLLOW_TARGET_RIGHT;
        }

        float interp = (m_state != CAMERA_STATE.CINEMATIC ? m_speed : m_cinematicSpeed) * Time.deltaTime;
        Vector3 pos = this.transform.position;
        if (m_state == CAMERA_STATE.ENDING)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, m_cinematicTarget.transform.rotation, m_cinematicRotationSpeed);
            pos.x = Mathf.Lerp(this.transform.position.x, m_cinematicTarget.transform.position.x, interp);
            pos.y = Mathf.Lerp(this.transform.position.y, m_cinematicTarget.transform.position.y, interp);
        }
        else if (m_state == CAMERA_STATE.CINEMATIC)
        {
            transform.rotation = m_cinematicTarget.transform.rotation;
            pos.x = Mathf.Lerp(this.transform.position.x, m_cinematicTarget.transform.position.x, interp);
            pos.y = Mathf.Lerp(this.transform.position.y, m_cinematicTarget.transform.position.y, interp);
        }
        else if(m_playerTarget != null)
        {
            transform.rotation = m_playerTarget.transform.rotation;
            Vector3 lookAheadVector = (((m_state == CAMERA_STATE.FOLLOW_TARGET_RIGHT) ? transform.right : -transform.right) * m_lookAhead);
            lookAheadVector += 0.35f * m_lookAhead * transform.up;
            pos.x = Mathf.Lerp(this.transform.position.x, m_playerTarget.transform.position.x + lookAheadVector.x, interp);
            pos.y = Mathf.Lerp(this.transform.position.y, m_playerTarget.transform.position.y + lookAheadVector.y, interp);
        }
        this.transform.position = pos;
    }

    public void ZoomOut(float amount = 0)
    {
        IEnumerator coroutine = ZoomOutCoroutine(m_state != CAMERA_STATE.ENDING ? amount : m_cinematicZoomDistance);
        StartCoroutine(coroutine);
    }

    public IEnumerator ZoomOutCoroutine(float amount = 0)
    {
        float prevAmount = this.GetComponent<Camera>().orthographicSize;
        if (amount == 0 && m_zoomDistancesIndex < m_zoomDistances.Length-1)
        {
            ++m_zoomDistancesIndex;
            amount = m_zoomDistances[m_zoomDistancesIndex];
        }

        while (this.GetComponent<Camera>().orthographicSize < m_zoomDistances[m_zoomDistancesIndex])
        {
            this.GetComponent<Camera>().orthographicSize = Mathf.Lerp(prevAmount, amount, (m_state != CAMERA_STATE.ENDING ? m_zoomOutSpeed : m_zoomOutSpeedEnding));
            yield return null;
        }
        this.GetComponent<Camera>().orthographicSize = amount;
    }

    public IEnumerator FadeInEndingCardCoroutine()
    {
        m_endingCard.SetActive(true);
        float fadeProgress = 0;
        Color c;

        while (m_fadeInEndingCardDelay > 0)
        {
            m_fadeInEndingCardDelay -= Time.deltaTime;
            yield return null;
        }

        while (fadeProgress < 1)
        {
            fadeProgress += Time.deltaTime / m_fadeInEndingCardTime;
            c = m_endingCardSpriteRenderer.color;
            c.a = Mathf.Lerp(0, 1, fadeProgress);
            m_endingCardSpriteRenderer.color = c;
            yield return null;
        }
        c = m_endingCardSpriteRenderer.color;
        c.a = 1;
        m_endingCardSpriteRenderer.color = c;
    }

    public void SetLookAhead_Right(bool lookRight)
    {
        m_lookAhead = (lookRight) ? Mathf.Abs(m_lookAhead) : -1 * Mathf.Abs(m_lookAhead);
    }

    public void EnterCinematicMode(GameObject cinematicTarget)
    {
        m_state = CAMERA_STATE.CINEMATIC;
        m_cinematicTarget = cinematicTarget;
        ZoomOut(m_cinematicZoomDistance);
    }

    public void EnterCinematicEndingMode(GameObject g = null)
    {
        if (g != null)
            m_cinematicTarget = g;
        StartCoroutine(FadeInEndingCardCoroutine());
        m_state = CAMERA_STATE.ENDING;
        ZoomOut(m_cinematicZoomDistance);

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
