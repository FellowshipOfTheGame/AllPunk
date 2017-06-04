using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_CameraController : MonoBehaviour {

	public float limitX; //maximo entre a posição da camera e posição do player no eixo X
	public float limitY;
	public GameObject player; //Referencia ao jogador
	private Transform camTrans;
	private float playerSpeed;

	// Use this for initialization
	void Start () {
		camTrans = this.gameObject.transform;
	}
	
	void Update () {
		//Deslocamento entre posição da camera e da 
		Vector2 offset = player.transform.position - camTrans.position;
		playerSpeed = player.GetComponent<Rigidbody2D>().velocity.magnitude;

		if (player.transform.position.x > camTrans.position.x + limitX
			|| player.transform.position.x < camTrans.position.x - limitX 
			|| player.transform.position.y > camTrans.position.y + limitY
			|| player.transform.position.y < camTrans.position.y - limitY) {
				offset = offset / offset.magnitude;//Deixa com norma 1
				camTrans.Translate(offset * playerSpeed * Time.deltaTime);
		}
	}
}