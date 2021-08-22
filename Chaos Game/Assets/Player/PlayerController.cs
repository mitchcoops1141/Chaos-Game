using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] LayerMask aimingLayerMask;

    Animator anim;
    CameraController cam;

    private void Awake() => anim = GetComponentInChildren<Animator>();

    // Start is called before the first frame update
    void Start()
    {
         cam = Camera.main.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Movement
        float hori = Input.GetAxisRaw("Horizontal");
        float verti = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(hori, 0, verti);

        if (movement.magnitude > 0)
        {
            movement.Normalize();
            //if sprinting
            if (Input.GetButton("Shift"))
            {
                anim.SetBool("Sprinting", true);
                movement *= sprintSpeed * Time.deltaTime;
            }
            //if not sprinting
            else
            {
                anim.SetBool("Sprinting", false);
                movement *= speed * Time.deltaTime;
            }
            transform.Translate(movement, Space.World);
        }
        #endregion

        #region Animations

        float velocityX = Vector3.Dot(movement.normalized, transform.right);
        float velocityZ = Vector3.Dot(movement.normalized, transform.forward);
        print(velocityX);

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

        #region Camera Controller
        cam.SetCameraPosition(transform.position);
        #endregion
    }
}
