using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bullet Config")]
public class BulletConfig : ScriptableObject
{
    public bool isMoveThroughObject = false;
    public float speed = 10f;
    public float lifespan = 20f;
    public GameObject explosionPrefab;

}
