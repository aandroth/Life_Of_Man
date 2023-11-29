using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAllChildren : MonoBehaviour
{
    public void ActivateAllChildrenNow()
    {
        foreach (GameObject go in gameObject.GetComponentsInChildren<GameObject>())
        {
            go.SetActive(true);
        }
    }
}
