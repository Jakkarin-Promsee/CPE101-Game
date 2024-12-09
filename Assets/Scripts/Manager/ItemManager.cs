using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class ItemManager : MonoBehaviour
{
    [Header("Inventory")]
    public List<string> Inventory;

    private GameObject currentItem;
    private ItemType currentItemType;
    private int currentItemIndex = 0;
    private Transform playerTransform;

    // Switch item
    [SerializeField] private KeyCode switchItemKey;

    // Drop item
    [SerializeField] private KeyCode dropItemKey;
    public float droppedItemScale = 0.5f; // Change size of dropped item

    // Pick up item
    [SerializeField] private KeyCode pickupItemKey;
    private bool isInPickupArea = false;
    private GameObject itemToPickUp;

    // Dictionary to work inventory
    private Dictionary<string, GameObject> itemInstantiate = new Dictionary<string, GameObject>();

    // Original prefabs to be instantiated later
    [Header("Item prefabs")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private GameObject akPrefab;
    [SerializeField] private GameObject pistolPrefab;
    [SerializeField] private GameObject rocketLauncherPrefab;
    [SerializeField] private GameObject magicOrbPrefab;
    [SerializeField] private GameObject bowPrefab;
    [SerializeField] private GameObject shotgunPrefab;

    // ! Test
    // Pick up item Text
    [Header("UI")]
    [SerializeField] private Text pickupItemText;
    // ! Test

    void Start()
    {
        playerTransform = gameObject.GetComponent<Transform>();

        // itemInstantiate Dict
        itemInstantiate.Add("Sword", swordPrefab);
        itemInstantiate.Add("AK", akPrefab);
        itemInstantiate.Add("Pistol", pistolPrefab);
        itemInstantiate.Add("Rocket Launcher", rocketLauncherPrefab);
        itemInstantiate.Add("Shotgun", shotgunPrefab);
        itemInstantiate.Add("Bow", bowPrefab);
        itemInstantiate.Add("Magic Orb", magicOrbPrefab);

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

        // Drop item
        if (Input.GetKeyDown(dropItemKey)){
            if(currentItem != null){
                DropItem();
            }
        }

        // Pick up item
        if (Input.GetKeyDown(pickupItemKey) && isInPickupArea){
            PickupItem(itemToPickUp);
        }

        // Switch guns with number keys (1, 2, 3, etc.)
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipItem(2);

        if (Input.GetKeyDown(switchItemKey)) // Switch guns with 'r' key
        {
            currentItemIndex++;
            if (currentItemIndex >= Inventory.Count) currentItemIndex -= Inventory.Count;
            EquipItem(currentItemIndex);
        }
    }

    void EquipItem(int index)
    {
        if (index >= 0 && index < Inventory.Count)
        {
            // Destroy the current weapon if it exists
            if (currentItem != null) Destroy(currentItem);

            // Instantiate the new weapon as a child of the player at position (0,0)
            currentItem = Instantiate(itemInstantiate[Inventory[index]], playerTransform.position, Quaternion.identity, playerTransform);
            
            // Set item type
            currentItemType = currentItem.GetComponent<ItemPickable>().itemScriptableObject.itemType;

            // Weapon setup
            if (currentItemType == ItemType.Gun)
            {
                // Link player object to weapon
                currentItem.GetComponent<Gun>().player = gameObject;
                currentItem.GetComponent<Gun>().weaponOwnerTag = gameObject.tag;
            }
            else if (currentItemType == ItemType.Melee)
            {
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

    private void DropItem(){
        // Instantiate the item at the player's current position (or in front, based on your choice)
        Vector3 dropPosition = playerTransform.position;
        GameObject droppedItem = Instantiate(itemInstantiate[Inventory[currentItemIndex]], dropPosition, Quaternion.identity);
        droppedItem.transform.localScale = droppedItem.transform.localScale;
        // droppedItem.transform.localScale = droppedItem.transform.localScale * droppedItemScale;
        droppedItem.tag = "Collectable";

        // Remove from Item Inventory
        Inventory.RemoveAt(currentItemIndex);

        // Remove from player's hand
        Destroy(currentItem);

        // Set currentItem to null
        currentItem = null;

        // Equip another item instead
        if(Inventory.Count > 0){
            if(currentItemIndex >= Inventory.Count){
                currentItemIndex--;
            }
            EquipItem(currentItemIndex);
        }
    }

    // Pick up item function
    private void PickupItem(GameObject other){
        if(other != null){
            // If Inventory full, drop holding item to pick another one instead
            if(Inventory.Count == 3){
                DropItem();
            }
            IPickable item = other.GetComponent<IPickable>();
            if(item != null){
                // Add into inventory
                Inventory.Add(other.GetComponent<ItemPickable>().itemScriptableObject.itemName);
                // Remove the item in the scene
                item.PickupItem();
            }
        }
    }

    // Player enter pick item area
    private void OnTriggerEnter2D(Collider2D other){
        // Pick up item
        if(other.CompareTag("Collectable")) {
            itemToPickUp = other.gameObject;
            isInPickupArea = true;

            // Tell that you're able to pick up item
            pickupItemText.gameObject.SetActive(true);
            pickupItemText.text = "Press E to pick up \"" + itemToPickUp.GetComponent<ItemPickable>().itemScriptableObject.itemName + "\"";
        }
    }

    // Prevent picking up item after left the area
    private void OnTriggerExit2D(Collider2D other){
        // If the player leaves the pickup area
        if (other.CompareTag("Collectable"))
        {
            itemToPickUp = null;  // Reset the item reference
            isInPickupArea = false;  // Set the flag to false

            // Hide text if player left pick up item area
            pickupItemText.gameObject.SetActive(false);
        }
    }
}

// Interface to work on a script that's on different object
// In this case work with ItemPickable script on item(Weapons, Misc...)
public interface IPickable{
    void PickupItem();
}