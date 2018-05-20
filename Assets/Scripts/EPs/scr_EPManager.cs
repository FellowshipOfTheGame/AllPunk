using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;

public class scr_EPManager : MonoBehaviour {

	/// <summary>
	/// EP keys Constants. 
	/// All keys of new EPs are to be placed inside this class
	/// </summary>
	public static class EPKeys
	{
		public const string KEY_BOILER = "torso_boiler"; 
		public const string KEY_GATLING = "arm_gatling_gun";
		public const string KEY_JETJUMP = "legs_jet_jump";
		public const string KEY_RIVETGUN = "arm_rivet_gun";
		public const string KEY_FIRESWORD = "arm_fire_sword";
		public const string KEY_STEAMBREATH = "arm_steam_breath";
		public const string KEY_STEAMGOGGLES = "head_steam_goggles";
	}

	[Tooltip("Array com EPS a serem equipadas")]
	public scr_EP[] EPs;
	/// <summary>
	/// Dicionário com string e referencia pra prefab
	/// </summary>
	public Dictionary<string, scr_EP> EPDictionary;
	/// <summary>
	/// Dicionário com string e status de desbloqueados
	/// </summary>
	public Dictionary<string, bool> UnlockedEPs;

	//Name of the equipped part
	private string currentHead = "None";
	private string currentTorso = "None";
	private string currentLegs = "None";
	private string currentRightArm = "None";
	private string currentLeftArm = "None";

	//Part reference
	private scr_EP refHead;
	private scr_EP refTorso;
	private scr_EP refLegs;
	private scr_EP refRightArm;
	private scr_EP refLeftArm;

	//Referência de meshes
	private SpriteMeshAnimation meshHead;
	private SpriteMeshAnimation meshTorso;
	private List<SpriteMeshAnimation> meshLegs;
	private List<SpriteMeshAnimation> meshLeftArm;
	private List<SpriteMeshAnimation> meshRightArm;

	//Bones reference
	private Transform boneRightArm;
	private Transform boneLeftArm;
	
	//Variavel pra saber se está virado ou não
	private bool flipped = false;

	/// <summary>
	/// Retorna a string da parte atualmente equipada
	/// </summary>
	/// <param name="type">Tipo a ser procurado. Se for procurar por arm, utilizar sobremetodo pra poder dizer qual braço</param>
	/// <returns>Parte equipada</returns>
	public string getCurrentPart(scr_EP.EpType type){
		if(type == scr_EP.EpType.Arm){
			return getCurrentPart(type, ArmToEquip.AnyArm);
		}
		switch(type) {
			case scr_EP.EpType.Head:
				return currentHead;
			case scr_EP.EpType.Torso:
				return currentTorso;
			case scr_EP.EpType.Legs:
				return currentLegs;
		}
		return "NULL";
	}

	/// <summary>
	/// Retorna o nome do braço equipado no lado dado. Pode ser usado para ver outras partes também
	/// </summary>
	/// <param name="type">Tipo procurado.</param>
	/// <param name="arm">Braço pelo qual está procurando. Não usar Any se estiver procurando por braço</param>
	/// <returns></returns>
	public string getCurrentPart(scr_EP.EpType type, ArmToEquip arm){
		if(type != scr_EP.EpType.Arm){
			return getCurrentPart(type);
		}
		switch(arm) {
			case ArmToEquip.AnyArm:
				Debug.LogWarning("Especificar qual braço quer examinar");
				return "None";
			case ArmToEquip.RightArm:
				return currentRightArm;
			case ArmToEquip.LeftArm:
				return currentLeftArm;
		}
		return "NULL";		
	}

