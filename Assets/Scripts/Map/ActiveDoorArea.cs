using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveDoorArea : MonoBehaviour
{
    public MapManager mapManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mapManager.CloseTheDoor();
            mapManager.ActiveEnemies();
        }
    }
}
