using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    // Example item type (HealthPotion, ManaPotion, Weapon, etc.)
    public enum ItemType { Gun, Melee, HealthPotion, ManaPotion }
    public ItemType itemType;

    // Reference to the player's object for pickup logic
    private Transform player;

    // Called when the item is initialized (set player reference)
    public void SetupItem(Transform playerTransform)
    {
        player = playerTransform;
    }

    // Detect when the player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the collider belongs to the player
        {
            PickupItem();
        }
    }

    // Handle item pickup
    private void PickupItem()
    {
        Collider2D itemCollider = GetComponent<Collider2D>();
        if(itemCollider != null){
            itemCollider.enabled = false;
        }
        Destroy(gameObject);
    }
}