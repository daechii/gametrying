using System.Collections;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject Projectile;

    public Transform _playerTransform;

    public float MinDamage = 5;
    public float MaxDamage = 10;
    public float ProjectileForce = 10;
    public float Cooldown = 2;

    void Start()
    {
       
        StartCoroutine(ShootingRoutine());
    }

    
    IEnumerator ShootingRoutine()
    {
        while (true)
        {

            if (_playerTransform != null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    _playerTransform = playerObj.transform;
                    Debug.Log("<color=green>Враг наконец-то нашел игрока!</color>");
                }
            }
            else
            {
                Debug.Log("<color=yellow>Враг ищет игрока, но его нет на сцене...</color>");
            }


            if (_playerTransform != null)
            {
                Shoot();
                yield return new WaitForSeconds(Cooldown);

            }
            else
            {
                Debug.Log("игрок пустой");

                yield return new WaitForSeconds(1f);

            }

        }
    }

    void Shoot()
    {
        GameObject spell = Instantiate(Projectile, transform.position, Quaternion.identity);

        Vector2 direction = (_playerTransform.position - transform.position).normalized;

        Rigidbody2D rb = spell.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * ProjectileForce;
        }

        if (spell.TryGetComponent<EnemyProjectile>(out EnemyProjectile proj))
        {
            proj._damage = Random.Range(MinDamage, MaxDamage);
        }
    }
}
