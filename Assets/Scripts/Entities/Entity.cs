using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int MaxHealthPoints { protected set; get; }
    public int CurrentHealthPoints { protected set; get; }
    public int DamagePoints { protected set; get; }
    public int SpeedPoints { protected set; get; }

    protected abstract void OnAttackedEntity(int damagePoints);
    protected abstract void OnDeadEntity();
}
