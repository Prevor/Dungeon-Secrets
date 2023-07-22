using UnityEngine;

public class PotionOfHealing : Collide
{
    int healingAmount = new System.Random().Next(1, 5);

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            FindFirstObjectByType<AudioManager>().Play("healing_sound");
            GameManager.instance.player.Heal(healingAmount);
            Destroy(gameObject);
        }
    }
}
