using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulletbot : MonoBehaviour
{
    public float Speed = 25f;
    public float Damage = 10f;
    public float Lifetime = 8f;

    private void Start()
    {
        // Destroy bullet after lifetime expires
        Destroy(gameObject, Lifetime);
    }

    private void Update()
    {
        // Move bullet forward manually
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collisions with other bullets
        if (other.CompareTag("BulletBot")) return;

        //Damage Player
        Health player = other.GetComponent<Health>();
        if (player != null)
        {
            player.TakeDamage(Damage);
            Destroy(gameObject);
            return;
        }


    }
}
