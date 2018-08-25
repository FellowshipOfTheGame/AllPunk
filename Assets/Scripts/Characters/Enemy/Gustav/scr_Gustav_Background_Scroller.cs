using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gustav_Background_Scroller : MonoBehaviour {

	public float runningSpeed = 20;
	public float xToScroll = 12;
	public float playerFollowCompensation;
	public Transform slideObject1;
	public Transform slideObject2;
	public Transform slideObject3;

	public float currentSpeed = 0;
	public bool isFollowingPlayer;
	private GameObject player;
	private Rigidbody2D rb2dPlayer;

	// Use this for initialization
	void Start () {
		slideObject1.localPosition = new Vector3(-xToScroll/2,0,0);
		slideObject2.localPosition = new Vector3(0,0,0);		
		slideObject3.localPosition = new Vector3(xToScroll/2,0,0);
		player = GameObject.FindGameObjectWithTag("Player");
		if(player!= null)
			rb2dPlayer = player.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		pos.x = player.transform.position.x - xToScroll/2;
		transform.position = pos;
		
		if(currentSpeed > 0 || isFollowingPlayer) {
			if(isFollowingPlayer){
				slideObject1.localPosition -= new Vector3(rb2dPlayer.velocity.x * playerFollowCompensation * Time.deltaTime,0,0); 	
				slideObject2.localPosition -= new Vector3(rb2dPlayer.velocity.x * playerFollowCompensation * Time.deltaTime,0,0); 	
				slideObject3.localPosition -= new Vector3(rb2dPlayer.velocity.x * playerFollowCompensation * Time.deltaTime,0,0); 		
			}
			slideObject1.localPosition -= new Vector3(currentSpeed * Time.deltaTime,0,0);
			slideObject2.localPosition -= new Vector3(currentSpeed * Time.deltaTime,0,0);
			slideObject3.localPosition -= new Vector3(currentSpeed * Time.deltaTime,0,0);
			if(slideObject1.localPosition.x < -xToScroll*2/3)
				slideObject1.localPosition = new Vector3(xToScroll*2/3,0,0);
			else if(slideObject1.localPosition.x > xToScroll*2/3)
				slideObject1.localPosition = new Vector3(-xToScroll*2/3,0,0);
			
			if(slideObject2.localPosition.x < -xToScroll*2/3)
				slideObject2.localPosition = new Vector3(xToScroll*2/3,0,0);
			else if(slideObject2.localPosition.x > xToScroll*2/3)
				slideObject2.localPosition = new Vector3(-xToScroll*2/3,0,0);

			if(slideObject3.localPosition.x < -xToScroll*2/3)
				slideObject3.localPosition = new Vector3(xToScroll*2/3,0,0);
			else if(slideObject3.localPosition.x > xToScroll*2/3)
				slideObject3.localPosition = new Vector3(-xToScroll*2/3,0,0);

			
		}
	}

	/// <summary>
	/// Muda a velocidade linearmente para a velocidade desejada
	/// </summary>
	/// <param name="targetSpeed">Velocidade desejada</param>
	/// <param name="deltaTime">Tempo de mudança</param>
	public void changeSpeed(float targetSpeed, float deltaTime) {
		StartCoroutine(tweenSpeed(targetSpeed,deltaTime));
	}

	/// <summary>
	/// Muda a velocidade instantaneamente. Tomar cuidado
	/// </summary>
	/// <param name="targetSpeed">Velocidade desejada</param>
	public void imediateChangeSpeed(float targetSpeed) {
		currentSpeed = targetSpeed;
	}

	private IEnumerator tweenSpeed(float toSpeed, float timeToTween) {
		float deltaSpeed = (toSpeed - currentSpeed)/timeToTween;
		float counter = 0;
		while(counter < timeToTween){
			currentSpeed += deltaSpeed * Time.deltaTime;
			counter += Time.deltaTime;
			yield return null;
		}
		currentSpeed = toSpeed;
	}
}
