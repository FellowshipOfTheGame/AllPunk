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

    private List<string> equipArm;
    private List<string> equipHead;
    private List<string> equipTorso;
    private List<string> equipLegs;

    private Animator animator;

    protected void Awake()
    {
        base.Awake();
        messageDisplayCanvas = transform.Find("MessageCanvas").GetComponent<Canvas>();
        menuCanvas = transform.Find("MenuCanvas").GetComponent<Canvas>();
        text = messageDisplayCanvas.GetComponentInChildren<Text>();
        messageDisplayCanvas.enabled = false;
        menuCanvas.enabled = false;
        hasUpdated = false;
        equipArm = new List<string>();
        equipHead = new List<string>();
        equipLegs = new List<string>();
        equipTorso = new List<string>();
        animator = GetComponent<Animator>();
    }

    public override bool Interact(scr_Interactor interactor)
    {
        if (interactor.gameObject.tag != "Player")
            return false;

        messageDisplayCanvas.enabled = false;
        menuCanvas.enabled = true;
        animator.SetTrigger("Close");
        updateDropdown(interactor);
        heal(interactor);
        if (!paused)
        {
            paused = true;
            scr_GameManager.instance.setPauseGame(true);
            scr_HUDController.hudController.canPause = false;
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

    private void updateDropdown(scr_Interactor interactor)
    {
        
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        scr_GameManager.instance.updatePlayerStats();
        scr_Player_Stats playerStats = scr_GameManager.instance.playerStats;
        scr_EPManager epMan = interactor.GetComponent<scr_EPManager>();

        Dropdown dropRight, dropLeft, dropHead, dropTorso, dropLegs;
        int currentValue = -1;
        dropRight = rightHandLayout.GetComponentInChildren<Dropdown>();
        dropLeft = leftHandLayout.GetComponentInChildren<Dropdown>();
        dropHead = headLayout.GetComponentInChildren<Dropdown>();
        dropTorso = torsoLayout.GetComponentInChildren<Dropdown>();
        dropLegs = legsLayout.GetComponentInChildren<Dropdown>();
        //Apagar o que já está lá
        dropRight.ClearOptions();
        dropLeft.ClearOptions();
        dropHead.ClearOptions();
        dropTorso.ClearOptions();
        dropLegs.ClearOptions();
        equipArm.Clear();
        equipHead.Clear();
        equipTorso.Clear();
        equipLegs.Clear();
        List<Dropdown.OptionData> optionsRight = new List<Dropdown.OptionData>();
        List<Dropdown.OptionData> optionsLeft = new List<Dropdown.OptionData>();
        List<Dropdown.OptionData> optionsTorso = new List<Dropdown.OptionData>();
        List<Dropdown.OptionData> optionsHead = new List<Dropdown.OptionData>();
        List<Dropdown.OptionData> optionsLegs = new List<Dropdown.OptionData>();

        //Adicionar a opção de não equipar nada
        optionsRight.Add(new Dropdown.OptionData("Nada"));
        optionsLeft.Add(new Dropdown.OptionData("Nada"));
        optionsTorso.Add(new Dropdown.OptionData("Nada"));
        optionsHead.Add(new Dropdown.OptionData("Nada"));
        optionsLegs.Add(new Dropdown.OptionData("Nada"));
        
        //Manter keynames seguros
        equipArm.Add("None");
        equipHead.Add("None");
        equipTorso.Add("None");
        equipLegs.Add("None");

        foreach (KeyValuePair<string, bool> pair in playerStats.unlockedEPs) {
            if(pair.Value){
                scr_EP ep = epMan.EPDictionary[pair.Key];
                switch(ep.getEpType()) {
                    case scr_EP.EpType.Arm:
                        optionsLeft.Add(new Dropdown.OptionData(ep.getEpName()));
                        optionsRight.Add(new Dropdown.OptionData(ep.getEpName()));
                        equipArm.Add(ep.getKeyName());
                        break;
                    case scr_EP.EpType.Head:
                        optionsHead.Add(new Dropdown.OptionData(ep.getEpName()));
                        equipHead.Add(ep.getKeyName());
                        break;
                    case scr_EP.EpType.Torso:
                        optionsTorso.Add(new Dropdown.OptionData(ep.getEpName()));
                        equipTorso.Add(ep.getKeyName());
                        break;
                    case scr_EP.EpType.Legs:
                        optionsLegs.Add(new Dropdown.OptionData(ep.getEpName()));
                        equipLegs.Add(ep.getKeyName());
                        break;
                }
            }
        }

        //Atualiza opções do braço direito
        string equippedPart;
        currentValue = 0;
        equippedPart = epMan.getCurrentPart(scr_EP.EpType.Arm,scr_EPManager.ArmToEquip.RightArm);
        for(int i = 0; i < equipArm.Count; i++) {
            if(equippedPart == equipArm[i])
                currentValue = i;
        }
        dropRight.AddOptions(optionsRight);
        dropRight.value = currentValue;

        //Atualiza opções para o braço esquerdo
        currentValue = 0;
        equippedPart = epMan.getCurrentPart(scr_EP.EpType.Arm,scr_EPManager.ArmToEquip.LeftArm);
        for(int i = 0; i < equipArm.Count; i++) {
            if(equippedPart == equipArm[i])
                currentValue = i;
        }
        dropLeft.AddOptions(optionsLeft);
        dropLeft.value = currentValue;

        //Atualiza opções para a cabeça
        currentValue = 0;
        equippedPart = epMan.getCurrentPart(scr_EP.EpType.Head);
        for(int i = 0; i < equipHead.Count; i++) {
            if(equippedPart == equipHead[i])
                currentValue = i;
        }
        dropHead.AddOptions(optionsHead);
        dropHead.value = currentValue;

        //Atualiza opções para o torso
        currentValue = 0;
        equippedPart = epMan.getCurrentPart(scr_EP.EpType.Torso);
        for(int i = 0; i < equipTorso.Count; i++) {
            if(equippedPart == equipTorso[i])
                currentValue = i;
        }
        dropTorso.AddOptions(optionsTorso);
        dropTorso.value = currentValue;

        //Atualiza opções para as pernas
        currentValue = 0;
        equippedPart = epMan.getCurrentPart(scr_EP.EpType.Legs);
        for(int i = 0; i < equipLegs.Count; i++) {
            if(equippedPart == equipLegs[i])
                currentValue = i;
        }
        dropLegs.AddOptions(optionsLegs);
        dropLegs.value = currentValue;
        
    }

    /*
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
    */

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

		string previusScenePath = SceneManager.GetActiveScene().path;
        string[] separator = {"Scenes/", ".unity"};
		playerStats.savePointScene = previusScenePath.Split(separator, System.StringSplitOptions.None)[1];
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
            scr_HUDController.hudController.canPause = true;
            animator.SetTrigger("Open");

        }

        //Show correct HUD
        //menuCanvas.enabled = false;
    }

    public void onClickEquipButtons(int origin)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //scr_Player_Stats playerStats = player.GetComponent<scr_PA_Manager>().playerStats;

        scr_EPManager epMan = player.GetComponent<scr_EPManager>();

        //Right Hand
        if (origin == 0)
        {
            Dropdown drop = rightHandLayout.GetComponentInChildren<Dropdown>();
            int selected = drop.value;
            epMan.equipPart(equipArm[selected], scr_EPManager.ArmToEquip.RightArm);
            print(origin + " " + drop.value);
            epMan.pauseWeaponScripts(false);
        }
        //Left Hand
        else if(origin == 1)
        {
            Dropdown drop = leftHandLayout.GetComponentInChildren<Dropdown>();
            int selected = drop.value;
            epMan.equipPart(equipArm[selected], scr_EPManager.ArmToEquip.LeftArm);
            print(origin + " " + drop.value);
            epMan.pauseWeaponScripts(false);
        }
        //Head
        else if(origin == 2)
        {
            Dropdown drop = headLayout.GetComponentInChildren<Dropdown>();
            int selected = drop.value;
            epMan.equipPart(equipHead[selected]);
            print(origin + " " + drop.value);
        }
        //Torso
        else if(origin == 3)
        {
            Dropdown drop = torsoLayout.GetComponentInChildren<Dropdown>();
            int selected = drop.value;
            epMan.equipPart(equipTorso[selected]);
            print(origin + " " + drop.value);
        }
        //Legs
        else if(origin == 4)
        {
            Dropdown drop = legsLayout.GetComponentInChildren<Dropdown>();
            int selected = drop.value;
            epMan.equipPart(equipLegs[selected]);
            print(origin + " " + drop.value);
        }

        /*/
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
        */
    }

}
