using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private ProtectHealth protect;

    [Header("Spawn Time")]
    [SerializeField] private float spawnInterval = 3f;

    [Header("Spawn Range")]
    [SerializeField] private float minSpawnRadius = 8f;
    [SerializeField] private float maxSpawnRadius = 12f;

    [Header("NavMesh")]
    [SerializeField] private float navMeshSearchRadius = 3f;
    [SerializeField] private int spawnSearchAttempts = 15;

    [Header("Hierarchy")]
    [SerializeField] private Transform enemyParent;

    private void Start()
    {
        // Protect를 직접 넣지 않았다면 자동으로 찾음
        if (protect == null)
        {
            protect = FindFirstObjectByType<ProtectHealth>();
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (protect != null && !protect.IsDead)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
            return;

        if (protect == null)
            return;

        if (!TryGetSpawnPosition(out Vector3 spawnPosition))
            return;

        // 생성될 때 Protect 방향을 바라보도록 설정
        Vector3 lookDirection =
            protect.transform.position - spawnPosition;

        lookDirection.y = 0f;

        Quaternion spawnRotation = Quaternion.identity;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            spawnRotation =
                Quaternion.LookRotation(lookDirection);
        }

        Instantiate(
            enemyPrefab,
            spawnPosition,
            spawnRotation,
            enemyParent
        );
    }

    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        for (int i = 0; i < spawnSearchAttempts; i++)
        {
            // 0 ~ 360도 랜덤 방향
            float angle =
                Random.Range(0f, 360f) *
                Mathf.Deg2Rad;

            // Protect와의 랜덤 거리
            float distance =
                Random.Range(
                    minSpawnRadius,
                    maxSpawnRadius
                );

            Vector3 randomDirection =
                new Vector3(
                    Mathf.Cos(angle),
                    0f,
                    Mathf.Sin(angle)
                );

            Vector3 randomPosition =
                protect.transform.position +
                randomDirection * distance;

            // 랜덤 위치 근처에 실제 NavMesh가 있는지 검사
            if (NavMesh.SamplePosition(
                randomPosition,
                out NavMeshHit hit,
                navMeshSearchRadius,
                NavMesh.AllAreas))
            {
                spawnPosition = hit.position;

                return true;
            }
        }

        Debug.LogWarning(
            "Enemy를 생성할 NavMesh 위치를 찾지 못했습니다."
        );

        spawnPosition = Vector3.zero;

        return false;
    }
}