using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;

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
    [Header("Atributos da arma")]
    //A arma deve seguir o mouse
    [Tooltip("Deve seguir o mouse ou não")]
    public bool followMouse;
    [Tooltip("Posição a ficar caso não siga o mouse")]
    public Vector2 fixedPosition;
    //Como é o ataque utilizado
    [Tooltip("Animação a ser utilizada quando atacar")]
    public AttackType attackType;
    [Tooltip("Qual variante de braço deve ser utilizada")]
    public scr_PA_Manager.WeaponPart armVariationSprite;
	//Tempo entre ativações da arma
	public float cooldownTime = 0;
	//Custo de energia para ativar
	public float energyDrain;
    //Deve forçar o layer desejado
    public bool forceLayer = false;
    //A ordem do sprite a ser utilizado quando no sentido normal
    public int frontLayer = 8;
    //A ordem do sprite a ser utilizado quando inverte
    public int backLayer = -4;
    [Header("Animation transition ")]
    [Tooltip("Minima distancia que move o mouse para parar animação e retomar mira")]
    public float mouseDeltaToAnim = 0.25f;
    [Tooltip("Tempo que demora para retomar a animação depois de mudar de posição do braço")]
    public float timeToAnimate = 3f;
    [Tooltip("Tempo de transição entre animação e mirando no alvo")]
    public float animationTransitionTime = 0.2f;
    //Qual variação de sprite vai ser utilizado no braço em inteiro
    [HideInInspector]
    public int armVariation = 0;
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
    //Sprite da arma
    protected SpriteRenderer sprite;
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
    //Variavel dizendo qual o offset que a mão tem que ter quando é a mão secundaria
    protected Vector3 offsetSecondary = new Vector3(0,0,0);
    //Variavel dizendo qual o offset que a mão tem que ter quando é a mão primária
    protected Vector3 offsetPrincipal = new Vector3(0,0,0);
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
        animCounter = 0;

        //Conversao do braço para int
        armVariation = (int)armVariationSprite;

		Transform parentTransform = GetComponentInParent<Rigidbody2D> ().transform;
		playerEnergy = GetComponentInParent<scr_PlayerEnergyController>();

		if(rightHand)
			lowerArm = parentTransform.transform.Find("Bones").Find("Hip").Find("UpperBody").Find("R.UpperArm").Find("R.LowerArm");
		else
			lowerArm = parentTransform.transform.Find("Bones").Find("Hip").Find("UpperBody").Find("L.UpperArm").Find("L.LowerArm");

        //Variaveis de animação
        //ikLimb = ik.GetComponent<IkLimb2D>();
        squaredAnimationDistance = mouseDeltaToAnim * mouseDeltaToAnim;
        animCounter = timeToAnimate;
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


            //if(direction.magnitude < maxDistance) {
            if (true){//PLACEHOLDER
				ik.transform.SetPositionAndRotation(mouseWorldPosition, ik.transform.rotation);
			}

			else {

				float distance = maxDistance;
				//Logica para saber se o braço utilizado é o que fica por trás, caso seja, fica mais próximo do
				//corpo

				if(!(rightHand ^ flipped))
					distance = lowerDistance;

				//Atualiza o angulo que o offset deveria ter, calculando o angulo e depois rotacionando o offset

				float alphaAngle;

				if(!flipped)
					alphaAngle = Mathf.Atan2(direction.y, direction.x);

				else
					alphaAngle = Mathf.Atan2(-direction.y, -direction.x);

				alphaAngle *= Mathf.Rad2Deg; //Conversao para grau

				//Rotaciona o vetor no eixo z
				Vector3 correctOffset =  Quaternion.AngleAxis(alphaAngle, new Vector3(0,0,1)) * currentOffset;

				//Faz com que o IK fique na circunferência do braço essticado
				ik.transform.SetPositionAndRotation(bone.position + direction.normalized * distance + correctOffset, 
					ik.transform.rotation);

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

		if ( (clicked || holding) && currCooldownTime == 0 && playerEnergy.getCurrentEnergy() >= energyDrain) {

			//Chama a função específica de cada arma, decrementa energia


			currCooldownTime = cooldownTime;

			AttackAction (noAnimation);

            //Verifica se o jogador estava em animação ou não e para a animação caso esteja
            if (armAnimationPlaying)
                stopArmAnimation();
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
     * Define a ordem dos sprites
     */
    public void setSpriteLayer(int frontLayer, int backLayer) {
        if (!forceLayer)
        {
            this.frontLayer = frontLayer;
            this.backLayer = backLayer;
        }
        else
        {
            //Verificar se não esta invertido
            if(frontLayer < 0)
            {
                int aux = this.frontLayer;
                this.frontLayer = this.backLayer;
                this.backLayer = aux;
            }
        }
        if (sprite != null)
        {
            if (!flipped)
                sprite.sortingOrder = this.frontLayer;
            else
                sprite.sortingOrder = this.backLayer;
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

        //Logica de offset (flipped XOR rightHand). Em caso de duvida verificar função setHandOffset
        if(flipped ^ rightHand)
            currentOffset = offsetPrincipal;
        else
            currentOffset = offsetSecondary;
        
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

    public void setHandOffset(Vector2 newOffset) {
        offsetSecondary = newOffset;
        if(rightHand)
            offsetSecondary.x *=-1;
        if(!flipped && !rightHand)
            currentOffset = offsetSecondary;
        else if (flipped && rightHand) {
            currentOffset = offsetSecondary;
        } else
            currentOffset = offsetPrincipal;
    }

    protected void useEnergy() {
        playerEnergy.drainEnergy(energyDrain);
    }

    protected void startArmAnimation()
    {
        armAnimationPlaying = true;
        StartCoroutine(changeIKWeight(1f,0f, animationTransitionTime*2));
    }

    protected void stopArmAnimation() {
        animCounter = timeToAnimate;
        armAnimationPlaying = false;
        StartCoroutine(changeIKWeight(0f, 1f, animationTransitionTime));
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
