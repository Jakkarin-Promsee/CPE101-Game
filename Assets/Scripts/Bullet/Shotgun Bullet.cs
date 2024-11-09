using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShotgunBullet : Bullet
{
    public int pelletCount = 5;
    public float spreadAngle = 15f;
    public Vector3 pos;

    public override void Start()
    {
        base.Start();
    }

    public void FirePellets(float angle)
    {
        for (int i = 0; i < pelletCount; i++)
        {
            // Calculate direction towards the mouse position
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            Vector3 direction = (mousePosition - pos).normalized;

            // Calculate rotation to face the mouse
            float buffer_angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float ran = Random.Range(-spreadAngle, spreadAngle);
            Instantiate(this, pos, Quaternion.Euler(0, 0, buffer_angle + angle + ran));
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}
