using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MyPlayerHealth : MonoBehaviour
{
    [SerializeField]
    public float health = 500;
    private float totalHealth;
    [SerializeField]
    public float Defaulthealth = 500;
    [SerializeField]
    Image healthBar;
    [SerializeField] Animator player;
    ShipMovement ship;
    FloatingGameEntityRealist floating;

    [SerializeField] ParticleSystem[] Effects;
    [SerializeField] AudioSource ExplosionSFX;

    [SerializeField] Camera DeathCamera;
    [SerializeField] Camera MainCamera;
    bool Flag;
    private bool damagedone;
    private void Start()
    {
        health = PlayerPrefs.GetFloat("Health", Defaulthealth);
        healthBar.fillAmount = 1;
        ship = gameObject.GetComponent<ShipMovement>();
        floating = gameObject.GetComponent<FloatingGameEntityRealist>();
        totalHealth = health;
        Flag = false;
        MainCamera = Camera.main;
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
        if (health <= 0 && Flag==false)
        {
            Flag = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            MainCamera.gameObject.SetActive(false);
            DeathCamera.gameObject.SetActive(true);          
            DeathCamera.transform.parent = null;          
            foreach (ParticleSystem Ex in Effects)
            {
                Ex.Play();
            }
            ExplosionSFX.Play();
            Invoke("DisableFloating", 3.5f);
            Invoke("EndLevel", 3f);          
            player.enabled = true;
            ship.Playerdead = true;
        }

    }
    void EndLevel()
    {
        GameManager.Instance.GameOver();
    }
    void DisableFloating()
    {
        floating.enabled = false;
    }
    public void Damage(float amount)
    {
        damagedone = true;
        health -= amount;
        healthBar.fillAmount = health / totalHealth;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FireBarrel"))
        {
            Damage(50);
        }
        if (other.gameObject.CompareTag("Chain_CannonballE"))
        {
            Damage(4);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("MortarballsE"))
        {
            Damage(3);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("CannonballE"))
        {
            Damage(5);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Enemy2"))
        {
            Damage(100);
            Destroy(other.gameObject, 2);
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            Damage(100);
            Destroy(other.gameObject, 2);
        }
    }
}
