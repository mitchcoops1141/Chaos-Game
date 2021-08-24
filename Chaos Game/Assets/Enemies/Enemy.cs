using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemy
{
    void TakeDamage(float dmg, bool isCrit);

    void Die();
}
