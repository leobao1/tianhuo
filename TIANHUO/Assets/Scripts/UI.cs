using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    //All spell image sprites
    public Sprite[] orb_sprites = new Sprite[4];
    //All spell image slots
    public Image[] images = new Image[6];

    public Image[] health_bars = new Image[2];
    public Image[] mana_bars = new Image[2];

    public Image mr_goose;

    /*
     *  changes sprite of some static image variable to new_spell_sprite. ball_to_change determines which is selected.
     *  effects: changes static member of class
     */

    void Awake()
    {
        change_spell_sprite(true, new int[] { 0, 0, 0 });
        change_spell_sprite(false, new int[] { 0, 0, 0 });
    }

    public void change_spell_sprite(bool is_player_one, int[] my_balls){
        if (is_player_one) {
            for (int i = 0; i < 3; ++i) {
                if (my_balls[i] == 1) {
                    images[i].sprite = orb_sprites[0];
                }
                else if (my_balls[i] == 2) {
                    images[i].sprite = orb_sprites[1];
                }
                else if (my_balls[i] == 3){
                    images[i].sprite = orb_sprites[2];
                }
                else
                {
                    images[i].sprite = orb_sprites[3];
                }
            }
        }else{
            for (int i = 0; i < 3; ++i) {
                if (my_balls[i] == 1) {
                    images[i + 3].sprite = orb_sprites[0];
                }
                else if (my_balls[i] == 2) {
                    images[i + 3].sprite = orb_sprites[1];
                }
                else if(my_balls[i] == 3){
                    images[i + 3].sprite = orb_sprites[2];
                }
                else
                {
                    images[i + 3].sprite = orb_sprites[3];
                }
            }
        }
    }

    public void change_mana(int current_mana , bool is_player_one){
        if(is_player_one){
            mana_bars[0].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, current_mana);
        }else{
            mana_bars[1].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, current_mana);
        }
    }

    public void change_health(int current_health , bool is_player_one){
        if(is_player_one){
            health_bars[0].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, current_health);
        }else{
            health_bars[1].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, current_health);
        }
        if(current_health <= 0){
            display_mr_goose(true);
        }
    }

    public void display_mr_goose(bool unhide){
        if (!unhide) {
                mr_goose.enabled = false;
        }else{
            mr_goose.enabled = true;
        }
    }
}
