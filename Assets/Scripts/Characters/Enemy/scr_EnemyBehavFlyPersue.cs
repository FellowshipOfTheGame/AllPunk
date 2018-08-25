using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Comportamento adicionado a um personagem para faze-lo perseguir o personagem se movimentando livremente no ar
/// atraves de rotacao e velocidade constante (eixo diferencial no eixo z)
/// </summary>
public class scr_EnemyBehavFlyPersue : MonoBehaviour {

    [Header("Geral")]
    [Tooltip("O personagem possui um trigger para detectar o jogador a distancia")]
    public bool useDetectRange;

    [Tooltip("Deve continuar seguindo o personagem, mesmo tendo perdido visão")]
    public bool persueAfterSeen = true;

    [Tooltip("Sobrevida após morrer")]
    public float timeTillDie = 2f;

    private bool hasDetected = false;

    [Header("Movimento")]
    [Tooltip("Velocidade desejada de movimento")]
    public float maxMovementSpeed = 5f;
    [Tooltip("Quao rapido o personagem consegue mudar a sua velocidade")]
    [Range(0,20)]
    public float responseTime = 5f;
    [Tooltip("Quao baixa a velocidade tem que ser para parar")]
    public float stopThreshold = 1f;

    [Header("Knock back")]
    [Tooltip("Se o personagem possuir vida, ele deve sofrer knockback")]
    public bool receiveKnockback = true;
    [Tooltip("Tempo que o personagem fica sobre knockback")]
    [Range(0, 2)]
    public float knockbackTime = 0.5f;

    [Header("Sound")]
    public scr_AudioClient audioClient;

    //Transform que vai ser rotacionado
    private Transform myTransform;
    //Angulo inicial do personagem de orientacao
    private Vector2 currentVelocity = Vector2.zero;
    //O personagem e capaz de ver o personagem ou nao
    private bool canSee;
    //Referencia para o player, o alvo a ser perseguido
    private Transform playerTransform;
    //Rigibody do personagem
    private Rigidbody2D rb2d;
    //O personagem esta sofrendo knockback ou nao
    private bool underKnockback = false;
    private Animator animator;


    [Header("Attack variables")]    
    [Tooltip("Dano recebido pelo jogador ao tocar no inimigo")]
	[SerializeField] float touchDamage = 10f;
    [Tooltip("Forca recebida pelo jogador ao tocar no inimigo")]
	[SerializeField] float repulseForce = 10f;
	[Tooltip("Tempo de cooldown entre danos")]
	[SerializeField]
	float cooldownTimer = 0.5f;
    [Tooltip("Deve parar depois de atacar")]
    public bool stopAfterAttack = true;

	private Vector2 resultVector;

    //O inimigo pode causar dano ou nao (ligado ao tempo de cooldown
    private bool canCauseDamage = true;


    private void Awake()
    {
        myTransform = transform;

        rb2d = GetComponent<Rigidbody2D>();

        currentVelocity = Vector2.zero;

        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (!useDetectRange){
            animator.SetFloat("HorizontalSpeed", 1);
            if(audioClient != null){
                audioClient.playAudioClip("Detect",scr_AudioClient.sources.local);
            }
            hasDetected = true;
        }
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        scr_HealthController health = GetComponent<scr_HealthController>();

        if (receiveKnockback)
        {
            if (health != null)
                health.addKnockbackCallback(this.onKnockback);
        }

        health.addDeathCallback(onDeath);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            canSee = true;
            if(useDetectRange){
                animator.SetFloat("HorizontalSpeed", 1);
                if(audioClient != null){
                    audioClient.playAudioClip("Detect",scr_AudioClient.sources.local);
                }
                hasDetected = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canSee = false;
            if (useDetectRange)
                animator.SetFloat("HorizontalSpeed", 0);

        }
    }

    private void Update()
    {
        if (playerTransform == null) {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (playerTransform != null && !underKnockback && canCauseDamage)
        {
            //Verificar se deve perseguir ou não
            if (!useDetectRange || canSee || (hasDetected && persueAfterSeen)) {
                Vector2 diference = playerTransform.position - myTransform.position;
                currentVelocity += diference.normalized * responseTime * Time.deltaTime;
                if (currentVelocity.magnitude >= maxMovementSpeed)
                    currentVelocity = currentVelocity.normalized * maxMovementSpeed;
            }
        }

        if (rb2d != null) {
            //Movimentar apenas se nao estiver sobre acao de knockback e perseguindo o jogador
            if (!underKnockback)
            {
                if ((!useDetectRange || canSee))
                {
                    rb2d.velocity = currentVelocity;
                }
                else
                {
                    if (currentVelocity.magnitude > stopThreshold)
                        currentVelocity = Vector2.Lerp(currentVelocity.normalized, Vector2.zero, Time.deltaTime * responseTime);
                    else
                        currentVelocity = Vector2.zero;


                    rb2d.velocity = currentVelocity;
                }
            }
            else
            {
                currentVelocity = rb2d.velocity;
            }
        }

    }

    private void onDeath() {
        animator.SetTrigger("Dead");
        StopAllCoroutines();
        underKnockback = true;
        canCauseDamage = false;
        gameObject.layer = 14;
        // gameObject.layer = LayerMask.NameToLayer("Corpse");
        rb2d.gravityScale = 1;
        rb2d.freezeRotation = false;
        Destroy(gameObject, timeTillDie);
        Destroy(this);
    }

    private void onKnockback() {
        animator.SetTrigger("Hurt");
        StartCoroutine(waitKnockback());
    }

    private void OnDestroy()
    {
        if (receiveKnockback)
        {
            scr_HealthController health = GetComponent<scr_HealthController>();
            if(health != null)
                health.removeKnockbackCallback(this.onKnockback);
        }
    }

    private IEnumerator waitKnockback() {
        underKnockback = true;
        float count = 0;
        while(count < knockbackTime)
        {
            count += Time.deltaTime;
            yield return null;
        }
        underKnockback = false;
    }


	private void OnCollisionStay2D(Collision2D targetRangeHit) {
		
		if (canCauseDamage && targetRangeHit.collider.CompareTag ("Player")) {
			scr_HealthController health = targetRangeHit.collider.gameObject.GetComponent<scr_HealthController>();
			resultVector = targetRangeHit.transform.position - transform.position;
			resultVector.Normalize();
			if (health != null)
			{
				//health.takeDamage(touchDamage, -1 * targetRangeHit.rigidbody.velocity.normalized * repulseForce);
				if(resultVector.x >= 0)
					health.takeDamage(touchDamage, Vector2.right * repulseForce);
				else
					health.takeDamage(touchDamage, Vector2.left * repulseForce);		
			}
			canCauseDamage = false;
			StartCoroutine(waitCooldown());
            if(stopAfterAttack){
                rb2d.velocity = Vector2.zero;
                currentVelocity = Vector2.zero;
            }
		}
	}

	private IEnumerator waitCooldown(){
		float counter = 0;
		while(counter < cooldownTimer){
			counter += Time.deltaTime;
			yield return null;
		}
		canCauseDamage = true;
	}
		

}
