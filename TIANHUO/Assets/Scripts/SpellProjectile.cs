using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile {

    private int damage;
    private int spriteType;
    private int manaCost;
    private float projSpeed;
    private Vector3 direction;
    private Vector3 startPos;
    private Wizard caster;
    private Quaternion rotation;

    public GameObject projectile;

    /* For type integer:
     * 0: Fireball
     * 1: BullshitArrow
     * 2: DeafeningBlast
     * 3: IceBullet*/
    public SpellProjectile(int dmg, float speed, Vector3 direc, Vector3 start, GameObject proj, Wizard player, int type, int mana)
    {
        damage = dmg;
        projSpeed = speed;
        direction = direc;
        startPos = start;
        spriteType = type;
        manaCost = mana;
        projectile = proj;
        caster = player;
        Create();
    }


    void Create()
    {
        caster.DrainMana(manaCost);
        Debug.Log(caster.GetMana());
        GameObject clone = GameObject.Instantiate(projectile);
        SetAngle(direction.x, direction.y, clone);
        clone.transform.position = startPos;
        clone.GetComponent<Rigidbody2D>().AddForce(direction * projSpeed);
        clone.GetComponent<DestroyOnCollision>().SetVariables(damage, caster.gameObject, spriteType);
    }

    void SetAngle(float x, float y, GameObject clone)
    {
        if (x == 0)
        {
            clone.transform.eulerAngles = new Vector3(clone.transform.eulerAngles.x, clone.transform.eulerAngles.y,
                (direction.y > 0) ? 90 : 270);
        }
        else if (y == 0)
        {
            clone.transform.eulerAngles = new Vector3(clone.transform.eulerAngles.x, clone.transform.eulerAngles.y,
                (direction.x > 0) ? 0 : 180);
        }
        else if (y > 0 && x > 0)
        {
            clone.transform.eulerAngles = new Vector3(clone.transform.eulerAngles.x, clone.transform.eulerAngles.y,
                clone.transform.eulerAngles.z + Mathf.Rad2Deg*Mathf.Atan(y/x));
        }
        else if (y > 0 && x < 0)
        {
            clone.transform.eulerAngles = new Vector3(clone.transform.eulerAngles.x, clone.transform.eulerAngles.y,
                clone.transform.eulerAngles.z + Mathf.Rad2Deg * Mathf.Atan(y / x) + 180);
        }
        else if (y < 0 && x < 0)
        {
            clone.transform.eulerAngles = new Vector3(clone.transform.eulerAngles.x, clone.transform.eulerAngles.y,
                clone.transform.eulerAngles.z + Mathf.Rad2Deg * Mathf.Atan(y / x) + 180);
        }
        else if (y < 0 && x > 0)
        {
            clone.transform.eulerAngles = new Vector3(clone.transform.eulerAngles.x, clone.transform.eulerAngles.y,
                clone.transform.eulerAngles.z + Mathf.Rad2Deg * Mathf.Atan(y / x));
        }
    }







}
