using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public void DestroyHandler()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
