using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyBehavTouchDamage : MonoBehaviour {

    [Tooltip("Dano recebido pelo jogador ao tocar no inimigo")]
    public float touchDamage = 10f;
    [Tooltip("Forca recebida pelo jogador ao tocar no inimigo")]
    public float repulseForce = 10f;
    [Tooltip("Tempo que o inimigo fica sem causar dano imediatamente depois de ter danificado o jogador")]
    public float attackCooldown = 0.5f;
    [Tooltip("Direcao da forca repulsiva e diretamente lateral ou depende da posicao de contato")]
    public bool ortogonalForce = false;


    //O inimigo pode causar dano ou nao (ligado ao tempo de cooldown
    private bool canCauseDamage = true;

	// Use this for initialization
	void Start () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canCauseDamage)
            checkPlayerCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (canCauseDamage)
            checkPlayerCollision(collision);
    }

    private void checkPlayerCollision(Collision2D collision) {
        if (collision.gameObject.tag == "Player")
        {
            scr_HealthController health = collision.gameObject.GetComponent<scr_HealthController>();
            if (health != null)
            {
                Vector3 diference = collision.transform.position - transform.position;
                Vector2 direction;
                if (ortogonalForce)
                    direction = (diference.x > 0) ? Vector2.right : Vector2.left;
                else
                    direction = diference.normalized;

                health.takeDamage(touchDamage, direction * repulseForce);
                StartCoroutine(waitCooldown());
            }
        }
    }

    private IEnumerator waitCooldown()
    {
        canCauseDamage = false;
        float counter = 0;
        while(counter < attackCooldown)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        canCauseDamage = true;
    }
}
