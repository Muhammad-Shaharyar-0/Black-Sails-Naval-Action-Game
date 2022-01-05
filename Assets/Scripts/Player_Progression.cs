using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player_Progression : MonoBehaviour
{
    float healthlevel = 1;
    float damagelevel = 1;
    float upgradePoints = 0;
    [SerializeField]
    TextMeshProUGUI Healthlevel;
    [SerializeField]
    Button HealthUpgrade;

    [SerializeField]
    TextMeshProUGUI Damagelevel;
    [SerializeField]
    Button DamageUpgrade;

    [SerializeField]
    TextMeshProUGUI UpgradePoints;

    private void Start()
    {
        healthlevel = PlayerPrefs.GetFloat("HealthLevel", 1);
        damagelevel = PlayerPrefs.GetFloat("DamageLevel", 1);
        upgradePoints = PlayerPrefs.GetFloat("UpgradePoints", 0);
        UpgradePoints.text = "Upgrade Points: " + upgradePoints;
        HealthUpgrade.interactable = false;
        DamageUpgrade.interactable = false;
        if (healthlevel < 3)
        {
            Healthlevel.text = "Health    Level " + healthlevel;
            if (upgradePoints != 0)
                HealthUpgrade.interactable = true;
        }
        else
        {
            Healthlevel.text = "Health    Level max";
        }
        if (damagelevel < 3 )
        {
            Damagelevel.text = "Damage Level " + damagelevel;
            if (upgradePoints != 0)
                DamageUpgrade.interactable = true;
        }
        else
        {
            Damagelevel.text = "Damage Level max";
        }
    }
    public void UpgradeHealth()
    {
        float health = PlayerPrefs.GetFloat("Health",500);
        PlayerPrefs.SetFloat("Health", health+100);
        healthlevel++;
        PlayerPrefs.SetFloat("HealthLevel", healthlevel);
        Healthlevel.text = "Health    Level " + healthlevel;
        upgradePoints--;
        PlayerPrefs.SetFloat("UpgradePoints", upgradePoints);
        HealthUpgrade.interactable = false;
        DamageUpgrade.interactable = false;
        if (healthlevel < 3)
        {
            Healthlevel.text = "Health    Level " + healthlevel;
            if (upgradePoints != 0)
                HealthUpgrade.interactable = true;
        }
        else
        {
            Healthlevel.text = "Health    Level max";
        }

    }
    public void UpgradeDamage()
    {
        float damage = PlayerPrefs.GetFloat("Damage", 1);
        PlayerPrefs.SetFloat("Damage", damage + 0.25f);
        damagelevel++;
        PlayerPrefs.SetFloat("DamageLevel", damagelevel);
        Damagelevel.text = "Damage Level " + damagelevel;
        upgradePoints--;
        PlayerPrefs.SetFloat("UpgradePoints", upgradePoints);
        HealthUpgrade.interactable = false;
        DamageUpgrade.interactable = false;
        if (damagelevel < 3)
        {
            Damagelevel.text = "Damage Level " + damagelevel;
            if (upgradePoints != 0)
                DamageUpgrade.interactable = true;
        }
        else
        {
            Damagelevel.text = "Damage Level max";
        }
    }
}
