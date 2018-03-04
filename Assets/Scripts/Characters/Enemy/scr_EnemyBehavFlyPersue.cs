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

    private void Awake()
    {
        myTransform = transform;

        rb2d = GetComponent<Rigidbody2D>();

        currentVelocity = Vector2.zero;

        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (!useDetectRange)
            animator.SetFloat("HorizontalSpeed", 1);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (receiveKnockback)
        {
            scr_HealthController health = GetComponent<scr_HealthController>();
            if (health != null)
                health.addKnockbackCallback(this.onKnockback);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            canSee = true;
            if(useDetectRange)
                animator.SetFloat("HorizontalSpeed", 1);
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

        if (playerTransform != null && !underKnockback)
        {
            //Verificar se deve perseguir ou não
            if (!useDetectRange || canSee) {
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

}
