using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
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
    //Tempo de invencibilidade
    public float invicibilityTime;
    //Medida de equilíbrio em que 0 é pouco equilibrado e 1 muito equilibrado
	//Usada para reduzir a força do knockback
	[Range(0,1)]
	public float poise;
	//To play Audio
	[SerializeField] scr_AudioClient audioClient;
	//Referencia ao Rigidbody2D da entidade
	private Rigidbody2D entityRigidBody;
	//Verificando se o objeto está morto
	private bool isDead;
    //Verifica se o personagem pode ou não levar dano
    private bool canBeHurt;

	/* //Variáveis para stun
	private float stunTime = 2;
	private float currStunTime = 0;
	*/
    //Referencia ao animator da entidade
    private Animator animator;

    //Referencia para um sistema de eventos responsavel por feedback de knockback
    UnityEvent knockBackCallback;

	/// <summary>
	/// The change callback, used to update the HUD
	/// </summary>
	UnityEvent healthChangeCallback;
    #endregion variables

	#region Monobehaviour methods
    private void Awake(){
		audioClient = (scr_AudioClient)GetComponent (typeof(scr_AudioClient));
		entityRigidBody = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		currentHp = maxHp;
        animator = GetComponent<Animator>();
        canBeHurt = true;
        if(knockBackCallback == null)
        {
            knockBackCallback = new UnityEvent();
        }
		if (healthChangeCallback == null)
			healthChangeCallback = new UnityEvent ();
    }

	private void OnDestroy()
	{
		if(knockBackCallback != null)
		{
			knockBackCallback.RemoveAllListeners();
			healthChangeCallback.RemoveAllListeners ();
		}

	}
	#endregion

	#region health Methods
	/**
	 * Método usado para tomar dano.
	 * O HP é alterado subtraindo damage - ou / defense
	 * O knockback é aplicado * 1-poise 
	 * 
	 * @param	damage	quantidade de dano a ser tomado
	 * @param	direction	Vetor de direção e intensidade do knockback
	 */
	public void takeDamage(float damage, Vector2 direction){

        if (canBeHurt)
        {
			
            float netDamage = damage - this.defense;
			if (netDamage > 0) {
				this.currentHp -= netDamage;

				///Plays the Audio
				if(audioClient != null && audioClient.hasLocalAudioSource())
					audioClient.playAudioClip ("Hurt", scr_AudioClient.sources.local);
				else if(audioClient != null)
					audioClient.playAudioClip ("Hurt", scr_AudioClient.sources.sfx);

				if (healthChangeCallback != null)
					healthChangeCallback.Invoke ();
			}
            StartCoroutine(waitInvinciTime());
        }
        else
            return;


		if (this.currentHp <= 0) {
			this.isDead = true;
			this.die ();
		} 
		else {
			
			//print("dir1 " + direction);
			direction = direction * (1 - poise);//Poise para reduzir knockback
            
			///Effect Knockback
			if (direction.magnitude != 0) {
				this.entityRigidBody.velocity = Vector2.zero;
				this.entityRigidBody.AddForce (direction, ForceMode2D.Impulse);

				if(knockBackCallback != null)
					knockBackCallback.Invoke();
			}

            if (animator != null)
            {
                animator.SetTrigger("Hurt");
            }
		}
	}

	/// <summary>
	/// Removes a certain amount of damage.
	/// </summary>
	/// <param name="damage">Damage to remove.</param>
	public void removeDamage(float damage){
		this.currentHp = Mathf.Clamp (this.currentHp + damage, 0, this.maxHp);
		healthChangeCallback.Invoke ();
	}

	public float getMaxHealth(){
		return this.maxHp;
	}

	public float getCurrentHealth(){
		return this.currentHp;
	}

	public void setCurrentHealth(float newHP) {
		currentHp = newHP;
		healthChangeCallback.Invoke ();
	}

	/**
	 * Método para matar a entidade.
	 * Deve ser overwriten para efeitos de morte específicos
	 * @param void
	 * @return void
	 */
	private void die (){
		audioClient.playAudioClip ("Die", scr_AudioClient.sources.sfx);
		StartCoroutine(waitInvinciTime());
		Destroy(this.gameObject);
	}
	#endregion

    private IEnumerator waitInvinciTime() {
        canBeHurt = false;
        yield return new WaitForSeconds(invicibilityTime);
        canBeHurt = true;
    }

	#region callback Methods
    public void addKnockbackCallback(UnityAction call)
    {
        if(knockBackCallback != null)
            knockBackCallback.AddListener(call);
    }
    
    public void removeKnockbackCallback(UnityAction call)
    {
        if(knockBackCallback != null)
            knockBackCallback.RemoveListener(call);
    }

	public void addHealthChangeCallback(UnityAction call)
	{
		if(healthChangeCallback != null)
			healthChangeCallback.AddListener(call);
	}

	public void removeHealthChangeCallback(UnityAction call)
	{
		if(healthChangeCallback != null)
			healthChangeCallback.RemoveListener(call);
	}
	#endregion
}
