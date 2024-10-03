using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public enum STATE { BRONZE, SILVER }
    public STATE m_state = STATE.BRONZE;
    public SpriteRenderer m_shieldSpriteRenderer;
    public Color m_bronzeShieldDefaultColor;
    public Color m_silverShieldDefaultColor;
    public float m_bronzeShieldProtectionTime;
    public IEnumerator m_fadeCoroutine;

    public void Start()
    {
        m_shieldSpriteRenderer = GetComponent<SpriteRenderer>();
        //m_fadeCoroutine = FadeAwayShield();
        //m_fadeCoroutine = FadeAwayShield();
    }

    // Start is called before the first frame update
    public void OnEnable()
    {
        m_fadeCoroutine = FadeAwayShield();
        if (m_state == STATE.BRONZE)
        {
            m_shieldSpriteRenderer.color = m_bronzeShieldDefaultColor;
            StartCoroutine(m_fadeCoroutine);
        }
    }

    public IEnumerator FadeAwayShield()
    {
        while (m_shieldSpriteRenderer.color.a > 0)
        {
            Color temp = m_shieldSpriteRenderer.color;
            temp.a -= (Time.deltaTime * 0.1f) / m_bronzeShieldProtectionTime;
            m_shieldSpriteRenderer.color = temp;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void UpgradeToSilver()
    {
        if(m_fadeCoroutine != null) StopCoroutine(m_fadeCoroutine);
        m_state = STATE.SILVER;
        m_shieldSpriteRenderer.color = m_silverShieldDefaultColor;
    }
}
