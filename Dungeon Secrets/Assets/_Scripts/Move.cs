using UnityEngine;

public abstract class Move : Fighter
{
    private Vector2 originalSize;

    protected BoxCollider2D boxCollider;
    protected Vector3 moveInput;
    protected RaycastHit2D hit;

    public float ySpeed = 0.75f;
    public float xSpeed = 1f;

    protected virtual void Start()
    {
        originalSize = transform.localScale;
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void UpdateMove(Vector2 input)
    {
        // Скидання параметрів
        moveInput = new Vector2(input.x * xSpeed, input.y * ySpeed);

        if (moveInput.x > 0)
            transform.localScale = originalSize;
        else if (moveInput.x < 0)
            transform.localScale = new Vector2(originalSize.x * -1, originalSize.y);

        // Додавання вектору поштовху
        moveInput += pushDirection;

        // Зменшує силу поштовху з кожним кадром, залежно від швидкості відновлення
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);

        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveInput.y), Mathf.Abs(moveInput.y * Time.deltaTime), LayerMask.GetMask("Actor", "Collision"));
        if (hit.collider == null)
        {
            transform.Translate(0, moveInput.y * Time.deltaTime, 0);
        }

        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveInput.x, 0), Mathf.Abs(moveInput.x * Time.deltaTime), LayerMask.GetMask("Actor", "Collision"));
        if (hit.collider == null)
        {
            transform.Translate(moveInput.x * Time.deltaTime, 0, 0);
        }
    }
}
