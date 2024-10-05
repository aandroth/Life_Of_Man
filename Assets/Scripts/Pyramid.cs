using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid : MonoBehaviour
{
    public bool m_pyramidIsRevealed = false;
    public int m_blockCount = 0;
    public List<GameObject> m_bronzeBlocks;
    public List<GameObject> m_newBlocks;
    public List<string> m_blockPlacementAnims;
    public List<string> m_RevealAnims;
    public List<string> m_HideBools;
    public enum POWER_STATE {BRONZE, SILVER, GOLD, DIAMOND}
    public POWER_STATE m_powerState = POWER_STATE.BRONZE;

    public void AddBlock()
    {
        if(m_powerState == POWER_STATE.GOLD)
        {
            if (m_blockCount < m_newBlocks.Count)
            {
                m_newBlocks[m_blockCount].SetActive(true);
                GetComponent<Animator>().Play(m_blockPlacementAnims[m_blockCount]);
                ++m_blockCount;
            }
        }
        else if(m_powerState == POWER_STATE.BRONZE)
        {
            if (m_blockCount == 3)
            {
                GetComponent<Animator>().Play(m_RevealAnims[3]);
                m_pyramidIsRevealed = true;
                ++m_blockCount;
            }
            else if(m_blockCount == 4)
            {
                GetComponent<Animator>().Play(m_blockPlacementAnims[3]);
                ++m_blockCount;
            }
        }
    }
    public void RevealPyramid()
    {
        if(m_powerState == POWER_STATE.GOLD)
        {
            if (!m_pyramidIsRevealed && m_blockCount < m_newBlocks.Count)
            {
                GetComponent<Animator>().Play(m_RevealAnims[m_blockCount]);
                m_pyramidIsRevealed = true;
            }
        }
        else
        {
            if (m_blockCount == 4)
            {
                GetComponent<Animator>().Play(m_RevealAnims[3]);
                m_pyramidIsRevealed = true;
            }
        }
    }

    public void HidePyramid()
    {
        if (m_pyramidIsRevealed)
        {
            if(m_powerState == POWER_STATE.GOLD && m_blockCount < m_newBlocks.Count)
            {
                GetComponent<Animator>().SetBool(m_HideBools[m_blockCount-1], true);
                m_pyramidIsRevealed = false;
            }
            else if(m_powerState == POWER_STATE.BRONZE)
            {
                if(m_blockCount == 4)
                {
                    GetComponent<Animator>().SetBool(m_HideBools[2], true);
                    m_pyramidIsRevealed = false;
                }
            }
        }
    }

    public GameObject GetNextBlock()
    {
        return m_newBlocks[Mathf.Min(m_blockCount + 1, m_newBlocks.Count - 1)];
    }
}