	// Use this for initialization
	void Start () {
		EPDictionary = new Dictionary<string, scr_EP>();
		UnlockedEPs = new Dictionary<string, bool>();
		//Fix names on EP dictionary
		foreach(scr_EP ep in EPs){
			EPDictionary.Add(ep.getKeyName(), ep );
			UnlockedEPs.Add(ep.getKeyName(), false);
		}

		//Get bones references
		Transform upperBody = transform.Find("Bones").Find("Hip").Find("UpperBody");
		boneRightArm = upperBody.Find("R.UpperArm");
		boneLeftArm = upperBody.Find("L.UpperArm");

		//Get Meshes references
		string[] parts;
		Transform part;		
		Transform meshRoot = transform.Find("Mesh");

		//Head
		part = meshRoot.Find("Head");
		meshHead = part.GetComponent<SpriteMeshAnimation>();

		//Torso
		part = transform.Find("Mesh").Find("Body");
		meshTorso = part.GetComponent<SpriteMeshAnimation>();

		//Legs
		meshLegs = new List<SpriteMeshAnimation>();
		parts = new string[] { "R.UpperLeg", "R.LowerLeg", "R.Foot", "L.UpperLeg", "L.LowerLeg", "L.Foot" };
		foreach(string p in parts){
			part = meshRoot.Find(p);
			meshLegs.Add(part.GetComponent<SpriteMeshAnimation>());
		}

		//Right Arm
		meshRightArm = new List<SpriteMeshAnimation>();
		parts = new string[] { "R.UpperArm", "R.LowerArm", "R.Hand"};
		foreach(string p in parts){
			part = meshRoot.Find(p);
			meshRightArm.Add(part.GetComponent<SpriteMeshAnimation>());
		}

		//Left Arm
		meshLeftArm = new List<SpriteMeshAnimation>();
		parts = new string[] { "L.UpperArm", "L.LowerArm", "L.Hand"};
		foreach(string p in parts){
			part = meshRoot.Find(p);
			meshLeftArm.Add(part.GetComponent<SpriteMeshAnimation>());
		}

	}
	
	/// <summary>
	/// Realiza a troca de sprites e posição do braço
	/// </summary>
	public void Flip(){
		//Flip weapon orientation
		if(refRightArm != null){
			((scr_Weapon) refRightArm).flipHand();
		}
		if(refLeftArm != null){
			((scr_Weapon) refLeftArm).flipHand();
		}

		//Fix right sprite at arm
		int curRightFrame = meshRightArm[0].frame;
		int curLeftFrame = meshLeftArm[0].frame;

		if(flipped){
			curRightFrame--;
			curLeftFrame--;
		} else {
			curRightFrame++;
			curLeftFrame++;
		}

		//Flip position
		flipped = !flipped;
		//Change animation frame and sorting order of each part of the arm
		for(int i = 0; i < meshRightArm.Count; i++){
			//Change frame
			meshRightArm[i].frame = curRightFrame;
			meshLeftArm[i].frame = curLeftFrame;

			//Change sorting order
			SpriteMeshInstance rightSprite = meshRightArm[i].GetComponent<SpriteMeshInstance>();
			SpriteMeshInstance leftSprite = meshLeftArm[i].GetComponent<SpriteMeshInstance>();
			int aux = rightSprite.sortingOrder;
			rightSprite.sortingOrder = leftSprite.sortingOrder;
			leftSprite.sortingOrder = aux;
		}

		//Change arm position
		Vector3 auxPosition = boneRightArm.position;
		boneRightArm.position = boneLeftArm.position;
		boneLeftArm.position = auxPosition;
	}

	/// <summary>
	/// Desbloqueia uma parte para ser equipada
	/// </summary>
	/// <param name="keyName">Chave da EP</param>
	public void unlockPart(string keyName){
		if(UnlockedEPs.ContainsKey(keyName))
			UnlockedEPs[keyName] = true;
	}

	/// <summary>
	/// Enumerável para decidir que braço equipar
	/// </summary>
	public enum ArmToEquip{
		LeftArm,
		RightArm,
		AnyArm
	}

