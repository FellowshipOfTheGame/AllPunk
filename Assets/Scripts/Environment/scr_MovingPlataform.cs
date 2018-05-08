﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_MovingPlataform : MonoBehaviour {

    [Tooltip("Pontos pelo qual a plataforma vai passar")]
    public Vector2[] movingPoints;
    [Tooltip("Deve voltar ao ponto inicial e reiniciar o ciclo?")]
    public bool loop = true;
    [Tooltip("Velocidade da plataforma")]
    public float speed = 10;
    [Tooltip("A plataforma pode se mexer")]
    public bool canMove = true;
    [Tooltip("A plataforma deve levar o jogador (AINDA NAO IMPLEMENTADO)")]
    public bool shouldCarry = true;


    private Transform myTransform;
    private Vector3 direction;
    private List<float> distances;
    private float totalDistance;
    private int currentIndex = -1;
    private int targetIndex = -1;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        myTransform = transform;
        rb2d = GetComponent<Rigidbody2D>();
    }

    //Vai tentar mover a plataforma pelo caminho
    private void Update()
    {
        if (canMove && movingPoints.Length > 0) {

            //Caso seja o primeiro ponto
            if (currentIndex == -1)
            {
                currentIndex = 0;
                myTransform.position = movingPoints[0];
                findNewTarget();
            }

            //Verifica se passou do ponto
            Vector3 target = new Vector3(movingPoints[targetIndex].x, movingPoints[targetIndex].y, myTransform.position.z);
            Vector3 diference = target - myTransform.position;
            //Produto escalar
            if(Vector3.Dot(diference, direction) <= 0)
            {
                currentIndex = targetIndex;
                findNewTarget();
            }

        }
    }

    /// <summary>
    /// Encontra o próximo ponto ao qual tem que ir e seta as variáveis necessárias
    /// </summary>
    private void findNewTarget()
    {
        targetIndex = currentIndex + 1;
        if (targetIndex >= movingPoints.Length)
        {
            if (loop)
            {
                targetIndex = 0;
                direction = movingPoints[targetIndex] - movingPoints[currentIndex];
                direction.Normalize();
                rb2d.velocity = direction * speed;
            }
            else
            {
                myTransform.position = movingPoints[targetIndex - 1];
                targetIndex = -1;
                canMove = false;
                direction = Vector2.zero;
                rb2d.velocity = Vector2.zero;
            }
        }
        else
        {
            myTransform.position = movingPoints[targetIndex - 1];
            direction = movingPoints[targetIndex] - movingPoints[currentIndex];
            direction.Normalize();
            rb2d.velocity = direction * speed;
        }
        print("Mudou para cur: " + currentIndex + " , next: " + targetIndex);
    }

    //Desenha o caminho que a plataforma vai fazer na cena
    private void OnDrawGizmos()
    {
        for(int i = 0; i < movingPoints.Length; i++)
        {
            if (i + 1 < movingPoints.Length)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(movingPoints[i], movingPoints[i + 1]);
                if (i == 0 || i == movingPoints.Length-1)
                    Gizmos.color = Color.green;
            }
            else if(loop)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(movingPoints[i], movingPoints[0]);
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(movingPoints[i], 0.5f);
        }
    }


}
