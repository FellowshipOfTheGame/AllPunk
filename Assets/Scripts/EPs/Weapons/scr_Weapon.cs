using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;

abstract public class scr_Weapon : scr_EP {

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
    [Header("Atributos da arma")]
    //A arma deve seguir o mouse
    [Tooltip("Deve seguir o mouse ou não")]
    public bool followMouse;
    [Tooltip("Posição a ficar caso não siga o mouse")]
    public Vector2 fixedPosition;
    //Como é o ataque utilizado
    [Tooltip("Animação a ser utilizada quando atacar")]
    public AttackType attackType;
	//Tempo entre ativações da arma
	public float cooldownTime = 0;

    [Header("Sprite")]
    [Tooltip("Sprite da arma")]
    public SpriteRenderer sprite;
    [Tooltip("A ordem do sprite a ser utilizado quando no sentido normal")]
    public int frontLayer = 8;
    [Tooltip("A ordem do sprite a ser utilizado quando inverte")]
    public int backLayer = -4;
    [Header("Animation transition ")]
    [Tooltip("Minima distancia que move o mouse para parar animação e retomar mira")]
    public float mouseDeltaToAnim = 0.25f;
    [Tooltip("Tempo que demora para retomar a animação depois de mudar de posição do braço")]
    public float timeToAnimate = 3f;
    [Tooltip("Tempo de transição entre animação e mirando no alvo")]
    public float animationTransitionTime = 0.2f;

    [Space]

    //Distancia máxima que o braço fica esticado, encontrado empiricamente
    protected float maxDistance = 1.5f;
    //Distancia que a mão fica do ombro quando o braço está em segundo plano. Encontrado empiricamente
    protected float lowerDistance = 1.5f;

	protected scr_PlayerEnergyController playerEnergy;
	//Referência ao transform do braço
	protected Transform lowerArm;
    //A IK que vai ser usado para mover o braço
    protected GameObject ik;
    //O animador do personagem
    protected Animator animator;
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
    //Qual o offset atual da mao
    protected Vector3 currentOffset;

    //Animação do braço
    //Referencia ao Script do ik
    protected IkLimb2D ikLimb;
    //Contador de tempo até começar animação
    protected float animCounter = 0f;
    //Booleano representando se o jogador está sobre animação dos braços ou não
    protected bool armAnimationPlaying = false;
    //Distancia em forma quadrada (reduzir processamento)
    protected float squaredAnimationDistance;
    //Corotina sendo usada
    protected Coroutine playingCoroutine = null; 

    #endregion Variables

    /**
     * Obter os componentes utilizados
     */
    protected void Awake()
    {
        if(sprite == null)
            this.sprite = GetComponent<SpriteRenderer>();
        ik = null;
        animator = null;
		currCooldownTime = 0;
        animCounter = 0;


        //Variaveis de animação
        //ikLimb = ik.GetComponent<IkLimb2D>();
        squaredAnimationDistance = mouseDeltaToAnim * mouseDeltaToAnim;
        animCounter = timeToAnimate;


        if (sprite != null)
        {
            if (!flipped)
                sprite.sortingOrder = this.frontLayer;
            else
                sprite.sortingOrder = this.backLayer;
        }

    }

    public override bool Equip(GameObject playerReference) {
        //Referencia para a energia
        playerEnergy = playerReference.GetComponent<scr_PlayerEnergyController>();
        
        Transform ikRoot = playerReference.transform.Find("IK");

        if(rightHand){
			lowerArm = playerReference.transform.Find("Bones").Find("Hip").Find("UpperBody").Find("R.UpperArm").Find("R.LowerArm");
            setIK(ikRoot.Find("IK_R_Hand").gameObject);
        }else {
			lowerArm = playerReference.transform.Find("Bones").Find("Hip").Find("UpperBody").Find("L.UpperArm").Find("L.LowerArm");
            setIK(ikRoot.Find("IK_L_Hand").gameObject);
        }

        animator = playerReference.GetComponent<Animator>();

        //Atualiza referência do animador
        setAnimator(animator);
        return true;
    }

    public override bool Unequip() {
        StopAllCoroutines();
        ikLimb.weight = 0;
        return true;
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

			//Pegar posição do mouse
			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPosition.z = transform.position.z;


            //Pegar inicio do ombro

            Transform bone = transform.parent.parent.parent;

			Vector3 direction = mouseWorldPosition - bone.position;

            ik.transform.SetPositionAndRotation(mouseWorldPosition, ik.transform.rotation);
			

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

		if ( (clicked || holding) && currCooldownTime == 0 && playerEnergy.getTotalCurrentEnergy() >= energyDrain) {

			//Chama a função específica de cada arma, decrementa energia


			currCooldownTime = cooldownTime;

			AttackAction (noAnimation);

            //Verifica se o jogador estava em animação ou não e para a animação caso esteja
            if (armAnimationPlaying)
                imediateStopArmAnimation();
            else
                animCounter = timeToAnimate;

        }


        //Verificar se houve movimento do mouse, para poder fazer animação de movimento ou não
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (mouseDelta.sqrMagnitude > squaredAnimationDistance)
        {
            if (armAnimationPlaying)
                stopArmAnimation();
            else
                animCounter = timeToAnimate;
        }

        //Começa a animação caso estore o timer
        if (animCounter > 0)
        {
            animCounter -= Time.deltaTime;
        }
        else
        {
            if(!armAnimationPlaying)
                startArmAnimation();
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
        ikLimb = ik.GetComponent<IkLimb2D>();
        ikLimb.weight = 1;
        Debug.Log("Setou");

        if (!followMouse)
        {
            ik.transform.localPosition = fixedPosition;
        }
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
        Debug.Log("Animação: "+ rightHand);
    }

	public float getCooldownTimer(){
		return cooldownTime;
	}

	public float getCurrentCooldownTimer(){
		return currCooldownTime;
	}

    protected void useEnergy() {
        playerEnergy.drainEnergy(energyDrain);
    }

    protected void startArmAnimation()
    {
        armAnimationPlaying = true;
        if(playingCoroutine != null)
            StopCoroutine(playingCoroutine);
        playingCoroutine = StartCoroutine(changeIKWeight(1f,0f, animationTransitionTime*2));
    }

    protected void stopArmAnimation() {
        animCounter = timeToAnimate;
        armAnimationPlaying = false;
        if(playingCoroutine != null)
            StopCoroutine(playingCoroutine);
        playingCoroutine = StartCoroutine(changeIKWeight(0f, 1f, animationTransitionTime));
    }

    protected void imediateStopArmAnimation() {
        if(playingCoroutine != null)
            StopCoroutine(playingCoroutine);
        animCounter = timeToAnimate;
        armAnimationPlaying = false;
        ikLimb.weight = 1;
    }

    protected IEnumerator changeIKWeight(float initialValue, float finalValue, float time)
    {
        float speed = (finalValue - initialValue) / time;
        float counter = time;
        float currentWeight = initialValue;

        Debug.Log("Speed: " + speed + ", Initial: " + initialValue + ", Final: " + finalValue);

        while (counter > 0)
        {
            currentWeight += speed * Time.deltaTime;
            ikLimb.weight = currentWeight;
            counter -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("Peso final: " + currentWeight);
        ikLimb.weight = finalValue;
    }

}
