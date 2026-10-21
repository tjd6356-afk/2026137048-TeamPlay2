using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField]
    private List<GameObject> enemyPrefabs =
        new List<GameObject>();

    [Header("References")]
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
        // Protect가 등록되어 있지 않으면 자동 검색
        if (protect == null)
        {
            protect =
                FindFirstObjectByType<ProtectHealth>();
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (protect != null && !protect.IsDead)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(
                spawnInterval
            );
        }
    }

    private void SpawnEnemy()
    {
        // 등록된 적이 하나도 없으면 생성 불가
        if (enemyPrefabs == null ||
            enemyPrefabs.Count == 0)
        {
            Debug.LogWarning(
                "EnemySpawner에 Enemy Prefab이 등록되어 있지 않습니다."
            );

            return;
        }

        if (protect == null)
            return;

        if (!TryGetSpawnPosition(
            out Vector3 spawnPosition))
        {
            return;
        }

        // 리스트에서 랜덤 Enemy 선택
        GameObject selectedEnemy =
            GetRandomEnemyPrefab();

        if (selectedEnemy == null)
            return;

        // Protect 방향 바라보기
        Vector3 lookDirection =
            protect.transform.position -
            spawnPosition;

        lookDirection.y = 0f;

        Quaternion spawnRotation =
            Quaternion.identity;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            spawnRotation =
                Quaternion.LookRotation(
                    lookDirection
                );
        }

        // Enemy 생성
        Instantiate(
            selectedEnemy,
            spawnPosition,
            spawnRotation,
            enemyParent
        );
    }

    private GameObject GetRandomEnemyPrefab()
    {
        if (enemyPrefabs == null ||
            enemyPrefabs.Count == 0)
        {
            return null;
        }

        // null 프리팹이 섞여있을 수도 있으므로
        // 몇 번 다시 선택
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            int randomIndex =
                Random.Range(
                    0,
                    enemyPrefabs.Count
                );

            GameObject selected =
                enemyPrefabs[randomIndex];

            if (selected != null)
            {
                return selected;
            }
        }

        Debug.LogWarning(
            "Enemy Prefabs 리스트에 유효한 프리팹이 없습니다."
        );

        return null;
    }

    private bool TryGetSpawnPosition(
        out Vector3 spawnPosition)
    {
        for (int i = 0;
             i < spawnSearchAttempts;
             i++)
        {
            float angle =
                Random.Range(0f, 360f) *
                Mathf.Deg2Rad;

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

            // NavMesh 위의 위치 검색
            if (NavMesh.SamplePosition(
                randomPosition,
                out NavMeshHit hit,
                navMeshSearchRadius,
                NavMesh.AllAreas))
            {
                spawnPosition =
                    hit.position;

                return true;
            }
        }

        Debug.LogWarning(
            "Enemy를 생성할 NavMesh 위치를 찾지 못했습니다."
        );

        spawnPosition =
            Vector3.zero;

        return false;
    }
}