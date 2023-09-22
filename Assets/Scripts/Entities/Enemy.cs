using System;
using UnityEngine;

public class Enemy : Entity
{
    public static event Action<int> OnEnemyHealthPointsChanged;
    public static event Action OnEnemyKilled;

    [SerializeField] private GameObject _monsterDrop;

    private Rigidbody2D _rigidBody;

    private void OnEnable()
    {
        OnEnemyHealthPointsChanged += OnAttackedEntity;
        OnEnemyKilled += OnDeadEntity;
    }

    private void OnDisable()
    {
        OnEnemyHealthPointsChanged -= OnAttackedEntity;
        OnEnemyKilled -= OnDeadEntity;
    }

    private void Awake()
    {
        MaxHealthPoints = 20;
        CurrentHealthPoints = MaxHealthPoints;
        DamagePoints = 10;
        SpeedPoints = 3;

        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Bullet>() != null)
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnEnemyHealthPointsChanged?.Invoke(bullet.DamagePoints);
        }
    }

    protected override void OnAttackedEntity(int damagePoints)
    {
        CurrentHealthPoints -= damagePoints;

        if (CurrentHealthPoints <= 0)
        {
            OnEnemyKilled?.Invoke();
        }
    }

    protected override void OnDeadEntity()
    {
        Instantiate(_monsterDrop, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

    
