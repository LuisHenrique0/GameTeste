using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 0;
    public BattleManager battleManager;

    private Vector3 moveDirection;

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (battleManager != null)
            {
                battleManager.ApplyDamageToEnemy(damage);
                Destroy(gameObject);
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}