	/// <summary>
	/// Equipa uma parte
	/// </summary>
	/// <param name="name">Chave da EP</param>
	/// <returns>Sucesso ou não em equipar</returns>
	public bool equipPart(string name){
		if(UnlockedEPs.ContainsKey(name) || UnlockedEPs[name] == true){
			scr_EP.EpType type = EPDictionary[name].getEpType();
			if(type == scr_EP.EpType.Arm)
				return equipPart(name, ArmToEquip.AnyArm);
			scr_EP instancedObject = Transform.Instantiate(EPDictionary[name],transform.position, 
			transform.rotation, transform);
			if(instancedObject == null) return false;
			switch(type){
				case scr_EP.EpType.Head:
					if(currentHead != "None"){
						refHead.Unequip();
						Destroy(refHead.gameObject);
					}
					currentHead = name;
					refHead = instancedObject;
					meshHead.frame = instancedObject.getMeshId();
					instancedObject.Equip(gameObject);
					break;
				case scr_EP.EpType.Torso:
					if(currentTorso != "None"){
						refTorso.Unequip();
						Destroy(refTorso.gameObject);
					}
					currentTorso = name;
					refTorso = instancedObject;
					meshTorso.frame = instancedObject.getMeshId();
					instancedObject.Equip(gameObject);
					break;
				case scr_EP.EpType.Legs:
					if(currentLegs != "None"){
						refLegs.Unequip();
						Destroy(refLegs.gameObject);
					}
					currentLegs = name;
					refLegs = instancedObject;
					foreach(SpriteMeshAnimation sma in meshLegs)
						sma.frame = instancedObject.getMeshId();
					instancedObject.Equip(gameObject);
					break;				
			}

			return true;
		}
		return false;
	}

	/// <summary>
	/// Equipa uma arma no braço. Se não for um braço, vai chamar a outra função
	/// </summary>
	/// <param name="name">Chave da EP</param>
	/// <param name="arm">Braço em que vai ser equipado</param>
	/// <returns></returns>
	public bool equipPart(string name, ArmToEquip arm){
		if(!UnlockedEPs.ContainsKey(name)){
			Debug.LogWarning("Não foi encontrado a key com o nome: " + name);
			return false;
		}
		if(UnlockedEPs[name] == true){
			//Garantir que seja equipada caso nao seja arma
			scr_EP.EpType type = EPDictionary[name].getEpType();
			if(type != scr_EP.EpType.Arm)
				return equipPart(name);
			
			//Verifica em qual braço equipar caso não seja especificado
			if(arm == ArmToEquip.AnyArm) {
				if (currentRightArm == "None")
					arm = ArmToEquip.RightArm;
				else if (currentLeftArm == "None")
					arm = ArmToEquip.LeftArm;
			}

			//Remove a arma anterior
			if(arm == ArmToEquip.RightArm){
				if(currentRightArm != "None"){
					refRightArm.Unequip();
					Destroy(refRightArm.gameObject);
				}
			} else {
				if(currentLeftArm != "None"){
					refLeftArm.Unequip();
					Destroy(refLeftArm.gameObject);
				}
			}

			//Instancia o objeto
			Transform whereToSpawn;
			if(arm == ArmToEquip.RightArm)
				whereToSpawn = boneRightArm.Find("R.LowerArm").Find("R.Hand");
			else
				whereToSpawn = boneLeftArm.Find("L.LowerArm").Find("L.Hand");
			scr_EP instanciatedObject = GameObject.Instantiate(EPDictionary[name],whereToSpawn);
			scr_Weapon weaponScript = instanciatedObject.GetComponent<scr_Weapon>();
			weaponScript.setRightHand(arm == ArmToEquip.RightArm);
			weaponScript.Equip(gameObject);

			int mesh = 2 * instanciatedObject.getMeshId();
			if(flipped) {
				mesh++;
				weaponScript.flipHand();
			}

			if(arm == ArmToEquip.RightArm){
				currentRightArm = name;
				refRightArm = instanciatedObject;
				foreach(SpriteMeshAnimation sma in meshRightArm)
					sma.frame = mesh;
			} else {
				currentLeftArm = name;
				refLeftArm = instanciatedObject;
				foreach(SpriteMeshAnimation sma in meshLeftArm)
					sma.frame = mesh;
			}
		}
		return false;
	}

	/// <summary>
	/// Remove a parte especificada do corpo
	/// </summary>
	/// <param name="type">Tipo a ser removido. Para Braços, usar sobrecarga com o braço.</param>
	/// <returns></returns>
	public void removeBodyPart(scr_EP.EpType type){
		if(type == scr_EP.EpType.Arm){
			removeBodyPart(type, ArmToEquip.AnyArm);
			return ;
		}
		switch(type) {
			case scr_EP.EpType.Head:
				removePart(currentHead);
				break;
			case scr_EP.EpType.Torso:
				removePart(currentTorso);
				break;
			case scr_EP.EpType.Legs:
				removePart(currentLegs);
				break;
		}
	}

