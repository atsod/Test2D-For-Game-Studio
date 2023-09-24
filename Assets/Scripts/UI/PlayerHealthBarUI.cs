
public class PlayerHealthBarUI : HealthBarUI
{
    private void OnEnable()
    {
        Player.OnPlayerHealthPointsChanged += OnHealthPointsChanged;
    }

    private void OnDisable()
    {
        Player.OnPlayerHealthPointsChanged -= OnHealthPointsChanged;
    }
}
