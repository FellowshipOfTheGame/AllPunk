﻿using System.Collections;
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

    [Range(0,2)]
    public float knockbackTime = 1f;

    //Referência para rigibody 2D
    private Rigidbody2D rb2D;

    //Saber se está indo para a direita
    private bool isFacingRight = true;
    //Saber se está no chão
    private bool isGrounded;
    //Saber se há chão na frente dele
    public bool nextFloor;
    //Saber se há parede na frente dele
    private bool nextWall;
    //Referencia do animator
    private Animator animator;
    //Pode mover ou não
    private bool cantMove;
    //Se esta sobre acao de knockback ou nao
    private bool underKnockback = false;


    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //Adiciona callback de knockback da vida
        scr_HealthController health = GetComponent<scr_HealthController>();
        if (health != null)
        {
            health.addKnockbackCallback(this.onKnockBack);
        }
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
            nextFloor = false;
            //Calcula se acabou a plataforma
            hits = Physics2D.OverlapCircleAll(transform.Find("NextFloorCollision").position, 0.1f);
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
            nextWall = false;
            foreach (Collider2D hit in hits)
            {
                nextWall = ((hit.gameObject.layer == LayerMask.NameToLayer("Ground")) || (hit.gameObject.layer == LayerMask.NameToLayer("Entity")));
                if (hit.isTrigger)
                    nextWall = false;
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
        if (isGrounded && !cantMove && !underKnockback) {
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

    public void StopMoving() {
        cantMove = true;
    }

    public void ResumeMoviment() {
        cantMove = false;
    }

    private void onKnockBack()
    {
        StartCoroutine(waitKnockback());
    }

    private IEnumerator waitKnockback()
    {
        underKnockback = true;
        float counter = 0;
        while (counter < knockbackTime)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        underKnockback = false;
    }

    private void OnDestroy()
    {
        scr_HealthController health = GetComponent<scr_HealthController>();
        if (health != null)
        {
            health.removeKnockbackCallback(this.onKnockBack);
        }
    }

}
