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
    private bool animationIsPlaying;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animationIsPlaying)
        {
            //if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
            //{
            //    animationIsPlaying = false;
            //    isAtacking = false;
            //}
            if (isAtacking)
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

    private void FixedUpdate()
    {
        //Decrementa o counter
        if (counter > 0) {
            counter -= Time.fixedDeltaTime;
        }

        //Calcula colisão
        int direction = ((transform.Find("NextWallCollision").position - transform.position).x > 0) ? 1 : -1;
        float height = (transform.Find("NextWallCollision").position - transform.Find("NextFloorCollision").position).y;
        Vector2 pos = new Vector2(transform.Find("NextWallCollision").position.x - direction * attackDistance / 2, transform.position.y);

        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(attackDistance, height), 0);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.tag == "Player")
            {
                if (animator == null)
                {
                    if (counter <= 0)
                    {
                        //print("Achei");
                        print(isAtacking);
                        if (isAtacking)
                        {
                            scr_HealthController player = hit.GetComponent<scr_HealthController>();
                            player.takeDamage(attackDamage, new Vector2(attackDirection.x * direction, attackDirection.y));
                            isAtacking = false;
                            counter = attackCooldown;
                        }
                        else
                        {
                            isAtacking = true;
                            counter = attackDelay;
                        }
                    }
                }
                else
                {
                    if (!animationIsPlaying)
                    {
                        animator.SetTrigger("Attack");
                        isAtacking = true;
                        animationIsPlaying = true;
                        //print("Aqui");
                        break;
                    }
                }
            }
        }
        
    }

    public void animationBeginNotify()
    {
        animationIsPlaying = true;
        isAtacking = true;
    }

    public void animationEndNotify() {
        print("Ta notificando");
        animationIsPlaying = false;
        isAtacking = false;
    }

    public void beginCollisionCheck() {
        isAtacking = true;
    }
}
