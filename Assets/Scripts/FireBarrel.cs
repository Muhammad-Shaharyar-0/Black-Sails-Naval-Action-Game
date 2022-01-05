using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBarrel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] ParticleSystem[] Effects;
    [SerializeField] AudioSource ExplosionSFX;
    public float damage = 50;
    bool Flag;
    private void Start()
    {
        damage = 50 * PlayerPrefs.GetFloat("Damage", 1);
        Flag = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Flag == false)
        {
            Flag = true;
            foreach (ParticleSystem Ex in Effects)
            {
                Ex.Play();
            }
            ExplosionSFX.Play();
            Destroy(gameObject, 1f);
        }
       
    }
}
