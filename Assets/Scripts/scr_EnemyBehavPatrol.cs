using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***
 * Script que gerencia o movimento e comportamento para Patrulha.
 * Vida e Morte são gerenciados por scr_Enemy
 */

public class scr_EnemyBehavPatrol : MonoBehaviour {

    //Velocidade dos inimigos
    public float Speed;

    //Saber ou não se vai usar limite de X
    public bool UseLimits = false;

    //Definir limite de distancia entre
    public Vector2 XLimit;

    //Referência para rigibody 2D
    private Rigidbody2D rb2D;

    //Saber se está indo para a direita
    private bool isFacingRight = true;
    //Saber se está no chão
    private bool isGrounded;
    //Saber se há chão na frente dele
    private bool nextFloor;
    //Saber se há parede na frente dele
    private bool nextWall;
    //Referencia do animator
    private Animator animator;
    //Pode mover ou não
    private bool cantMove;
    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate () {

        //Calcula se o personagem está encostado no chão
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.Find("FootPos").position, 0.2f);
        foreach (Collider2D hit in hits) {
            isGrounded = hit.gameObject.layer == LayerMask.NameToLayer("Ground");
            if (isGrounded == true)
                break;

        }

        if (!UseLimits)
        {

            //Calcula se acabou a plataforma
            hits = Physics2D.OverlapCircleAll(transform.Find("NextFloorCollision").position, 0.2f);
            foreach (Collider2D hit in hits)
            {
                nextFloor = (hit.gameObject.layer == LayerMask.NameToLayer("Ground"));
                if (nextFloor == true)
                    break;

            }

            //Calcula colisão com parede
            //hits = Physics2D.OverlapCircleAll(transform.Find("NextWallCollision").position, 0.2f);
            float height = (transform.Find("NextWallCollision").position - transform.Find("NextFloorCollision").position).y;
            hits = Physics2D.OverlapBoxAll(transform.Find("NextWallCollision").position, new Vector2(0.2f, height), 0);
            //print(hits);
            foreach (Collider2D hit in hits)
            {
                nextWall = (hit.gameObject.layer == LayerMask.NameToLayer("Ground"));
                if (nextWall == true)
                    break;

            }
            //print(nextWall);

            //Rotaciona o personagem
            if ((!nextFloor || nextWall) && isGrounded)
            {
                Flip();
                nextFloor = true;
                nextWall = false;
            }
        }
        else {
            if ((transform.position.x > XLimit.y) || (transform.position.x < XLimit.x)) {
                Flip();
            }
        }

        //Atualiza velocidade
        if (isGrounded && !cantMove) {
            int direction = (isFacingRight) ? 1 : -1;
            rb2D.velocity = new Vector2(direction *
            Speed, rb2D.velocity.y);
        }

        //Atualiza animação
        if (animator != null) {
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("HorizontalSpeed", Mathf.Abs(rb2D.velocity.x));
            animator.SetFloat("VerticalSpeed", rb2D.velocity.y);
        }
        
	}

    void Flip()
    {
        // Switch the way the player is labeled as facing.
        isFacingRight = !isFacingRight;
        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void StopMoving() {
        cantMove = true;
    }

    void ResumeMoviment() {
        cantMove = false;
    }

}