	/// <summary>
	/// Remove o braço especificado
	/// </summary>
	/// <param name="type">Tipo a ser removido, de preferencia Arm</param>
	/// <param name="arm">Qual o braço a ser removido. Não usar Any</param>
	/// <returns></returns>
	public void removeBodyPart(scr_EP.EpType type, ArmToEquip arm){
		if(type != scr_EP.EpType.Arm){
			removeBodyPart(type);
			return ;
		}
		switch(arm) {
			case ArmToEquip.AnyArm:
				Debug.LogWarning("Especificar qual braço quer remover");
				break ;
			case ArmToEquip.RightArm:
				removePart(currentRightArm, ArmToEquip.RightArm);
				break ;
			case ArmToEquip.LeftArm:
				removePart(currentLeftArm, ArmToEquip.LeftArm);
				break;
		}
	}

	/// <summary>
	/// Remove todas as partes equipadas
	/// </summary>
	public void removeAll(){
		if(currentHead != "None")
			removePart(currentHead);
		if(currentTorso != "None")
			removePart(currentTorso);
		if(currentLegs != "None")
			removePart(currentLegs);
		if(currentLeftArm != "None")
			removePart(currentLeftArm, ArmToEquip.LeftArm);
		if(currentRightArm != "None")
			removePart(currentRightArm, ArmToEquip.RightArm);
	}

	/// <summary>
	/// Remove a parte equipada com a key desejada
	/// </summary>
	/// <param name="name">Key da chave da EP a ser removida</param>
	public void removePart(string name) {
		//Avoid null reference
		if(name == "None")
			return ;
		if(currentHead == name){
			refHead.Unequip();
			Destroy(refHead.gameObject);
			currentHead = "None";
			meshHead.frame = 0;
		}
		else if(currentTorso == name){
			refTorso.Unequip();
			Destroy(refTorso.gameObject);
			currentTorso = "None";
			meshTorso.frame = 0;
		}
		else if(currentLegs == name){
			refLegs.Unequip();
			Destroy(refLegs.gameObject);
			currentLegs = "None";
			foreach(SpriteMeshAnimation sma in meshLegs)
				sma.frame = 0;
		}
		else if(currentRightArm == name) {
			refRightArm.Unequip();
			Destroy(refRightArm.gameObject);
			currentRightArm = "None";
			foreach(SpriteMeshAnimation sma in meshRightArm)
				sma.frame = 0;

		}
		else if(currentLeftArm == name){
			refLeftArm.Unequip();
			Destroy(refLeftArm.gameObject);
			currentLeftArm = "None";
			foreach(SpriteMeshAnimation sma in meshLeftArm)
				sma.frame = 0;
		}

	}

	/// <summary>
	/// Remover o braço especificado
	/// </summary>
	/// <param name="name">Chave da EP</param>
	/// <param name="arm">Braço a ser removido</param>
	public void removePart(string name, ArmToEquip arm) {
		//Avoid null reference
		if(name == "None")
			return ;

		if(arm == ArmToEquip.AnyArm) {
			if (currentRightArm == name)
				arm = ArmToEquip.RightArm;
			else if (currentLeftArm == name)
				arm = ArmToEquip.LeftArm;
		}
		if(arm == ArmToEquip.RightArm && currentRightArm == name) {
			refRightArm.Unequip();
			Destroy(refRightArm.gameObject);
			currentRightArm = "None";
			foreach(SpriteMeshAnimation sma in meshRightArm)
				sma.frame = 0;

		}
		else if(arm == ArmToEquip.LeftArm && currentLeftArm == name){
			refLeftArm.Unequip();
			Destroy(refLeftArm.gameObject);
			currentLeftArm = "None";
			foreach(SpriteMeshAnimation sma in meshLeftArm)
				sma.frame = 0;
		}
	}

	/**
	 * Método usado no pause do jogo, para o script das armas
	 */
	public void pauseWeaponScripts(bool isPause){
		if(refRightArm != null)
			refRightArm.GetComponent<scr_Weapon> ().enabled = isPause;
		if(refLeftArm != null)
			refLeftArm.GetComponent<scr_Weapon> ().enabled = isPause;
	}

}
