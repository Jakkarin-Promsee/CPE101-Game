using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TreasureRoom : MonoBehaviour
{
    public Transform player;
    public MapManager mapManager;
    public GameObject leftTreasureArea;
    public Transform leftTreasurePivot;
    public GameObject rightTreasureArea;
    public Transform rightTreasurePivot;
    public GameObject middleActiveArea;
    public Transform middleTreasurePivot;
    public Color blinkColor = Color.black;
    public float blinkInterval = 0.3f;
    public GameObject leftWeapon;
    public GameObject rightWeapon;
    private bool isPlayerTab = false;

    private bool isCallBlink = false;

    private void Update()
    {
        if (mapManager.isStageClear)
        {
            if (!isCallBlink)
            {
                StartCoroutine(BlinkArea());
                isCallBlink = true;
            }

            float distance = Vector2.Distance(player.position, middleTreasurePivot.position);

            if (distance < 3)
            {
                isPlayerTab = true;

                Instantiate(leftWeapon, leftTreasurePivot.position, Quaternion.identity);
                Instantiate(rightWeapon, rightTreasurePivot.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
    }

    private IEnumerator BlinkArea()
    {
        Tilemap tilemap = middleActiveArea.GetComponent<Tilemap>();

        Color originalColor = tilemap.color;

        while (!isPlayerTab)
        {
            tilemap.color = blinkColor;
            yield return new WaitForSeconds(blinkInterval);
            tilemap.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
