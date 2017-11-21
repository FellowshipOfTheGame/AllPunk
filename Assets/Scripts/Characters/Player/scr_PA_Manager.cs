using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;

public class scr_PA_Manager : MonoBehaviour {

    //Armas disponiveis
    public GameObject[] weaponsPrefabs;
    //ID da arma na mão esquerda
    public int leftWeaponID = -1;
    //ID da arma na mão direita
    public int rightWeaponID = -1;
    //Referencia para os IKs da mão esquerda e direita
    public GameObject leftIK;
    public GameObject rightIK;
    //Referencia para o transform das mãos
    public GameObject leftSocket;
    public GameObject rightSocket;
    //Layer de visibilidade do sprite da arma para quando está à frente
    //do jogador e atrás dele
    public int weaponFrontLayer = 8;
    public int weaponBackLayer = -4;

    //IDs das partes do corpo
    public int bodyID = 0;
    public int headID = 0;
    public int legsID = 0;

    //Debbug variables
    public bool TEST_ARM;
    public bool canChangeWeapons;
    private List<int> unlockedWeapons;
    public int testWeapon = 0;
    private int currentWeapon = 0;

    //Armas possuidas pelo jogador
    private GameObject leftWeapon;
    private GameObject rightWeapon;

    //Contadores de cooldown
	private float leftCooldown;
	private float rightCooldown;
	private float leftCurrCooldown;
	private float rightCurrCooldown;

    //Referêcia para o animador
    private Animator animator;
    //O personagem está virado ou não
    private bool flipped = false;

    //MeshAnimators: Used to switch mesh
    private SpriteMeshAnimation animRUpperArm;
    private SpriteMeshAnimation animLUpperArm;
    private SpriteMeshAnimation animRLowerArm;
    private SpriteMeshAnimation animLLowerArm;
    private SpriteMeshAnimation animRHand;
    private SpriteMeshAnimation animLHand;

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
        currentWeapon = testWeapon;
        //instanciateWeapon(TEST_ARM, testWeapon);
        unlockedWeapons = new List<int>();
    }

    private void instanciateWeapon(bool right, int ID) {
        if (ID < weaponsPrefabs.Length && ID > -1   ) {
            scr_Weapon weapon = null;
            if (right)
            {
                if(rightWeapon != null)
                    Destroy(rightWeapon);
                rightWeapon = GameObject.Instantiate(weaponsPrefabs[ID], rightSocket.transform);
                rightWeaponID = ID;
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
                leftWeaponID = ID;
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
        if (canChangeWeapons)
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
                    currentWeapon = weaponsPrefabs.Length - 1;
                instanciateWeapon(TEST_ARM, currentWeapon);
            }
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

    /**
     *  ID da arma para equipar, a variavel arm pode ser:
     *  -1: Don't care
     *  0: No braço direito
     *  1: No braço esquerdo
     */
    public void equipWeapon(int weaponID, int armToEquip) {
        if (!unlockedWeapons.Contains(weaponID))
            unlockedWeapons.Add(weaponID);
        if (armToEquip == -1)
        {
            if (rightWeaponID == -1)
            {
                instanciateWeapon(true, weaponID);
            }
            else if (leftWeaponID == -1)
            {
                instanciateWeapon(false, weaponID);
            }
        }
        else if (armToEquip == 0)
            instanciateWeapon(true, weaponID);
        else if (armToEquip == 1)
            instanciateWeapon(false, weaponID);
    }

    /**
     *  ID da arma para desequipar, a variavel arm pode ser:
     *  0: No braço direito
     *  1: No braço esquerdo
     */
    public void removeWeapon(int armToEquip) {
        if (armToEquip == 0)
        {
            if (rightWeapon != null)
                Destroy(rightWeapon);
            rightWeapon = null;
            rightWeaponID = -1;
        }
        else if (armToEquip == 1) {
            if (leftWeapon != null)
                Destroy(leftWeapon);
            leftWeapon = null;
            leftWeaponID = -1;
        }
    }

    public void equipBody(int bodyToEquip) {
        Transform bodyTrans = transform.Find("Mesh").Find("Body");
        SpriteMeshAnimation meshAnim = bodyTrans.GetComponent<SpriteMeshAnimation>();

        meshAnim.frame = bodyToEquip;
        bodyID = bodyToEquip;
    }

    public void equipHead(int headToEquip)
    {
        Transform headTrans = transform.Find("Mesh").Find("Head");
        SpriteMeshAnimation meshAnim = headTrans.GetComponent<SpriteMeshAnimation>();

        meshAnim.frame = headToEquip;
        headID = headToEquip;
    }

    public void equipLegs(int legsToEquip)
    {
        string[] parts = new string[] { "R.UpperLeg", "R.LowerLeg", "R.Foot", "L.UpperLeg", "L.LowerLeg", "L.Foot" };

        Transform meshRoot = transform.Find("Mesh");
        foreach (string part in parts) {
            Transform partTrans = meshRoot.Find(part);
            SpriteMeshAnimation meshAnim = partTrans.GetComponent<SpriteMeshAnimation>();
            meshAnim.frame = legsToEquip;
        }

        legsID = legsToEquip;
    }

}
