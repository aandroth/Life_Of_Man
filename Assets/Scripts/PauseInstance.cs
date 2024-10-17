using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseInstance : MonoBehaviour
{
    public static bool m_isPaused = false;
    public GameObject m_pauseScreen;
    private static PauseInstance m_instance = null;
    public GameController m_gameController = null;

    public void Start()
    {
        if (m_instance != null)
            Destroy(this.gameObject);
        else
        {
            m_instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (m_gameController == null)
            m_gameController = GameObject.FindObjectOfType<GameController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name != "StartScene")
            {
                m_isPaused = !m_isPaused;
                Pause();
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = m_isPaused ? 0 : 1;
        m_pauseScreen.SetActive(m_isPaused);
    }

    public void ResetLevel()
    {
        m_isPaused = false;
        Pause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetCheckpoint()
    {
        m_isPaused = false;
        Pause();
        m_gameController.LoadLastCheckpoint();
    }

    public void BackToStartMenu()
    {
        m_isPaused = false;
        Pause();
        m_gameController.ReloadStartScreenBeginCoroutine();
    }
}
                                                                                                                                                                                                                                 