using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats _playerStats;

    public GameObject player;
    public Slider HealthBar;

    public float Health;
    public float MaxHealth = 100;

    bool _dead;

    public void BindPlayer(GameObject instance)
    {
        player = instance;
        _dead = false;
        Health = MaxHealth;
        if (HealthBar != null)
            SetHealUI();
    }

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
        if (_dead)
            return;
        Health += heal;
        CheckOverheal();
        SetHealUI();
    }

    private void SetHealUI() 
    {
        if (HealthBar == null)
            return;
        HealthBar.value = CalculcateHealthPercentage();
    }

    private void CheckDeath()
    {
        if (_dead || Health > 0)
            return;

        _dead = true;

        // Destroy только объекта в сцене. Ссылка на префаб из Project даёт ошибку "Destroying assets is not permitted".
        GameObject target = player;
        if (target == null || !target.scene.IsValid())
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
                target = found;
        }

        if (target != null && target.scene.IsValid())
            Destroy(target);
    }

    public void DealDamage(float damage)
    {
        if (_dead)
            return;
        Health -= damage;
        CheckDeath();
        if (!_dead)
            SetHealUI();
    }

    float CalculcateHealthPercentage()
    {
        return Health / MaxHealth;
    }
}
