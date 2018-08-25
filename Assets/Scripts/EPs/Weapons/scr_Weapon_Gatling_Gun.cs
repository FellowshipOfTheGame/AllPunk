using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Weapon_Gatling_Gun : scr_Weapon {

    [Header("Gatling variables")]
    [Tooltip("Distancia máxima que é possível atacar com a arma")]
    public float maxAttackDistance = 5f;
    [Tooltip("Tempo que vai ser necessário para começar a atirar depois de apertar o botao")]
    public float initialWindUp = 0.5f;
    [Tooltip("Cadência de tiro")]
    public float fireRate = 0.2f;
    [Tooltip("Dano causado por cada tiro")]
    public float bulletDamage = 5f;
    [Tooltip("Força causada por cada tiro")]
    public float bulletForce = 2f;
    [Tooltip("Referência para o ponto de onde começa o tiro")]
    public Transform targetPoint;
    [Tooltip("Variação angular entre a direção correta e o alvo (Em graus)")]
    public float angularVariation = 0;

    [Header("Cosmedic")]
    [Tooltip("Maxima distancia que deve ser desenhada a linha de tiro")]
    public float lineDistanceMax = 2f;
    [Tooltip("Tempo que leva para a linha desaparecer após o tiro")]
    public float fadeOfTime = 0.1f;

    private float fireCounter = 0f;
    private bool animationIsShooting = false;
    private LineRenderer line;
    private Animator gunAnimator;
    private bool hasBegunShootingSound = false;

    private void Awake()
    {
        base.Awake();
        line = GetComponent<LineRenderer>();
        if (line != null)
            line.enabled = false;
        cooldownTime = 0f;
        gunAnimator = GetComponent<Animator>();
    }


	public override bool Unequip ()
	{
		Debug.Log("Removed: "+ epName);
		return true;
	}

    protected override void AttackAction(bool noAnimation)
    {
        //Verifica se o jogador acabou de clicar
        if (clicked)
        {
            fireCounter = initialWindUp;
            animationIsShooting = true;
            gunAnimator.SetBool("Shooting", true);
        }
        else if (holding)
        {
            if(fireCounter <= 0)
            {
                //Começa a toca o som se não tinha
                if(!hasBegunShootingSound){
                    audioClient.playLoopClip("Firing", scr_AudioClient.sources.local);
                    hasBegunShootingSound = true;
                }
                //Drena energia
                useEnergy();

                //Pega a direcao do tiro
                Vector3 direction = targetPoint.position - transform.position;
                float angle = Mathf.Atan2(direction.y,direction.x);
                //Faz a mudança do angulo
                angle += (UnityEngine.Random.Range(-angularVariation,angularVariation) * Mathf.Deg2Rad);
                direction = new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0); 
                //direction.Normalize();

                //Verifica se colidiu com alguma coisa
                RaycastHit2D hit = Physics2D.Raycast(targetPoint.position, direction, maxAttackDistance);
                if(hit.collider != null)
                {
                    if(hit.transform.gameObject.tag == "Enemy")
                    {
                        scr_HealthController health = hit.transform.GetComponent<scr_HealthController>();
                        if(health != null)
                        {
                            health.takeDamage(bulletDamage, direction * bulletForce);
                        }
                    }
                }


                //Desenha a linha de ataque
                if(line != null)
                {
                    line.enabled = true;
                    line.SetPosition(0, targetPoint.position);
                    line.SetPosition(1, targetPoint.position + direction * lineDistanceMax);
                    StartCoroutine(fadeOfLine());
                }

                fireCounter = fireRate;

                

            }
        }
    }

	
	// Update is called once per frame
	void Update () {
        //Mantem funcionamento da classe
        base.Update();

        //Verifica se o jogador está segurando o botao ou nao
        if (holding)
        {
            //Atualiza timers
            fireCounter -= Time.deltaTime;
        }
        else
        {
            //Desliga a animacao caso esteja tocando
            if (animationIsShooting)
            {
                animationIsShooting = false;
                gunAnimator.SetBool("Shooting", false);
                audioClient.stoplocalClip();
                hasBegunShootingSound = false;
            }
        }
	}

    /// <summary>
    /// Faz a cor da linha ir desaparecendo
    /// </summary>
    /// <returns></returns>
    private IEnumerator fadeOfLine()
    {
        Color initialColor = line.startColor;
        float counter = fadeOfTime;
        while(counter > 0)
        {
            initialColor.a = counter / fadeOfTime;
            line.startColor = initialColor;

            //Ajust position
            // Vector3 direction = targetPoint.position - transform.position;
            // direction.Normalize();
            // line.SetPosition(0, targetPoint.position);
            // line.SetPosition(1, targetPoint.position + direction * lineDistanceMax);

            counter -= Time.deltaTime;
            yield return null;
        }
        initialColor.a = 1;
        line.startColor = initialColor;
        line.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector3 direction = targetPoint.position - transform.position;
            Gizmos.DrawRay(targetPoint.position, direction.normalized * maxAttackDistance);
        }
        
    }

}
