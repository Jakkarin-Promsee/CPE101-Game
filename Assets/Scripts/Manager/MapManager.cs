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

    public Tilemap groundTilemap;  // Reference to the ground tilemap
    public Tilemap obstructionTilemap;  // Reference to the obstruction tilemap
    public GameObject[] enemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        doorActiveObj.GetComponent<ActiveDoorArea>().mapManager = gameObject.GetComponent<MapManager>();
        OpenTheDoor();

        InvokeRepeating(nameof(SponEnemyRandom), 3f, 1f);
    }

    private void SponEnemyRandom()
    {
        int numberOfEnemies = 2;
        List<Vector3> freeSpaces = GetFreeSpaces();

        // Spawn enemies on random free spaces
        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (freeSpaces.Count == 0)
            {
                Debug.LogWarning("No free spaces available to spawn enemies!");
                break;
            }

            // Pick a random free space
            int randomIndex = Random.Range(0, freeSpaces.Count);
            int prefabIndex = Random.Range(0, enemyPrefab.Length);
            Vector3 spawnPosition = freeSpaces[randomIndex];

            // Spawn the enemy
            Instantiate(enemyPrefab[prefabIndex], spawnPosition, Quaternion.identity);

            // Remove the chosen position to avoid spawning multiple enemies at the same spot
            freeSpaces.RemoveAt(randomIndex);
        }
    }

    List<Vector3> GetFreeSpaces()
    {
        List<Vector3> freeSpaces = new List<Vector3>();
        BoundsInt bounds = groundTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                // Check if the ground tile exists
                if (groundTilemap.HasTile(cellPosition))
                {
                    // Check if there is no obstruction tile
                    if (!obstructionTilemap.HasTile(cellPosition))
                    {
                        // Convert cell position to world position and add to the free spaces list
                        Vector3 worldPosition = groundTilemap.CellToWorld(cellPosition) + groundTilemap.tileAnchor;
                        freeSpaces.Add(worldPosition);
                    }
                }
            }
        }


        return freeSpaces;
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
