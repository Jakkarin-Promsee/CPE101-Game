using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public GameObject[] weaponPrefabs;  // Array of ranged weapon prefabs

    // Tempo variable
    private GameObject currentWeapon;   // Holds the currently equipped weapon


    private int currentWeaponIndex = 0;
    private Transform playerTransform;

    void Start()
    {

        playerTransform = this.GetComponent<Transform>(); // Initilize variable
        EquipWeapon(currentWeaponIndex);  // Initilize weapon at first
    }

    void Update()
    {

        if (Input.GetMouseButton(0)) // Check 'left click'
        {
            currentWeapon.GetComponent<Gun>().Fire();
        }

        // Switch guns with number keys (1, 2, 3, etc.)
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        if (Input.GetKeyDown(KeyCode.R)) // Switch guns with 'r' key
        {
            currentWeaponIndex++;
            if (currentWeaponIndex >= weaponPrefabs.Length) currentWeaponIndex -= weaponPrefabs.Length;
            EquipWeapon(currentWeaponIndex);
        }
    }

    void EquipWeapon(int index)
    {
        if (index >= 0 && index < weaponPrefabs.Length)
        {
            // Destroy the current weapon if it exists
            if (currentWeapon != null) Destroy(currentWeapon);

            // Instantiate the new weapon as a child of the player at position (0,0)
            currentWeapon = Instantiate(weaponPrefabs[index], playerTransform.position, Quaternion.identity, playerTransform);

            // Add gunAim component, aim weapon to mouse position
            currentWeapon.AddComponent<GunAim>();
            currentWeapon.transform.localPosition = new Vector3(0, 0, -3);

            // Link player object to weapon
            currentWeapon.GetComponent<Gun>().player = gameObject;

            // Update the current weapon index
            currentWeaponIndex = index;
        }
    }
}
