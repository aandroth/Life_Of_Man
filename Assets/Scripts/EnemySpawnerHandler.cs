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

    public void SetTargetHandler_AndTarget_AndActivateSpawner(GameObject targetHandler, GameObject target)
    {
        m_targetHandler = targetHandler;
        m_spawner.m_target = target;
        m_spawner.gameObject.SetActive(true);
    }

    public void DespawnAllEnemiesAndDeactivateSpawner()
    {
        m_spawner.DespawnAllEnemies();
        m_spawner.gameObject.SetActive(true);
    }
}
