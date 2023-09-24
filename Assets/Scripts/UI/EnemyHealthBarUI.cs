
public class EnemyHealthBarUI : HealthBarUI
{
    private void OnEnable()
    {
        ((Enemy) _liveEntity).OnEnemyHealthPointsChanged += OnHealthPointsChanged;
    }

    private void OnDisable()
    {
        ((Enemy)_liveEntity).OnEnemyHealthPointsChanged -= OnHealthPointsChanged;
    }
}
