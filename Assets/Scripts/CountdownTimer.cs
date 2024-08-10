using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public delegate void ReportRanOutOfTime();
    public ReportRanOutOfTime m_reportRanOutOfTime;
    public int m_seconds = 160; private int m_secondsTotal = 160;
    public TextMeshProUGUI m_timerText;
    public Image m_circle;
    public IEnumerator m_runningTimerCoroutine;
    // Start is called before the first frame update
    //public void OnEnable()
    //{
    //    //StartTimer(160);
    //    //StartTimer(4);
    //}

    public void StartTimer(int seconds)
    {
        m_seconds = seconds;
        m_secondsTotal = seconds;
        m_timerText.text = $"{m_seconds}";
        if (m_timerText.color != Color.yellow)
        {
            m_timerText.color = Color.yellow;
            m_circle.color = Color.white;
            m_circle.fillAmount = 1;
        }
        m_runningTimerCoroutine = RunningTimer();
        StartCoroutine(m_runningTimerCoroutine);
    }

    public void PauseTimer()
    {
        StopCoroutine(m_runningTimerCoroutine);
    }

    public void StartFadeTimer()
    {
        StopCoroutine(m_runningTimerCoroutine);
        StartCoroutine(FadeTimer());
    }

    IEnumerator FadeTimer(float duration = 3)
    {
        Color cColor = m_circle.color;
        Color tColor = m_timerText.color;
        while(m_timerText.color.a > 0)
        {
            float timeChange = Time.deltaTime * (1 / duration);
            float tempC = cColor.a - timeChange;
            float tempT = tColor.a - timeChange;
            cColor.a = tempC;
            tColor.a = tempT;
            m_circle.color = cColor;
            m_timerText.color = tColor;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    IEnumerator RunningTimer()
    {
        while(m_seconds > 0)
        {
            yield return new WaitForSeconds(1);
            --m_seconds;
            m_circle.fillAmount -= (float)(1.0 / (float)m_secondsTotal);
            m_timerText.text = $"{m_seconds}";

            if (m_timerText.color == Color.yellow && m_seconds < 11)
            {
                m_timerText.color = Color.red;
                m_circle.color = Color.red;
            }
        }

        m_seconds = 3;
        StartCoroutine(FadeTimer(3));

        while (m_seconds > 0)
        {
            yield return new WaitForSeconds(1);
            --m_seconds;
        }

        if(m_reportRanOutOfTime != null) m_reportRanOutOfTime.Invoke();
    }
}
