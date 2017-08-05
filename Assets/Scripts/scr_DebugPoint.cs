using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_DebugPoint : MonoBehaviour {
	public float timeToLive = 10; //em Segundos

	public void Update(){
		if (timeToLive <= 0)
			Die ();
		else
			timeToLive -= Time.deltaTime;
	}

	void Die(){
		Destroy (this.gameObject);
		Destroy (this);
	}
}
