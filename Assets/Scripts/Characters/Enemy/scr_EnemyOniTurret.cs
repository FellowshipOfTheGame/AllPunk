using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyOniTurret : MonoBehaviour {

	#region variables
	private LineRenderer lineRen;
	private BoxCollider2D triggerZone;
	private GameObject target;

	//Tempo em segundos entre tiros
	public float rateOfFire;
	//Tempo em segundos que leva para começar a atacar
	public float setUpTime;

    //Angulo máximo que a turret pode enxergar acima
    public float maxUpperAngle = 30;
    //Angulo máximo que a turret pode enxergar abaixo
    public float maxLowerAngle = -30;
    //Angulo inicial
    private float initialAngle;

    //Tamanho máximo da linha
    public float maxLineSize = 0.5f;


	private float currTimeToFire;
	private float currSetUpTime;

	public GameObject projectilePrefab;
	//Objeto filho que deve conter o barril
	public GameObject rotationAxis;
    private GameObject barrel;

	/**DESABILITADO PARA DEMO
	 * Game object vazio que armazena a posição do fim do laser. Usado para fazer a translação suave entre
	 *a posição do fim laser e a posição do alvo. 
	 */
	private GameObject aimFocus;

    //Se a turret está virada para direita ou pra esquerda
    public bool isFacingRight;

    public bool flippedY;

	//Direção do projétil
	private Vector3 direction;

    private scr_AudioClient audioClient;

	#endregion

	#region timer functions
	/***
	 * Função genérica para decrementar um timer
	 * @timer = referencia (ponteiro) para o timer a ser decrementado
	 */
	void decrementTimer(ref float timer){
		if (timer > 0)
			timer -= Time.deltaTime;
		else if (timer <= 0)
			timer = 0;
	}
		
	void resetTimer(ref float timer, float timeToReset){
		timer = timeToReset;
	}
	#endregion

	// Use this for initialization
	void Awake () {
		currSetUpTime = setUpTime;
		currTimeToFire = rateOfFire;

		lineRen = GetComponentInChildren<LineRenderer>();
		triggerZone = GetComponent<BoxCollider2D> ();
        audioClient = GetComponent<scr_AudioClient>();

        //Pega referencia publica do barril, não vai usar a abaixo
        //barrel = this.gameObject.transform.GetChild (0).gameObject;
        //aimFocus = this.gameObject.transform.GetChild (0).GetChild(0).gameObject;	//TENTATIVA
        barrel = rotationAxis.transform.Find("barrel").gameObject;
        initialAngle = rotationAxis.transform.localRotation.eulerAngles.z;

		//Posição original do laser é a própria torreta----
		lineRen.SetPosition (0, barrel.transform.position);
		lineRen.SetPosition (1, barrel.transform.position);

		direction = Vector3.zero;

	}

	/**
	 * ALVO ENTROU NA KILLZONE
	 * Setar alvo, alterar posição do Line renderer
	 */
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {//Player que entrou
            //Coreção de bug, na Update volta a posição ao normal
			lineRen.SetPosition(1, barrel.transform.position);
			target = col.gameObject;

            int direction = ((target.transform.position - transform.position).x > 0) ? 1 : -1;
            if ((direction == 1 && !isFacingRight) || (direction == -1 && isFacingRight))
                Flip();
		}
	}

	/**
	 * ALVO SAIU DA KILLZONE
	 * Deixar alvo como nulo, resetar o line renderer e resetar timers
	 */
	void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.tag == "Player") {//Player que entrou
			target = null;

            lineRen.SetPosition(0, barrel.transform.position);
            lineRen.SetPosition(1, barrel.transform.position);
			lineRen.widthMultiplier = maxLineSize;

			resetTimer(ref currSetUpTime, setUpTime);
			resetTimer (ref currTimeToFire, rateOfFire);
		}
	}

	void Update () {

		//print ("S: " + currSetUpTime  + " F: " + currTimeToFire);

		//Torreta tem um alvo válido
		if (target != null) {

            /* COMENTADO PARA A DEMO --
			//Se a mira não está encima do alvo, mova a mira 
			/*if (aimFocus.transform.position != target.transform.position) {
				//print ("a: " + aimFocus.transform.position + " t: " + target.transform.position + " diff: " + Vector3.Normalize(target.transform.position - aimFocus.transform.position));
				//aimFocus.transform.Translate(Vector3.Normalize(-target.transform.position + aimFocus.transform.position) * 1 * Time.deltaTime);
				aimFocus.transform.position = target.transform.position;
			}*/
            //lineRen.SetPosition (1, aimFocus.transform.position); //COMENTADO PELA DEMO*/

            //Verifica a rotação
            int facingDirection = ((target.transform.position - transform.position).x > 0) ? 1 : -1;
            if ((facingDirection == 1 && !isFacingRight) || (facingDirection == -1 && isFacingRight))
                Flip();

            //Verifica se o angulo está dentro dos limites
            Vector3 targetDirection = target.transform.position - barrel.transform.position;
            float targetAngle = Mathf.Atan2(targetDirection.y, Mathf.Abs(targetDirection.x));
            targetAngle *= Mathf.Rad2Deg;
            // if (facingDirection == -1)
            //     targetAngle = 180 - targetAngle;
            Debug.Log(targetAngle);

            //(targetAngle <= maxUpperAngle) && (targetAngle >= maxLowerAngle)
            if ((targetAngle <= maxUpperAngle) && (targetAngle >= maxLowerAngle))
            {
                //Rotaciona a ponta da arma
                if(!flippedY)
                    rotationAxis.transform.localRotation = Quaternion.Euler(0,0,initialAngle - targetAngle);
                else
                    rotationAxis.transform.localRotation = Quaternion.Euler(0,0,initialAngle + targetAngle);

                //Atualiza a posição do line renderer
                lineRen.SetPosition(0, barrel.transform.position);
                lineRen.SetPosition(1, target.transform.position);
                direction = lineRen.GetPosition(1) - lineRen.GetPosition(0);
                //direction = aimFocus.transform.position - this.gameObject.transform.position;

                //Altera a rotação do cano
                barrel.transform.rotation = Quaternion.LookRotation(direction.normalized);
                //Hack para que o sprite não saia do plano XY
                barrel.transform.right = direction;

                //Decrementa timer de setup
                if (currSetUpTime > 0)
                {
                    decrementTimer(ref currSetUpTime);
                    //lineRen.endColor = Color.green;
                    lineRen.startColor = Color.green;
                    lineRen.material.color = new Color(1f, 1f, 1f, 0f);
                }

                //Setup finalizado , pronto para disparar
                if (currSetUpTime <= 0)
                    lineRen.startColor = Color.red;

                //Setup feito mas projetil "recarregando"
                if (currTimeToFire > 0 && currSetUpTime <= 0)
                {
                    decrementTimer(ref currTimeToFire);
                    lineRen.widthMultiplier = maxLineSize * currTimeToFire / rateOfFire;
                }

                //Setup pronto, pode atirar
                if (currSetUpTime <= 0 && currTimeToFire <= 0)
                {

                    GameObject projectile = GameObject.Instantiate(projectilePrefab,
                                    barrel.transform.position + direction.normalized * 3,
                                    barrel.transform.rotation);

                    scr_Projectile projectileScr = projectile.GetComponent<scr_Projectile>();

                    projectileScr.Fire(direction, this.tag);

                    audioClient.playAudioClip("Shoot", scr_AudioClient.sources.local);

                    //reseta tempo para atirar
                    resetTimer(ref currTimeToFire, rateOfFire);
                }
            }
            else{
                decrementTimer(ref currSetUpTime);
                decrementTimer(ref currTimeToFire);
                lineRen.SetPosition(1, barrel.transform.position);
            }
		}
	}


    void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
        lineRen.SetPosition(0, barrel.transform.position);
    }
}
