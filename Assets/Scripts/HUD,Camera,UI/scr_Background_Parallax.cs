using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Background_Parallax : MonoBehaviour {
    
    //Planos de fundo que vão ser movimentados;
    public Transform[] backgrounds;
    //Quão rápido os planos vão se mexer
    public float smothing = 1f;
    //Escala de o quanto deve ser movimentado
    private float[] parallaxScales;

    private Transform cam;
    private Vector3 previousCamPosition;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    // Use this for initialization
    void Start () {
        previousCamPosition = cam.position;

        parallaxScales = new float[backgrounds.Length];

        //Definindo as escalas
        for(int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z*-1;
        }
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < backgrounds.Length; i++)
        {
            //O paralax vai em sentido contrário à câmera
            float parallax = (previousCamPosition.x - cam.position.x) * parallaxScales[i];

            //Define a posição x do background
            float backgroundTargetX = backgrounds[i].position.x + parallax;
            //Define o vetor posição do background
            Vector3 newPosition = new Vector3(backgroundTargetX, backgrounds[i].position.y, backgrounds[i].position.z);

            //Suavização
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, newPosition, smothing * Time.deltaTime);
        }

        //Atualiza posição da camera
        previousCamPosition = cam.position;
	}
}
