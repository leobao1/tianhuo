using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellAOE
{

    private int damage;
    private int manaCost;
    private float duration;
    private float warningDuration;
    private Vector3 startPos;
    private Wizard caster;
    private Quaternion rotation;

    public GameObject aoe;

    public SpellAOE(int dmg, int mana, float wTime, float time, Vector3 start, GameObject spell, Wizard player)
    {
        damage = dmg;
        manaCost = mana;
        startPos = start;
        aoe = spell;
        caster = player;
        duration = time;
        warningDuration = wTime;
        rotation = new Quaternion(0, 0, 0, 1);
        Create();
    }


    void Create()
    {
        caster.DrainMana(manaCost);
        GameObject clone = GameObject.Instantiate(aoe, startPos, rotation);
        clone.GetComponent<AOEControl>().SetVariables(warningDuration, duration, damage, caster.gameObject);
    }







}
