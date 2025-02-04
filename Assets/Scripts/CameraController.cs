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
    public bool m_pauseMenuIsUp = false;
    public float m_cinematicZoomDistance = 5, m_cinematicZoomDistanceEnding = 20;
    public int m_zoomDistancesIndex;
    public float[] m_zoomDistances;
    public float m_zoomOutSpeed = 1, m_zoomOutSpeedEnding = 0.25f;
    public float m_fadeInEndingCardTime = 3f, m_fadeInEndingCardDelay = 3f;
    public float m_cameraLerpValue = 0f;
    public GameObject m_blankCard;
    public GameObject m_level1CompleteCard;
    public GameObject m_level2CompleteCard;
    public GameObject m_level3CompleteCard;
    public GameObject m_gameOverCard;
    private Color m_blankCardColorSaved;

    public Vector3 m_grandfatherPosOffset;
    public Vector3 m_grandfatherRotOffset;

    private SpriteRenderer m_blankCardSpriteRenderer;
    private SpriteRenderer m_level1CompleteCardSpriteRenderer;
    private SpriteRenderer m_level2CompleteCardSpriteRenderer;
    private SpriteRenderer m_level3CompleteCardSpriteRenderer;
    private SpriteRenderer m_gameOverCardSpriteRenderer;
    private IEnumerator m_fadeInGameOverCardCoroutine;

    public void Start()
    {
        m_blankCardSpriteRenderer = m_blankCard.GetComponent<SpriteRenderer>();
        m_level1CompleteCardSpriteRenderer = m_level1CompleteCard.GetComponent<SpriteRenderer>();
        m_level2CompleteCardSpriteRenderer = m_level2CompleteCard.GetComponent<SpriteRenderer>();
        m_level3CompleteCardSpriteRenderer = m_level3CompleteCard.GetComponent<SpriteRenderer>();
        m_gameOverCardSpriteRenderer = m_gameOverCard.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_playerIsAdult && !PauseInstance.m_isPaused)
        {
            if (Input.GetKeyDown(KeyCode.A) && m_state == CAMERA_STATE.FOLLOW_TARGET_RIGHT) m_state = CAMERA_STATE.FOLLOW_TARGET_LEFT;
            if (Input.GetKeyDown(KeyCode.D) && m_state == CAMERA_STATE.FOLLOW_TARGET_LEFT) m_state = CAMERA_STATE.FOLLOW_TARGET_RIGHT;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) PauseMenuActivate();


        float interp = (m_state != CAMERA_STATE.CINEMATIC ? m_speed : m_cinematicSpeed) * Time.deltaTime;
        Vector3 pos = transform.position;
        if (m_state == CAMERA_STATE.ENDING)
        {
            //transform.rotation = Quaternion.Lerp(transform.rotation, m_cinematicTarget.transform.rotation, m_cinematicRotationSpeed);
            //pos.z = Mathf.Lerp(this.transform.position.z, m_cinematicZoomDistanceEnding, interp);
        }
        else if (m_state == CAMERA_STATE.CINEMATIC)
        {
            transform.rotation = m_cinematicTarget.transform.rotation;
            pos.x = Mathf.Lerp(transform.position.x, m_cinematicTarget.transform.position.x, interp);
            pos.y = Mathf.Lerp(transform.position.y, m_cinematicTarget.transform.position.y, interp);
        }
        else if (m_playerTarget != null)
        {
            transform.rotation = m_playerTarget.transform.rotation;
            Vector3 lookAheadVector = (((m_state == CAMERA_STATE.FOLLOW_TARGET_RIGHT) ? transform.right : -transform.right) * m_lookAhead);
            lookAheadVector += 0.35f * m_lookAhead * transform.up;
            if (m_zoomDistancesIndex < 9)
            {
                pos.x = Mathf.Lerp(transform.position.x, m_playerTarget.transform.position.x + lookAheadVector.x, interp);
                pos.y = Mathf.Lerp(transform.position.y, m_playerTarget.transform.position.y + lookAheadVector.y, interp);
            }
            else // Apply Grandfather offset
            {
                Vector3 tR = m_playerTarget.transform.rotation.eulerAngles;
                float targetDist = Vector3.Distance(m_playerTarget.transform.position, Vector3.zero) + m_grandfatherPosOffset.y;
                Vector3 targetPos = Quaternion.Euler(tR.x, tR.y, tR.z) * new Vector3(0, targetDist, 0);
                pos.x = Mathf.Lerp(transform.position.x, targetPos.x, interp);
                pos.y = Mathf.Lerp(transform.position.y, targetPos.y, interp);
            }
        }
        transform.position = pos;
    }

    public void PauseMenuActivate()
    {
        m_pauseMenuIsUp = !m_pauseMenuIsUp;
        if (m_pauseMenuIsUp)
        {
            m_blankCard.SetActive(true);
            m_blankCardColorSaved = m_blankCardSpriteRenderer.color;
            m_blankCardSpriteRenderer.color = new Color(0, 0, 0, 0.5f);
        }
        else
        {
            m_blankCardSpriteRenderer.color = m_blankCardColorSaved;
            m_blankCard.SetActive(false);
        }
    }

    public void ZoomOut(float amount = 0)
    {
        IEnumerator coroutine = ZoomOutCoroutine(m_state != CAMERA_STATE.ENDING ? amount : m_cinematicZoomDistance, true);
        StartCoroutine(coroutine);
    }

    public void ZoomToValue(float zoomValue = 0)
    {
        IEnumerator coroutine = ZoomOutCoroutine(m_state != CAMERA_STATE.ENDING ? zoomValue : m_cinematicZoomDistance, false);
        StartCoroutine(coroutine);
    }

    public void ZoomToIdx(int zoomIdx = 0)
    {
        m_zoomDistancesIndex = zoomIdx;
        IEnumerator coroutine = ZoomOutCoroutine(m_zoomDistances[m_zoomDistancesIndex], false);
        StartCoroutine(coroutine);
    }

    public IEnumerator ZoomOutCoroutine(float amount = 0, bool shouldIncrement = true)
    {
        float prevAmount = this.GetComponent<Camera>().orthographicSize;
        if (m_state != CAMERA_STATE.ENDING && m_zoomDistancesIndex < m_zoomDistances.Length - 1)
        {
            if(shouldIncrement) ++m_zoomDistancesIndex;
            amount = m_zoomDistances[m_zoomDistancesIndex];
        }
        else
            amount = m_cinematicZoomDistanceEnding;

        float zoomProgress = 0f;
        while (zoomProgress < (m_state != CAMERA_STATE.ENDING ? m_zoomOutSpeed : m_zoomOutSpeedEnding))
        {
            zoomProgress += Time.deltaTime;
            this.GetComponent<Camera>().orthographicSize = Mathf.Lerp(prevAmount, amount, zoomProgress/(m_state != CAMERA_STATE.ENDING ? m_zoomOutSpeed : m_zoomOutSpeedEnding));
            yield return null;
        }
        this.GetComponent<Camera>().orthographicSize = amount;
    }

    public void FadeInBlankCard_Duration_Delay(float duration = 2, float delay = 3) { Debug.Log("FadeIn called"); ; StartCoroutine(FadeInCardCoroutine(m_blankCard, m_blankCardSpriteRenderer, duration, delay)); }
    public void FadeOutBlankCard_Duration_Delay(float duration = 2, float delay = 3) { Debug.Log("FadeOut called"); StartCoroutine(FadeOutCardCoroutine(m_blankCard, m_blankCardSpriteRenderer, duration, delay)); }
    public void FadeInLevel1CompleteCard_Duration_Delay(float duration = 2, float delay = 3) { StartCoroutine(FadeInCardCoroutine(m_level1CompleteCard, m_level1CompleteCardSpriteRenderer, duration, delay)); }
    public void FadeInLevel2CompleteCard_Duration_Delay(float duration = 2, float delay = 3) { StartCoroutine(FadeInCardCoroutine(m_level2CompleteCard, m_level2CompleteCardSpriteRenderer, duration, delay)); }
    public void FadeInLevel3CompleteCard_Duration_Delay(float duration = 2, float delay = 3) { StartCoroutine(FadeInCardCoroutine(m_level3CompleteCard, m_level3CompleteCardSpriteRenderer, duration, delay)); }
    public void FadeInGameOverCard_Duration_Delay(float duration = 2, float delay = 3) { m_fadeInGameOverCardCoroutine = FadeInCardCoroutine(m_gameOverCard, m_gameOverCardSpriteRenderer, duration, delay);
                                                                                         StartCoroutine(m_fadeInGameOverCardCoroutine); }

    public IEnumerator FadeInCardCoroutine(GameObject card, SpriteRenderer cardSpriteRenderer, float fadeDuration = 2, float fadeDelay = 3)
    {
        card.SetActive(true);
        float fadeProgress = 0;
        Color c = cardSpriteRenderer.color;

        yield return new WaitForSeconds(fadeDelay);
        if(fadeDuration > 0)
        {
            c.a = 0;
            cardSpriteRenderer.color = c;
            while (fadeProgress < 1)
            {
                fadeProgress += Time.deltaTime / fadeDuration;
                c.a = Mathf.Lerp(0, 1, fadeProgress);
                cardSpriteRenderer.color = c;
                yield return null;
            }
        }
        c.a = 1;
        cardSpriteRenderer.color = c;
    }
    public IEnumerator FadeOutCardCoroutine(GameObject card, SpriteRenderer cardSpriteRenderer, float fadeDuration, float fadeDelay = 3)
    {
        card.SetActive(true);
        float fadeProgress = 0;
        Color c = cardSpriteRenderer.color;

        yield return new WaitForSeconds(fadeDelay);
        if (fadeDuration > 0)
        {
            c.a = 1;
            cardSpriteRenderer.color = c;
            while (fadeProgress < 1)
            {
                fadeProgress += Time.deltaTime / m_fadeInEndingCardTime;
                c.a = Mathf.Lerp(1, 0, fadeProgress);
                cardSpriteRenderer.color = c;
                yield return null;
            }
        }
        c.a = 0;
        cardSpriteRenderer.color = c;
    }

    public void SetLookAhead_Right(bool lookRight)
    {
        m_lookAhead = (lookRight) ? Mathf.Abs(m_lookAhead) : -1 * Mathf.Abs(m_lookAhead);
    }

    public void EnterCinematicMode(GameObject cinematicTarget = null)
    {
        m_state = CAMERA_STATE.CINEMATIC;
        if (m_cinematicTarget != null)
            m_cinematicTarget = cinematicTarget;
        else
            m_cinematicTarget = gameObject;
        ZoomToValue(m_cinematicZoomDistance);
    }

    public void EnterCinematicEndingMode(GameObject g = null)
    {
        if (g != null)
            m_cinematicTarget = g;
        else
            m_cinematicTarget = gameObject;
        m_state = CAMERA_STATE.ENDING;
        ZoomToValue(m_cinematicZoomDistance);

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
