using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class scr_Trigger : MonoBehaviour {

	public string collideWithTag = "Player";
	public UnityEvent callFunc;

	private void Awake() {
		if(callFunc == null){
			callFunc = new UnityEvent();
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.tag == collideWithTag) {
			callFunc.Invoke();
		}
	}
}
