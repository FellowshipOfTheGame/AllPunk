using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_EnemyDropItem : MonoBehaviour {

	[Range(0,1)]
	[Tooltip("Chance de derrubar item")]
	public float[] dropChance;
	[Tooltip("Prefabs dos itens, seguindo a ordem")]
	public GameObject[] prefabs;
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
		float random =  UnityEngine.Random.Range(0,1);
		float aux = 0;
		for(int i = 0; i < dropChance.Length; i++){
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
}
