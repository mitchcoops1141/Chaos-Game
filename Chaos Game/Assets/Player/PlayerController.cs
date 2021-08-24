using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float health;

    [Header("Dashing")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    [Header("Aiming")]
    [SerializeField] LayerMask aimingLayerMask;

    [Header("Weapons")]
    [SerializeField] private GameObject weaponOne;
    [SerializeField] private GameObject weaponTwo;


    bool canGetHit = true;

    [Header("Getting Hit")]
    [SerializeField] private SkinnedMeshRenderer[] meshes;
    [SerializeField] private float redTime;

    Weapon activeWeapon;

    bool canDash = true;
    bool canMove = true;
    bool canPrimarySkill = true;
    bool canSecondarySkill = true;

    float maxHealth;

    Animator anim;
    CameraController cam;

    private void Awake() => anim = GetComponentInChildren<Animator>();

    // Start is called before the first frame update
    void Start()
    {  
        cam = Camera.main.GetComponent<CameraController>();

        maxHealth = health;
        UIManager.instance.UpdateHealthBarMaxValue(maxHealth);
        UIManager.instance.UpdateHealthBarValue(health);

        activeWeapon = weaponOne.GetComponent<Weapon>();
        weaponOne.SetActive(true);
        weaponTwo.SetActive(false);
        SetAnimationStyle();


    }

    void SetAnimationStyle()
    {
        switch (activeWeapon.GetWeaponType())
        {
            case WeaponType.Gun:
                anim.SetLayerWeight(1, 1);
                anim.SetLayerWeight(2, 0);
                break;

            case WeaponType.Melee:
                anim.SetLayerWeight(2, 1);
                anim.SetLayerWeight(1, 0);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        #region Movement
        
        float hori = Input.GetAxisRaw("Horizontal");
        float verti = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(hori, 0, verti);

        if (canMove)
        {
            if (Input.GetButtonDown("Shift"))
            {
                if (canDash)
                    StartCoroutine("DashIE");
            }

            if (movement.magnitude > 0)
            {
                movement.Normalize();
                movement *= speed * Time.deltaTime;
                transform.Translate(movement, Space.World);
            }
        }
        #endregion

        #region Animations

        float velocityX = Vector3.Dot(movement.normalized, transform.right);
        float velocityZ = Vector3.Dot(movement.normalized, transform.forward);

        anim.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
        anim.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        #endregion

        #region Aiming
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimingLayerMask))
        {
            var dir = hitInfo.point - transform.position;
            dir.y = 0f;
            dir.Normalize();
            transform.forward = dir;
        }
        #endregion

        #region Primary Skill
        if (Input.GetMouseButton(0))
        {
            if (canPrimarySkill)
                activeWeapon.PrimarySkill(anim);
        }
        #endregion

        #region Secondary Skill
        if (Input.GetMouseButton(1))
        {
            if (canSecondarySkill)
                activeWeapon.SecondarySkill(anim);
        }

        #endregion

        #region Swapping Weapons
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("SwapWeapons");
            canPrimarySkill = false;
            canSecondarySkill = false;
        }
        #endregion

        #region Camera Controller
        cam.SetCameraPosition(transform.position);
        #endregion
    }

    IEnumerator DashIE()
    {
        canDash = false;

        float cacheSpeed = speed;
        speed = dashSpeed;

        yield return new WaitForSeconds(dashTime);

        speed = cacheSpeed;
        
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    public void StopMovement()
    {
        canMove = false;
    }

    public void PlayMovement()
    {
        canMove = true;
    }

    public void TakeDamage(float dmg)
    {
        if (canGetHit)
        {
            StartCoroutine("MeshBlinking");
            health -= dmg;

            UIManager.instance.UpdateHealthBarValue(health);

            if (health <= 0)
            {
                Die();
            }
        }

    }

    void Die()
    {
        print("DEAD");
    }

    IEnumerator MeshBlinking()
    {
        canGetHit = false;

        foreach(SkinnedMeshRenderer mesh in meshes)
        {
            mesh.enabled = false;
        }

        yield return new WaitForSeconds(0.2f);

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.enabled = true;
        }

        yield return new WaitForSeconds(0.2f);

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.enabled = false;
        }

        yield return new WaitForSeconds(0.2f);

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.enabled = true;
        }

        yield return new WaitForSeconds(0.2f);

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.enabled = false;
        }

        yield return new WaitForSeconds(0.2f);

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.enabled = true;
        }

        canGetHit = true;
    }

    public void SwapWeaponsAnimTrigger()
    {
        weaponOne.SetActive(false);
        weaponTwo.SetActive(false);

        if (activeWeapon == weaponOne.GetComponent<Weapon>())
        {
            activeWeapon = weaponTwo.GetComponent<Weapon>();
            weaponTwo.SetActive(true);
        }
        else
        {
            activeWeapon = weaponOne.GetComponent<Weapon>();
            weaponOne.SetActive(true);
        }

        activeWeapon.SetLocation();

        canPrimarySkill = true;
        canSecondarySkill = true;

        SetAnimationStyle();
    }

    public void SetHealth(float h)
    {
        if (h >= maxHealth)
        {
            health = maxHealth;
        }
        else
            health = h;

        UIManager.instance.UpdateHealthBarValue(health);
    }

    public float GetHealth()
    {
        return health;
    }

    #region Holy Sword Hitboxes
    public void ShowHolySwordHitbox()
    {
        if (activeWeapon == weaponOne.GetComponent<Weapon>())
        {
            weaponOne.GetComponent<HolySword>().ShowHitBox();
        }
        else
        {
            weaponTwo.GetComponent<HolySword>().ShowHitBox();
        }
    }

    public void HideHolySwordHitbox()
    {
        if (activeWeapon == weaponOne.GetComponent<Weapon>())
        {
            weaponOne.GetComponent<HolySword>().HideHitBox();
        }
        else
        {
            weaponTwo.GetComponent<HolySword>().HideHitBox();
        }
    }

    public void ShowHolySwordSpinHitbox()
    {
        if (activeWeapon == weaponOne.GetComponent<Weapon>())
        {
            weaponOne.GetComponent<HolySword>().ShowSpinHitbox();
        }
        else
        {
            weaponTwo.GetComponent<HolySword>().ShowSpinHitbox();
        }
    }
    #endregion
}