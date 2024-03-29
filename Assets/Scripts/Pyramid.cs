using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid : MonoBehaviour
{
    public int m_blockCount = 0;
    public List<GameObject> m_newBlocks;
    public List<string> m_blockPlacementAnims;
    public List<string> m_RevealAnims;
    public List<string> m_HideBools;
    public enum POWER_STATE {BRONZE, SILVER, GOLD, DIAMOND}
    public POWER_STATE m_powerState = POWER_STATE.BRONZE;

    public void AddBlock()
    {
        if (m_blockCount < m_newBlocks.Count)
        {
            m_newBlocks[m_blockCount].SetActive(true);
            GetComponent<Animator>().Play(m_blockPlacementAnims[m_blockCount]);
            ++m_blockCount;
        }
    }
    public void RevealPyramid()
    {
        if (m_blockCount < m_newBlocks.Count)
        {
            GetComponent<Animator>().Play(m_RevealAnims[m_blockCount]);
        }
    }

    public void HidePyramid()
    {
        if (m_blockCount < m_newBlocks.Count)
        {
            GetComponent<Animator>().SetBool(m_HideBools[m_blockCount-1], true);
        }
    }

    public GameObject GetNextBlock()
    {
        return m_newBlocks[Mathf.Min(m_blockCount + 1, m_newBlocks.Count - 1)];
    }
}
