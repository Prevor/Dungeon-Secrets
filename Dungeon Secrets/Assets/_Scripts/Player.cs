using UnityEngine;
using UnityEngine.SceneManagement;
using Input = UnityEngine.Input;

public class Player : Move
{
    private Animator animator;

    private bool isAlive = true;

    protected override void Start()
    {
        base.Start();
        GameManager.instance.SetMaxHelth(hitpoint);
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        UpdateMove(new Vector2(x, y));

        if (y == 0 && x == 0)
        {
            animator.SetBool("isRunning", false);
        }
        else
            animator.SetBool("isRunning", true);
    }

    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isAlive)
            return;

        animator.SetTrigger("hit");
        FindFirstObjectByType<AudioManager>().Play("hurt_player");

        base.ReceiveDamage(dmg);
        GameManager.instance.SetHealth(hitpoint);
    }

    protected override void Death()
    {
        isAlive = false;
        GameManager.instance.deathMenuAnim.SetTrigger("Show");
    }

    public void OnLevelUp()
    {
        maxHitPoint++;
        hitpoint = maxHitPoint;
    }

    public void SetLevel(int level)
    {
        for (int i = 0; i < level; i++)
            OnLevelUp();
    }

    public void Heal(int healingAmount)
    {
        hitpoint += healingAmount;

        if (hitpoint >= maxHitPoint)
        {
            hitpoint = maxHitPoint;
            GameManager.instance.ShowText("HP _MAX_", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
            GameManager.instance.SetHealth(hitpoint);
        }
        else
        {
            GameManager.instance.ShowText("+" + healingAmount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
            GameManager.instance.SetHealth(hitpoint);
        }

        Debug.Log(healingAmount + "  2");

    }

    public void RespawnPlayer()
    {
        Heal(maxHitPoint);
        isAlive = true;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
    }
}
