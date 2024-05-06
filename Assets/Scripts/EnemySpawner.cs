using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject m_enemyPrefab;
    public float m_spawnFrequencyMin = 0, m_spawnFrequencyMax = 2;
    public float m_spawnTimer = 0f;
    public float m_spawnRadius = 10f;
    public uint m_enemyObjectPoolCount = 10;
    public List<GameObject> m_enemyObjectPool = null;
    public GameObject m_target;

    // Start is called before the first frame update
    void Start()
    {
        for(int ii=0; ii<m_enemyObjectPoolCount; ++ii)
        {
            GameObject g = Instantiate(m_enemyPrefab, transform.position, Quaternion.identity);
            g.GetComponent<Enemy>().SetToChasing();
            g.SetActive(false);
            m_enemyObjectPool.Add(g);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target != null)
        {
            if (m_spawnTimer >= m_spawnFrequencyMax)
            {
                SpawnEnemy();
                m_spawnTimer = 0;
            }
            m_spawnTimer += Time.deltaTime;
        }
    }

    public void SpawnEnemy()
    {
        foreach(GameObject g in m_enemyObjectPool)
        {
            if (!g.activeSelf)
            {
                float randRot = Random.Range(1, 360);
                float randPos = Random.Range(0, m_spawnRadius);
                Vector3 pos = Vector3.zero;
                pos.x = randPos;
                g.transform.position = transform.position + pos;
                g.transform.RotateAround(transform.position, transform.forward, randRot);
                g.SetActive(true);
                break;
            }
        }
    }

    public void SetAllEnemiesSpeed(float speed)
    {
        foreach (GameObject g in m_enemyObjectPool)
        {
            g.GetComponent<Enemy>().m_moveSpeed = speed;
        }
    }

    public void DespawnAllEnemies()
    {
        foreach (GameObject g in m_enemyObjectPool)
        {
            if(g.activeSelf)
                g.SetActive(false);
        }
    }
}
