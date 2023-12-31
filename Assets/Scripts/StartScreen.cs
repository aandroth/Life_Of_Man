using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public float m_loadSceneDelay = 2f;
    public float m_fadeTime = 1f;
    public Image m_fadeOutPanel;
    public void ChoseLevel_1()
    {
        IEnumerator c = ChoseLevel_Async("Level1_Scene");
        StartCoroutine(c);
    }
    public void ChoseLevel_2()
    {
        IEnumerator c = ChoseLevel_Async("Level2_Scene");
        StartCoroutine(c);
    }
    public void ChoseLevel_3()
    {
        IEnumerator c = ChoseLevel_Async("Level3_Scene");
        StartCoroutine(c);
    }

    public IEnumerator ChoseLevel_Async(string _levelName)
    {
        float timeFaded = 0;
        while (timeFaded < m_fadeTime)
        {
            timeFaded += Time.deltaTime;
            m_fadeOutPanel.color = new Color(m_fadeOutPanel.color.r, m_fadeOutPanel.color.g, m_fadeOutPanel.color.b, timeFaded / m_fadeTime);
            yield return null;
        }
        m_fadeOutPanel.color = new Color(m_fadeOutPanel.color.r, m_fadeOutPanel.color.g, m_fadeOutPanel.color.b, 1f);

        yield return new WaitForSeconds(m_loadSceneDelay);

        SceneManager.LoadScene(_levelName);
    }
}

