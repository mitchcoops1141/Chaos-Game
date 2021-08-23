using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M4 : MonoBehaviour, Weapon
{

    [Header("Stats")]
    [SerializeField] [Tooltip("Amount of bullets per second")] private float firerate;
    [SerializeField] private float damage;
    
    [Header("Connections")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Transform muzzleFlashLocation;
    [SerializeField] private GameObject hitFlash;
    [SerializeField] private LayerMask layersToHit;

    bool canShoot = true;
    void Weapon.PrimarySkill()
    {
        if (canShoot)
            StartCoroutine("ShootIE");
    }

    void Weapon.SecondarySkill()
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
                    
                    enemy.TakeDamage(damage);
                }
            }
        }

        


        //firerate wait time
        yield return new WaitForSeconds(1 / firerate);

        canShoot = true;
    }
}
