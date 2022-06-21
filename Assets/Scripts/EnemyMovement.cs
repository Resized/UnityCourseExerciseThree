using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private EnemyStates enemyState;
    private EnemyStates previousState;
    private RaycastHit hit;
    private GameObject eyeHeight;
    private int healthPoints;
    public enum EnemyStates
    {
        Idle,
        Roam,
        Chase,
        Attack,
        Hit,
        Dead
    }
    public float chaseRange = 1000;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        enemyState = EnemyStates.Idle;
        eyeHeight = transform.Find("EyeHeight").gameObject;
        healthPoints = 100;
    }
    
    public void Hit(int hitAmount)
    {
        if (healthPoints <= 0)
        {
            enemyState = EnemyStates.Dead;
            return;
        }
        if (enemyState != EnemyStates.Hit)
        {
            previousState = enemyState;
        }
        enemyState = EnemyStates.Hit;
        healthPoints -= hitAmount;
    }

    // Update is called once per frame
    void Update()
    {    
        switch (enemyState)
        {
            case EnemyStates.Idle:
                Idle();
                break;
            case EnemyStates.Roam:
                Roam();
                break;
            case EnemyStates.Chase:
                Chase();
                break;
            case EnemyStates.Attack:
                Attack();
                break;
            case EnemyStates.Hit:
                Hit();
                break;
            case EnemyStates.Dead:
                Dead();
                break;
            default:
                break;
        }



    }

    private void Dead()
    {
        animator.SetInteger("State", (int)EnemyStates.Dead);
        //destroy enemy collision
        Destroy(GetComponent<Collider>());
        agent.enabled = false;
    }

    private void Hit()
    {
        agent.isStopped = true;
        // wait 1 second for animation to end
        StartCoroutine(WaitForHit());
    }

    private void Attack()
    {
        agent.isStopped = true;
        animator.SetInteger("State", (int)EnemyStates.Attack);
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetInteger("State", (int)EnemyStates.Chase);
    }

    private void Roam()
    {
        agent.isStopped = false;
        animator.SetInteger("State", (int)EnemyStates.Roam);
    }

    private void Idle()
    {
        animator.SetInteger("State", (int)EnemyStates.Idle);

        // if player is in range, change state to chase
        if (Physics.Raycast(eyeHeight.transform.position, player.position - eyeHeight.transform.position, out hit, chaseRange))
        {
            if (hit.collider.tag == "Player")
            {
                // if it hits, set the enemy state to chase
                enemyState = EnemyStates.Chase;
            }
        }
    }

    IEnumerator WaitForHit()
    {
        yield return new WaitForSeconds(1);
        enemyState = previousState;
    }
}
