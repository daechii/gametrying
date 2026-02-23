using UnityEngine;

public class EnemyRecieveDamage : MonoBehaviour
{
    public float Health;
    public float MaxHealth;
    void Start()
    {
        Health -= MaxHealth;
    }

    private void CheckOverheal()
    {
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    private void CheckDeath()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void DealDamage(float damage)
    {
        Health -= damage;
        CheckDeath();
    }
}
