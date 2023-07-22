using UnityEngine;

public class Weapon : Collect
{
    public int[] damagePoint = { 1, 2, 3, 4, 5, 6, 7, 8 };                  // Кількість завданої шкоди
    public float[] pushForce = { 10f, 20f, 30f, 40f, 50f, 60, 70f, 80f };   // Сила поштовху

    // Оновлення
    public int weaponLevel = 0;
    public SpriteRenderer spriteRenderer;

    private Animator animator; 

    private float cooldown = 0.5f;  // перезарядка
    private float lastSwing;        // останній удар

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time - lastSwing > cooldown) 
            {
                lastSwing = Time.time;
                animator.SetTrigger("Swing");

                FindFirstObjectByType<AudioManager>().Play("swing_sword");
            }
        }
    }
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter")
        {
            if (coll.tag == "Player")
                return;
            
            // Створюємо об'єкт шкоди, який надішлемо ворогу коли з ним буде контакт
            Damage dmg = new Damage
            {
                damageAmount = damagePoint[weaponLevel],
                origin = transform.position,
                pushForce = pushForce[weaponLevel]
            };
           
            coll.SendMessage("ReceiveDamage", dmg);
        }
    }

    public void UpgradeWeapon()
    {
        weaponLevel++;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }

    public void SetWeaponLevel(int level)
    {
        weaponLevel = level;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }
}
