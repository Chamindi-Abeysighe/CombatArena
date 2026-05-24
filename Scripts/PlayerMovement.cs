using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float gravity = -9.81f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 15f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
        Shoot();
    }

    void Move()
    {
        // Input
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(x, 0f, z);

        if (dir.magnitude > 1f)
            dir.Normalize();

        // Rotate player to movement direction
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        // Move horizontally
        Vector3 move = dir * moveSpeed;

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f; // small downward force to stick to ground

        velocity.y += gravity * Time.deltaTime;

        // Apply movement + gravity
        controller.Move((move + velocity) * Time.deltaTime);
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 shootDir = (hit.point - bulletSpawnPoint.position).normalized;
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(shootDir));
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.velocity = shootDir * bulletSpeed;
            }
        }
    }
}
