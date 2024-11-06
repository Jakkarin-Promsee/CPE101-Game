using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public GameObject[] weaponPrefabs;          // Array of available guns

    private GameObject currentWeapon;   // Holds the currently equipped weapon
    private int currentWeaponIndex = 0;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = this.GetComponent<Transform>();
        EquipWeapon(currentWeaponIndex);  // Equip the first weapon by default
    }

    void Update()
    {
        if (Input.GetMouseButton(0))  // Fire when the left mouse button is held
        {
            currentWeapon.GetComponent<Gun>().Fire();
        }

        // Switch guns with number keys (1, 2, 3, etc.)
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);
    }

    void EquipWeapon(int index)
    {
        if (index >= 0 && index < weaponPrefabs.Length)
        {
            // Destroy the current weapon if it exists
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            // Instantiate the new weapon as a child of the player at position (0,0)
            currentWeapon = Instantiate(weaponPrefabs[index], playerTransform.position, Quaternion.identity, playerTransform);
            currentWeapon.AddComponent<GunAim>();
            currentWeapon.transform.localPosition = new Vector3(0, 0, -3);  // Ensure it's at (0,0) relative to the player

            // Update the current weapon index
            currentWeaponIndex = index;
        }
    }
}
