using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class EnemyActionController : MonoBehaviour
{

    private float nextMovement = 0f;
    public void TakeKnockback(Vector3 force, float knockbackTime)
    {
        nextMovement = Time.time + knockbackTime;
        float mass = gameObject.GetComponent<Rigidbody2D>().mass;
        gameObject.GetComponent<Rigidbody2D>().AddForce(force * mass, ForceMode2D.Impulse);
    }
}
