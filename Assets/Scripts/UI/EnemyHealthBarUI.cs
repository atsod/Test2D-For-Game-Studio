using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : HealthBarUI
{
    private void OnEnable()
    {
        Enemy.OnEnemyHealthPointsChanged += OnHealthPointsChanged;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyHealthPointsChanged -= OnHealthPointsChanged;
    }
}
