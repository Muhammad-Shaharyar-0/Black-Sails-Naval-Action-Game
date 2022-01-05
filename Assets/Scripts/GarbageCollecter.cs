using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollecter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Chain_Cannonball"))
        {
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Mortarballs"))
        {
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Cannonball"))
        {
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Chain_CannonballE"))
        {
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("MortarballsE"))
        {
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("CannonballE"))
        {
            Destroy(other.gameObject);
        }

    }
}
