using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public GameObject doorActiveObj;
    public GameObject openDoorObj;
    public GameObject closeDoorObj;
    public GameObject[] setupEnemies;


    // Start is called before the first frame update
    void Start()
    {
        doorActiveObj.GetComponent<ActiveDoorArea>().mapManager = gameObject.GetComponent<MapManager>();
        OpenTheDoor();
    }

    public void ActiveEnemies()
    {
        for (int i = 0; i < setupEnemies.Length; i++)
        {
            setupEnemies[i].GetComponent<EnemyActionController>().active = true;
        }
    }

    public void OpenTheDoor()
    {
        openDoorObj.GetComponent<TilemapRenderer>().sortingOrder = 0;
        openDoorObj.GetComponent<TilemapCollider2D>().isTrigger = false;
        closeDoorObj.GetComponent<TilemapRenderer>().sortingOrder = -30;
        closeDoorObj.GetComponent<TilemapCollider2D>().isTrigger = true;
    }

    public void CloseTheDoor()
    {
        openDoorObj.GetComponent<TilemapRenderer>().sortingOrder = -30;
        openDoorObj.GetComponent<TilemapCollider2D>().isTrigger = true;
        closeDoorObj.GetComponent<TilemapRenderer>().sortingOrder = 0;
        closeDoorObj.GetComponent<TilemapCollider2D>().isTrigger = false;
    }
}
