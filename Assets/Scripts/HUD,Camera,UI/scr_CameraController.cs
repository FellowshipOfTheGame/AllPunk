using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_CameraController : MonoBehaviour {

	public float limitX = 2; //maximo entre a posição da camera e posição do player no eixo X
	public float limitY = 1;
	public GameObject player; //Referencia ao jogador
	private Transform camTrans;
	private Camera camComp;//Componente de câmera 
	private float playerSpeed;

	// Use this for initialization
	void Awake () {
		camTrans = this.gameObject.transform;
		camComp = GetComponent<Camera> ();
	}
	
	void Update () {


		if (Input.GetKey(KeyCode.LeftControl) && Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			//ZOOM+
			/*if(camComp.orthographicSize > 6 && 
			camComp.orthographicSize++
			print("++");*/
		}
		else if (Input.GetKey(KeyCode.LeftControl) && Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			//ZOOM-
			print("--");
		}
	


		if (this.player == null) {
			camTrans.position = (new Vector3 (0, 0, camTrans.position.z));
		} else {
			//Deslocamento entre posição da camera e da 
			Vector2 offset = player.transform.position - camTrans.position;
			//playerSpeed = player.GetComponent<Rigidbody2D> ().velocity.magnitude;
			playerSpeed = player.GetComponent<scr_PlayerController>().speed;

			if (player.transform.position.x > camTrans.position.x + limitX
			   || player.transform.position.x < camTrans.position.x - limitX
			   || player.transform.position.y > camTrans.position.y + limitY
			   || player.transform.position.y < camTrans.position.y - limitY) {
				offset = offset / offset.magnitude;//Deixa com norma 1
				camTrans.Translate (offset * playerSpeed * Time.deltaTime);
			}
		}
	}
}