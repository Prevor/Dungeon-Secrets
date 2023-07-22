using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Move
{
    // Experience
    public int xpValue = 1;

    public HealthBarEnemy healthBar;

    // Logic
    public float triggerLenght = 1;
    public float chaseLenght = 5;
    private bool chasing;
    private bool collidingWithPlayer;

    private Transform playerTransform;
    private Vector3 startingPosition;
    private Animator animator;

    // Hitbox
    public ContactFilter2D filter;
    private BoxCollider2D hitbox;
    private Collider2D[] hits = new Collider2D[10];
  

    protected override void Start()
    {
        base.Start();

        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        healthBar.SetHealth(hitpoint, maxHitPoint);
    }

    protected override void ReceiveDamage(Damage dmg)
    {
        base.ReceiveDamage(dmg);
        animator.SetTrigger("hit");
        FindFirstObjectByType<AudioManager>().Play("hurt_enemy");
    }

    private void FixedUpdate()
    {
        healthBar.SetHealth(hitpoint, maxHitPoint);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLenght)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLenght)
                chasing = true;

            if (chasing)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMove((playerTransform.position - transform.position).normalized);
                    animator.SetBool("isRunning", true);
                }
            }
            else
            {
                UpdateMove(startingPosition - transform.position);
                animator.SetBool("isRunning", false);
            }
        }
        else
        {
            UpdateMove((startingPosition - transform.position) * 0.2f);
            chasing = false;
        }

        // Перевірка зіткнення
        collidingWithPlayer = false;
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (hits[i].tag == "Player")
            {
                collidingWithPlayer = true;
            }
            hits[i] = null;
        }
        UpdateMove(Vector3.zero);
    }

    protected override void Death()
    {
        Destroy(gameObject);
        GameManager.instance.GrantXp(xpValue);

        GameManager.instance.ShowText("+" + xpValue + " XP", 30, Color.magenta, transform.position, Vector3.up * 40, 1.2f);
    }
}
