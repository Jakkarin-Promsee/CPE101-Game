using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveDoorArea : MonoBehaviour
{
    public MapManager mapManager;
    public MapBossManager mapBossManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (mapManager != null)
            {
                mapManager.CloseTheDoor();
                mapManager.ActiveEnemies();
            }
            if (mapBossManager != null)
            {
                mapBossManager.CloseTheDoor();
                mapBossManager.ActiveBoss();
            }

        }
    }
}
