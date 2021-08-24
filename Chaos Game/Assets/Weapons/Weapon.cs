using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Weapon
{
    void PrimarySkill(Animator anim);

    void SecondarySkill(Animator anim);

    WeaponType GetWeaponType();

    void SetLocation();
}
