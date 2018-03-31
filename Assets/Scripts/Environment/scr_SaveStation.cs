using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class scr_SaveStation : scr_Interactable
{

    [Tooltip("Esse save point recupera a vida do jogador")]
    public bool recoverHP = true;
    [Tooltip("Esse save point recupera a energia do jogador")]
    public bool recoverEnergy = false;

    [Header("Referencias de HUD")]
    public GameObject rightHandLayout;
    public GameObject leftHandLayout;
    public GameObject headLayout;
    public GameObject torsoLayout;
    public GameObject legsLayout;


    private Canvas messageDisplayCanvas;
    private Canvas menuCanvas;
    private Text text;
    private bool hasUpdated = false;
    private bool paused = false;

    protected void Awake()
    {
        base.Awake();
        messageDisplayCanvas = transform.Find("MessageCanvas").GetComponent<Canvas>();
        menuCanvas = transform.Find("MenuCanvas").GetComponent<Canvas>();
        text = messageDisplayCanvas.GetComponentInChildren<Text>();
        messageDisplayCanvas.enabled = false;
        menuCanvas.enabled = false;
        hasUpdated = false;
    }

    public override bool Interact(scr_Interactor interactor)
    {
        if (interactor.gameObject.tag != "Player")
            return false;

        messageDisplayCanvas.enabled = false;
        menuCanvas.enabled = true;
        updateDropdown();
        heal(interactor);
        if (!paused)
        {
            paused = true;
            scr_GameManager.instance.setPauseGame(true);
            scr_GameManager.instance.canPause = false;
        }


        if (!hasUpdated)
        {
            hasUpdated = true;
            //showButtons();
        }


        return true;

        //string newText = "Saved with sucess";
        ////Garantir que é o jogador		
        //if (interactor.gameObject.tag != "Player")
        //    return false;
        //scr_GameManager.instance.updatePlayerStats();
        //scr_Player_Stats playerStats = scr_GameManager.instance.playerStats;
        //if (recoverHP)
        //{
        //    playerStats.currentHp = playerStats.maxHp;
        //    scr_HealthController health = interactor.GetComponent<scr_HealthController>();
        //    health.setCurrentHealth(health.getMaxHealth());
        //    newText += "\nHealth recovered!";
        //}
        //if (recoverEnergy)
        //{
        //    playerStats.currentEnergy = playerStats.maxEnergy;
        //    scr_PlayerEnergyController energy = interactor.GetComponent<scr_PlayerEnergyController>();
        //    energy.setCurrentEnergy(energy.getMaxEnergy());
        //    newText += "\nEnergy recovered!";
        //}
        //playerStats.savePointScene = SceneManager.GetActiveScene().name;
        //playerStats.savePointName = gameObject.name;
        //scr_GameManager.instance.playerStats = playerStats;
        //bool result = scr_GameManager.instance.Save();
        //if (result && text != null)
        //    text.text = newText;
        //return result;

    }

    protected override void BecameInterable()
    {
        messageDisplayCanvas.enabled = true;
        menuCanvas.enabled = false;
        //showButtons();
    }

    protected override void StopInterable()
    {
        messageDisplayCanvas.enabled = false;
        menuCanvas.enabled = false;

        CloseWindow();
        //hasUpdated = false;
    }

    private void updateDropdown()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        scr_Player_Stats playerStats = player.GetComponent<scr_PA_Manager>().playerStats;

        Dropdown drop;
        int currentValue = -1;
        drop = rightHandLayout.GetComponentInChildren<Dropdown>();
        drop.ClearOptions();
        List<Dropdown.OptionData> options;
        options = new List<Dropdown.OptionData>();
        currentValue = -1;
        for (int i = 0; i < playerStats.unlockedWeapons.Count; i++)
        {
            options.Add(new Dropdown.OptionData(playerStats.unlockedWeapons[i].ToString()));
            if (playerStats.rightWeaponEquiped == playerStats.unlockedWeapons[i])
                currentValue = i;
        }
        drop.AddOptions(options);
        drop.value = currentValue;

        //Left hand
        drop = leftHandLayout.GetComponentInChildren<Dropdown>();
        drop.ClearOptions();
        options.Clear();
        currentValue = -1;
        for (int i = 0; i < playerStats.unlockedWeapons.Count; i++)
        {
            options.Add(new Dropdown.OptionData(playerStats.unlockedWeapons[i].ToString()));
            if (playerStats.leftWeaponEquiped == playerStats.unlockedWeapons[i])
                currentValue = i;
        }
        drop.AddOptions(options);
        drop.value = currentValue;

        //Head
        drop = headLayout.GetComponentInChildren<Dropdown>();
        drop.ClearOptions();
        options.Clear();
        currentValue = -1;
        for (int i = 0; i < playerStats.unlockedHeads.Count; i++)
        {
            options.Add(new Dropdown.OptionData(playerStats.unlockedHeads[i].ToString()));
            if (playerStats.headEquiped == playerStats.unlockedHeads[i])
                currentValue = i;
        }
        drop.AddOptions(options);
        drop.value = currentValue;

        //Torso
        drop = torsoLayout.GetComponentInChildren<Dropdown>();
        drop.ClearOptions();
        options.Clear();
        currentValue = -1;
        for (int i = 0; i < playerStats.unlockedTorsos.Count; i++)
        {
            options.Add(new Dropdown.OptionData(playerStats.unlockedTorsos[i].ToString()));
            if (playerStats.torsoEquiped == playerStats.unlockedTorsos[i])
                currentValue = i;
        }
        drop.AddOptions(options);
        drop.value = currentValue;

        //Legs
        drop = legsLayout.GetComponentInChildren<Dropdown>();
        drop.ClearOptions();
        options.Clear();
        currentValue = -1;
        for (int i = 0; i < playerStats.unlockedLegs.Count; i++)
        {
            options.Add(new Dropdown.OptionData(playerStats.unlockedLegs[i].ToString()));
            if (playerStats.legsEquiped == playerStats.unlockedLegs[i])
                currentValue = i;
        }
        drop.AddOptions(options);

    }

    public void equipPart(scr_PA_Manager.PartType type, int id, bool isRight)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        scr_PA_Manager paMan = player.GetComponent<scr_PA_Manager>();
        switch (type)
        {
            case scr_PA_Manager.PartType.Head:
                paMan.equipHead(id);
                break;
            case scr_PA_Manager.PartType.Legs:
                paMan.equipLegs(id);
                break;
            case scr_PA_Manager.PartType.Torso:
                paMan.equipBody(id);
                break;
            case scr_PA_Manager.PartType.Weapon:
                int arm = (isRight) ? 0 : 1;
                if (id == 0)
                    paMan.removeWeapon(arm);
                else
                {
                    paMan.equipWeapon(id - 1, arm);
                    paMan.pauseWeaponScripts(false);
                }
                break;
        }
    }

    private void heal(scr_Interactor interactor)
    {
        if (interactor.gameObject.tag != "Player")
            return;

        if (recoverHP)
        {
            scr_HealthController health = interactor.GetComponent<scr_HealthController>();
            health.setCurrentHealth(health.getMaxHealth());
        }
        if (recoverEnergy)
        {
            scr_PlayerEnergyController energy = interactor.GetComponent<scr_PlayerEnergyController>();
			energy.setCurrentResEnergy(energy.getMaxResEnergy());
        }
    }

    public void onSaveButtonClick()
    {
        scr_GameManager.instance.updatePlayerStats();
        scr_Player_Stats playerStats = scr_GameManager.instance.playerStats;

        playerStats.savePointScene = SceneManager.GetActiveScene().name;
        playerStats.savePointName = gameObject.name;
        scr_GameManager.instance.playerStats = playerStats;
        bool result = scr_GameManager.instance.Save();

        Debug.Log("Player salvou: " + result);

        //if (result && text != null)
        //    text.text = newText;
        //return result;

    }

    public void CloseWindow()
    {
        if (paused)
        {
            paused = false;
            scr_GameManager.instance.setPauseGame(false);
            scr_GameManager.instance.canPause = true;
        }

        //Show correct HUD
        menuCanvas.enabled = false;
    }

    public void onClickEquipButtons(int origin)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        scr_Player_Stats playerStats = player.GetComponent<scr_PA_Manager>().playerStats;

        //Right Hand
        if (origin == 0)
        {
            Dropdown drop = rightHandLayout.GetComponentInChildren<Dropdown>();
            int selected = drop.value;
            int id = (int) playerStats.unlockedWeapons[selected];
            equipPart(scr_PA_Manager.PartType.Weapon, id, true);
            print(origin + " " + drop.value);
        }
        //Left Hand
        else if(origin == 1)
        {
            Dropdown drop = leftHandLayout.GetComponentInChildren<Dropdown>();
            int selected = drop.value;
            int id = (int)playerStats.unlockedWeapons[selected];
            equipPart(scr_PA_Manager.PartType.Weapon, id, false);
            print(origin + " " + drop.value);
        }
        //Head
        else if(origin == 2)
        {
            Dropdown drop = headLayout.GetComponentInChildren<Dropdown>();
            int selected = drop.value;
            int id = (int)playerStats.unlockedHeads[selected];
            equipPart(scr_PA_Manager.PartType.Head, id,false);
            print(origin + " " + drop.value);
        }
        //Torso
        else if(origin == 3)
        {
            Dropdown drop = torsoLayout.GetComponentInChildren<Dropdown>();
            int selected = drop.value;
            int id = (int)playerStats.unlockedTorsos[selected];
            equipPart(scr_PA_Manager.PartType.Torso,id , false);
            print(origin + " " + drop.value);
        }
        //Legs
        else if(origin == 4)
        {
            Dropdown drop = legsLayout.GetComponentInChildren<Dropdown>();
            int selected = drop.value;
            int id = (int)playerStats.unlockedLegs[selected];
            equipPart(scr_PA_Manager.PartType.Legs, id, true);
            print(origin + " " + drop.value);
        }
    }

}
