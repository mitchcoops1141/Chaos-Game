using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolySword : MonoBehaviour, Weapon
{
    [Header("Standard Stats")]
    [SerializeField] WeaponClass holySwordWeapon;

    [Header("Unique Stats")]
    [SerializeField] [Tooltip("Percentage of damage")] private float lifesteal;
    [SerializeField] private float spinAttackCooldown;
    [SerializeField] private float spinAttackDamage;

    [Header("Connections")]
    [SerializeField] private ParticleSystem trail;
    [SerializeField] private GameObject hitbox;
    [SerializeField] private AnimationClip spinAttackAnim;

    bool canAttack = true;
    bool canSpinAttack = true;
    bool isSpinAttacking = false;

    void  Weapon.PrimarySkill(Animator anim)
    {
        if (canAttack)
            StartCoroutine("Attack", anim);  
    }

    IEnumerator Attack(Animator anim)
    {
        canAttack = false;
        canSpinAttack = false;

        anim.SetTrigger("SwordAttack");

        yield return new WaitForSeconds(holySwordWeapon.GetFireRate());

        canAttack = true;
        canSpinAttack = true;
    }

    void Weapon.SecondarySkill(Animator anim)
    {
        if (canSpinAttack)
            StartCoroutine("SpinAttack", anim);
    }

    IEnumerator SpinAttack(Animator anim)
    {
        canSpinAttack = false;
        isSpinAttacking = true;

        //stop walking and attacking
        canAttack = false;
        anim.GetComponent<PlayerController>().StopMovement();

        anim.SetTrigger("SwordSpinAttack");

        print(spinAttackAnim.length);
        yield return new WaitForSeconds(spinAttackAnim.length - 0.1f);

        isSpinAttacking = false;

        //can attack and walk
        canAttack = true;
        transform.root.GetComponent<PlayerController>().PlayMovement();

        yield return new WaitForSeconds(spinAttackCooldown - spinAttackAnim.length);

        canSpinAttack = true;
    }


    WeaponType Weapon.GetWeaponType()
    {
        return holySwordWeapon.GetWeaponType();
    }

    public void ShowHitBox()
    {
        trail.Play();
        hitbox.SetActive(true);
        AudioManager.instance.Play("Holy Sword Slash");
    }

    public void HideHitBox()
    {
        trail.Stop();
        hitbox.SetActive(false);
    }

    public void ShowSpinHitbox()
    {
        trail.Play();
        hitbox.SetActive(true);
        AudioManager.instance.Play("Holy Sword Spin");
    }

    private void OnTriggerEnter(Collider other)
    {
        //deal damage
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            PlayerController pc = transform.root.GetComponent<PlayerController>();
            //if spin attacking
            if (isSpinAttacking)
            {
                if (holySwordWeapon.IsCrit())
                {
                    enemy.TakeDamage(spinAttackDamage * holySwordWeapon.GetCritDamage(), true);
                
                    pc.SetHealth(pc.GetHealth() + ((lifesteal / 100) * (spinAttackDamage * holySwordWeapon.GetCritDamage())));
                }
                else
                {
                    enemy.TakeDamage(spinAttackDamage, false);

                    //lifesteal
                    pc.SetHealth(pc.GetHealth() + ((lifesteal / 100) * spinAttackDamage));
                }
                    


                
            }
            //if normal attacking
            else
            {
                if (holySwordWeapon.IsCrit())
                {
                    enemy.TakeDamage(holySwordWeapon.GetDamage() * holySwordWeapon.GetCritDamage(), true);
                    pc.SetHealth(pc.GetHealth() + ((lifesteal / 100) * (holySwordWeapon.GetDamage() * holySwordWeapon.GetCritDamage())));

                }
                else
                {
                    enemy.TakeDamage(holySwordWeapon.GetDamage(), false);

                    //lifesteal
                    pc.SetHealth(pc.GetHealth() + ((lifesteal / 100) * holySwordWeapon.GetDamage()));

                }
            }

        } 
    }

    void Weapon.SetLocation()
    {
        transform.parent.localPosition = holySwordWeapon.GetPosition();
        transform.parent.localEulerAngles = holySwordWeapon.GetRotation();
    }
}
