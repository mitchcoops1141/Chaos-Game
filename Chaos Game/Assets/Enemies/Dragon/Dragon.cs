using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour, Enemy
{
    [SerializeField] private float health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //moves:
    //fire breath
    //bite
    //fly

    int[] tests;

    void Enemy.TakeDamage(float dmg, bool isCrit)
    {

        health -= dmg;

        if (health <= 0)
        {
            GetComponent<Enemy>().Die();
        }
    }

    void Enemy.Die()
    {
        //die code
        Destroy(gameObject);
    }
}
