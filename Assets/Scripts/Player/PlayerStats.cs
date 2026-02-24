using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats _playerStats;

    public GameObject player;
    public Text HealthStat;
    public Slider HealthBar;

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
        SetHealUI();
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
        SetHealUI();
    }

    private void SetHealUI() 
    {
        HealthBar.value = CalculcateHealthPercentage();
        HealthStat.text = Mathf.Ceil(Health).ToString() + "/" + Mathf.Ceil(MaxHealth).ToString();
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
        SetHealUI();
    }

    float CalculcateHealthPercentage()
    {
        return Health / MaxHealth;
    }
}
