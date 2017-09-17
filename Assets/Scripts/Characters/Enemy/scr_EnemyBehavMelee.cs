using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyBehavMelee : MonoBehaviour {

    //Tempo que o inimigo precisa esperar para atacar
    public float attackCooldown = 1f;
    //Tempo que o inimigo espera para causar dano depois de colidir
    public float attackDelay = 0;
    //Dano que o inimigo causa
    public float attackDamage = 10;
    //Distancia de ataque
    public float attackDistance;
    //Direção de ataque
    public Vector2 attackDirection = new Vector2(1,0);
    //Colisores para verificar se atacou
    public Collider2D[] attackColliders;

    //Verifica se o inimigo está atacando
    private bool isAtacking = false;
    //Contador para o personagem
    private float counter = 0;
    //Referencia para o animator
    private Animator animator;
    //Está atacando
    private bool isAttacking;
    //A animação de ataque está acontecendo
    private bool animationStarted;
    //Referencia para o movimento
    private scr_EnemyBehavPatrol movementScript;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movementScript = GetComponent<scr_EnemyBehavPatrol>();
    }

    private void Update()
    {
        if (isAtacking) {
            bool correctAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");
            if (!animationStarted && correctAnimation)
            {
                animationStarted = true;
                movementScript.StopMoving();
            }
            if (animationStarted) {
                if (!correctAnimation)
                {
                    isAtacking = false;
                    animationStarted = false;
                    movementScript.ResumeMoviment();
                    foreach (Collider2D collider in attackColliders)
                        collider.enabled = false;
                }
                else
                {
                    foreach (Collider2D hitCollider in attackColliders)
                    {
                        if (!hitCollider.isActiveAndEnabled)
                            continue;
                        Collider2D[] colHits = new Collider2D[10];
                        ContactFilter2D ct2D = new ContactFilter2D();
                        hitCollider.OverlapCollider(ct2D, colHits);
                        foreach (Collider2D col in colHits)
                        {
                            if (col == null)
                                break;

                            scr_HealthController life = col.GetComponent<scr_HealthController>();
                            if (life != null && col.tag != "Enemy")
                            {
                                Vector2 attackDir = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
                                life.takeDamage(attackDamage, attackDir);
                            }
                        }
                    }
                }
            }
        }
        //if (animationIsPlaying)
        //{
        //    //if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
        //    //{
        //    //    animationIsPlaying = false;
        //    //    isAtacking = false;
        //    //}
        //    if (isAtacking)
        //    {
        //        foreach (Collider2D hitCollider in attackColliders)
        //        {
        //            if (!hitCollider.isActiveAndEnabled)
        //                continue;
        //            Collider2D[] colHits = new Collider2D[10];
        //            ContactFilter2D ct2D = new ContactFilter2D();
        //            hitCollider.OverlapCollider(ct2D, colHits);
        //            foreach (Collider2D col in colHits)
        //            {
        //                if (col == null)
        //                    break;

        //                scr_HealthController life = col.GetComponent<scr_HealthController>();
        //                if (life != null && col.tag != "Enemy")
        //                {
        //                    Vector2 attackDir = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
        //                    life.takeDamage(attackDamage, attackDir);
        //                }
        //            }
        //        }
        //    }
        //}
    }

    private void FixedUpdate()
    {
        //Decrementa o counter
        if (counter > 0) {
            counter -= Time.fixedDeltaTime;
        }

        //Calcula colisão
        int direction = ((transform.Find("NextWallCollision").position - transform.position).x > 0) ? 1 : -1;
        float height = (transform.Find("NextWallCollision").position - transform.Find("NextFloorCollision").position).y;
        Vector2 pos = new Vector2(transform.Find("NextWallCollision").position.x + direction * attackDistance / 2, transform.position.y);

        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(attackDistance, height), 0);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.tag == "Player")
            {
                if (!isAtacking)
                {
                    isAtacking = true;
                    animator.SetTrigger("Attack");
                }
            }
        }
        
    }

}
