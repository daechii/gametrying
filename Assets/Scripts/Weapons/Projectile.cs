using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float _damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        if (collision.TryGetComponent<EnemyRecieveDamage>(out EnemyRecieveDamage enemy))
        {
            enemy.DealDamage(_damage);
        }

        Destroy(gameObject);
    }
}
