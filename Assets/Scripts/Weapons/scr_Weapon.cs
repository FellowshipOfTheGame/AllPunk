using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class scr_Weapon : MonoBehaviour {

    /**
     * Enumerador dos tipos de ataque
     * 
     */
    public enum AttackType
    {
        UpAttack,
        ThrustAttack,
        RangedAttack,
        None
    };

    #region Variables

    //Se essa é ou não a mão direita
    [HideInInspector]
    public bool rightHand;
    //A arma deve seguir o mouse
    public bool followMouse;
    //Como é o ataque utilizado
    public AttackType attackType;
    //Qual variação de sprite vai ser utilizado no braço
    public int armVariation = 0;
	//Tempo entre ativações da arma
	public float cooldownTime = 0;
	//Custo de energia para ativar
	public float energyDrain;


	protected scr_PlayerEnergyController playerEnergy;
	//Referência ao transform do braço
	protected Transform lowerArm;
    //A IK que vai ser usado para mover o braço
    protected GameObject ik;
    //O animador do personagem
    protected Animator animator;
    //Sprite da arma
    protected SpriteRenderer sprite;
    //A ordem do sprite a ser utilizado quando no sentido normal
    protected int frontLayer;
    //A ordem do sprite a ser utilizado quando inverte
    protected int backLayer;
    //Se o botão do ataque foi clicado
    protected bool clicked;
    //Se o botão de ataque foi segurado
    protected bool holding;
    //Se a arma foi invertido
    protected bool flipped;
	//Tempo atual de cooldown, se é zero ela pode disparar
	protected float currCooldownTime;
    //Se está ou não tocando a animação
    protected bool playingAnimation;
    #endregion Variables

    /**
     * Obter os componentes utilizados
     */
    protected void Awake()
    {
        this.sprite = GetComponent<SpriteRenderer>();
        ik = null;
        animator = null;
		currCooldownTime = 0;

		Transform parentTransform = GetComponentInParent<Rigidbody2D> ().transform;
		lowerArm = parentTransform.transform.Find("Bones").Find("Hip").Find("UpperBody").Find("R.UpperArm").Find("R.LowerArm");
		playerEnergy = GetComponentInParent<scr_PlayerEnergyController>();
    }

    protected void Update()
    {
		if (currCooldownTime > 0) {
			currCooldownTime -= Time.deltaTime;
			//print ("~ " + currentTimeToFire);
			if (currCooldownTime <= 0)
				currCooldownTime = 0;
		}

        //Move o IK para a posição do mouse    
        if (followMouse && ik != null) {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = transform.position.z;


			ik.transform.SetPositionAndRotation (mouseWorldPosition, ik.transform.rotation);


			/*Vector3 aux = (mouseWorldPosition - 
				this.transform.position);*/

			//Se maior do que o limite
			/*if (aux.magnitude > deadzoneRadius) {
				print ("ok");
				ik.transform.SetPositionAndRotation (mouseWorldPosition, ik.transform.rotation);
			} else {
				ik.transform.SetPositionAndRotation (mouseWorldPosition, ik.transform.rotation);
				print ("nope");
			}*/

        }


        //Verifica se está sendo realizado alguma animação de ataque
        bool noAnimation = false;
        if (animator != null)
        {
            int animLayer = (rightHand) ? 1 : 2;
            noAnimation = animator.GetCurrentAnimatorStateInfo(animLayer).IsName("Moving");
            if (noAnimation)
            {
                if (rightHand)
                {
                    animator.ResetTrigger("R_Attack");
                }
                else
                {
                    animator.ResetTrigger("L_Attack");
                }
            }
        }

        playingAnimation = !noAnimation;
		string fireButton = (rightHand) ? "Fire1" : "Fire2";
		clicked = Input.GetButtonDown(fireButton);
		holding = Input.GetButton(fireButton);

		/*Condições para ativar a arma: 
		 * Clicado, cooldown = 0 e player tem energia suficiente
		 */
		if (clicked && currCooldownTime == 0 && playerEnergy.getCurrentEnergy() >= energyDrain) {
			//Chama a função específica de cada arma, decrementa energia
			playerEnergy.drainEnergy(energyDrain);
			currCooldownTime = cooldownTime;
			AttackAction (noAnimation);
		}

    }


    /**
     * Função específica de cada arma. Recebe como parâmetro um booleano indicando se
     * tem alguma animação sendo realizada ou não.
     * É chamada toda função de update
     */
    abstract protected void AttackAction(bool noAnimation);

    public void setIK(GameObject ik) {
        this.ik = ik;
    }

    /**
     * Define o animador a ser utilizado pela arma
     * 
     */
    public void setAnimator(Animator animator)
    {
        this.animator = animator;
        string name;
        if (rightHand)
            name = "R_";
        else
            name = "L_";
        //Resetar as variaveis
        animator.SetBool(name + "UpAttack",false);
        animator.SetBool(name + "ThrustAttack",false);
        animator.SetBool(name + "GunRecoil",false);
        //Seleciona qual animação vai ser utilizada
        switch (attackType)
        {
            case AttackType.UpAttack:

                name = name + "UpAttack";
                break;
            case AttackType.ThrustAttack:
                name = name + "ThrustAttack";
                break;
            case AttackType.RangedAttack:
                name = name + "GunRecoil";
                break;
            case AttackType.None:
                break;
            default:

                break;
        }
        animator.SetBool(name, true);

    }

    /**
     * Define se está utilizando o braço esquerdo ou direito
     */
    public void setRightHand(bool RightHand) {
        this.rightHand = RightHand;
    }

    /**
     * Define a ordem dos sprites
     */
    public void setSpriteLayer(int frontLayer, int backLayer) {
        this.frontLayer = frontLayer;
        this.backLayer = backLayer;
        if (sprite != null)
        {
            if (!flipped)
                sprite.sortingOrder = frontLayer;
            else
                sprite.sortingOrder = backLayer;
        }
    }

    /**
     * Troca a ordem dos sprites a serem usados
     */
    public void flipHand()
    {
        flipped = !flipped;
        if (sprite != null)
        {
            if (!flipped)
                sprite.sortingOrder = frontLayer;
            else
                sprite.sortingOrder = backLayer;
        }
    }

    /**
     * Começa animação de ataque
     */
    protected void StartAttackAnimation() {
        if (animator == null)
            return;
        if (rightHand)
            animator.SetTrigger("R_Attack");
        else
            animator.SetTrigger("L_Attack");
        
    }

	public float getCooldownTimer(){
		return cooldownTime;
	}

	public float getCurrentCooldownTimer(){
		return currCooldownTime;
	}
}
