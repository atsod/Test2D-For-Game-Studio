using UnityEngine;

public class BoundsChecker : MonoBehaviour
{
    private Transform playerTransform;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private void Awake()
    {
        playerTransform = GetComponent<Transform>();

        minX = -14.2f;
        maxX = 15.4f;
        minY = -11;
        maxY = 10;
    }

    private void FixedUpdate()
    {
        CheckBounds();
    }

    private void CheckBounds()
    {
        float x = 0;
        float y = 0;

        if (playerTransform.position.x < minX) x = minX;
        if (playerTransform.position.x > maxX) x = maxX;
        if (playerTransform.position.y < minY) y = minY;
        if (playerTransform.position.y > maxY) y = maxY;

        if(x != 0)
        {
            playerTransform.position = new Vector3(x, playerTransform.position.y, playerTransform.position.z);
        }
        if(y != 0)
        {
            playerTransform.position = new Vector3(playerTransform.position.x, y, playerTransform.position.z);
        }
    }
}
