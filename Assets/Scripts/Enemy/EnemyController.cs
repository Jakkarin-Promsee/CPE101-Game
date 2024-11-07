using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float hp = 100;

    void Start()
    {

    }

    void Update()
    {
        if (hp <= 0) Destroy(gameObject);
    }
}
