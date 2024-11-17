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

    void Start()
    {

        playerTransform = gameObject.GetComponent<Transform>();
        EquipWeapon(currentWeaponIndex);
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Check 'left hold'
        {
            if (currentWeaponType == WeaponType.Gun)
            {
                currentWeapon.GetComponent<Gun>().Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            else if (currentWeaponType == WeaponType.Melee)
            {
                currentWeapon.GetComponent<Melee>().Swing();
            }
        }
        if (Input.GetKeyDown(KeyCode.F)){
            if(currentWeaponType == WeaponType.Melee)
                currentWeapon.GetComponent<Melee>().Reflect();
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

            // Weapon setup
            if (currentWeapon.GetComponent<Gun>())
            {
                // Link player object to weapon
                currentWeaponType = WeaponType.Gun;
                currentWeapon.GetComponent<Gun>().player = gameObject;
                currentWeapon.GetComponent<Gun>().weaponOwnerTag = gameObject.tag;
            }
            else if (currentWeapon.GetComponent<Melee>())
            {
                currentWeaponType = WeaponType.Melee;
                SetUpMeleeWeapon();
            }

            // Add weaponAim component, aim weapon to mouse position
            // And set weapon layer to -3
            currentWeapon.AddComponent<WeaponAim>();
            currentWeapon.transform.localPosition = new Vector3(0, 0, -3);

            // Update the current weapon index
            currentWeaponIndex = index;
        }
    }

    void SetUpMeleeWeapon()
    {
        GameObject weaponPivot = new GameObject("WeaponPivot");
        weaponPivot.transform.position = transform.position;  // Position it near the player or weapon

        // Set pivot (Whole weapon) as a child of player
        weaponPivot.transform.SetParent(playerTransform);

        // Set the weapon as a child of the pivot
        currentWeapon.transform.SetParent(weaponPivot.transform);

        // Position the weapon correctly
        currentWeapon.transform.localPosition = new Vector3(0, 0, -3);

        // Add WeaponAim script to the pivot (to control aiming)
        weaponPivot.AddComponent<WeaponAim>();

        // Optionally, you can store the pivot for further use if needed
        currentWeapon.GetComponent<Melee>().player = gameObject;
        currentWeapon.GetComponent<Melee>().weaponPivot = weaponPivot;
        currentWeapon.GetComponent<Melee>().weaponOwnerTag = gameObject.tag;
    }
}
