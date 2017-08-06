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
    public int weaponFrontLayer = 8;
    public int weaponBackLayer = -4;

    public bool TEST_ARM;


    private GameObject leftWeapon;
    private GameObject rightWeapon;

	private float leftCooldown;
	private float rightCooldown;
	private float leftCurrCooldown;
	private float rightCurrCooldown;

    private Animator animator;
    private bool flipped = false;

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
		rightCooldown = 0;
		rightCurrCooldown = 0;
		leftCooldown = 0;
		leftCurrCooldown = 0;


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
                weapon.setSpriteLayer(weaponFrontLayer, weaponBackLayer);
                //Set the right arm variation for that weapon
                int armID = 2 * weapon.armVariation;
                if (flipped)
                    armID++;
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
                weapon.setSpriteLayer(weaponBackLayer, weaponFrontLayer);
                //Set the right arm variation for that weapon
                int armID = 2 * weapon.armVariation;
                if (flipped)
                    armID++;
                animLUpperArm.frame = armID;
                animLLowerArm.frame = armID;
                animLHand.frame = armID;
            }
            weapon.setRightHand(right);
            weapon.setAnimator(animator);
            if (flipped)
                weapon.flipHand();
        }
    }

    private void Update()
    {
		
		if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			currentWeapon++;
			if (currentWeapon >= weaponsPrefabs.Length)
				currentWeapon = 0;
			instanciateWeapon(TEST_ARM, currentWeapon);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			currentWeapon--;
			if (currentWeapon < 0)
				currentWeapon = weaponsPrefabs.Length-1;
			instanciateWeapon(TEST_ARM, currentWeapon);
		}


		/**
		 * Verifica se alguma arma está em cooldown, se estiver, manda para o player e o player manda para o HUD
		 */
		if (rightWeapon != null) {
			scr_Weapon rscr = rightWeapon.GetComponent<scr_Weapon> ();
			rightCooldown = rscr.getCooldownTimer ();
			rightCurrCooldown = rscr.getCurrentCooldownTimer ();

			//print ("Cooldown " + rightCooldown);
		}
		if (leftWeapon != null) {
			scr_Weapon lscr = leftWeapon.GetComponent<scr_Weapon> ();
			leftCooldown = lscr.getCooldownTimer ();
			leftCurrCooldown = lscr.getCurrentCooldownTimer ();

			//print ("Cooldown " + leftCooldown);
		}
		/*
        if (Input.GetKeyDown(KeyCode.R))
        {
            Flip();
            print("Flip");
        }
        */
    }

    public void Flip() {

        if (leftWeapon != null)
            leftWeapon.GetComponent<scr_Weapon>().flipHand();
        if (rightWeapon != null)
            rightWeapon.GetComponent<scr_Weapon>().flipHand();

        int leftArmID = animLUpperArm.frame;
        int rightArmID = animRUpperArm.frame;
        if (flipped)
        {
            leftArmID--;
            rightArmID--;
        }
        else
        {
            leftArmID++;
            rightArmID++;
        }
        //Change sprite
        flipped = !flipped;
        animLUpperArm.frame = leftArmID;
        animLLowerArm.frame = leftArmID;
        animLHand.frame = leftArmID;
        animRUpperArm.frame = rightArmID;
        animRLowerArm.frame = rightArmID;
        animRHand.frame = rightArmID;

        //Change sprite Layer
        //Upper arm layer
        int auxLayer;
        SpriteMeshInstance rightSprite = animLUpperArm.GetComponent<SpriteMeshInstance>();
        SpriteMeshInstance leftSprite = animRUpperArm.GetComponent<SpriteMeshInstance>();
        auxLayer = rightSprite.sortingOrder;
        rightSprite.sortingOrder = leftSprite.sortingOrder;
        leftSprite.sortingOrder = auxLayer;

        //Change  lower arm Layer
        rightSprite = animLLowerArm.GetComponent<SpriteMeshInstance>();
        leftSprite = animRLowerArm.GetComponent<SpriteMeshInstance>();
        auxLayer = rightSprite.sortingOrder;
        rightSprite.sortingOrder = leftSprite.sortingOrder;
        leftSprite.sortingOrder = auxLayer;

        //Change hand layer
        rightSprite = animRHand.GetComponent<SpriteMeshInstance>();
        leftSprite = animLHand.GetComponent<SpriteMeshInstance>();
        auxLayer = rightSprite.sortingOrder;
        rightSprite.sortingOrder = leftSprite.sortingOrder;
        leftSprite.sortingOrder = auxLayer;

        //Find arm transform
        Transform upperBody = transform.Find("Bones").Find("Hip").Find("UpperBody");
        Transform rightArm = upperBody.Find("R.UpperArm");
        Transform leftArm = upperBody.Find("L.UpperArm");

        //Change Arm position
        Vector3 newPosition = rightArm.transform.localPosition;
        rightArm.transform.localPosition = leftArm.transform.localPosition;
        leftArm.transform.localPosition = newPosition;
    }

	/*private float calculateArmLimit(){
		//Acessa barços e calcula o tamanho deles
	}*/


	/***
	 * Método que retorna os timers das armas
	 * X = Direito
	 * Y = Direito Atual
	 * Z = Esquerdo
	 * W = Esquerdo Atual
	 */
	public Vector4 getCountdownTimers(){
		Vector4 timers = new Vector4 (rightCurrCooldown, rightCooldown, leftCurrCooldown, leftCooldown);
		return timers;
	}

	/**
	 * Método usado no pause do jogo, para o script das armas
	 */
	public void pauseWeaponScripts(bool isPause){
		if(rightWeapon != null)
			rightWeapon.GetComponent<scr_Weapon> ().enabled = isPause;
		if(leftWeapon != null)
			leftWeapon.GetComponent<scr_Weapon> ().enabled = isPause;
	}
}
