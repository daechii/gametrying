using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class EnemyChasePlayer : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1.2f;
    [SerializeField] float stopDistance = 0.28f;
    [SerializeField] float reacquirePlayerInterval = 0.35f;

    [Header("Урон при касании")]
    [SerializeField] bool dealContactDamage = true;
    [SerializeField] float contactDamage = 8f;
    [SerializeField] float contactDamageCooldown = 0.75f;

    Transform _player;
    float _nextPlayerSearchTime;
    float _nextDamageTime;
    Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        _nextPlayerSearchTime = 0f;
    }

    void FixedUpdate()
    {
        if (_player == null)
        {
            if (Time.time < _nextPlayerSearchTime)
                return;
            _nextPlayerSearchTime = Time.time + reacquirePlayerInterval;
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                _player = p.transform;
            return;
        }

        Vector2 self = transform.position;
        Vector2 target = _player.position;
        Vector2 delta = target - self;
        float dist = delta.magnitude;
        if (dist <= stopDistance)
        {
            if (_rb != null)
                _rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = delta / dist;
        if (_rb != null)
            _rb.linearVelocity = dir * moveSpeed;
        else
            transform.position += (Vector3)(dir * moveSpeed * Time.fixedDeltaTime);
    }

    void OnDisable()
    {
        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!dealContactDamage || !other.CompareTag("Player"))
            return;
        if (Time.time < _nextDamageTime)
            return;
        if (PlayerStats._playerStats != null)
        {
            PlayerStats._playerStats.DealDamage(contactDamage);
            _nextDamageTime = Time.time + contactDamageCooldown;
        }
    }
}
