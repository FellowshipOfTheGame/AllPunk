using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SpriteRenderer))]
public class scr_Background_Tiling : MonoBehaviour {

    //Offset de segurança
    public int offsetX = 2;
    public int offsetY = 2;
    
    //Se deve ou não fazer tile vertical
    public bool tileVertical = true;

    //Se possui um tile a esquerda e direita
    [HideInInspector]
    public bool hasRightBuddy = false;
    [HideInInspector]
    public bool hasLeftBuddy = false;
    [HideInInspector]
    public bool hasUpperBuddy = false;
    [HideInInspector]
    public bool hasLowerBuddy = false;
    public string backgroundName = "Back";

    //Se o objeto é escalável ou não
    public bool reverseScale = false;

    //Tamanho do sprite
    private float spriteWidth = 0f;
    private float spriteHeight = 0f;
    private Camera cam;
    private Transform myTransform;
    private int positionX = 0;
    private int positionY = 0;

	// Use this for initialization
	void Awake () {
        cam = Camera.main;
        myTransform = transform;
	}

    private void Start()
    {
        spriteWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        spriteHeight = GetComponent<SpriteRenderer>().sprite.bounds.size.y;
    }

    // Update is called once per frame
    void Update () {
        if (!hasLeftBuddy || !hasRightBuddy) {
            myTransform = transform;
            //Calcula a extensão (metade do comprimento) do que a câmera consegue ver
            //Cam.orthographicSize retorna a extensão vertical, então fazemos a conversão pelo ratio
            float camHorizontalExtend = cam.orthographicSize * Screen.width / Screen.height;

            //Calcula a posição que a câmera consegue ver a beirada do sprite
            float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth / 2) - camHorizontalExtend;
            float edgeVisiblePositionLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtend;

            if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && !hasRightBuddy)
            {
                CheckHasBuddy();
                if (!hasRightBuddy)
                    MakeNewHorizontalBuddy(1);
            }
            else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && !hasLeftBuddy) {
                CheckHasBuddy();
                if (!hasLeftBuddy)
                    MakeNewHorizontalBuddy(-1);
            }
        }
        if ((!hasUpperBuddy || !hasLowerBuddy) && tileVertical) {
            myTransform = transform;

            //Calcula a extensão vertical(metade da altura) do que a câmera consegue ver
            float camVerticalExtend = cam.orthographicSize;

            //Calcula a posição que a câmera consegue ver a beirada do sprite
            float edgeVisiblePositionUp = (myTransform.position.y + spriteHeight / 2) - camVerticalExtend;
            float edgeVisiblePositionDown = (myTransform.position.y - spriteHeight / 2) + camVerticalExtend;

            if (cam.transform.position.y >= edgeVisiblePositionUp - offsetY && !hasUpperBuddy)
            {
                CheckHasBuddy();
                if(!hasUpperBuddy)
                    MakeNewVerticalBuddy(1);
            }
            else if (cam.transform.position.y <= edgeVisiblePositionDown + offsetY && !hasLowerBuddy)
            {
                CheckHasBuddy();
                if (!hasLowerBuddy)
                    MakeNewVerticalBuddy(-1);
            }
        }
	}

    void CheckHasBuddy()
    {
        foreach(Transform child in transform.parent)
        {
            string childBackground = child.name.Split('_')[0];
            if (childBackground.Equals(backgroundName))
            {
                string positions = child.name.Split('_')[1];
                int childPositionX = int.Parse(positions.Split(',')[0]);
                int childPositionY = int.Parse(positions.Split(',')[1]);
                if (childPositionX == positionX + 1 && childPositionY == positionY)
                    hasRightBuddy = true;
                else if (childPositionX == positionX - 1 && childPositionY == positionY)
                    hasLeftBuddy = true;
                else if (childPositionX == positionX && childPositionY == positionY + 1)
                    hasUpperBuddy = true;
                else if (childPositionX == positionX && childPositionY == positionY - 1)
                    hasLowerBuddy = true;
            }
        }
    }

    //Cria um novo sprite à direita (1) ou a esquerda(-1)
    void MakeNewHorizontalBuddy(int rightOrLeft) {
        //Posição do companheiro
        Vector3 newPosition = new Vector3(myTransform.position.x + spriteWidth* rightOrLeft, myTransform.position.y, myTransform.position.z);
        Transform newBuddy = Instantiate(myTransform, newPosition, myTransform.rotation) as Transform;

        //Se não for escalavel, inverter o sprite para conectar
        if (reverseScale == true) {
            newBuddy.localScale = new Vector3(myTransform.localScale.x * -1, myTransform.localScale.y, myTransform.localScale.z);
        }

        newBuddy.parent = myTransform.parent;
        scr_Background_Tiling buddyTiling = newBuddy.GetComponent<scr_Background_Tiling>();
        if (rightOrLeft > 0)
        {
            buddyTiling.hasLeftBuddy = true;
            hasRightBuddy = true;
            buddyTiling.setPosition(positionX + 1, positionY);
        }
        else {
            buddyTiling.hasRightBuddy = true;
            hasLeftBuddy = true;
            buddyTiling.setPosition(positionX - 1, positionY);
        }

        int orderInLayer = GetComponent<SpriteRenderer>().sortingOrder;
        newBuddy.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
    }

    //Cria um novo sprite à cima (1) ou a baixo(-1)
    void MakeNewVerticalBuddy(int upOrDown)
    {
        //Posição do companheiro
        Vector3 newPosition = new Vector3(myTransform.position.x, myTransform.position.y + spriteHeight * upOrDown, myTransform.position.z);
        Transform newBuddy = Instantiate(myTransform, newPosition, myTransform.rotation) as Transform;

        //Se não for escalavel, inverter o sprite para conectar
        if (reverseScale == true)
        {
            newBuddy.localScale = new Vector3(myTransform.localScale.x, myTransform.localScale.y * -1, myTransform.localScale.z);
        }

        newBuddy.parent = myTransform.parent;
        scr_Background_Tiling buddyTiling = newBuddy.GetComponent<scr_Background_Tiling>();
        if (upOrDown > 0)
        {
            buddyTiling.hasLowerBuddy = true;
            hasUpperBuddy = true;
            buddyTiling.setPosition(positionX, positionY + 1);
        }
        else
        {
            buddyTiling.hasUpperBuddy = true;
            hasLowerBuddy = true;
            buddyTiling.setPosition(positionX, positionY - 1);
        }

        int orderInLayer = GetComponent<SpriteRenderer>().sortingOrder;
        newBuddy.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
    }

    public void setPosition(int x, int y) {
        positionX = x;
        positionY = y;
        gameObject.name = backgroundName + '_' + x + ',' + y;
    }
}
