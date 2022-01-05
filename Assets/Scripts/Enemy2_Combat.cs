using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2_Combat : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] ParticleSystem[] Effects;
    [SerializeField] AudioSource ExplosionSFX;
    public bool Flag;
    private void Start()
    {
        Flag = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(this.enabled==true)
        {
            if (Flag == false)
            {
                Flag = true;
                foreach (ParticleSystem Ex in Effects)
                {
                    Ex.Play();
                }
                ExplosionSFX.Play();
                Destroy(gameObject, 3f);
            }
        }
       

    }
    private void OnTiggerEnter(Collider other)
    {
        if (this.enabled == true)
        {
            if (Flag == false)
            {

                Flag = true;
                foreach (ParticleSystem Ex in Effects)
                {
                    Ex.Play();
                }
                ExplosionSFX.Play();
                Destroy(gameObject, 3f);
            }
        }

    }
}
