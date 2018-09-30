using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main : MonoBehaviour {

    public GameObject projectile;
    public GameObject AOE;

    public Vector2 input_direction_1 = new Vector2();
    public Vector2 input_direction_2 = new Vector2();


    public Wizard player1;
    public Wizard player2;
    public UI ui;

    // CONSTANTS FOR MOVEMENT
    const string HORIZONTAL1 = "Horizontal1";
    const string HORIZONTAL2 = "Horizontal2";
    const string VERTICAL1 = "Vertical1";
    const string VERTICAL2 = "Vertical2";

    // CONSTANTS FOR KEY BINDINGS
    const KeyCode KEY_QUAS_1 = KeyCode.Alpha1;
    const KeyCode KEY_WEX_1 = KeyCode.Alpha2;
    const KeyCode KEY_EXORT_1 = KeyCode.Alpha3;
    const KeyCode KEY_INVOKE_1 = KeyCode.Alpha4;
    const KeyCode KEY_RESET_1 = KeyCode.BackQuote;

    const KeyCode KEY_QUAS_2 = KeyCode.M;
    const KeyCode KEY_WEX_2 = KeyCode.Comma;
    const KeyCode KEY_EXORT_2 = KeyCode.Period;
    const KeyCode KEY_INVOKE_2 = KeyCode.Slash;
    const KeyCode KEY_RESET_2 = KeyCode.N;

    // CONSTANTS FOR MANA COSTS
    const int COST_SMALL = 5;
    const int COST_MED = 20;
    const int COST_LARGE = 50;
    const float TIME_TO_NEXT_MANA_STEP = 1f;
    const float TIME_TO_NEXT_MANA_REGEN_STEP = 5f;
    const float TIME_BETWEEN_HIT_CHECKS = 0.1f;
    // UNORDERED CONSTANTS FOR SPELLS 
    readonly int[] RECIPE_FIREBALL = { 3, 0, 0 };
    readonly int[] RECIPE_ICESHOT = { 2, 0, 0 };
    readonly int[] RECIPE_SUNSTRIKE = { 1, 0, 0 };

    readonly int[] RECIPE_SPREAD = { 3, 3, 0 };
    readonly int[] RECIPE_ICESPREAD = { 2, 2, 0 };
    readonly int[] RECIPE_CATACLYSM = { 1, 1, 0 };

    readonly int[] RECIPE_DOOMSDAY = { 1, 1, 1 };
    readonly int[] RECIPE_BIGSPREAD = { 3, 3, 3 };
    readonly int[] RECIPE_BIGICESPREAD = { 2, 2, 2 };
    readonly int[] RECIPE_DEAFBLAST = { 1, 2, 3 };

    int[][] recipes;

    int p1Dir = 0;
    int p2Dir = 0;
    int p1Diry = 0;
    int p2Diry = 0;

    float time_since_last_mana_regen_step = 0f;
    float time_since_last_mana_step = 0f;
    float time_since_last_hit_check = 0f;

    // Use this for initialization
    void Awake () {
    }

    private void Start() {
        player1.SetProjectileSpeed(6000f);
        player2.SetProjectileSpeed(1000f);
        player1.SetMoveSpeed(50f);
        player2.SetMoveSpeed(75f);
    }

    // FixedUpdate is called once per physics step (0.02) sec
    void FixedUpdate()
    {
        time_since_last_mana_regen_step += Time.fixedDeltaTime;
        time_since_last_mana_step += Time.fixedDeltaTime;
        time_since_last_hit_check += Time.fixedDeltaTime;
        //Increase mana regen over time
        if(time_since_last_mana_regen_step > TIME_TO_NEXT_MANA_REGEN_STEP){
            player1.SetManaRegen(false);
            player2.SetManaRegen(false);
            time_since_last_mana_regen_step = 0f;
        }
        //Increase mana over time
        if(time_since_last_mana_step > TIME_TO_NEXT_MANA_STEP){
            player1.SetMana();
            player2.SetMana();
            time_since_last_mana_step = 0f;
            ui.change_mana(player1.GetMana(), true);
            ui.change_mana(player2.GetMana(), false);
        }

        if(time_since_last_hit_check >= TIME_BETWEEN_HIT_CHECKS){
            if(player1.GetHasBeenHit()){
                ui.change_health(player1.GetHealth(), true);
                player1.SetHasBeenHit(false);
            }
            if(player2.GetHasBeenHit()){
                ui.change_health(player2.GetHealth(), false);
                player2.SetHasBeenHit(false);
            }
        }
        // UPDATE OBJECTS
        player1.Countdown();
        player2.Countdown();

        // PLAYER MOVEMENT
        if (Input.GetAxis(HORIZONTAL1) != 0f || Input.GetAxis(VERTICAL1) != 0f){
            player1.GetComponent<Animator>().Play("walkAnimate");
        } else
        {
            player1.GetComponent<Animator>().Play("idleAnimate");
        }


        if (Input.GetAxis(HORIZONTAL2) != 0f || Input.GetAxis(VERTICAL2) != 0f)
        {
            player2.GetComponent<Animator>().Play("walkAnimate");
        }
        else
        {
            player2.GetComponent<Animator>().Play("idleAnimate");
        }

        input_direction_1.x = Input.GetAxis(HORIZONTAL1);
        input_direction_1.y = Input.GetAxis(VERTICAL1);
        input_direction_1.Normalize();

        input_direction_2.x = Input.GetAxis(HORIZONTAL2);
        input_direction_2.y = Input.GetAxis(VERTICAL2);
        input_direction_2.Normalize();


        GetShootingDirection(player1.GetPosition(), player2.GetPosition());
        player1.Move(input_direction_1);
        player2.Move(input_direction_2);

        // PLAYER SPELLS 
        if (Input.anyKeyDown)
        {
            // QUAS, WEX, EXORT
            if (Input.GetKeyDown(KEY_QUAS_1)) MakeBall(1, 1);
            else if (Input.GetKeyDown(KEY_WEX_1)) MakeBall(1, 2);
            else if (Input.GetKeyDown(KEY_EXORT_1)) MakeBall(1, 3);

            else if (Input.GetKeyDown(KEY_QUAS_2)) MakeBall(2, 1);
            else if (Input.GetKeyDown(KEY_WEX_2)) MakeBall(2, 2);
            else if (Input.GetKeyDown(KEY_EXORT_2)) MakeBall(2, 3);

            // INVOKE SPELLS
            else if (Input.GetKeyDown(KEY_INVOKE_1)) Invoke(1);
            else if (Input.GetKeyDown(KEY_INVOKE_2)) Invoke(2);

            // RESET BALLS
            else if (Input.GetKeyDown(KEY_RESET_1)) ResetBalls(1);
            else if (Input.GetKeyDown(KEY_RESET_2)) ResetBalls(2);
        }


    }

    void MakeBall(int playerNumber, int ballType)
    {
        Debug.Log("Added ball: " + ballType.ToString());
        Debug.Log(player1.GetBalls()[0].ToString() + ", " + player1.GetBalls()[1].ToString());
        if (playerNumber == 1)
        {
            player1.AddBall(ballType);
            // (Display on UI as well)
            ui.change_spell_sprite(true, player1.GetBalls());
        }
        else
        {
            player2.AddBall(ballType);
            // (Display on UI as well)
            ui.change_spell_sprite(false, player2.GetBalls());
        }
    }

    /*For the last argument in the SpellProjectile constructor:
     * 0: Fireball
     * 1: Arrow
     * 2: DeafeningBlast
     * 3: IceBullet*/
    void Invoke(int playerNumber)
    {
        if (playerNumber == 1)
        {
            if (CompareRecipes(player1.GetBalls(), RECIPE_FIREBALL))
            {
                CastFireball(player1, p1Dir);
                ui.change_mana(player1.GetMana(), true);
            }
            else if (CompareRecipes(player1.GetBalls(), RECIPE_ICESHOT))
            {
                CastIceBlast(player1, p1Diry);
                ui.change_mana(player1.GetMana(), true);
            }
            else if (CompareRecipes(player1.GetBalls(), RECIPE_SPREAD))
            {
                if (player1.GetMana() >= COST_MED)
                {
                    SpellProjectile newProj = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(p1Dir * 1, 0), player1.GetPosition(), projectile, player1, 0,
                                                                            COST_MED);
                    SpellProjectile newProj1 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(p1Dir * 1, 0.1f), player1.GetPosition(), projectile, player1, 0,
                                                                                0);
                    SpellProjectile newProj2 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(p1Dir * 1, -0.1f), player1.GetPosition(), projectile, player1, 0,
                                                                                0);
                }

            }
            else if (CompareRecipes(player1.GetBalls(), RECIPE_ICESPREAD))
            {
                if (player1.GetMana() >= COST_MED)
                {
                    SpellProjectile newProj = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(0, p1Diry * 1), player1.GetPosition(), projectile, player1, 3,
                                                                            COST_MED);
                    SpellProjectile newProj1 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(0.1f, p1Diry * 1), player1.GetPosition(), projectile, player1, 3,
                                                                                0);
                    SpellProjectile newProj2 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(-0.1f, p1Diry * 1), player1.GetPosition(), projectile, player1, 3,
                                                                                0);
                }

            }
            else if (CompareRecipes(player1.GetBalls(), RECIPE_DEAFBLAST))
            {
                if (player1.GetMana() >= COST_LARGE)
                {

                    SpellProjectile newProj = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(1, 0), player1.GetPosition()
                                                                       , projectile, player1, 2, COST_LARGE);
                    SpellProjectile newProj2 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(0, 1), player1.GetPosition()
                                                                       , projectile, player1, 2, 0);
                    SpellProjectile newProj3 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(-1, 0), player1.GetPosition()
                                                                       , projectile, player1, 2, 0);
                    SpellProjectile newProj4 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(0, -1), player1.GetPosition()
                                                                       , projectile, player1, 2, 0);
                    SpellProjectile newProj5 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(1, 1).normalized, player1.GetPosition()
                                                                       , projectile, player1, 2, 0);
                    SpellProjectile newProj6 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(-1, 1).normalized, player1.GetPosition()
                                                                       , projectile, player1, 2, 0);
                    SpellProjectile newProj7 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(-1, -1).normalized, player1.GetPosition()
                                                                       , projectile, player1, 2, 0);
                    SpellProjectile newProj8 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(1, -1).normalized, player1.GetPosition()
                                                                       , projectile, player1, 2, 0);
                    ui.change_mana(player1.GetMana(), true);
                }
            }
            else if (CompareRecipes(player1.GetBalls(), RECIPE_BIGSPREAD))
            {
                if (player1.GetMana() >= COST_LARGE)
                {
                    SpellProjectile newProj = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(p1Dir * 1, 0), player1.GetPosition(), projectile, player1, 0,
                                                                            COST_LARGE);
                    SpellProjectile newProj1 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(p1Dir * 1, 0.1f), player1.GetPosition(), projectile, player1, 0,
                                                                                0);
                    SpellProjectile newProj2 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(p1Dir * 1, -0.1f), player1.GetPosition(), projectile, player1, 0,
                                                                                0);
                    SpellProjectile newProj3 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(p1Dir * 1, 0.05f), player1.GetPosition(), projectile, player1, 0,
                                                                                0);
                    SpellProjectile newProj4 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(p1Dir * 1, -0.05f), player1.GetPosition(), projectile, player1, 0,
                                                                                0);
                }

            }
            else if (CompareRecipes(player1.GetBalls(), RECIPE_BIGICESPREAD))
            {
                if (player1.GetMana() >= COST_LARGE)
                {
                    SpellProjectile newProj = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(0, p1Diry * 1), player1.GetPosition(), projectile, player1, 3,
                                                                            COST_LARGE);
                    SpellProjectile newProj1 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(0.1f, p1Diry * 1), player1.GetPosition(), projectile, player1, 3,
                                                                                0);
                    SpellProjectile newProj2 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(-0.1f, p1Diry * 1), player1.GetPosition(), projectile, player1, 3,
                                                                                0);
                    SpellProjectile newProj3 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(0.05f, p1Diry * 1), player1.GetPosition(), projectile, player1, 3,
                                                                                0);
                    SpellProjectile newProj4 = new SpellProjectile(10, player1.GetProjectileSpeed(), new Vector3(-0.05f, p1Diry * 1), player1.GetPosition(), projectile, player1, 3,
                                                                                0);
                }

            }
            else if (CompareRecipes(player1.GetBalls(), RECIPE_SUNSTRIKE))
            {
                if (player1.GetMana() >= COST_SMALL)
                {
                    SpellAOE spellaoe = new SpellAOE(10, COST_SMALL, 0.5f, 1, player2.GetPosition(), AOE, player1);
                }
            }
            else if (CompareRecipes(player1.GetBalls(), RECIPE_CATACLYSM))
            {
                if (player1.GetMana() >= COST_MED)
                {
                    SpellAOE spellaoe = new SpellAOE(10, COST_MED, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe1 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe2 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe3 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe4 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe5 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe6 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe7 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe8 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                }
            }
            else if (CompareRecipes(player1.GetBalls(), RECIPE_DOOMSDAY))
            {
                if (player1.GetMana() >= COST_LARGE)
                {
                    SpellAOE spellaoe = new SpellAOE(10, COST_LARGE, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe1 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe2 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe3 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe4 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe5 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe6 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe7 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe8 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe9 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe10 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe11 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe12 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe13 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe14 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe15 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe16 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe17 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe18 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe19 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe20 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                    SpellAOE spellaoe21 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player1);
                }
            }
            ResetBalls(1); // Resets the balls
        }
        else
        {
            if (CompareRecipes(player2.GetBalls(), RECIPE_FIREBALL))
            {
                CastFireball(player2, p2Dir);
                ui.change_mana(player2.GetMana(), false);
            }
            else if (CompareRecipes(player2.GetBalls(), RECIPE_ICESHOT))
            {
                CastIceBlast(player2, p2Diry);
                ui.change_mana(player2.GetMana(), false);
            }
            else if (CompareRecipes(player2.GetBalls(), RECIPE_SPREAD))
            {
                if (player2.GetMana() >= COST_MED)
                {
                    SpellProjectile newProj = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(p2Dir * 1, 0), player2.GetPosition(), projectile, player2, 0,
                                                                            COST_MED);
                    SpellProjectile newProj1 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(p2Dir * 1, 0.1f), player2.GetPosition(), projectile, player2, 0,
                                                                                0);
                    SpellProjectile newProj2 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(p2Dir * 1, -0.1f), player2.GetPosition(), projectile, player2, 0,
                                                                                0);
                }

            }
            else if (CompareRecipes(player2.GetBalls(), RECIPE_ICESPREAD))
            {
                if (player2.GetMana() >= COST_MED)
                {
                    SpellProjectile newProj = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(0, p2Diry * 1), player2.GetPosition(), projectile, player2, 3,
                                                                            COST_MED);
                    SpellProjectile newProj1 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(0.1f, p2Diry * 1), player2.GetPosition(), projectile, player2, 3,
                                                                                0);
                    SpellProjectile newProj2 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(-0.1f, p2Diry * 1), player2.GetPosition(), projectile, player2, 3,
                                                                                0);
                }

            }
            else if (CompareRecipes(player2.GetBalls(), RECIPE_DEAFBLAST))
            {
                if (player2.GetMana() >= COST_LARGE)
                {

                    SpellProjectile newProj = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(1, 0), player2.GetPosition()
                                                                       , projectile, player2, 2, COST_LARGE);
                    SpellProjectile newProj2 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(0, 1), player2.GetPosition()
                                                                       , projectile, player2, 2, 0);
                    SpellProjectile newProj3 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(-1, 0), player2.GetPosition()
                                                                       , projectile, player2, 2, 0);
                    SpellProjectile newProj4 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(0, -1), player2.GetPosition()
                                                                       , projectile, player2, 2, 0);
                    SpellProjectile newProj5 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(1, 1).normalized, player2.GetPosition()
                                                                       , projectile, player2, 2, 0);
                    SpellProjectile newProj6 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(-1, 1).normalized, player2.GetPosition()
                                                                       , projectile, player2, 2, 0);
                    SpellProjectile newProj7 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(-1, -1).normalized, player2.GetPosition()
                                                                       , projectile, player2, 2, 0);
                    SpellProjectile newProj8 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(1, -1).normalized, player2.GetPosition()
                                                                       , projectile, player2, 2, 0);
                    ui.change_mana(player2.GetMana(), false);
                }
            }
            else if (CompareRecipes(player2.GetBalls(), RECIPE_BIGSPREAD))
            {
                if (player2.GetMana() >= COST_LARGE)
                {
                    SpellProjectile newProj = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(p2Dir * 1, 0), player2.GetPosition(), projectile, player2, 0,
                                                                            COST_LARGE);
                    SpellProjectile newProj1 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(p2Dir * 1, 0.1f), player2.GetPosition(), projectile, player2, 0,
                                                                                0);
                    SpellProjectile newProj2 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(p2Dir * 1, -0.1f), player2.GetPosition(), projectile, player2, 0,
                                                                                0);
                    SpellProjectile newProj3 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(p2Dir * 1, 0.05f), player2.GetPosition(), projectile, player2, 0,
                                                                                0);
                    SpellProjectile newProj4 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(p2Dir * 1, -0.05f), player2.GetPosition(), projectile, player2, 0,
                                                                                0);
                }

            }
            else if (CompareRecipes(player2.GetBalls(), RECIPE_BIGICESPREAD))
            {
                if (player2.GetMana() >= COST_LARGE)
                {
                    SpellProjectile newProj = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(0, p2Diry * 1), player2.GetPosition(), projectile, player2, 3,
                                                                            COST_LARGE);
                    SpellProjectile newProj1 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(0.1f, p2Diry * 1), player2.GetPosition(), projectile, player2, 3,
                                                                                0);
                    SpellProjectile newProj2 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(-0.1f, p2Diry * 1), player2.GetPosition(), projectile, player2, 3,
                                                                                0);
                    SpellProjectile newProj3 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(0.05f, p2Diry * 1), player2.GetPosition(), projectile, player2, 3,
                                                                                0);
                    SpellProjectile newProj4 = new SpellProjectile(10, player2.GetProjectileSpeed(), new Vector3(-0.05f, p2Diry * 1), player2.GetPosition(), projectile, player2, 3,
                                                                                0);
                }

            }
            else if (CompareRecipes(player2.GetBalls(), RECIPE_SUNSTRIKE))
            {
                if (player2.GetMana() >= COST_SMALL)
                {
                    SpellAOE newAOE = new SpellAOE(10, COST_SMALL, 0.5f, 1, player1.GetPosition(), AOE, player2);
                }
            }
            else if (CompareRecipes(player2.GetBalls(), RECIPE_CATACLYSM))
            {
                if (player2.GetMana() >= COST_MED)
                {
                    SpellAOE newAOE = new SpellAOE(10, COST_MED, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE1 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE2 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE3 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE4 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE5 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE6 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE7 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE8 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                }
            }
            else if (CompareRecipes(player2.GetBalls(), RECIPE_DOOMSDAY))
            {
                if (player2.GetMana() >= COST_LARGE)
                {
                    SpellAOE newAOE = new SpellAOE(10, COST_LARGE, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE1 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE2 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE3 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE4 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE5 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE6 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE7 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE8 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE9 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE10 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE11 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE12 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE13 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE14 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE15 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE16 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE17 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE18 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE19 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE20 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                    SpellAOE newAOE21 = new SpellAOE(10, 0, 0.5f, 1, new Vector2(Random.Range(-80, 80), Random.Range(-20, 40)), AOE, player2);
                }
            }
            ResetBalls(2); // Resets the balls
        }

    }


    void ResetBalls(int playerNumber)
    {
        if (playerNumber == 1)
        {
            player1.RemoveBalls();
            ui.change_spell_sprite(true, player1.GetBalls());
        }
        else
        {
            player2.RemoveBalls();
            ui.change_spell_sprite(false, player2.GetBalls());
        }
    }

    /// <summary>
    /// CompareRecipe() compares two recipes without taking into account the order of the numbers. True if the recipes are the same
    /// </summary>
    /// <param name="Recipe1"></param>
    /// <param name="Recipe2"></param>
    bool CompareRecipes(int [] recipe1, int [] recipe2, int maxBalls = 3)
    {
        // Method of differentiation: weights 
        // Types are defined by integers, each integer i has a weight (maxBalls + 1)^i (to give each unique unordered set a unique weight)
        // -> we simply sum up the weights of both recipes and compare them. 
        int weight1 = 0;
        int weight2 = 0; 
        for (int i = 0; i < maxBalls; i++)
        {
            weight1 += IntPow((maxBalls + 1), recipe1[i]);
            weight2 += IntPow((maxBalls + 1), recipe2[i]);
        }
        return (weight2 == weight1);
    }

    /// <summary>
    /// Returns the integer power (O(i))
    /// </summary>
    /// <param name="b"></param>
    /// <param name="exp"></param>
    /// <returns></returns>
    int IntPow(int b, int exp)
    {
        return (exp == 0) ? 1 : b * IntPow(b, exp - 1);
    }

    //Still ambiguous
    Vector2 PolarToVector(int angleDeg) {
        float theta = Mathf.Deg2Rad * angleDeg;
        float x = 0;
        float y = 0;

        if (Mathf.Tan(theta) != 0) {
            y = 1;
            x = y / Mathf.Tan(theta);
        }

        Vector2 retVector = new Vector2(x, y).normalized;
        return retVector;
    }

    void GetShootingDirection(Vector2 p1Pos, Vector2 p2Pos)
    {
        if (p2Pos.x - p1Pos.x <= 0)
        {
            p1Dir = -1;
            player1.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            p2Dir = 1;
            player2.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            p1Dir = 1;
            player1.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            p2Dir = -1;
            player2.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (p2Pos.y - p1Pos.y <= 0)
        {
            p1Diry = -1;
            p2Diry = 1;
        } else
        {
            p1Diry = 1;
            p2Diry = -1;
        }
    }


    // SPELLS
    void CastFireball(Wizard player, int directionModifier)
    {
        if (player.GetMana() >= COST_SMALL)
        {
            SpellProjectile newProj = new SpellProjectile(10, player.GetProjectileSpeed(), new Vector3(directionModifier * 1, 0), player.GetPosition(), projectile, player, 0,
                                                                            COST_SMALL);
        }
    }

    void CastIceBlast(Wizard player, int directionModifier)
    {
        if (player.GetMana() >= COST_SMALL)
        {
            SpellProjectile newProj = new SpellProjectile(10, player.GetProjectileSpeed(), new Vector3(0, directionModifier * 1), player.GetPosition(), projectile, player, 3,
                                                                            COST_SMALL);
        }
    }
}