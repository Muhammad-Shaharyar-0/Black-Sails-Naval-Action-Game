using UnityEngine;


public class Grenade : MonoBehaviour
{
    [SerializeField] GameObject m_ExplosionObject;

    [Space]

    [SerializeField] float m_DetonationTime = 2f;
    [SerializeField] bool m_IsDetonateOnCollision = true;

    [Space]

    [SerializeField] float m_ExplosionExistenceTime = 4f;

    float m_CurrentTime;

    void Start()
    {
        m_CurrentTime = m_DetonationTime;
    }

    void Update()
    {
        if (m_CurrentTime > 0f)
        {
            m_CurrentTime -= 1f * Time.deltaTime;
        }
        else
        {
            SpawnExplosion();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (m_IsDetonateOnCollision)
        {
            SpawnExplosion();
        }
    }

    void SpawnExplosion()
    {
        GameObject explosionInstance = Instantiate(m_ExplosionObject, transform.position, transform.rotation);
        Destroy(gameObject);
        Destroy(explosionInstance, m_ExplosionExistenceTime);
    }

}  



