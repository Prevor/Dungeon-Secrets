using UnityEngine;

public class Collect : Collide
{
    protected bool collected;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            OnCollect();
        }
    }

    protected virtual void OnCollect()
    {
        collected = true;
    }
}
