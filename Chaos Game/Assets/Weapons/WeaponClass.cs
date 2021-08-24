using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponClass
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private WeaponType weapontype;
    [SerializeField] private float firerate;
    [SerializeField] private float damage;
    [SerializeField] [Tooltip("Multiplier")] private float critDamage;
    [SerializeField] [Tooltip("Percentage")] private float critChance;


    public WeaponType GetWeaponType()
    {
        return weapontype;
    }

    public float GetFireRate()
    {
        return firerate;
    }

    public float GetDamage()
    {
        return damage;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public Vector3 GetRotation()
    {
        return rotation;
    }

    public float GetCritChance()
    {
        return critChance;
    }

    public float GetCritDamage()
    {
        return critDamage;
    }

    public bool IsCrit()
    {
        int n = (Random.Range(1, 101));
        if (n <= critChance)
            return true;
        else
            return false;
    }
}
