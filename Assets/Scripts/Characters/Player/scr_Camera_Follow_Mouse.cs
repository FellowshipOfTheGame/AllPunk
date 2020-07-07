using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Camera_Follow_Mouse : MonoBehaviour {

	public Transform player;
	
	[Range(0,2)]
	public float smoothMouse = 2f;
	
	public Vector3 offset = new Vector3(0,0,-10);
	public float maxMouseOffset = 10;

	private Transform myTransform;
	private Camera myCamera;
	private Vector3 lastMousePosition;

	private void Start() {
		if(player == null) {
			GameObject playerObj = scr_GameManager.instance.player;
			if(playerObj != null)
				player = playerObj.transform;
		}
		myTransform = transform;
		myCamera = GetComponent<Camera>();
		lastMousePosition = Input.mousePosition;
	}

	private void LateUpdate() {
		if(Time.timeScale != 0 && player != null){
			lastMousePosition = Vector3.Lerp(lastMousePosition, Input.mousePosition, Time.deltaTime * smoothMouse);
			Vector3 mouseOffset = myCamera.ScreenToWorldPoint(lastMousePosition) - player.position;
			mouseOffset.z = 0;
			if(mouseOffset.magnitude > maxMouseOffset){
				mouseOffset = mouseOffset.normalized * maxMouseOffset;
			}
			
			myTransform.position = (player.position + offset + mouseOffset * 0.5f);
		}
	}


}
