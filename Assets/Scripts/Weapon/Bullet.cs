using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Player PlayerObject;
    public int DamagePoints;

    private void Start()
    {
        PlayerObject = Player.Instance;
        DamagePoints = PlayerObject.DamagePoints;
    }
}
