using UnityEngine;

public class EnemyHitBox : Collide
{
    // Шкода
    public int damage = 1;
    public float pushForce = 5;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            // Створено новий об'єкт пошкодження
            Damage dmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = pushForce
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
}
