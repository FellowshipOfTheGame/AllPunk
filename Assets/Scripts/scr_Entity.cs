using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Collider2D), typeof(Rigidbody2D) )]

abstract public class scr_Entity : MonoBehaviour {

	#region variables

	//Maximo da vida
	public float maxHp;
	//Vida atual
	protected float currentHp;
	//Defesa, usada para diminuir dano
	public float defense;
	//Medida de equilíbrio em que 0 é pouco equilibrado e 1 muito equilibrado
	//Usada para reduzir a força do knockback
	[Range(0,1)]
	public float poise;
	//Verificando se o objeto está morto
	protected bool isDead;
	//Referencia ao Rigidbody2D da entidade
	protected Rigidbody2D entityRigidBody;

	#endregion variables

	protected void Awake(){
		entityRigidBody = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
	}
		
	//private ou public?
	/**
	 * Método usado para tomar dano.
	 * O HP é alterado subtraindo damage - ou / defense
	 * O knockback é aplicado * 1-poise 
	 */
	public void takeDamage(float damage, Vector2 direction){

		float netDamage = damage - this.defense;
		if(netDamage > 0)
			this.currentHp -= netDamage;
		
		if (this.currentHp <= 0) {
			this.isDead = true;
			this.die ();
		} else {
			this.entityRigidBody.AddForce (direction * (1-this.poise), ForceMode2D.Impulse);
		}
	}

	/**
	 * Método para matar a entidade.
	 * Como cada entidade pode ter um comportamento diferente de morte,
	 * ela é abstrata
	 * @param void
	 * @return void
	 */
	abstract protected void die ();
}
