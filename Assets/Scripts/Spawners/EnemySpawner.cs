using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;

    private int minX;
    private int maxX;
    private int minY;
    private int maxY;

    private void Awake()
    {
        minX = -4;
        maxX = 14;
        minY = -11;
        maxY = 8;
    }

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            int randomX = Random.Range(minX, maxX);
            int randomY = Random.Range(minY, maxY);

            Vector3 enemyPosition = new Vector3(randomX, randomY, 1);

            Instantiate(_enemyPrefab, enemyPosition, Quaternion.identity);
        }
    }
}
