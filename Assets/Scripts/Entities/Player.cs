using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public static event Action<int> OnPlayerHealthPointsChanged;
    public static event Action OnPlayerKilled;

    public static Player Instance { get; private set; }

    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private GameObject _bullet;

    private Vector3 _bulletOffset;
    private float _bulletSpeed;

    private Rigidbody2D _rigidBody;

    private bool _isInvulnerable;

    private void OnEnable()
    {
        OnPlayerHealthPointsChanged += OnAttackedEntity;
        OnPlayerKilled += OnDeadEntity;
    }

    private void OnDisable()
    {
        OnPlayerHealthPointsChanged -= OnAttackedEntity;
        OnPlayerKilled -= OnDeadEntity;
    }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        MaxHealthPoints = 100;
        CurrentHealthPoints = MaxHealthPoints;
        DamagePoints = 5; // В будущем через паттерн "Стратегия" урон будет варьироваться, в зависимости от оружия.
        SpeedPoints = 3;

        _bulletOffset = new Vector3(0, 0.4f, 0);
        _bulletSpeed = 5f;

        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        _rigidBody.MovePosition(_rigidBody.position + SpeedPoints * Time.fixedDeltaTime * _joystick.Direction);
    }
     
    public void Shoot()
    {
        // Пуля будет лететь по направлению к противнику
        Vector2 direction = Vector2.right;

        GameObject bullet = Instantiate(_bullet, transform.position + _bulletOffset, Quaternion.identity);
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        bulletRB.velocity = _bulletSpeed * direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Item>() != null)
        {
            Item item = collision.gameObject.GetComponent<Item>();
            Inventory.Instance.SearchForSameItem(item, item.Count);
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if(!_isInvulnerable)
            {
                OnPlayerHealthPointsChanged?.Invoke(enemy.DamagePoints);
                StartCoroutine(InvulnerabilityCooldown(3f));
            }
        }
    }

    private IEnumerator InvulnerabilityCooldown(float invulnerabilityCoolDown)
    {
        _isInvulnerable = true;

        yield return new WaitForSeconds(invulnerabilityCoolDown);

        _isInvulnerable = false;
    }

    protected override void OnAttackedEntity(int damagePoints)
    {
        CurrentHealthPoints -= damagePoints;

        if (CurrentHealthPoints <= 0)
        {
            OnPlayerKilled?.Invoke();
        }
    }

    protected override void OnDeadEntity()
    {
        SpeedPoints = 0;
    }
}
