using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weaponPrefabs;
    public enum WeaponType { Gun, Melee }

    public WeaponType currentWeaponType;
    private GameObject currentWeapon;
    private int currentWeaponIndex = 0;
    private Transform playerTransform;

    private void Start()
    {

        playerTransform = gameObject.GetComponent<Transform>();
        EquipWeapon(currentWeaponIndex);
    }

    private void Update()
    {
        // ! Test
        // if (Input.GetMouseButton(0)) // Check 'left hold'
        // {
        //     if (currentWeaponType == WeaponType.Gun)
        //     {
        //         currentWeapon.GetComponent<Gun>().Fire();
        //     }
        //     else if (currentWeaponType == WeaponType.Melee)
        //     {
        //         currentWeapon.GetComponent<Melee>().Swing();
        //     }
        // }
        // ! Test

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

            // Gun setup
            if (currentWeapon.GetComponent<Gun>()){
                // Link player object to weapon
                currentWeapon.GetComponent<Gun>().player = gameObject;
            }

            // Add weaponAim component, aim weapon to mouse position
            // And set weapon layer to -3
            currentWeapon.AddComponent<WeaponAim>();
            currentWeapon.transform.localPosition = new Vector3(0, 0, -3);

            // Update the current weapon index
            currentWeaponIndex = index;
        }
    }
}
