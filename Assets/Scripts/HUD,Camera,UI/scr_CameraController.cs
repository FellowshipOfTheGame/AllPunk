using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_CameraController : MonoBehaviour {

	//public float panSensitivity;

	public float limitX = 2; //maximo entre a posição da camera e posição do player no eixo X
	public float limitY = 1;
	public GameObject player; //Referencia ao jogador
	private Transform camTrans;
	private Camera camComp;//Componente de câmera 
	private float playerSpeed;
	private bool isPanning; //Se o jogador está dando pan na camera ou não

	private Vector3 lastCamPos;  //Ultima posição da camera
	private Vector3 mouseRelativeToPlayer; //Posição do mouse relativo ao centro
	// Use this for initialization
	void Awake () {
		camTrans = this.gameObject.transform;
		camComp = GetComponent<Camera> ();
		isPanning = false;
	}
	
	void Update () {

		//ZOOM+
		if (Input.GetButton("EnableZoom") && Input.GetAxis ("Mouse ScrollWheel") < 0f){
			if (camComp.orthographicSize < 12) {
				camComp.orthographicSize++;
			}
		}
		//ZOOM-
		else if (Input.GetButton("EnableZoom") && Input.GetAxis ("Mouse ScrollWheel") > 0f) {
			if (camComp.orthographicSize > 6) {
				camComp.orthographicSize--;
			}
		}

		//PAN MANUAL
		if (Input.GetButtonDown ("EnablePan")) {
			isPanning = true;
			lastCamPos = camTrans.position;
		}
		if (Input.GetButton ("EnablePan")) {
			mouseRelativeToPlayer = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
			Vector2 mouseDirection = mouseRelativeToPlayer.normalized;
			Vector2 offset = camTrans.position - player.transform.position;

			/*Vector3 newCamPos = camTrans.position + mouseDirection*5;
			camTrans.Translate (newCamPos * Time.deltaTime);*/
			//camTrans.pos + (mouseGlobal - camTransPos) - camTransPos
			if( offset.magnitude < 5){
				//camTrans.Translate (mouseDirection * 60 * Time.deltaTime);
				camTrans.Translate (mouseDirection * 60 * Time.deltaTime, player.transform);
			}

			//print ( lastCamPos + "|||" + mouseDirection + "|||" + offset);
		}
		if (Input.GetButtonUp ("EnablePan")) {
			isPanning = false;
			Vector2 backVector = lastCamPos - camTrans.position;
			camTrans.Translate (backVector.normalized * 60 * Time.deltaTime);
		}
			

		//PAN AUTOMÁTICO 
		if (this.player == null) {
			camTrans.position = (new Vector3 (0, 0, camTrans.position.z));
		} else {
			//Deslocamento entre posição da camera e da 
			Vector2 offset = player.transform.position - camTrans.position;
			playerSpeed = player.GetComponent<scr_PlayerController>().speed;

			if (!isPanning && (player.transform.position.x > camTrans.position.x + limitX
			    || player.transform.position.x < camTrans.position.x - limitX
			    || player.transform.position.y > camTrans.position.y + limitY
			    || player.transform.position.y < camTrans.position.y - limitY)) {
				offset = offset / offset.magnitude;//Deixa com norma 1
				camTrans.Translate (offset * playerSpeed * Time.deltaTime);
			} 
			else if (isPanning) {
				//Está fazendo panning manual, translada pelo vetor velocidade do player para a camera acompanhar
				camTrans.Translate (player.GetComponent<scr_PlayerController> ().getVelocity () * Time.deltaTime);
			}
		}
	}
}