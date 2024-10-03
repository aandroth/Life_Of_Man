using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject m_enemyWorldHandlerPrefab;
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
            GameObject g = Instantiate(m_enemyWorldHandlerPrefab, Vector3.zero, transform.rotation);
            g.GetComponentInChildren<Enemy>().SetToChasing();
            g.transform.GetChild(0).gameObject.SetActive(false);
            m_enemyObjectPool.Add(g.transform.GetChild(0).gameObject);
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
                float randRot = Random.Range(-m_spawnRadius, m_spawnRadius);
                float randPos = Random.Range(-m_spawnRadius, m_spawnRadius);
                g.transform.localPosition = transform.localPosition + new Vector3(0, randPos, 0);
                g.GetComponent<Enemy>().m_worldHandler.transform.RotateAround(Vector3.zero, transform.forward, randRot + transform.parent.transform.eulerAngles.z);
                g.GetComponent<Enemy>().m_target = m_target;
                g.SetActive(true);
                break;
            }
            else if (!g.transform.parent.gameObject.activeSelf)
            {
                float randRot = Random.Range(-m_spawnRadius, m_spawnRadius);
                float randPos = Random.Range(-m_spawnRadius, m_spawnRadius);
                g.transform.localPosition = transform.localPosition + new Vector3(0, randPos, 0);
                g.GetComponent<Enemy>().m_worldHandler.transform.RotateAround(Vector3.zero, transform.forward, randRot + transform.parent.transform.eulerAngles.z);
                g.GetComponent<Enemy>().m_target = m_target;
                g.transform.parent.gameObject.SetActive(true);
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
                g.GetComponent<Enemy>().Die();
        }
    }
}
