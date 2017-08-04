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

    //Verifica se o inimigo está atacando
    private bool isAtacking = false;
    //Contador para o personagem
    private float counter = 0;
    //Referencia para o animator
    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
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
                if (counter <= 0)
                {
                    print("Achei");
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
        }

        //Atualiza as animações
        if (animator != null) {
            animator.SetBool("Attack", isAtacking);
        }
    }
}
