using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollect : MonoBehaviour
{

    public int coinsCollected = 0;
    public int SheildCollected = 0;
    public int WeaponCollected = 0;
    public int MagicPickupCollected = 0;
    public int HealthPotionCollected = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin")) { coinsCollected++; Destroy(other.gameObject); }
        else if (other.CompareTag("Sheild")) { SheildCollected++; Destroy(other.gameObject); }
        else if (other.CompareTag("Weapon")) { WeaponCollected++; Destroy(other.gameObject); }
        else if (other.CompareTag("MagicPickup")) { MagicPickupCollected++; Destroy(other.gameObject); }
        else if (other.CompareTag("HealthPotion")) { HealthPotionCollected++; Destroy(other.gameObject); }
    }
}
