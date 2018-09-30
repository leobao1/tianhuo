using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEControl : MonoBehaviour {

    float duration;
    float warningDuration;
    float currentTime = 0;
    int damage;
    private GameObject caster;
    bool damaged;
    bool warning;
    public Sprite[] sprites;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && !other.gameObject.Equals(caster) && !damaged & warning){
            other.GetComponent<Wizard>().Damage(damage);
            other.GetComponent<Wizard>().SetHasBeenHit(true);
            damaged = true;
            Debug.Log("damaged");
        }
    }

    public void SetVariables(float wTime, float time, int dmg, GameObject player)
    {
        duration = time;
        warningDuration = wTime;
        damage = dmg;
        caster = player;
        damaged = false;
        warning = false;
        GetComponent<SpriteRenderer>().sprite = sprites[0];
    }

    private void FixedUpdate() { 


        if (!warning)
        {
            warningDuration -= Time.fixedDeltaTime;
            if (warningDuration <= 0)
            {
                warning = true;
                GetComponent<SpriteRenderer>().sprite = sprites[1];
            }
        } else
        {
            duration -= Time.fixedDeltaTime;
            if (duration <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
