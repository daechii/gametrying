using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats _playerStats;

    public GameObject player;

    public float Health;
    public float MaxHealth = 100;

    private void Awake()
    {
        if (_playerStats != null)
        {
            Destroy(_playerStats);
        }
        else
        {
            _playerStats = this;
        }
        DontDestroyOnLoad(_playerStats);
    }

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
    }

    public void HealCharacter(float heal)
    {
        Health += heal;
        CheckOverheal();
    }

    private void CheckDeath()
    {
        if (Health <= 0)
        {
            Destroy(this.player);
        }
    }

    public void DealDamage(float damage)
    {
        Health -= damage;
        CheckDeath();
    }
}
