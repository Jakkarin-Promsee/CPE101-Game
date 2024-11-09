using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weaponPrefabs;  // Array of ranged weapon prefabs

    // Tempo variable
    private GameObject currentWeapon;   // Holds the currently equipped weapon


    private int currentWeaponIndex = 0;
    private Transform playerTransform;

    //  ! Melee add
    // Weapon types
    public enum WeaponType{
        Gun, Melee
    }
    // Store current weapon type
    public WeaponType currentWeaponType;
    //  ! Melee add


    void Start()
    {

        playerTransform = this.GetComponent<Transform>(); // Initilize variable
        EquipWeapon(currentWeaponIndex);  // Initilize weapon at first
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Check 'left hold'
        {
            // currentWeapon.GetComponent<Gun>().Fire();
            // ! Melee add
            if(currentWeaponType == WeaponType.Gun){
                currentWeapon.GetComponent<Gun>().Fire();
            }
            else if(currentWeaponType == WeaponType.Melee){
                currentWeapon.GetComponent<Melee>().Swing();
            }
            // ! Melee add
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

            // !Melee add
            // Weapon setup
            if(currentWeapon.GetComponent<Gun>()){
                currentWeaponType = WeaponType.Gun;
                // Link player object to weapon
                currentWeapon.GetComponent<Gun>().player = gameObject;
            }else if(currentWeapon.GetComponent<Melee>()){
                currentWeaponType = WeaponType.Melee;
                SetUpMeleeWeapon();
            }
            // !Melee add
            // Add weaponAim component, aim weapon to mouse position
            currentWeapon.AddComponent<WeaponAim>();
            currentWeapon.transform.localPosition = new Vector3(0, 0, -3);


            // Update the current weapon index
            currentWeaponIndex = index;
        }
    }

    void SetUpMeleeWeapon(){
        GameObject weaponPivot = new GameObject("WeaponPivot");
        weaponPivot.transform.position = transform.position;  // Position it near the player or weapon

        // Set pivot (Whole weapon) as a child of player
        weaponPivot.transform.SetParent(playerTransform);

        // Set the weapon as a child of the pivot
        currentWeapon.transform.SetParent(weaponPivot.transform);

        // Position the weapon correctly
        currentWeapon.transform.localPosition = Vector3.zero;

        // Add WeaponAim script to the pivot (to control aiming)
        weaponPivot.AddComponent<WeaponAim>();

        // Optionally, you can store the pivot for further use if needed
        currentWeapon.GetComponent<Melee>().weaponPivot = weaponPivot;
    }
}
