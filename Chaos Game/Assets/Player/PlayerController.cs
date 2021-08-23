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
    bool canPrimarySkill = true;
    bool canSecondarySkill = true;

    Animator anim;
    CameraController cam;

    private void Awake() => anim = GetComponentInChildren<Animator>();

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.GetComponent<CameraController>();
        activeWeapon = weaponOne.GetComponent<Weapon>();

        UIManager.instance.UpdateHealthBarMaxValue(health);
        UIManager.instance.UpdateHealthBarValue(health);
    }

    // Update is called once per frame
    void Update()
    {
        #region Movement
        float hori = Input.GetAxisRaw("Horizontal");
        float verti = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(hori, 0, verti);

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
                activeWeapon.PrimarySkill();
        }
        #endregion

        #region Secondary Skill
        if (Input.GetMouseButton(1))
        {
            if (canSecondarySkill)
                activeWeapon.SecondarySkill();
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
}