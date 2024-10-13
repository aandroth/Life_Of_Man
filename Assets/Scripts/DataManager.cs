using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{
    public class GameData
    {
        public string m_playerName;
        public bool m_level_2_Unlocked;
        public bool m_level_3_Unlocked;

        public GameData()
        {
            m_playerName = "THE_player";
            m_level_2_Unlocked = false;
            m_level_3_Unlocked = false;
        }
    }

    public GameData m_gameData;
    public static DataManager m_dataManagerInstance = null;



    public void Start()
    {
        if(m_dataManagerInstance != null)
            Destroy(this.gameObject);
        else
        {
            m_dataManagerInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public bool LevelUnlockedCheck(int idx)
    {
        if (idx == 2)
            return m_gameData.m_level_2_Unlocked;
        else if (idx == 3)
            return m_gameData.m_level_3_Unlocked;
        else
            return false;
    }

    public void LevelUnlock(int idx)
    {
        if (idx == 2)
            m_gameData.m_level_2_Unlocked = true;
        else if (idx == 3)
            m_gameData.m_level_3_Unlocked = true;
        Save();
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(Application.dataPath, "Resources", "GameDataTextAsset.json");
        m_gameData = null;

        if(File.Exists(fullPath))
        {
            try
            {
                string gameDataText = Resources.Load<TextAsset>("GameDataTextAsset").ToString();
                m_gameData = JsonUtility.FromJson<GameData>(gameDataText);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log($"Exception thrown: {e.Message}");
#endif
            }
        }
        return m_gameData;
    }


    public void Save()
    {
        string fullPath = Path.Combine(Application.dataPath, "Resources", "GameDataTextAsset.json");
        string gameDataText = JsonUtility.ToJson(m_gameData);
        try
        {
            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(gameDataText);
                }
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.Log($"Exception thrown: {e.Message}");
#endif
        }
    }
}
