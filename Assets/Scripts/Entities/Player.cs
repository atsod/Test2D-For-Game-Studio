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
    private Vector2 _playerSight;

    private bool _isInvulnerable;

    private GameObject[] _enemies;

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
        DamagePoints = 5;
        SpeedPoints = 3f;
        FOV_Distance = 5f;

        _bulletOffset = new Vector3(0, 0.4f, 0);
        _bulletSpeed = 5f;

        _rigidBody = GetComponent<Rigidbody2D>();
        _playerSight = transform.localScale;
    }

    private void Start()
    {
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 moveDirection = _joystick.Direction;

        _rigidBody.MovePosition(_rigidBody.position + SpeedPoints * Time.fixedDeltaTime * moveDirection);
        
        RotateBody(moveDirection);
    }

    private void RotateBody(Vector2 direction)
    {
        if (direction.x < 0)
        {
            _playerSight = new(-1, 1);
        }
        else if (direction.x > 0)
        {
            _playerSight = new(1, 1);
        }

        transform.localScale = _playerSight;
    }
     
    public void Shoot()
    {
        if(!IsEnemyInFOV(out GameObject nearestEnemy))
        {
            return;
        }

        bool hasBullet = Inventory.Instance.DeleteAmountOfItem(1, 1);

        if (!hasBullet)
        {
            return;
        }

        Vector3 enemyPosition = nearestEnemy.transform.position;
        Vector3 playerPosition = transform.position;

        Vector2 direction = (enemyPosition - playerPosition).normalized;
        
        GameObject bullet = Instantiate(_bullet, transform.position + _bulletOffset, Quaternion.identity);
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        bulletRB.velocity = _bulletSpeed * direction;
    }

    private bool IsEnemyInFOV(out GameObject nearestEnemy)
    {
        foreach(GameObject enemy in _enemies)
        {
            if(enemy != null && FOV_Distance > Vector2.Distance(enemy.transform.position, transform.position))
            {
                nearestEnemy = enemy;
                return true;
            }
        }

        nearestEnemy = null;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Item>() != null)
        {
            Item item = collision.gameObject.GetComponent<Item>();
            Inventory.Instance.SearchForSameItem(item, item.Count);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (!_isInvulnerable)
            {
                OnPlayerHealthPointsChanged?.Invoke(enemy.DamagePoints);
                StartCoroutine(InvulnerabilityCooldown(1f));
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
