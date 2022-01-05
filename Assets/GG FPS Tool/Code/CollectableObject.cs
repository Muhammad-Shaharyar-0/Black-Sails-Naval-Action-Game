using UnityEngine;


public class CollectableObject : MonoBehaviour
{
    public enum CollectionType { Weapon, Ammo }

    [SerializeField] CollectionType m_CollectionType = CollectionType.Weapon;

    [SerializeField] Weapon m_Weapon;
    [SerializeField] Ammo m_Ammo;

    [SerializeField] bool m_Enable = true;
    [SerializeField] int m_AmmoInWeapon;
    [SerializeField] int m_AddToAmmoTotal;

    [SerializeField] GameObject m_AfterCollectionObject;
    [SerializeField] float m_AfterCollectionDespawnTime;


    WeaponSpace m_CurrentWeaponSpace;

    void OnTriggerEnter(Collider other)
    {
        m_CurrentWeaponSpace = other.GetComponentInChildren<WeaponSpace>();

        if (m_CurrentWeaponSpace != null)
        {
            if (m_CollectionType == CollectionType.Weapon)
            {
                m_CurrentWeaponSpace.EnableWeapon(m_Weapon, m_Enable, m_AmmoInWeapon);
                m_CurrentWeaponSpace.IncreaseAmmoCount(m_Weapon.ammo, m_AddToAmmoTotal);
                    
            }
            else if (m_CollectionType == CollectionType.Ammo)
            {
                m_CurrentWeaponSpace.IncreaseAmmoCount(m_Ammo, m_AddToAmmoTotal);
            }

            if (m_AfterCollectionObject != null)
            {
                GameObject afterObjectInstance = Instantiate(m_AfterCollectionObject, transform.position, transform.rotation);
                Destroy(afterObjectInstance, m_AfterCollectionDespawnTime);
            }

            Destroy(gameObject);
        }
    }
}





