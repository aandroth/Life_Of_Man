using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public float m_loadSceneDelay = 2f;
    public float m_fadeTime = 1f;
    public Image m_fadeOutPanel;

    public List<GameObject> m_levelButtonsList;


    public void OnEnable()
    {
        DataManager dM = GameObject.FindObjectOfType<DataManager>();

#if UNITY_EDITOR
        Debug.Log($"Start Screen OnEnable()");
#endif
        if (dM != null)
        {
            if(dM.m_gameData == null)
                dM.Load();

            if (dM.LevelUnlockedCheck(2))
            {
                m_levelButtonsList[1].SetActive(true);
#if UNITY_EDITOR
                Debug.Log($"Level 2 showing");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log($"Level 2 hidden");
#endif
            }
            if (dM.LevelUnlockedCheck(3))
            {
                m_levelButtonsList[2].SetActive(true);
#if UNITY_EDITOR
                Debug.Log($"Level 3 showing");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log($"Level 3 hidden");
#endif
            }
        }
    }

    public void ChoseLevel_1()
    {
        IEnumerator c = LoadLevel_Async("Level1_Scene");
        StartCoroutine(c);
    }
    public void ChoseLevel_2()
    {
        IEnumerator c = LoadLevel_Async("Level2_Scene");
        StartCoroutine(c);
    }
    public void ChoseLevel_3()
    {
        IEnumerator c = LoadLevel_Async("Level3_Scene");
        StartCoroutine(c);
    }
    public void Exit()
    {
        Application.Quit();
    }

    public IEnumerator LoadLevel_Async(string _levelName)
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

