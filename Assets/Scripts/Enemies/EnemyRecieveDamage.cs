using UnityEngine;
using UnityEngine.UI;

public class EnemyRecieveDamage : MonoBehaviour
{
    public float Health;
    public float MaxHealth = 100;

    public GameObject HealthBar;
    public Slider HealthBarSlider;

    void Start()
    {
        Health = MaxHealth;
    }

    private void CheckOverheal()
    {
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        HealthBarSlider.value = CalculateHealth();

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
        HealthBar.SetActive(true);
        Health -= damage;
        CheckDeath();
        HealthBarSlider.value = CalculateHealth();

    }


    public void HealCharacter(float heal)
    {
        Health += heal;
        CheckOverheal();
    }

    private float CalculateHealth()
    {
        return (Health / MaxHealth);
    }
}
