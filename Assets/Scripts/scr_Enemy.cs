using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Enemy : scr_Entity {

	protected override void die(){
		Destroy(this.gameObject);
	}

}
