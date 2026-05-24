using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Bot1 : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    public float CurrentHealth;

    [SerializeField] FloatingHealthBar healthBar;

    public enum NPCStates { Patrol, Chase, Attack, Retreat }

    [Header("References")]
    public NavMeshAgent agent;
    public Transform Player;
    public GameObject Bullet;
    public Transform BotBulletSpawnPoint;

    [Header("Visuals")]
    public MeshRenderer meshRenderer;
    public Material PatrolMaterial, ChaseMaterial, AttackMaterial, RetreatMaterial;

    [Header("Patrol")]
    public Vector3[] PatrolPoints;
    private int nextPatrolPoint = 0;

    [Header("Combat")]
    public float ChaseRange = 8f;
    public float AttackRange = 5f;
    public float FireRate = 1.5f;
    private float nextShootTime = 0f;
    public float RetreatDistance = 2f;

    private NPCStates currentState = NPCStates.Patrol;
    private NPCStates lastState;

    private void Awake()
    {
        if (healthBar == null)
            healthBar = GetComponentInChildren<FloatingHealthBar>();

        if (healthBar != null)
            healthBar.Initialize(transform);
    }

    void Start()
    {
        CurrentHealth = maxHealth;
        if (healthBar != null)
            healthBar.UpdateHealthBar(CurrentHealth, maxHealth);

        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();

        if (PatrolPoints.Length > 0)
            agent.SetDestination(PatrolPoints[nextPatrolPoint]);
    }

    void Update()
    {
        if (!agent.isOnNavMesh || Player == null)
            return;

        switch (currentState)
        {
            case NPCStates.Patrol: Patrol(); break;
            case NPCStates.Chase: Chase(); break;
            case NPCStates.Attack: Attack(); break;
            case NPCStates.Retreat: Retreat(); break;
        }
        if (Player != null)
            DrawPathToTarget(Player.position);

        if (currentState != lastState)
        {
            lastState = currentState;
            if (healthBar != null)
                healthBar.UpdateState(currentState.ToString());
        }


        if (CurrentHealth <= 0)
            Die();
    }

    #region FSM States
    private void Patrol()
    {
        meshRenderer.material = PatrolMaterial;

        float dist = Vector3.Distance(transform.position, Player.position);
        if (dist <= ChaseRange)
            currentState = NPCStates.Chase;

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            nextPatrolPoint = (nextPatrolPoint + 1) % PatrolPoints.Length;
            agent.SetDestination(PatrolPoints[nextPatrolPoint]);
        }
    }

    private void Chase()
    {
        meshRenderer.material = ChaseMaterial;

        agent.SetDestination(Player.position);
        LookAtPlayer();

        float dist = Vector3.Distance(transform.position, Player.position);
        if (dist <= AttackRange)
            currentState = NPCStates.Attack;
        else if (dist > ChaseRange)
            currentState = NPCStates.Patrol;
    }

    private void Attack()
    {
        meshRenderer.material = AttackMaterial;

        agent.SetDestination(Player.position);
        LookAtPlayer();

        float dist = Vector3.Distance(transform.position, Player.position);

        if (Time.time >= nextShootTime)
        {
            nextShootTime = Time.time + FireRate;
            Shoot();
        }

        if (dist < RetreatDistance)
            currentState = NPCStates.Retreat;
        else if (dist > AttackRange)
            currentState = NPCStates.Chase;
    }

    private void Retreat()
    {
        meshRenderer.material = RetreatMaterial;

        Vector3 dir = (transform.position - Player.position).normalized;
        agent.SetDestination(transform.position + dir * 3f);

        float dist = Vector3.Distance(transform.position, Player.position);
        if (dist > AttackRange)
            currentState = NPCStates.Chase;
    }
    #endregion

    private void Shoot()
    {
        if (Bullet == null || BotBulletSpawnPoint == null) return;

        GameObject b = Instantiate(Bullet, BotBulletSpawnPoint.position, BotBulletSpawnPoint.rotation);
        Bullet bulletComp = b.GetComponent<Bullet>();
        if (bulletComp != null)
        {
            Rigidbody rb = b.GetComponent<Rigidbody>();
            rb.velocity = transform.forward * bulletComp.Speed;
        }
    }
    private void DrawPathToTarget(Vector3 targetPosition)
    {
        if (agent == null) return;

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPosition, path);

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth < 0) CurrentHealth = 0;

        if (healthBar != null)
            healthBar.UpdateHealthBar(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void LookAtPlayer()
    {
        Vector3 dir = (Player.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10f);
            Destroy(collision.gameObject);
        }
    }
}
