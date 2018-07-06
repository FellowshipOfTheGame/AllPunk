using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_MovingPlataform : MonoBehaviour {

    [Tooltip("Pontos pelo qual a plataforma vai passar")]
    public GameObject[] targetPoints;

    [Tooltip("Deve voltar ao ponto inicial e reiniciar o ciclo?")]
    public bool loop = true;
    [Tooltip("Velocidade da plataforma")]
    public float speed = 10;
    [Tooltip("A plataforma pode se mexer")]
    public bool canMove = true;
    [Tooltip("A plataforma deve levar o jogador (AINDA NAO IMPLEMENTADO)")]
    public bool shouldCarry = true;
    [Tooltip("Os tags do que a plataforma deve carregar")]
    public string[] carryTag = {"Player", "Enemy"};

    private List<Vector2> movingPoints;
    private Dictionary<Transform, Transform> previousParent;

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
        previousParent = new Dictionary<Transform, Transform>();
        movingPoints = new List<Vector2>();
        for(int i = 0; i < targetPoints.Length; i++){
            if(movingPoints != null)
            movingPoints.Add(targetPoints[i].transform.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Transform otherTrans = other.transform;
        bool canCarry = false;
        string tag  = other.gameObject.tag;
        foreach(string s in carryTag){
            if(s.Equals(tag)){
                canCarry = true;
                break;
            }
        }
        if(canCarry && !previousParent.ContainsKey(otherTrans)){
            previousParent.Add(otherTrans,otherTrans.parent);
            otherTrans.SetParent(myTransform);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        Transform otherTrans = other.transform;
        bool canCarry = false;
        string tag  = other.gameObject.tag;
        foreach(string s in carryTag){
            if(s.Equals(tag)){
                canCarry = true;
                break;
            }
        }
        if(canCarry && previousParent.ContainsKey(otherTrans)){
            otherTrans.parent = previousParent[otherTrans];
            previousParent.Remove(otherTrans);
        }
    }

    //Vai tentar mover a plataforma pelo caminho
    private void Update()
    {
        if (canMove && movingPoints.Count > 0) {

            //Caso seja o primeiro ponto
            if (currentIndex == -1)
            {
                currentIndex = 0;
                myTransform.position = movingPoints[0];
                findNewTarget();
            }

            myTransform.position += direction * speed * Time.deltaTime;

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
        if (targetIndex >= movingPoints.Count)
        {
            if (loop)
            {
                targetIndex = 0;
                direction = movingPoints[targetIndex] - movingPoints[currentIndex];
                direction.Normalize();
                //rb2d.velocity = direction * speed;
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
            //rb2d.velocity = direction * speed;
        }
        print("Mudou para cur: " + currentIndex + " , next: " + targetIndex);
    }

    //Desenha o caminho que a plataforma vai fazer na cena
    private void OnDrawGizmos()
    {
        for(int i = 0; i < targetPoints.Length; i++)
        {
            if(targetPoints[i] == null)
                continue;
            if (i + 1 < targetPoints.Length)
            {
                if(targetPoints[i + 1] == null)
                    continue;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(targetPoints[i].transform.position, targetPoints[i + 1].transform.position);
                if (i == 0 || i == targetPoints.Length-1)
                    Gizmos.color = Color.green;
            }
            else if(loop)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(targetPoints[i].transform.position, targetPoints[0].transform.position);
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(targetPoints[i].transform.position, 0.5f);
        }
    }


}
