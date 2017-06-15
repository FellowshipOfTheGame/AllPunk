using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controlador do jogador para o protótipo do ALLPUNK
 * @author João Victor L. da S. Guimarães
 */
public class scr_PlayerController : scr_Entity {


	#region variables

	//Variable to track how much movement is needed from input
	private float movePlayerVector;

	// For determining which way the player is currently facing.
	private bool isFacingRight;

	// Speed modifier for player movement
	public float speed = 4.0f;

	// Speed modifier for player movement
	public float jumpForce = 4.0f;

    //Define se o personagem tem ou vai usar os IKs para movimentar os braços
    public bool useArmIK = true;

    //Distancia considerada até inverter o personagem
    public float armOffset;

    //booleano se determina se o jogador 
    private bool isGrounded;

	//Transform que armazena a posição dos pés
	private Transform playerFeetPosition;

    //Transform do IK do braço direito
    private Transform rightArmIK;

    //Transform do IK do braço esquerdo
    private Transform leftArmIK;

    //Distancia de ataque melee
    public float meleeAtackDistance;

    //Dano do ataque melee
    public float meleeAtackDamage;
    #endregion variables


    //Chamado ao carregar o script, inicializa variáveis
    protected void Awake()
	{
		base.Awake ();
		isFacingRight = true;
		entityRigidBody = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		playerFeetPosition = this.transform.Find("playerFeetPosition").GetComponent<Transform>(); //PEGAR O COLLIDER CIRCULAR NOS PÉS;
        if (useArmIK)
        {
            rightArmIK = this.transform.Find("IK").Find("IK_RHand").GetComponent<Transform>();
            leftArmIK = this.transform.Find("IK").Find("IK_LHand").GetComponent<Transform>();
        }
    }



	/**
	 * Chamado uma vez por frame, usada para gerenciar Rigibody
	 */
	void FixedUpdate(){
		// Pega input horizontal
		movePlayerVector = Input.GetAxisRaw("Horizontal");

		//Verifica contato com o chão
		isGrounded = touchesGround (playerFeetPosition.position);

		/** 
		  * Usar entityRigidBody.velocity para que o movimento do personagem
		  * sofra a influência da física. Um transform direto poderia quebrar
		  * A física em parte
		  */
		entityRigidBody.velocity = new Vector2 (movePlayerVector * speed, entityRigidBody.velocity.y);

		//TODO configurar tecla
		if (Input.GetKey (KeyCode.Space) && isGrounded )
			entityRigidBody.AddForce(new Vector2(0,jumpForce), ForceMode2D.Impulse);

        if (!useArmIK)
        {
            //Indo para a direita e virado para a esquerda
            if (movePlayerVector > 0 && !isFacingRight)
                Flip();
            //Indo para a esquerda e virado para a direita
            else if (movePlayerVector < 0 && isFacingRight)
                Flip();
        }

        if (useArmIK)
        {
            //Change arm orientation
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = transform.position.z;

            rightArmIK.SetPositionAndRotation(mouseWorldPosition, rightArmIK.rotation);
            leftArmIK.SetPositionAndRotation(mouseWorldPosition,leftArmIK.rotation);

            Vector3 relativeMouse = mouseWorldPosition - transform.position;
            if (relativeMouse.x + armOffset < 0 && isFacingRight)
                Flip();
            if (relativeMouse.x - armOffset > 0 && !isFacingRight)
                Flip();
        }

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int direction = (isFacingRight) ? 1 : -1;
            Vector2 pos = new Vector2(transform.position.x + direction*meleeAtackDistance / 2, transform.position.y);

            Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(meleeAtackDistance / 2,2), 0);

            foreach (Collider2D hit in hits) {
                if (hit.gameObject.tag == "Enemy") {
                    scr_Enemy enemy = hit.GetComponent<scr_Enemy>();
                    enemy.takeDamage(meleeAtackDamage, new Vector2(10*direction,1));
                }
            }
        }
    }



    /**
	 * Método que faz a verificação se o jogador está fazendo contato com o chão.
	 * @return true		Está tocando o chão
	 */
    bool touchesGround(Vector2 pos){
		/*Array de todos os colliders que colidem com os pés do jogador.
		 * recebe de argumento um Vector2, raio do círculo*/
		bool isGrounded = true;
		Collider2D [] array = Physics2D.OverlapCircleAll (pos, 0.2f);
		foreach (Collider2D obj in array) {
			//Verificação manual da layer
			if (obj.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
				isGrounded = true;
				break;
			} else {
				isGrounded = false;
			}	
		}
		return isGrounded;
	}
		

		
	/***
	 * Método que troca o sentido do sprite.
	 */
	void Flip()
	{
		//muda a direção que o jogador está encarando
		isFacingRight = !isFacingRight;
        if (!useArmIK)
        {
            //Multiplica a escala por -1
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        //Inverte a orientação do esqueleto
        if (useArmIK)
        {
            Transform skeletonTransform = transform.Find("Skeleton");
            Vector3 theScale = skeletonTransform.localScale;
            Vector3 ikScale = leftArmIK.localScale;
            theScale.x *= -1;
            ikScale.x *= -1;
            skeletonTransform.localScale = theScale;
            leftArmIK.localScale = ikScale;
            rightArmIK.localScale = ikScale;
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null) {
                bool Flip = !sprite.flipX;
                sprite.flipX = Flip;
            }
        }
	}
		
	protected override void die(){
		print ("Morreu");
		Destroy(this.gameObject);
	}

}
