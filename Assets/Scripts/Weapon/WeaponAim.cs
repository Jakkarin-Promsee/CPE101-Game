using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    public Vector3 GetDefaultDirection()
    {
        // Get mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate direction to the mouse
        return mousePosition - transform.position;
    }
    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate direction to the mouse
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (gameObject.name == "Bow(Clone)") angle += 45.387f;

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
