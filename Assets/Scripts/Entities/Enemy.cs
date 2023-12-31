using System;
using UnityEngine;

public class Enemy : Entity
{
    public event Action<int> OnEnemyHealthPointsChanged;
    public event Action OnEnemyKilled;

    [SerializeField] private GameObject _monsterDrop;

    private Rigidbody2D _rigidBody;

    private GameObject _player;

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
        SpeedPoints = 2f;
        FOV_Distance = 5f;

        _rigidBody = GetComponent<Rigidbody2D>();

        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (IsPlayerInFOV()) MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        Vector3 playerPosition = _player.transform.position;
        Vector3 enemyPosition = transform.position;

        Vector3 direction = (playerPosition - enemyPosition).normalized;
        
        _rigidBody.MovePosition(_rigidBody.position + SpeedPoints * Time.fixedDeltaTime * (Vector2) direction);
    }

    private bool IsPlayerInFOV()
    {
        return FOV_Distance > Vector2.Distance(_player.transform.position, transform.position);
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

    
