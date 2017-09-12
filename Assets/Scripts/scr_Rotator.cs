using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Rotator : MonoBehaviour {

    public Vector3 rotationAxis = new Vector3(0f, 2f, 0f);
    public float rotationSpeed = 1f;
    private Transform myTransform;

    private void Start()
    {
        myTransform = transform;
        rotationAxis = rotationAxis.normalized;
    }


    // Update is called once per frame
    void Update () {
        Vector3 currentRotation = myTransform.localRotation.eulerAngles;
        Vector3 desiredRotation = currentRotation + rotationAxis * rotationSpeed * Time.deltaTime * Time.timeScale;
        myTransform.localRotation = Quaternion.Euler(desiredRotation);
	}
}
