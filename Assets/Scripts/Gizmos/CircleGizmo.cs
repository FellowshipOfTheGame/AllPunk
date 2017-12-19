using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGizmo : MonoBehaviour {

	public float radius = 1.0f;
	public Color color = Color.white;

	void OnDrawGizmos(){
		Gizmos.color = color;
		Gizmos.DrawSphere (transform.position, radius);
	}
}
