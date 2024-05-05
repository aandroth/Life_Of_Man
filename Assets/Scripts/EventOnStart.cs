using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnStart : MonoBehaviour
{
    public UnityEvent m_events;
    public bool m_eventStartAfterRandomTime = false;
    public float m_minRandomDelayTime = 0, m_maxRandomDelayTime = 0;
    public float m_delay = 0;
    private float m_delayTimed = 0;
    public bool m_eventsInvoked = false;
    public bool m_resetOnEachStartup = false;

    public void OnEnable()
    {
        if (m_eventStartAfterRandomTime)
        {
            m_delay = Random.Range(m_minRandomDelayTime, m_maxRandomDelayTime);
        }
        m_delayTimed = m_delay;

        if (m_resetOnEachStartup)
            m_eventsInvoked = false;

        if (!m_eventsInvoked)
            StartCoroutine(ExecuteEventsAfterDelay());
    }

    public IEnumerator ExecuteEventsAfterDelay()
    {
        while (m_delayTimed > -1)
        {
            if (!m_eventsInvoked && m_delayTimed <= 0)
            {
                m_events.Invoke();
                m_eventsInvoked = true;
            }

            m_delayTimed -= Time.deltaTime;
            yield return null;
        }
    }
}
