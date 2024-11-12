using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponAim : MonoBehaviour
{
    public Transform player;
    public Transform enemy;
    public float attackRange;

    void Start()
    {
        // Call the specified function every 0.5 seconds
        InvokeRepeating(nameof(MoveAngle), 0f, 0.2f);
    }

    private void MoveAngle()
    {
        if (Vector3.Distance(enemy.position, player.position) < attackRange)
        {
            // Calculate direction to the player
            Vector3 direction = player.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (angle > 90 || angle < -90)
            {
                angle -= 180;
                if (transform.localScale.x > 0) transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                if (transform.localScale.x < 0) transform.localScale = new Vector3(transform.localScale.x * (-1), transform.localScale.y, transform.localScale.z);
            }

            // Rotate the gun point to face the mouse
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
