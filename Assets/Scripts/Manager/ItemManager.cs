using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    // Item types
    public enum ItemType {
        Gun,
        Melee,
        HealthPotion,
        ManaPotion
    };
    
    [Header("Keys")]
    [SerializeField] KeyCode throwItemKey;
    [SerializeField] KeyCode pickItemKey;

    [Header("Item inventory")]
    public List<GameObject> itemPrefabs;

    private GameObject currentItem;
    public ItemType currentItemType;
    private int currentItemIndex = 0;
    private Transform playerTransform;

    // ! Test
    private GameObject droppedItem;
    public float droppedItemScale = 0.5f;

    // ! Test

    void Start()
    {
        playerTransform = gameObject.GetComponent<Transform>();
        EquipItem(currentItemIndex);
    }

    void Update()
    {
        if(currentItem != null){
            // Check 'left hold'
            if (Input.GetMouseButton(0))
            {
                // Fire if player's holding Gun
                if (currentItemType == ItemType.Gun)
                {
                    currentItem.GetComponent<Gun>().Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
                // Swing if player's holding Melee
                else if (currentItemType == ItemType.Melee)
                {
                    currentItem.GetComponentInChildren<Melee>().Swing();
                }
            }
            // Check for F
            if (Input.GetKeyDown(KeyCode.F)){
                // Start Bullet reflecting skill
                if(currentItemType == ItemType.Melee)
                    currentItem.GetComponentInChildren<Melee>().Reflect();
            }
        }

        // ! Test
        if (Input.GetKeyDown(KeyCode.G)){
            if(currentItem != null){
                DropItem();
            }
        }
        // ! Test

        // Switch guns with number keys (1, 2, 3, etc.)
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipItem(2);

        if (Input.GetKeyDown(KeyCode.R)) // Switch guns with 'r' key
        {
            currentItemIndex++;
            if (currentItemIndex >= itemPrefabs.Count) currentItemIndex -= itemPrefabs.Count;
            EquipItem(currentItemIndex);
        }
    }

    void EquipItem(int index)
    {
        if (index >= 0 && index < itemPrefabs.Count)
        {
            // Destroy the current weapon if it exists
            if (currentItem != null) Destroy(currentItem);

            // Instantiate the new weapon as a child of the player at position (0,0)
            currentItem = Instantiate(itemPrefabs[index], playerTransform.position, Quaternion.identity, playerTransform);

            // Weapon setup
            if (currentItem.GetComponent<Gun>())
            {
                // Link player object to weapon
                currentItemType = ItemType.Gun;
                currentItem.GetComponent<Gun>().player = gameObject;
                currentItem.GetComponent<Gun>().weaponOwnerTag = gameObject.tag;
            }
            else if (currentItem.GetComponentInChildren<Melee>())
            {
                currentItemType = ItemType.Melee;   
                currentItem.GetComponentInChildren<Melee>().player = gameObject;
                currentItem.GetComponentInChildren<Melee>().weaponOwnerTag = gameObject.tag;
            }

            // Add weaponAim component, aim weapon to mouse position
            // And set weapon layer to -3
            currentItem.AddComponent<WeaponAim>();
            currentItem.transform.localPosition = new Vector3(0, 0, -3);

            // Update the current weapon index
            currentItemIndex = index;
        }
    }

    // Function to drop the item
    private void DropItem(){
        // Instantiate the item at the player's current position (or in front, based on your choice)
        Vector3 dropPosition = playerTransform.position;
        droppedItem = Instantiate(itemPrefabs[currentItemIndex], dropPosition, Quaternion.identity);
        droppedItem.transform.localScale = droppedItem.transform.localScale * droppedItemScale;

        // Remove from Item Inventory
        itemPrefabs.RemoveAt(currentItemIndex);

        // Remove from World
        Destroy(currentItem);

        // Equip another item instead
        if(itemPrefabs.Count > 0){
            if(currentItemIndex >= itemPrefabs.Count){
                currentItemIndex--;
            }
            EquipItem(currentItemIndex);
        }
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ItemManager : MonoBehaviour
// {
//     public enum ItemType { Gun, Melee}
    
//     [Header("Keys")]
//     [SerializeField] KeyCode throwItemKey;
//     [SerializeField] KeyCode pickItemKey;

//     [Header("Item gameObjects")]
//     public GameObject[] itemPrefabs;

//     private GameObject currentItem;
//     public ItemType currentItemType;
//     private int currentItemIndex = 0;
//     private Transform playerTransform;

//     void Start()
//     {
//         playerTransform = gameObject.GetComponent<Transform>();
//         EquipItem(currentItemIndex);
//     }

//     void Update()
//     {
//         // Check 'left hold'
//         if (Input.GetMouseButton(0))
//         {
//             // Fire if player's holding Gun
//             if (currentItemType == ItemType.Gun)
//             {
//                 currentItem.GetComponent<Gun>().Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
//             }
//             // Swing if player's holding Melee
//             else if (currentItemType == ItemType.Melee)
//             {
//                 currentItem.GetComponentInChildren<Melee>().Swing();
//             }
//         }
//         // Check for F
//         if (Input.GetKeyDown(KeyCode.F)){
//             // Start Bullet reflecting skill
//             if(currentItemType == ItemType.Melee)
//                 currentItem.GetComponentInChildren<Melee>().Reflect();
//         }

//         // Switch guns with number keys (1, 2, 3, etc.)
//         if (Input.GetKeyDown(KeyCode.Alpha1)) EquipItem(0);
//         if (Input.GetKeyDown(KeyCode.Alpha2)) EquipItem(1);
//         if (Input.GetKeyDown(KeyCode.Alpha3)) EquipItem(2);

//         if (Input.GetKeyDown(KeyCode.R)) // Switch guns with 'r' key
//         {
//             currentItemIndex++;
//             if (currentItemIndex >= itemPrefabs.Length) currentItemIndex -= itemPrefabs.Length;
//             EquipItem(currentItemIndex);
//         }
//     }

//     void EquipItem(int index)
//     {
//         if (index >= 0 && index < itemPrefabs.Length)
//         {
//             // Destroy the current weapon if it exists
//             if (currentItem != null) Destroy(currentItem);

//             // Instantiate the new weapon as a child of the player at position (0,0)
//             currentItem = Instantiate(itemPrefabs[index], playerTransform.position, Quaternion.identity, playerTransform);

//             // Weapon setup
//             if (currentItem.GetComponent<Gun>())
//             {
//                 // Link player object to weapon
//                 currentItemType = ItemType.Gun;
//                 currentItem.GetComponent<Gun>().player = gameObject;
//                 currentItem.GetComponent<Gun>().weaponOwnerTag = gameObject.tag;
//             }
//             else if (currentItem.GetComponentInChildren<Melee>())
//             {
//                 currentItemType = ItemType.Melee;   
//                 currentItem.GetComponentInChildren<Melee>().player = gameObject;
//                 currentItem.GetComponentInChildren<Melee>().weaponOwnerTag = gameObject.tag;
//             }

//             // Add weaponAim component, aim weapon to mouse position
//             // And set weapon layer to -3
//             currentItem.AddComponent<WeaponAim>();
//             currentItem.transform.localPosition = new Vector3(0, 0, -3);

//             // Update the current weapon index
//             currentItemIndex = index;
//         }
//     }
// }
