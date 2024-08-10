using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFillDrop : MonoBehaviour
{
    public float m_duration = 4;
    private Image m_image;

    // Start is called before the first frame update
    void OnEnable()
    {
        m_image = GetComponent<Image>();
        StartCoroutine(UnfillFromFull());
    }

    public void SetDuration(float duration)
    {
        m_duration = duration;
    }

    public IEnumerator UnfillFromFull()
    {
        m_image.fillAmount = 1;
        Debug.Log($"Unfilling start");
        while (m_image.fillAmount > 0)
        {
            Debug.Log($"Unfilling");
            m_image.fillAmount -= Time.deltaTime / m_duration;
            yield return null;
        }
        //gameObject.SetActive(false);
    }
}
