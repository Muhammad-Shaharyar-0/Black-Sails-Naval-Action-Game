using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy_health : MonoBehaviour
{
    [SerializeField]
    public float health = 500;
    float Fullhealth = 0;
    [SerializeField] Animator player;
    [SerializeField] NavMeshAgent ship;
    public bool isDead = false;
    [SerializeField]
    Enemy2_Combat chase;

    [SerializeField] ParticleSystem[] Effects;
    [SerializeField] AudioSource ExplosionSFX;

    bool Flag;

    private bool damagedone;
    void Start()
    {
        isDead = false;
        Flag = false;
        Fullhealth = health;
        damagedone = false;
    }
    public void SetDamageDone(bool value)
    {
        damagedone = value;
    }
    public bool GetDamageDone()
    {
        return damagedone;
    }
    private void Update()
    {
        if (health <= 0 && isDead==false)
        {
            isDead = true;
            OnDeath();
        }

    }
    public float GetHealth()
    {
        return health;
    }
    public bool IsHealthHalf()
    {
        if (health <= Fullhealth / 2)
        {
                return true;
        }
        return false;
    }
    public bool NearDeath()
    {
        if (health <= 50)
        {
            if (chase != null)
                chase.enabled = true;
            return true;
        }
        return false;
    }
    void OnDeath()
    {
        if (chase == null)
        {
            if (Flag == false)
            {
                Flag = true;
                foreach (ParticleSystem Ex in Effects)
                {
                    Ex.Play();
                }
                ExplosionSFX.Play();
            }

        }
        else
        {
            if (chase.Flag == false)
            {
                chase.Flag = true;
                foreach (ParticleSystem Ex in Effects)
                {
                    Ex.Play();
                }
                ExplosionSFX.Play();
            }
        }
        ship.enabled = false;
        Invoke("DestroyShip", 3f);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.enemyCount--;
            if (GameManager.Instance.enemyCount == 0)
            {
                Invoke("NextLevel", 2.5f);
            }
        }   
        if (SpawnManager.Instance!=null)
        {
            SpawnManager.Instance.EnemiesAlive--;
        }
        player.enabled = true;

    }
    public bool EnemyDead()
    {
        if (health == 0)
            return true;
        return false;
    }
    public void DestroyShip()
    {
        Destroy(gameObject);
    }
    void NextLevel()
    {
        GameManager.Instance.LevelEnded();
    }
    public void Damage(float amount)
    {
        damagedone = true;
        health -= amount;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FireBarrel"))
        {
            float damage = collision.gameObject.GetComponent<FireBarrel>().damage;
            Damage(damage);
        }
        if (collision.gameObject.CompareTag("Chain_Cannonball"))
        {
            float damage = collision.gameObject.GetComponent<Cannonball>().damage;
            Damage(damage);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Mortarballs"))
        {
            float damage = collision.gameObject.GetComponent<Cannonball>().damage;
            Damage(damage);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Cannonball"))
        {
            float damage = collision.gameObject.GetComponent<Cannonball>().damage;
            Damage(damage);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            if (chase != null)
            {
                if (chase.enabled == true)
                    health = 0;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FireBarrel"))
        {
            float damage = other.gameObject.GetComponent<FireBarrel>().damage;
            Damage(damage);
            Damage(50);
        }
        if (other.gameObject.CompareTag("Chain_Cannonball"))
        {
            float damage = other.gameObject.GetComponent<Cannonball>().damage;
            Damage(damage);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Mortarballs"))
        {
            float damage = other.gameObject.GetComponent<Cannonball>().damage;
            Damage(damage);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Cannonball"))
        {
            float damage = other.gameObject.GetComponent<Cannonball>().damage;
            Damage(damage);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            if (chase != null)
            {
                if (chase.enabled == true)
                    health = 0;
            }
        }
    }
   

}
