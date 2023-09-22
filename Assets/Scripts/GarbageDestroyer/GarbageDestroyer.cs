using UnityEngine;

public class GarbageDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Bullet>() != null)
        {
            Destroy(collision.gameObject);
        }
    }
}
