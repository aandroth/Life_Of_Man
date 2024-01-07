using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory_Father : MonoBehaviour
{
    public GameObject m_fatherHandlerNPC = null;
    public GameObject m_fatherHandlerPlayer = null;

    public GameObject CreateFather_Player()
    {
        if (m_fatherHandlerPlayer == null)
        {
            Debug.LogError($"CreateFather_Player called without prefab");
            return null;
        }
        GameObject newFather = GameObject.Instantiate(m_fatherHandlerPlayer);

        if (newFather == null)
        {
            Debug.LogError($"CreateFather_Player creation failed!");
            return null;
        }

        return newFather;
    }
    public GameObject CreateFather_NPC()
    {
        if (m_fatherHandlerPlayer == null)
        {
            Debug.LogError($"CreateFather_NPC called without prefab");
            return null;
        }

        GameObject newFather = GameObject.Instantiate(m_fatherHandlerNPC);

        if (newFather == null)
        {
            Debug.LogError($"CreateFather_NPC creation failed!");
            return null;
        }

        return newFather;
    }
}
