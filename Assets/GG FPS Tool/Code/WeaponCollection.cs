using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class WeaponCollectionTooltip
{
    public const string
        weapons = "Weapons included in this weapon collection.";
}

[CreateAssetMenu(fileName = "New WeaponCollection", menuName = "WeaponCollection")]
public class WeaponCollection : ScriptableObject
{
    public int lastWeaponListSize;

    [Tooltip(WeaponCollectionTooltip.weapons)] public List<Weapon> weapons;
}
