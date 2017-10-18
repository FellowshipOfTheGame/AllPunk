using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public GameObject activate;
    public GameObject deactivate;
    public string scene;
    public bool quit;

    public void Click(){
        if (quit)
            Application.Quit();
        else if (!string.IsNullOrEmpty(scene))
            SceneManager.LoadScene(scene);
        else{
			if(activate)activate.SetActive(true);
			if(deactivate)deactivate.SetActive(false);
        }
		unPause ();
    }

	public void unPause(){
		Time.timeScale = 1f;
	}
}
