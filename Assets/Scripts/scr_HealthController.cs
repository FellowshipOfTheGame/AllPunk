using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script genérico para gerenciar o estado de saude e defesa de objetos do jogo
 */

public class scr_HealthController : MonoBehaviour {


	#region variables

	//Maximo da vida
	public float maxHp;
	//Vida atual
	private float currentHp;
	//Defesa, usada para diminuir dano
	public float defense;
	//Medida de equilíbrio em que 0 é pouco equilibrado e 1 muito equilibrado
	//Usada para reduzir a força do knockback
	[Range(0,1)]
	public float poise;
	//Referencia ao Rigidbody2D da entidade
	private Rigidbody2D entityRigidBody;
	//Verificando se o objeto está morto
	private bool isDead;

	/* //Variáveis para stun
	private float stunTime = 2;
	private float currStunTime = 0;
	*/
    //Referencia ao animator da entidade
    private Animator animator;
    #endregion variables

	private void Awake(){
		entityRigidBody = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		currentHp = maxHp;
        animator = GetComponent<Animator>();
	}

	/**
	 * Método usado para tomar dano.
	 * O HP é alterado subtraindo damage - ou / defense
	 * O knockback é aplicado * 1-poise 
	 * 
	 * @param	damage	quantidade de dano a ser tomado
	 * @param	direction	Vetor de direção e intensidade do knockback
	 */
	public void takeDamage(float damage, Vector2 direction){

		float netDamage = damage - this.defense;
		if(netDamage > 0)
			this.currentHp -= netDamage;

		if (this.currentHp <= 0) {
			this.isDead = true;
			this.die ();
		} else {
			//this.transform.position += new Vector3 (0, 0.5f, 0); //levemente levanta do chao
			this.transform.position += new Vector3 (0, 1f, 0); //levemente levanta do chao
			print("dir1 " + direction);
			direction = direction * (1 - poise);//Poise para reduzir knockback
			print("dir2 " + direction);
			this.entityRigidBody.AddForce (direction, ForceMode2D.Impulse);
            if (animator != null)
            {
                animator.SetTrigger("Hurt");
                print("Ai");
            }
		}
	}

	/* //TENTATIVA DE STUN
	public void Update(){
		if (currStunTime > 0) {
			this.entityRigidBody.velocity = new Vector2 (0f, 0f);
			currStunTime -= Time.deltaTime;
			if (currStunTime < 0)
				currStunTime = 0;
		}
	}*/

	public float getMaxHealth(){
		return this.maxHp;
	}

	public float getCurrentHealth(){
		return this.currentHp;
	}

	/**
	 * Método para ma		tar a entidade.
	 * Deve ser overwriten para efeitos de morte específicos
	 * @param void
	 * @return void
	 */
	private void die (){
		Destroy(this.gameObject);
	}

}
