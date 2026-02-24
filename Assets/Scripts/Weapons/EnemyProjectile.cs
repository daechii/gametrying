using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float _damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) return;

        if (collision.CompareTag("Player"))
        {
            if (PlayerStats._playerStats != null)
            {
                PlayerStats._playerStats.DealDamage(_damage);
            }

            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
