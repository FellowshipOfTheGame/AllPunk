using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_CameraController : MonoBehaviour {

	//public float panSensitivity;

	public float limitX = 2; //maximo entre a posição da camera e posição do player no eixo X
	public float limitY = 1;
	public GameObject player; //Referencia ao jogador

	public int _minCameraZoom = 6;
	public int _maxCameraZoom = 12;

	public float _maxCameraDist = 7.5f;
	public float _camPanSpeed = 10.0f;

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

	void OnDrawGizmos(){
		/*
		//Mouse Relative to Player
		Gizmos.DrawLine (player.transform.position, Camera.main.ScreenToWorldPoint (Input.mousePosition));

		Gizmos.color = Color.red;
		//Offset
		if(camTrans != null)
			Gizmos.DrawLine(player.transform.position, camTrans.position);*/
	}


	/// <summary>
	/// Manually pans the camera
	/// </summary>
	void manualPan(){

		///Limits the camera X and Y positions to a radius defined by the player position + _radius
		camTrans.position = new Vector3 (
			Mathf.Clamp(camTrans.position.x, player.transform.position.x - _maxCameraDist, player.transform.position.x + _maxCameraDist),
			Mathf.Clamp(camTrans.position.y, player.transform.position.y - _maxCameraDist, player.transform.position.y + _maxCameraDist),
			camTrans.position.z);

		if (Input.GetButtonDown ("EnablePan")) {
			isPanning = true;
			lastCamPos = camTrans.position;
		}
		if (Input.GetButton ("EnablePan")) {
			mouseRelativeToPlayer = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;

			Vector2 mouseDirection = mouseRelativeToPlayer.normalized;
			//Vector3 offset = camTrans.position - player.transform.position;

			camTrans.Translate (mouseDirection * _camPanSpeed * Time.deltaTime);
			//camTrans.position = Vector2.SmoothDamp (camTrans.position, mouseDirection * 15, , 1f, 2f, 5f);
		}
		if (Input.GetButtonUp ("EnablePan")) {
			isPanning = false;
			Vector2 backVector = lastCamPos - camTrans.position;
			camTrans.Translate (backVector.normalized * _camPanSpeed*2 * Time.deltaTime);
		}
	}

	/// <summary>
	/// Manually zooms the câmera.
	/// </summary>
	void manualZoom(){
		//ZOOM+
		if (Input.GetButton("EnableZoom") && Input.GetAxis ("Mouse ScrollWheel") < 0f){
			if (camComp.orthographicSize < _maxCameraZoom) {
				camComp.orthographicSize++;
			}
		}
		//ZOOM-
		else if (Input.GetButton("EnableZoom") && Input.GetAxis ("Mouse ScrollWheel") > 0f) {
			if (camComp.orthographicSize > _minCameraZoom) {
				camComp.orthographicSize--;
			}
		}
	}

	/// <summary>
	/// Automatically pans the camera towards the player
	/// </summary>
	void autoPan(){
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

	void Update () {
		manualZoom ();

		manualPan ();

		autoPan ();
	}
}