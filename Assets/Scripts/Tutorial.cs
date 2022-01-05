using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<GameObject> tutorialmessages;
    bool shown1;
    bool shown2;
    bool shown3;
    bool shown4;
    bool shown5;
    bool shown6;
    bool shown7;
    private void Start()
    {
        if (PlayerPrefs.GetInt("FirstTime", 1) == 1)
        {
           
            EnableShipMovement();
            shown1 = false;
            shown2 = false;
            shown3 = false;
            shown4 = false;
            shown5 = false;
            shown6 = false;
            shown7 = false;
            PlayerPrefs.SetInt("FirstTime", 0);
        }
        else
        {
            shown1 = true;
            shown2 = true;
            shown3 = true;
            shown4 = true;
            shown5 = true;
            shown6 = true;
            shown7 = true;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            DisableShipMovement();
            if (shown2 == false)
            {
                Invoke("EnableFiringCannons", 1f);
            }

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DisableEverything();
            if (shown3 == false)
            {
                Invoke("EnableWeaponSwitching", 1f);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            DisableEverything();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DisableEverything();
            if (shown4 == false)
            {
                EnableChain_Cannonballs();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DisableEverything();
            if (shown5 == false)
            {
                EnableOilBarrels();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DisableEverything();
            if (shown6 == false)
            {
                EnableGun();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            DisableEverything();
            if (shown7 == false)
            {
                EnableMortor();
            }
        }
    }
    void DisableEverything()
    {
        DisableChain_Cannonballs();
        DisableOilBarrels();
        DisableGun();
        DisableMortor();
        DisableFiringCannons();
        DisableWeaponSwitching();
        DisableShipMovement();
    }
    public void EnableShipMovement()
    {
        shown1 = true;
        tutorialmessages[0].SetActive(true);
    }
    public void DisableShipMovement()
    {
        tutorialmessages[0].SetActive(false);
    }
    public void EnableFiringCannons()
    {
        shown2 = true;
        tutorialmessages[1].SetActive(true);
    }
    public void DisableFiringCannons()
    {
        tutorialmessages[1].SetActive(false);
    }
    public void EnableWeaponSwitching()
    {
        shown3 = true;
        tutorialmessages[2].SetActive(true);
    }
    public void DisableWeaponSwitching()
    {
        tutorialmessages[2].SetActive(false);
    }
    public void EnableChain_Cannonballs()
    {
        shown4 = true;
        tutorialmessages[3].SetActive(true);
    }
    public void DisableChain_Cannonballs()
    {
        tutorialmessages[3].SetActive(false);
    }
    public void EnableOilBarrels()
    {
        shown5 = true;
        tutorialmessages[4].SetActive(true);
    }
    public void DisableOilBarrels()
    {
        tutorialmessages[4].SetActive(false);
    }
    public void EnableGun()
    {
        shown6= true;
        tutorialmessages[5].SetActive(true);
    }
    public void DisableGun()
    {
        tutorialmessages[5].SetActive(false);
    }
    public void EnableMortor()
    {
        shown7 = true;
        tutorialmessages[6].SetActive(true);
    }
    public void DisableMortor()
    {
        tutorialmessages[6].SetActive(false);
    }
}
