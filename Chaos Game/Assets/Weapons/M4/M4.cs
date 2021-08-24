using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M4 : MonoBehaviour, Weapon
{
    [Header("Stats")]
    [SerializeField] WeaponClass M4Weapon;
    
    [Header("Connections")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Transform muzzleFlashLocation;
    [SerializeField] private GameObject hitFlash;
    [SerializeField] private LayerMask layersToHit;

    bool canShoot = true;
    void Weapon.PrimarySkill(Animator anim)
    {
        if (canShoot)
            StartCoroutine("ShootIE");
    }

    void Weapon.SecondarySkill(Animator anim)
    {
        print("Do Secondary Skill STUFFFFFFF");
    }

    IEnumerator ShootIE()
    {
        canShoot = false;

        //muzzle flash stuff
        GameObject cacheFlash = Instantiate(muzzleFlash, muzzleFlashLocation);
        cacheFlash.GetComponent<ParticleSystem>().Play();
        Destroy(cacheFlash, 0.5f);

        //sound
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().Play("Gun Shot");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layersToHit))
        {
            var hitPoint = hitInfo.point;

            //ray
            RaycastHit hit;
            if (Physics.Raycast(muzzleFlashLocation.position, hitPoint - muzzleFlashLocation.position, out hit, Mathf.Infinity))
            {
                //Hit Flash
                GameObject cacheHit = Instantiate(hitFlash, hit.point, Quaternion.Euler(0,0,0));
                cacheHit.transform.LookAt(-transform.root.position);
                cacheHit.GetComponent<ParticleSystem>().Play();
                Destroy(cacheHit, 0.5f);

                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    if (M4Weapon.IsCrit())
                        enemy.TakeDamage(M4Weapon.GetDamage() * M4Weapon.GetCritDamage(), true);
                    else
                        enemy.TakeDamage(M4Weapon.GetDamage(), false);

                }
            }
        }

        


        //firerate wait time
        yield return new WaitForSeconds(1 / M4Weapon.GetFireRate());

        canShoot = true;
    }


    WeaponType Weapon.GetWeaponType()
    {
        return M4Weapon.GetWeaponType();
    }

    void Weapon.SetLocation()
    {
        transform.parent.localPosition = M4Weapon.GetPosition();
        transform.parent.localEulerAngles = M4Weapon.GetRotation();
    }
}
