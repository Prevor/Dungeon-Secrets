using UnityEngine;

public class Chest : Collect
{
    private int goldAmount = new System.Random().Next(50, 150);

    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            GameManager.instance.gold += goldAmount;
            FindFirstObjectByType<AudioManager>().Play("collect_coin");
            GameManager.instance.ShowText("+" + goldAmount + " GOLD!", 25, Color.yellow, transform.position, Vector3.up * 25, 1.5f);
            Destroy(gameObject);
        }
    }
}
