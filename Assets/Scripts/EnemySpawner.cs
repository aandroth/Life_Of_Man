using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject m_enemyPrefab;
    public float m_spawnFrequency = 2;
    public float m_spawnTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_spawnTimer >= m_spawnFrequency)
        {
            GameObject.Instantiate(m_enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
