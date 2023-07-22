using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public int hitpoint = 10;
    public int maxHitPoint = 10;
    public float pushRecoverySpeed = 1f;

    //Immunity
    protected float immuneTime = 1.0f;
    protected float lastImmune;

    //Напрямок поштовху
    protected Vector3 pushDirection;

    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            hitpoint -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            // GameManager.instance.ShowText(dmg.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.zero, 0.5f);

            if (hitpoint <= 0)
            {
                hitpoint = 0;
                Death();
            }
        }
    }
    protected virtual void Death() { }
}
