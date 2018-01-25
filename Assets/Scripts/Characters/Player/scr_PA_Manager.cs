using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;


public class scr_PA_Manager : MonoBehaviour {

#region Enumerators

/// <summary>
/// Enumerador utilizado para categorizar cada parte diferente de arma
/// </summary>
public enum WeaponPart
{
    None,
    Sabre,
    SteamBreath,
    RivetGun,
    GatlingGun
};

/// <summary>
/// Enumerador utilizado para categorizar cada parte diferente que pode ser equipada no torso
/// </summary>
public enum TorsoPart
{
    None,
    Boiler
};

/// <summary>
/// Enumerador utilizado para categorizar cada parte diferente que pode ser equipada nas pernas
/// </summary>
public enum LegPart
{
    None,
    JetJump
};

/// <summary>
/// Enumerador utilizado para categorizar cada parte diferente que pode ser equipada na cabeça
/// </summary>
public enum HeadPart
{
    None,
    Goggles
};

public enum PartType
{
    Weapon,
    Head,
    Torso,
    Legs
}

#endregion Enumerators

#region Variables

    //Armas disponiveis
    public GameObject[] weaponsPrefabs;
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

    //Partes desbloqueadas e equipadas
    public scr_Player_Stats playerStats;

    //Offset que a mão esquerda deve possuir
    public Vector2 leftHandOffset = new Vector2 (0,0);

    //Debbug variables
    public bool TEST_ARM;
    public bool canChangeWeapons;
    public int testWeapon = 0;
    private WeaponPart currentWeapon = WeaponPart.None;

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


	private Transform upperBody;
	private Transform rightArm;
	private Transform leftArm;

#endregion Variables

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
        currentWeapon = (WeaponPart) testWeapon;
        //instanciateWeapon(TEST_ARM, testWeapon);

        playerStats = scr_GameManager.instance.playerStats;
        
        //Equipar partes equipadas anteriormente
        equipBody((int) playerStats.torsoEquiped);
        equipLegs((int) playerStats.legsEquiped);
        equipHead((int) playerStats.headEquiped);
        if(playerStats.rightWeaponEquiped != WeaponPart.None)
            equipWeapon(((int) playerStats.rightWeaponEquiped)-1, 0);
        if(playerStats.leftWeaponEquiped != WeaponPart.None)
            equipWeapon(((int) playerStats.leftWeaponEquiped)-1, 1);

