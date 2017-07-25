using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Weapon_Sword : scr_Weapon {

    public float damage;
    public float attackImpulse;

    //Update function for UpAttack
    override protected void AttackAction(bool noAnimation)
    {
        if (clicked && noAnimation)
        {
            if (animator != null)
                animator.SetBool("Attack", true);
            print("Atacou");
        }
        //Verify sword collider for hits
        if (!noAnimation)
        {
            Collider2D[] hits = new Collider2D[10];
            Collider2D collider = GetComponent<Collider2D>();
            ContactFilter2D ct2D = new ContactFilter2D();
            collider.OverlapCollider(ct2D, hits);
            foreach (Collider2D col in hits)
            {
                if (col == null)
                    continue;
                if (col.gameObject.tag == "Player")
                    continue;
                scr_Entity entity = col.GetComponent<scr_Entity>();
                if (entity != null)
                {
                    float xComponent = ((col.transform.position - transform.position).x > 0) ? 1 : -1;
                    Vector2 direction = new Vector2(xComponent, 0);
                    entity.takeDamage(damage, direction * attackImpulse);
                }
            }
        }
    }
}
