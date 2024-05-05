using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    public static GameData m_gameData;

    public static GameData Load()
    {
        string gameDataText = Resources.Load<TextAsset>("GameDataTextAsset").ToString();
        m_gameData = JsonUtility.FromJson<GameData>(gameDataText);
        return m_gameData;
    }
    public static void Save()
    {
        string gameDataText = JsonUtility.ToJson(m_gameData);
        File.WriteAllText(Application.dataPath + "/Resources/" + "GameDataTextAsset.json", gameDataText);
    }
}
