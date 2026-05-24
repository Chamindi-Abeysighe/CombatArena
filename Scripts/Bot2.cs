using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot2 : MonoBehaviour
{
    public enum NPCStates { Patrol, HideAttack }

    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    public float CurrentHealth;

    [SerializeField] FloatingHealthBar healthBar;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform Player;
    public GameObject Bullet;
    public Transform BotBulletSpawnPoint;

    [Header("Visuals")]
    public MeshRenderer meshRenderer;
    public Material PatrolMaterial;
    public Material HideMaterial;
    public Material AttackMaterial;
    public Material ShootMaterial;

    [Header("Patrol")]
    public Vector3[] PatrolPoints;
    private int nextPatrolPoint = 0;

    [Header("Combat")]
    public float FireRate = 1.5f;
    private float nextShootTime = 0f;

    [Header("Hide Points")]
    public List<Transform> HidePoints;
    private Transform currentHidePoint;
    private bool isAtHide = false;

    private NPCStates currentState = NPCStates.Patrol;
    private NPCStates lastState;

    // Distance thresholds
    private float HideDistance = 5f;  // hide if player ≤ 5
    private float ShootDistance = 7f; // shoot if player ≤ 7

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

        if (PatrolPoints.Length > 0)
            agent.SetDestination(PatrolPoints[nextPatrolPoint]);
    }

    private void Patrol()
    {
        meshRenderer.material = PatrolMaterial;

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            nextPatrolPoint = (nextPatrolPoint + 1) % PatrolPoints.Length;
            agent.SetDestination(PatrolPoints[nextPatrolPoint]);
        }

        // Check if player is close enough to trigger HideAttack
        float distToPlayer = Vector3.Distance(transform.position, Player.position);
        if (distToPlayer <= ShootDistance) // <=7 units
        {
            currentState = NPCStates.HideAttack;
            currentHidePoint = null; // reset hide point
            isAtHide = false;
        }
    }

    void Update()
    {
        if (!agent.isOnNavMesh || Player == null) return;

        switch (currentState)
        {
            case NPCStates.Patrol: Patrol(); break;
            case NPCStates.HideAttack: HideAndAttack(); break;
        }

        if (currentState != lastState)
        {
            lastState = currentState;
            if (healthBar != null)
                healthBar.UpdateState(currentState.ToString());
        }

        if (CurrentHealth <= 0) Die();
    }

    private void HideAndAttack()
    {
        float distToPlayer = Vector3.Distance(transform.position, Player.position);

        // Player very close → hide
        if (distToPlayer <= HideDistance)
        {
            meshRenderer.material = HideMaterial;

            if (currentHidePoint == null)
                ChooseHidePoint();

            if (!isAtHide && currentHidePoint != null)
            {
                agent.SetDestination(currentHidePoint.position);
                if (Vector3.Distance(transform.position, currentHidePoint.position) < 0.5f)
                    isAtHide = true;
            }
        }
        // Player at shooting distance → shoot
        else if (distToPlayer <= ShootDistance)
        {
            meshRenderer.material = ShootMaterial;
            agent.SetDestination(transform.position); // stay in place
            isAtHide = false; // reset for next hide

            // Look at player
            Vector3 dir = (Player.position - transform.position).normalized;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);

            // Shoot
            if (Time.time >= nextShootTime)
            {
                nextShootTime = Time.time + FireRate;
                Shoot();
            }
        }
        // Player far → resume patrol
        else
        {
            currentState = NPCStates.Patrol;
            isAtHide = false;
            currentHidePoint = null;
        }
    }

    private void ChooseHidePoint()
    {
        if (HidePoints.Count == 0) return;
        int index = Random.Range(0, HidePoints.Count);
        currentHidePoint = HidePoints[index];
        isAtHide = false;
    }

    private void Shoot()
    {
        if (Bullet == null || BotBulletSpawnPoint == null) return;

        GameObject b = Instantiate(Bullet, BotBulletSpawnPoint.position, BotBulletSpawnPoint.rotation);
        Rigidbody rb = b.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = transform.forward * 25f; // bullet speed
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(10f);
            Destroy(collision.gameObject);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}



