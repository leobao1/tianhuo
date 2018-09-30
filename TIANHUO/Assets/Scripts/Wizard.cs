using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour {

    private Rigidbody2D rb; // The Physics object for the wizard

    private Vector2 velocity = new Vector2();

    int health;
    int mana;
    float move_speed;
    float projectile_speed;
    int current_regen;
    bool has_been_hit_since_check = false;
    // States as floats - any number zero or smaller indicates it is in that state 
    float stunned; // Can take damage, can't do anything
    float eulsd; // Can't take damage, can't do anything, no collider
    float invulnerable; // Can't take damage, can do things

    // States
    bool alive;

    const float deltaTime = 0.02f;
    const int maxHP = 100;
    const int maxMana = 100;
    const int maxBalls = 3;
    const int default_move_speed = 50;
    const int BASE_MANA_REGEN = 3;
    const int MANA_REGEN_STEP = 0;

    int[] balls = { 0, 0, 0 };

    private void Awake()
    {
        // Initialize components
        rb = GetComponent<Rigidbody2D>();

        // Set stats
        health = maxHP;
        mana = maxMana;
        move_speed = default_move_speed;

        current_regen = BASE_MANA_REGEN;
        stunned = 0;
        eulsd = 0;
        invulnerable = 0;
        alive = true;

        // 0 - Nothing 
        // 1 - Quas (Ice) 
        // 2 - Wex (Wind)
        // 3 - Exort (Fire)
        balls = new int[3] { 0, 0, 0 }; // this int array will function like a queue that always keeps track of head 
    }
    // GET FUNCTIONS

    public Rigidbody2D getRigidBody() {
        return rb;
    }

    public bool IsAlive()
    {
        return alive;
    }

    public Vector2 GetPosition()
    {
        return rb.position;
    }

    public int[] GetBalls()
    {
        return balls;
    }

    public int GetMana()
    {
        return mana;
    }

    public void SetMana(){
        mana += current_regen;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetManaRegen(bool reset){
        if (!reset) {
            current_regen += MANA_REGEN_STEP;
        }else{
            current_regen = BASE_MANA_REGEN;
        }
    }

    public void SetProjectileSpeed(float speed){
        projectile_speed = speed;
    }

    public float GetProjectileSpeed(){
        return projectile_speed;
    }

    public void SetMoveSpeed(float speed){
        move_speed = speed;
    }

    public float GetMoveSpeed(){
        return move_speed;
    }

    public bool GetHasBeenHit(){
        return has_been_hit_since_check;
    }

    public void SetHasBeenHit(bool state){
        has_been_hit_since_check = state;
    }
    /// <summary>
    /// Return the number of balls of that specific type
    /// </summary>
    /// <param name="type">  0 - Nothing , 1 - Quas (Ice) , 2 - Wex (Wind) , 3 - Exort (Fire) </param>
    /// <returns></returns>
    public int GetBallTypeCount(int type)
    {
        int count = 0; 
        for (int i = 0; i < maxBalls; i++)
        {
            if (balls[i] == type) count++;
        }
        return count;
    }

    // OTHERFUNCTIONS 

    /// <summary>
    /// Call this function every frame
    /// </summary>
    public void Countdown()
    {
        // Count down states
        if (stunned > 0) stunned -= deltaTime;
        if (eulsd > 0) eulsd -= deltaTime;
        if (invulnerable > 0) invulnerable -= deltaTime;
        
        // Checks 
        if (stunned < 0) stunned = 0;
        if (eulsd < 0) eulsd = 0;
        if (invulnerable < 0) invulnerable = 0;

        if (health > maxHP) health = maxHP;
        if (mana > maxMana) mana = maxMana;

        if (health <= 0) alive = false; 

        // Refresh mana
    }

    public bool Move(Vector2 velocity_direction)
    {
        if (stunned == 0 && eulsd == 0)
        {
            velocity = velocity_direction * move_speed;
            rb.velocity = velocity;
            return true;
        }
        else
        {
            rb.velocity = Vector2.zero;
            return false;
        }
    }

    /// <summary>
    /// Note: Negative damage = healing
    /// </summary>
    /// <param name="dmg"></param>
    public void Damage(int dmg)
    {
        health -= dmg;
    }
    public void DamagePercent(float dmg)
    {
        int damageDealt = System.Convert.ToInt32(System.Math.Round(health * dmg));
        health -= damageDealt; 
    }
    public void DrainMana(int numMana)
    {
        mana -= numMana;
    }
    public void ChangeMoveSpeed(float newSpeed)
    {
        move_speed = newSpeed;
    }
    public void ResetMoveSpeed()
    {
        move_speed = default_move_speed;
    }

    public void AddBall(int ball)
    {
        // Shift every ball to the right, remove last ball
        balls[2] = balls[1];
        balls[1] = balls[0];
        balls[0] = ball; 
    }
    public void RemoveBalls()
    {
        balls[0] = 0;
        balls[1] = 0;
        balls[2] = 0;
    }

}
