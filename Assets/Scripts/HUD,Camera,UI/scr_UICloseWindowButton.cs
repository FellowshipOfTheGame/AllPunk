using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_UICloseWindowButton : MonoBehaviour {

	[SerializeField] GameObject parentWindow;

	void Start(){
		if (parentWindow == null)
			parentWindow = transform.parent.gameObject;
	}


	public void closeParentWindow(){
		parentWindow.SetActive (false);
	}
}
