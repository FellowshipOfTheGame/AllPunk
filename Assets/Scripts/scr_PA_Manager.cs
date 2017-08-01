using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;

public class scr_PA_Manager : MonoBehaviour {

    public GameObject[] weaponsPrefabs;
    public int leftWeaponID = 0;
    public int rightWeaponID = -1;
    public GameObject leftIK;
    public GameObject rightIK;
    public GameObject leftSocket;
    public GameObject rightSocket;
    public bool TEST_ARM;

    private GameObject leftWeapon;
    private GameObject rightWeapon;
    private Animator animator;

    //MeshAnimators: Used to switch mesh
    private SpriteMeshAnimation animRUpperArm;
    private SpriteMeshAnimation animLUpperArm;
    private SpriteMeshAnimation animRLowerArm;
    private SpriteMeshAnimation animLLowerArm;
    private SpriteMeshAnimation animRHand;
    private SpriteMeshAnimation animLHand;


    //Teste
    private int currentWeapon = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Transform meshTransform = transform.Find("Mesh");
        animRUpperArm = meshTransform.Find("R.UpperArm").GetComponent<SpriteMeshAnimation>();
        animRLowerArm = meshTransform.Find("R.LowerArm").GetComponent<SpriteMeshAnimation>();
        animRHand = meshTransform.Find("R.Hand").GetComponent<SpriteMeshAnimation>();
        animLUpperArm = meshTransform.Find("L.UpperArm").GetComponent<SpriteMeshAnimation>();
        animLLowerArm = meshTransform.Find("L.LowerArm").GetComponent<SpriteMeshAnimation>();
        animLHand = meshTransform.Find("L.Hand").GetComponent<SpriteMeshAnimation>();
    }

    private void Start()
    {
        instanciateWeapon(TEST_ARM, currentWeapon);
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
                //Set the right arm variation for that weapon
                int armID = weapon.armVariation;
                animRUpperArm.frame = armID;
                animRLowerArm.frame = armID;
                animRHand.frame = armID;
            }
            else
            {
                if (leftWeapon != null)
                    Destroy(leftWeapon);
                leftWeapon = GameObject.Instantiate(weaponsPrefabs[ID], leftSocket.transform);
                weapon = leftWeapon.GetComponent<scr_Weapon>();
                weapon.setIK(leftIK);
                //Set the right arm variation for that weapon
                int armID = weapon.armVariation;
                animLUpperArm.frame = armID;
                animLLowerArm.frame = armID;
                animLHand.frame = armID;
            }
            weapon.setRightHand(right);
            weapon.setAnimator(animator);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            currentWeapon++;
            if (currentWeapon >= weaponsPrefabs.Length)
                currentWeapon = 0;
            instanciateWeapon(TEST_ARM, currentWeapon);
        }
    }
}
