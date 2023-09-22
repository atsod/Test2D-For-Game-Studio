using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
