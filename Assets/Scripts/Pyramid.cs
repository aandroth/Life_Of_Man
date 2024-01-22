using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid : MonoBehaviour
{
    public int m_blockCount = 0;
    public List<GameObject> m_newBlocks;
    public List<string> m_blockPlacementAnims;

    public void AddBlock()
    {
        Debug.Log($"{gameObject.name}: AddBlock");
        if (m_blockCount < m_newBlocks.Count)
        {
            Debug.Log($"{gameObject.name}: m_blockCount: {m_blockCount}, m_newBlocks.Count: {m_newBlocks.Count}");
            m_newBlocks[m_blockCount].SetActive(true);
            GetComponent<Animator>().Play(m_blockPlacementAnims[m_blockCount]);
            ++m_blockCount;
        }
    }
}