		Transform upperBody = transform.Find("Bones").Find("Hip").Find("UpperBody");
		Transform rightArm = upperBody.Find("R.UpperArm");
		Transform leftArm = upperBody.Find("L.UpperArm");
    }

    /// <summary>
    /// Function used for instanciate a weapon at a given arm
    /// </summary>
    /// <param name="right">Is in right arm (true) or the left arm (false)</param>
    /// <param name="ID">What weapon is going to be equiped</param>
    private void instanciateWeapon(bool right, int ID) {
        if (ID < weaponsPrefabs.Length && ID > -1   ) {
            scr_Weapon weapon = null;
            if (right)
            {
                if(rightWeapon != null)
                    Destroy(rightWeapon);
                rightWeapon = GameObject.Instantiate(weaponsPrefabs[ID], rightSocket.transform);
                playerStats.rightWeaponEquiped = (WeaponPart) (ID+1);
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
                playerStats.leftWeaponEquiped = (WeaponPart) (ID+1);
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
            weapon.setHandOffset(leftHandOffset);
            if (flipped)
                weapon.flipHand();
            updateSave();
        }
    }

    private void Update()
    {
        if (canChangeWeapons)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                currentWeapon++;
                if ((int)currentWeapon >= weaponsPrefabs.Length)
                    currentWeapon = (WeaponPart) 0;
                instanciateWeapon(TEST_ARM, (int) currentWeapon);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                currentWeapon--;
                if (currentWeapon < 0)
                    currentWeapon = (WeaponPart) (weaponsPrefabs.Length - 1);
                instanciateWeapon(TEST_ARM, (int) currentWeapon);
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

     /// <summary>
     /// Equipar a arma com dado ID ao braçoID da arma para equipar a variavel arm pode ser:
     /// </summary>
     /// <param name="weaponID">ID da arma a ser equipada</param>
     /// <param name="armToEquip">-1: Primeiro braço que estiver sem equipamento, 0: No braço direito,
     /// 1: No braço esquerdo</param>
     /// <return>Verdadeiro caso a arma esteja desbloqueada, falso caso a arma esteja bloqueada </return>
    public bool equipWeapon(int weaponID, int armToEquip) {
        if (!playerStats.unlockedWeapons.Contains((WeaponPart) (weaponID + 1)))
           return false;
        if (armToEquip == -1)
        {
            if (playerStats.rightWeaponEquiped == WeaponPart.None)
            {
                instanciateWeapon(true, weaponID);
            }
            else if (playerStats.leftWeaponEquiped == WeaponPart.None)
            {
                instanciateWeapon(false, weaponID);
            }
        }
        else if (armToEquip == 0)
            instanciateWeapon(true, weaponID);
        else if (armToEquip == 1)
            instanciateWeapon(false, weaponID);
        return true;
    }

     /// <summary>
     /// Remove a arma que está equipada naquele braço
     /// </summary>
     /// <param name="armToEquip">ID da arma para desequipar, a variavel arm pode ser: 
     /// 0: No braço direito
     /// 1: No braço esquerdo</param>
    public void removeWeapon(int armToEquip) {
        if (armToEquip == 0)
        {
            if (rightWeapon != null)
                Destroy(rightWeapon);
            rightWeapon = null;
            playerStats.rightWeaponEquiped = WeaponPart.None;
        }
        else if (armToEquip == 1) {
            if (leftWeapon != null)
                Destroy(leftWeapon);
            leftWeapon = null;
            playerStats.leftWeaponEquiped = WeaponPart.None;
        }
        updateSave();
    }

/// <summary>
/// Equipa o corpo no personagem, e caso ainda não tenha obtido essa parte, adiciona na lista de corpos desbloqueados.
/// </summary>
/// <param name="bodyToEquip">ID da parte a ser equipada no sigma</param>
    public bool equipBody(int bodyToEquip) {
        if(!playerStats.unlockedTorsos.Contains((TorsoPart) bodyToEquip))
            return false;
        Transform bodyTrans = transform.Find("Mesh").Find("Body");
        SpriteMeshAnimation meshAnim = bodyTrans.GetComponent<SpriteMeshAnimation>();

        meshAnim.frame = bodyToEquip;
        playerStats.torsoEquiped = (TorsoPart) bodyToEquip;
        updateSave();
        return true;
    }

/// <summary>
/// Equipa a cabeça no personagem, e caso ainda não tenha obtido essa parte, adiciona na lista de corpos desbloqueadas.
/// </summary>
/// <param name="headToEquip">ID da parte a ser equipada no sigma</param>
    public bool equipHead(int headToEquip)
    {
        if(!playerStats.unlockedHeads.Contains((HeadPart) headToEquip))
            return false;
        Transform headTrans = transform.Find("Mesh").Find("Head");
        SpriteMeshAnimation meshAnim = headTrans.GetComponent<SpriteMeshAnimation>();

        meshAnim.frame = headToEquip;
        playerStats.headEquiped = (HeadPart) headToEquip;
        updateSave();
        return true;
    }

/// <summary>
/// Equipa as pernas no personagem, e caso ainda não tenha obtido essa parte, adiciona na lista de pernas desbloqueadas.
/// </summary>
/// <param name="legsToEquip">ID da parte a ser equipada no sigma</param>
    public bool equipLegs(int legsToEquip)
    {
        if(!playerStats.unlockedLegs.Contains((LegPart) legsToEquip))
            return false;

        string[] parts = new string[] { "R.UpperLeg", "R.LowerLeg", "R.Foot", "L.UpperLeg", "L.LowerLeg", "L.Foot" };

        Transform meshRoot = transform.Find("Mesh");
        foreach (string part in parts) {
            Transform partTrans = meshRoot.Find(part);
            SpriteMeshAnimation meshAnim = partTrans.GetComponent<SpriteMeshAnimation>();
            meshAnim.frame = legsToEquip;
        }

        playerStats.legsEquiped = (LegPart) legsToEquip;
        updateSave();
        return true;
    }

/// <summary>
/// Desbloqueia uma nova parte para o jogador, permitindo que ele possa equipa-la posteriormente
/// </summary>
/// <param name="type">Enumerador para as diferentes partes do corpo que podem ser desbloqueadas</param>
/// <param name="partID">O ID da parte a ser desbloqueada</param>
    public void unlockPart(PartType type, int partID) {
        switch(type) {
            case PartType.Weapon:
                if(!playerStats.unlockedWeapons.Contains((WeaponPart) partID +1))
                    playerStats.unlockedWeapons.Add((WeaponPart)partID+1);
                break;
            case PartType.Head:
                if(!playerStats.unlockedHeads.Contains((HeadPart) partID))
                    playerStats.unlockedHeads.Add((HeadPart) partID);
                break;
            case PartType.Legs:
                if(!playerStats.unlockedLegs.Contains((LegPart) partID))
                    playerStats.unlockedLegs.Add((LegPart) partID);
                break;
            case PartType.Torso:
                if(!playerStats.unlockedTorsos.Contains((TorsoPart) partID))
                    playerStats.unlockedTorsos.Add((TorsoPart) partID);
                break;
        }
        //Salva modificações
        updateSave();
        print("Unlocked " + type.ToString() + ". ID: " + partID);
    }

    private void updateSave(){
        scr_GameManager.instance.playerStats = playerStats;
        scr_GameManager.instance.updatePlayerStats();
        scr_GameManager.instance.Save();
    }

}
