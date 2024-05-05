using UnityEngine;

public class EnemySpawnerHandler : MonoBehaviour
{
    public EnemySpawner m_spawner;
    public GameObject m_targetHandler;
    public float m_leadingAmount = 30f;

    void Update()
    {
        if(m_targetHandler != null)
        {
            Vector3 newRot = transform.eulerAngles;
            newRot.z = m_targetHandler.transform.eulerAngles.z - m_leadingAmount;
            transform.eulerAngles = newRot;
        }
    }

    public void DespawnAllEnemies()
    {
        m_spawner.DespawnAllEnemies();
    }
}
