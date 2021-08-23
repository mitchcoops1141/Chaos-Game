using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Golem : MonoBehaviour, Enemy
{
    private enum States
    {
        Wandering,
        Agro,
        Attacking,
        Blocking,
        Charge
    }

    [SerializeField] private Billboard billboard;
    [SerializeField] private SkinnedMeshRenderer mesh;

    [Header("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float chargeSpeed;

    [SerializeField] private float agroRange;
    [SerializeField] private float attackingRange;

    States state;
    PlayerController player;
    Animator anim;

    float maxHealth;
    bool shouldWanderingIE = true;
    bool shouldAgroIE = true;
    bool shouldBlockingIE = true;
    bool shouldChargeIE = true;
    bool shouldAttackIE = true;
    bool canFunction = true;
    bool canEnterChargeAtHalfHealth = true;
    int agroTimer = 0;

    Vector3 wanderToPos = Vector3.zero;
    float startSpeed;
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        startSpeed = speed;
        billboard.UpdateHealthBarMaxValue(maxHealth);
        billboard.UpdateHealthBarValue(health);
        state = States.Wandering;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canFunction)
        {
            Vector3 movement;

            switch (state)
            {
                case States.Agro:
                    //movement
                    movement = (player.transform.position - transform.position).normalized;
                    movement *= speed * Time.deltaTime;
                    transform.Translate(movement, Space.World);

                    if (Vector3.Distance(transform.position, player.transform.position) <= 1.5f)
                    {
                        speed = 0;
                    }
                    else
                        speed = startSpeed;

                    //rotation
                    transform.LookAt(player.transform);

                    if (shouldAgroIE)
                        StartCoroutine("AgroIE");
                    break;

                case States.Attacking:
                    if (shouldAttackIE)
                        StartCoroutine("AttackingIE");
                    break;

                case States.Blocking:
                    //set animation trigger to be blocking
                    if (shouldBlockingIE)
                        StartCoroutine("BlockingIE");
                    break;

                case States.Wandering:
                    //movement
                    movement = (wanderToPos - transform.position).normalized;
                    movement *= speed * Time.deltaTime;
                    transform.Translate(movement, Space.World);

                    if (shouldWanderingIE)
                        StartCoroutine("WanderingIE");
                    break;

                case States.Charge:
                    //movement
                    movement = (player.transform.position - transform.position).normalized;
                    movement *= chargeSpeed * Time.deltaTime;
                    transform.Translate(movement, Space.World);

                    transform.LookAt(player.transform.position);
                    if (shouldChargeIE)
                        StartCoroutine("ChargeIE");
                    break;
            }
        }
    }
    IEnumerator WanderingIE()
    {
        shouldWanderingIE = false;

        float cacheSpeed = speed;
        //rotation
        transform.LookAt(wanderToPos);
        if (wanderToPos == Vector3.zero || Vector3.Distance(transform.position, wanderToPos) < 0.5)
        {
            wanderToPos = Random.insideUnitCircle * agroRange;
            wanderToPos.z = wanderToPos.y;
            wanderToPos.y = 0f;
            speed = 0;
        }

        yield return new WaitForSeconds(0.25f);

        //check agro range
        if (Vector3.Distance(transform.position, player.transform.position) < agroRange)
        {
            state = States.Agro;
            StartCoroutine("AgroTimerIE");
        }

        speed = cacheSpeed;

        shouldWanderingIE = true;
    }

    IEnumerator AgroTimerIE()
    {
        while(state == States.Agro)
        {
            yield return new WaitForSeconds(1f);

            agroTimer++;

            if (agroTimer >= 5)
            {
                state = States.Blocking;
            }
        }
    }

    IEnumerator AgroIE()
    {
        shouldAgroIE = false;

        yield return new WaitForSeconds(0.5f);

        //check the range to see if can attack
        if (Vector3.Distance(transform.position, player.transform.position) < attackingRange)
            state = States.Attacking;

        shouldAgroIE = true;
    }

    IEnumerator BlockingIE()
    {
        shouldBlockingIE = false;
        anim.SetBool("Blocking", true);

        yield return new WaitForSeconds(4f);

        state = States.Charge;

        anim.SetBool("Blocking", false);
        shouldBlockingIE = true;
    }

    IEnumerator ChargeIE()
    {
        shouldChargeIE = false;

        while(Vector3.Distance(transform.position, player.transform.position) > 0.5f)
        {
            yield return null;
        }

        state = States.Attacking;
        shouldChargeIE = true;
    }

    IEnumerator AttackingIE()
    {
        shouldAttackIE = false;

        float cacheSpeed = speed;
        speed = 0;
        transform.LookAt(player.transform);

        if (Random.Range(0, 2) == 0)
            anim.SetTrigger("Punch");
        else
            anim.SetTrigger("DoublePunch");

        yield return new WaitForSeconds(0.8f);

        speed = cacheSpeed;
        state = States.Agro;
        shouldAttackIE = true;
    }

    bool canGoRed = true;
    void Enemy.TakeDamage(float dmg)
    {
        if (canFunction)
        {
            if (state == States.Blocking)
                dmg /= 2;

            health -= dmg;

            billboard.UpdateHealthBarValue(health);

            if (canGoRed)
                StartCoroutine("Hurt");

            if (health > 0 && health < maxHealth / 2 && canEnterChargeAtHalfHealth && state != States.Charge)
            {
                state = States.Blocking;
                canEnterChargeAtHalfHealth = false;
            }


            if (health <= 0)
                GetComponent<Enemy>().Die();
        }

    }

    
    IEnumerator Hurt()
    {
        canGoRed = false;

        mesh.materials[0].color = Color.red;

        yield return new WaitForSeconds(0.2f);

        mesh.materials[0].color = Color.white;

        canGoRed = true;
    }

    void Enemy.Die()
    {
        canFunction = false;

        anim.SetTrigger("Dead");

        StartCoroutine("DieIE");
    }

    IEnumerator DieIE()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length + 2f);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player.gameObject)
        {
            player.TakeDamage(damage);
        }
    }
}
