using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TricksPointerManager : MonoBehaviour
{
    public List<GameObject> m_tricksHandlersList;

    public void OnEnable()
    {
        foreach (GameObject g in m_tricksHandlersList)
            g.SetActive(true);
    }
}
