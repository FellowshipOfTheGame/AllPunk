using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class scr_Weapon : MonoBehaviour {

    /**
     * Enumerador dos tipos de ataque
     * 
     */
    public enum AttackType
    {
        UpAttack,
        ThrustAttack,
        RangedAttack
    };

    #region Variables

    [HideInInspector]
    public bool rightHand;
    //A arma deve seguir o mouse
    public bool followMouse;
    //Como é o ataque utilizado
    public AttackType attackType;

    protected GameObject ik;
    protected Animator animator;
    protected SpriteRenderer sprite;
    protected int frontLayer;
    protected int backLayer;
    protected bool clicked;
    protected bool holding;

    #endregion Variables

    /**
     * Obter os componentes utilizados
     */
    protected void Awake()
    {
        this.sprite = GetComponent<SpriteRenderer>();
        ik = null;
        animator = null;
    }

    protected void Update()
    {
        //Move o IK para a posição do mouse    
        if (followMouse && ik != null) {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = transform.position.z;

            ik.transform.SetPositionAndRotation(mouseWorldPosition, ik.transform.rotation);
        }


        //Verifica se está sendo realizado alguma animação de ataque
        bool noAnimation = false;
        if (animator != null)
        {
            int animLayer = (rightHand) ? 1 : 2;
            noAnimation = animator.GetCurrentAnimatorStateInfo(animLayer).IsName("Moving");
            if (noAnimation)
            {
                animator.SetBool("Attack", false);
            }
        }


        //Define se está havendo algum input
        int mouseIndex = (rightHand) ? 1 : 0;
        clicked = Input.GetMouseButtonDown(mouseIndex);
        holding = Input.GetMouseButton(mouseIndex);

        //Chama a função específica de cada arma
        AttackAction(noAnimation);

    }


    /**
     * Função específica de cada arma. Recebe como parâmetro um booleano indicando se
     * tem alguma animação sendo realizada ou não
     * 
     */
    abstract protected void AttackAction(bool noAnimation);

    public void setIK(GameObject ik) {
        this.ik = ik;
    }

    public void setAnimator(Animator animator)
    {
        this.animator = animator;

        //Seleciona qual animação vai ser utilizada
        switch (attackType)
        {
            case AttackType.UpAttack:
                animator.SetBool("UpAttack", true);
                break;
            case AttackType.ThrustAttack:

                break;
            case AttackType.RangedAttack:
                animator.SetBool("GunRecoil", true);
                break;
            default:

                break;
        }
    }

    public void setRightHand(bool RightHand) {
        this.rightHand = RightHand;
    }

    public void setSpriteLayer(int frontLayer, int backLayer) {
        this.frontLayer = frontLayer;
        this.backLayer = backLayer;
    }

    /**
     * Troca os sprites a serem usados
     */
    public void flipHand()
    {
        if (sprite.sortingLayerID == backLayer)
            sprite.sortingLayerID = frontLayer;
        else
            sprite.sortingLayerID = backLayer;
    }
}
