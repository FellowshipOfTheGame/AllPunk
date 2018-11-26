using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyDropItem : MonoBehaviour {

	[Range(0,1)]
	[Tooltip("Chance de derrubar item")]
	public float[] dropChance;
	[Tooltip("Prefabs dos itens, seguindo a ordem")]
	public GameObject[] prefabs;
	[Tooltip("Nome da parte necessária para dropar")]
	public string[] requirePart;
	[Tooltip("Deve matar o personagem depois de spawnar item")]
	public bool shouldKill = true;


	// Use this for initialization
	void Start () {
		scr_HealthController health = GetComponent<scr_HealthController>();
		if(health != null) {
			health.addDeathCallback(onDeath);
		}
	}
	
	private void onDeath(){
		float random =  UnityEngine.Random.Range(0.0f,1.0f);
		float aux = 0;
		for(int i = 0; i < dropChance.Length; i++){
			if(!hasPart(requirePart[i]))
				continue;
			aux += dropChance[i];
			if(random <= aux){
				spawnPrefab(prefabs[i]);
				break;
			}
		}
		if(shouldKill)
			Destroy(gameObject);
	}

	private void spawnPrefab(GameObject prefab){
		GameObject.Instantiate(prefab, transform.position, Quaternion.identity);
	}

	private bool hasPart(string part){
		if(part == "")
			return true;
		if(scr_GameManager.instance != null) {
			if(scr_GameManager.instance.player != null) {
				scr_EPManager epMan = scr_GameManager.instance.player.GetComponent<scr_EPManager>();
				return epMan.hasEquipped(part);

			}
		}
		return false;
	}
}
