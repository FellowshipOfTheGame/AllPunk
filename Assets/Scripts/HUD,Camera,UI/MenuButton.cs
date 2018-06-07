﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public GameObject loadButton;
    public string scene;

    private void Start()
    {
        if(loadButton != null)
        {
            scr_SaveManager saveManager = new scr_SaveManager();
            if (!saveManager.hasSaveGame())
            {
                Button button = loadButton.GetComponent<Button>();
                button.interactable = false;
            }
        }
    }

    public void ExitClick(){
        print("Clicou em Exit");
        Application.Quit();
    }

	public void newGameClick()
    {
        print("Clicou em New Game");
        Destroy(scr_GameManager.instance);
        SceneManager.LoadScene(scene);
    }

    public void loadGameClick()
    {
        print("Clicou em load");
        scr_GameManager.instance.LoadGame();
    }

    public void creditsClick()
    {
        print("Clicou em creditos");
    }

    public void optionsClick()
    {
        print("Clicou em options");
    }


}