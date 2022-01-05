using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class Tests
{
  
    void LoadScene1()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene != 1)
        {
            SceneManager.LoadScene("Level 1");
            
        }
    }
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayerHealthDecreases_Test()
    {

        LoadScene1();
        yield return null;
        MyPlayerHealth health = GameObject.Find("PlayerShip").GetComponentInChildren<MyPlayerHealth>();
        float Fhealth = health.health;
        health.SetDamageDone(false);
        while (!health.GetDamageDone())
        {
            yield return null;
        }
        Assert.AreNotEqual(Fhealth, health.health);
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
       // yield return null;
    }
    [UnityTest]
    public IEnumerator PlayerShipMovement_Test()
    {
        LoadScene1();
        yield return null;
        ShipMovement ship = GameObject.Find("PlayerShip").GetComponentInChildren<ShipMovement>();
        float speed = ship.ReturnSpeed();
        float sails = ship.Getsails();
        yield return null;
        while (!Input.GetKeyDown(KeyCode.W))
            yield return null;
        if(ship.Getsails() != 2)
        {
            Assert.AreNotEqual(speed, ship.ReturnSpeed());
        }
        else
        {
            Assert.AreEqual(speed, ship.ReturnSpeed());
        }
        speed = ship.ReturnSpeed();
        sails = ship.Getsails();
        yield return null;
        while (!Input.GetKeyDown(KeyCode.W))
            yield return null;
        if (sails != 2)
        {
            Assert.AreNotEqual(speed, ship.ReturnSpeed());
        }
        else
        {
            Assert.AreEqual(speed, ship.ReturnSpeed());
        }
        speed = ship.ReturnSpeed();
        sails = ship.Getsails();
        yield return null;
        while (!Input.GetKeyDown(KeyCode.S))
            yield return null;
        if (sails != 0)
        {
            Assert.AreNotEqual(speed, ship.ReturnSpeed());
        }
        else
        {
            Assert.AreEqual(speed, ship.ReturnSpeed());
        }
        speed = ship.ReturnSpeed();
        sails = ship.Getsails();
        yield return null;
        while (!Input.GetKeyDown(KeyCode.S))
            yield return null;
        if (sails != 0)
        {
            Assert.AreNotEqual(speed, ship.ReturnSpeed());
        }
        else
        {
            Assert.AreEqual(speed, ship.ReturnSpeed());
        }
    }
    [UnityTest]
    public IEnumerator PlayerShipRotation_Test()
    {
        LoadScene1();
        yield return null;
        ShipMovement ship = GameObject.Find("PlayerShip").GetComponentInChildren<ShipMovement>();
        float speed = ship.ReturnRotationSpeed();
        float sails = ship.Getsails();
        yield return null;
        while (!Input.GetKeyDown(KeyCode.W))
            yield return null;
        if (sails != 2)
        {
            Assert.AreNotEqual(speed, ship.ReturnRotationSpeed());
        }
        else
        {
            Assert.AreEqual(speed, ship.ReturnRotationSpeed());
        }
        speed = ship.ReturnRotationSpeed();
        sails = ship.Getsails();
        yield return null;
        while (!Input.GetKeyDown(KeyCode.W))
            yield return null;
        if (sails != 2)
        {
            Assert.AreNotEqual(speed, ship.ReturnRotationSpeed());
        }
        else
        {
            Assert.AreEqual(speed, ship.ReturnRotationSpeed());
        }
        speed = ship.ReturnRotationSpeed();
        sails = ship.Getsails();
        yield return null;
        while (!Input.GetKeyDown(KeyCode.S))
            yield return null;
        if (sails != 0)
        {
            Assert.AreNotEqual(speed, ship.ReturnRotationSpeed());
        }
        else
        {
            Assert.AreEqual(speed, ship.ReturnRotationSpeed());
        }
        speed = ship.ReturnRotationSpeed();
        sails = ship.Getsails();
        yield return null;
        while (!Input.GetKeyDown(KeyCode.S))
            yield return null;
        if (sails != 0)
        {
            Assert.AreNotEqual(speed, ship.ReturnRotationSpeed());
        }
        else
        {
            Assert.AreEqual(speed, ship.ReturnRotationSpeed());
        }
    }

    [UnityTest]
    public IEnumerator EnemyHealthDecreases_Test()
    {
        LoadScene1();
        yield return null;
        Enemy_health health = GameObject.Find("Enemy 1").GetComponent<Enemy_health>();
        float Fhealth = health.health;
        health.SetDamageDone(false);
        while (!health.GetDamageDone())
        {
            yield return null;
        }
        Assert.AreNotEqual(Fhealth, health.health);
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        // yield return null;
    }

    [UnityTest]
    public IEnumerator WeaponSwitching_Test()
    {

        LoadScene1();
        yield return null;
        ShipCombat ship = GameObject.Find("PlayerShip").GetComponent<ShipCombat>();
        yield return null;
        while (!Input.GetKeyDown(KeyCode.Alpha1))
            yield return null;
        Assert.AreEqual(1, ship.GetWeaponSelected());
        while (!Input.GetKeyDown(KeyCode.Alpha2))
            yield return null;
        Assert.AreEqual(2, ship.GetWeaponSelected());
        while (!Input.GetKeyDown(KeyCode.Alpha3))
            yield return null;
        Assert.AreEqual(3, ship.GetWeaponSelected());
        while (!Input.GetKeyDown(KeyCode.Alpha4))
            yield return null;
        Assert.AreEqual(4, ship.GetWeaponSelected());
        while (!Input.GetKeyDown(KeyCode.Alpha5))
            yield return null;
        Assert.AreEqual(5, ship.GetWeaponSelected());
    }
    [UnityTest]
    public IEnumerator LaunchCannonballs_Test()
    {
        LoadScene1();
        yield return null;
        ShipCombat ship = GameObject.Find("PlayerShip").GetComponent<ShipCombat>();
        ship.cannonsFired = false;
        while (ship.GetWeaponSelected() != 1 && ship.GetWeaponSelected() != 2)
        {     
            yield return null;
        }
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;

        }
        Assert.AreEqual(true, ship.cannonsFired);
    }
    [UnityTest]
    public IEnumerator LaunchRightCannonballs_Test()
    {
        LoadScene1();
        yield return null;
        ShipCombat ship = GameObject.Find("PlayerShip").GetComponent<ShipCombat>();
        ship.RightcannonsFired = false;
        ship.LeftcannonsFired = false;
        while ((ship.GetWeaponSelected() != 1 && ship.GetWeaponSelected() != 2) || ship.GetCameraIndex()!=1)
        {
            yield return null;
        }
        ship.RightcannonsFired = false;
        ship.LeftcannonsFired = false;
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;

        }
        Assert.AreEqual(true, ship.RightcannonsFired);
        Assert.AreEqual(false, ship.LeftcannonsFired);
    }
    [UnityTest]
    public IEnumerator LaunchLeftCannonballs_Test()
    {
        LoadScene1();
        yield return null;
        ShipCombat ship = GameObject.Find("PlayerShip").GetComponent<ShipCombat>();
        while ((ship.GetWeaponSelected() != 1 && ship.GetWeaponSelected() != 2) || ship.GetCameraIndex() != 2)
        {
            yield return null;
        }
        ship.RightcannonsFired = false;
        ship.LeftcannonsFired = false;
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;

        }
        Assert.AreEqual(true, ship.LeftcannonsFired);
        Assert.AreEqual(false, ship.RightcannonsFired);
    }
    [UnityTest]
    public IEnumerator LaunchFireBarrels_Test()
    {
        LoadScene1();
        yield return null;
        ShipCombat ship = GameObject.Find("PlayerShip").GetComponent<ShipCombat>();
        while (ship.GetWeaponSelected() != 3)
        {
            yield return null;
        }
        ship.OilBarrelsFired = false;
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;

        }
        Assert.AreEqual(true, ship.OilBarrelsFired);
    }
  
    [UnityTest]
    public IEnumerator LaunchMortor_Test()
    {
        LoadScene1();
        yield return null;
        ShipCombat ship = GameObject.Find("PlayerShip").GetComponent<ShipCombat>();
        while (ship.GetWeaponSelected() != 5)
        {
            yield return null;
        }
        ship.MortorFired = false;
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;

        }
        Assert.AreEqual(true, ship.MortorFired);
    }

    [UnityTest]
    public IEnumerator FireRightGun_Test()
    {
        LoadScene1();
        yield return null;
        ShipCombat ship = GameObject.Find("PlayerShip").GetComponent<ShipCombat>();
        while ((ship.GetWeaponSelected() != 4) || ship.GetCameraIndex() != 3)
        {
            yield return null;
        }
        WeaponSpace gun = GameObject.Find("PlayerShip").GetComponentInChildren<WeaponSpace>();
        while (!Input.GetKeyDown(KeyCode.Mouse0))
        {
            yield return null;

        }
        Assert.AreEqual(true, gun.GetGunFired());
    }
    [UnityTest]
    public IEnumerator FireLeftGun_Test()
    {
        LoadScene1();
        yield return null;
        ShipCombat ship = GameObject.Find("PlayerShip").GetComponent<ShipCombat>();
        while ((ship.GetWeaponSelected() != 4) || ship.GetCameraIndex() != 4)
        {
            yield return null;
        }
        WeaponSpace gun = GameObject.Find("PlayerShip").GetComponentInChildren<WeaponSpace>();
        while (!Input.GetKeyDown(KeyCode.Mouse0))
        {
            yield return null;

        }
        Assert.AreEqual(true, gun.GetGunFired());
    }
}
