using UnityEngine;
using UnityEngine.InputSystem;

public class TestSpell : MonoBehaviour
{
    public GameObject Projectile;
    public float MinDamage;
    public float MaxDamage;
    public float ProjectileForce;

    private PlayerInput _playerInput;
    private InputAction _action;

    private bool _isAttackPressed;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _action = _playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        _action.started += OnAttack;
    }
    private void OnDisable()
    {
        _action.started -= OnAttack;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
         _isAttackPressed = true;
    }

    void Update()
    {
        if (_isAttackPressed)
        {
            PerformAttack();
            _isAttackPressed = false; 
        }
    }

    private void PerformAttack()
    {
        GameObject spell = Instantiate(Projectile, transform.position, Quaternion.identity);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 myPos = transform.position;
        Vector2 direction = (mousePos - myPos).normalized;

        Rigidbody2D rb = spell.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * ProjectileForce;
        }

        spell.GetComponent<Projectile>()._damage = Random.Range(MinDamage, MaxDamage);
    }
}
