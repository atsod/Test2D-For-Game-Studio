using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int MaxHealthPoints { protected set; get; }
    public int CurrentHealthPoints { set; get; }
    public int DamagePoints { protected set; get; }
    public float SpeedPoints { protected set; get; }
    public float FOV_Distance { protected set; get; }

    protected abstract void OnAttackedEntity(int damagePoints);
    protected abstract void OnDeadEntity();
}
