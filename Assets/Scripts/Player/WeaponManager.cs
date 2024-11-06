using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weaponPrefabs;  // Array of available weapon prefabs

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
        // Example of switching weapons with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1)) { EquipWeapon(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { EquipWeapon(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { EquipWeapon(2); }
    }

    public void EquipWeapon(int index)
    {
        Debug.Log(index);
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
