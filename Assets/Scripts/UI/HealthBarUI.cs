using UnityEngine;
using UnityEngine.UI;

public abstract class HealthBarUI : MonoBehaviour
{
    [SerializeField] protected Entity _liveEntity;
    [SerializeField] private Image _fillHealthBar;

    private Vector3 _healthBarOffset;
    private float _maxHP;
    private float _HP;

    private void Awake()
    {
        _healthBarOffset = new Vector3(0, 1.5f, 0);
    }

    private void Start()
    {
        _HP = _liveEntity.CurrentHealthPoints;
        _maxHP = _liveEntity.MaxHealthPoints;

        _fillHealthBar.fillAmount = _HP / _maxHP;
    }

    private void Update()
    {
        transform.position = _liveEntity.transform.position + _healthBarOffset;
    }

    protected void OnHealthPointsChanged(int damage)
    {
        _HP -= damage;

        _fillHealthBar.fillAmount = _HP / _maxHP;
    }
}
