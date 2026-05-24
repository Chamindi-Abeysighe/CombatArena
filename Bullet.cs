using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
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
        if (other.CompareTag("Bullet")) return;

      
        // Damage Bot1
        Bot1 bot1 = other.GetComponent<Bot1>();
        if (bot1 != null)
        {
            bot1.TakeDamage(Damage);
            Destroy(gameObject);
            return;
        }

        // Damage Bot2
        Bot2 bot2 = other.GetComponent<Bot2>();
        if (bot2 != null)
        {
            bot2.TakeDamage((int)Damage);
            Destroy(gameObject);
            return;
        }

    }
}
