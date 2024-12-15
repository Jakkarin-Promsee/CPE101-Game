using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapBossManager : MonoBehaviour
{
    public GameObject doorActiveObj;
    public GameObject openDoorObj;
    public GameObject closeDoorObj;
    public GameObject boss;
    private bool isStageClear = false;


    void Start()
    {
        doorActiveObj.GetComponent<ActiveDoorArea>().mapManager = gameObject.GetComponent<MapManager>();

        UnActiveBoss();
        OpenTheDoor();
    }

    public void ActiveBoss()
    {
        boss.GetComponent<BossHpBar>().hpBar.SetActive(true);
        boss.GetComponent<SpiritKing>().Active();
    }

    public void UnActiveBoss()
    {
        boss.GetComponent<BossHpBar>().hpBar.SetActive(false);
        boss.GetComponent<SpiritKing>().UnActive();
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
        if (!isStageClear)
        {
            openDoorObj.GetComponent<TilemapRenderer>().sortingOrder = -30;
            openDoorObj.GetComponent<TilemapCollider2D>().isTrigger = true;
            closeDoorObj.GetComponent<TilemapRenderer>().sortingOrder = 0;
            closeDoorObj.GetComponent<TilemapCollider2D>().isTrigger = false;
        }
    }
}
