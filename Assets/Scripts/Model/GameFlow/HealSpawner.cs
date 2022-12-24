using UnityEngine;

public class HealSpawner : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private LayerMask healObjectLayer;
    [SerializeField] private HealerObject healerObject;
    [SerializeField] private float timeToSpawnHealAfterDamaged;
    [SerializeField] private Transform[] possibleHealPositions;

    private PlayerHealth _playerHealth;

    private Collider2D[] enemiesInRange = new Collider2D[30];
    private readonly float _enemiesCheckRange = 40f;

    private void Start()
    {
        _playerHealth = FindObjectOfType<PlayerHealth>();

        if (_playerHealth == null)
        {
            Debug.LogWarning("No PlayerHealth script on scene, so HealSpawner will be destroyed");
            Destroy(gameObject);
        }

        _playerHealth.DamagedEvent.AddListener(() => Invoke(nameof(SpawnHealObjectInBestPosition), timeToSpawnHealAfterDamaged));
    }

    private void SpawnHealObjectInBestPosition()
    {
        Instantiate(healerObject, ChooseHealPosition(), Quaternion.identity);
    }

    private Vector2 ChooseHealPosition()
    {
        Vector2 bestHealPosition = possibleHealPositions[0].position;
        var enemiesCountInBestPosition = 0;

        for (var i = 0; i < possibleHealPositions.Length; i++)
        {
            var healObjectInRange = Physics2D.OverlapCircle(possibleHealPositions[i].position, _enemiesCheckRange, healObjectLayer);
            if (healObjectInRange)
                continue;

            var enemyCountInRange = Physics2D.OverlapCircleNonAlloc(possibleHealPositions[i].position, _enemiesCheckRange, enemiesInRange, enemyLayers);

            if (enemyCountInRange > enemiesCountInBestPosition)
            {
                bestHealPosition = possibleHealPositions[i].position;
                enemiesCountInBestPosition = enemyCountInRange;
            }
        }

        return bestHealPosition;
    }
}
