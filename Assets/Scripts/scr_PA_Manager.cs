using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PA_Manager : MonoBehaviour {

    public GameObject[] weaponsPrefabs;
    public int leftWeaponID = 0;
    public int rightWeaponID = -1;
    public GameObject leftIK;
    public GameObject rightIK;
    public GameObject leftSocket;
    public GameObject rightSocket;

    private GameObject leftWeapon;
    private GameObject rightWeapon;
    private Animator animator;

    //Teste
    private int currentWeapon = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        instanciateWeapon(false, currentWeapon);
    }

    private void instanciateWeapon(bool right, int ID) {
        if (ID < weaponsPrefabs.Length && ID > -1   ) {
            scr_Weapon weapon = null;
            if (right)
            {
                if(rightWeapon != null)
                    Destroy(rightWeapon);
                rightWeapon = GameObject.Instantiate(weaponsPrefabs[ID], rightSocket.transform);
                weapon = rightWeapon.GetComponent<scr_Weapon>();
                weapon.setIK(rightIK);
            }
            else
            {
                if (leftWeapon != null)
                    Destroy(leftWeapon);
                leftWeapon = GameObject.Instantiate(weaponsPrefabs[ID], leftSocket.transform);
                weapon = leftWeapon.GetComponent<scr_Weapon>();
                weapon.setIK(leftIK);
            }
            weapon.setRightHand(right);
            weapon.setAnimator(animator);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            currentWeapon++;
            if (currentWeapon >= weaponsPrefabs.Length)
                currentWeapon = 0;
            instanciateWeapon(false, currentWeapon);
        }
    }
}
