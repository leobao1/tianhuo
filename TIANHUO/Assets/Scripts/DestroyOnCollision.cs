using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{

    private int damage;
    private int spriteType;
    private GameObject caster;
    public Sprite[] sprites;

    public main main;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Wall")
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !other.gameObject.Equals(caster))
        {
            other.GetComponent<Wizard>().Damage(damage);
            other.GetComponent<Wizard>().SetHasBeenHit(true);
            Destroy(this.gameObject);
        }
    }

        

    public void SetVariables(int dmg, GameObject player, int type)
    {
        damage = dmg;
        caster = player;
        spriteType = type;
        SetSprite();
    }

    public void SetSprite()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[spriteType];
    }
}
