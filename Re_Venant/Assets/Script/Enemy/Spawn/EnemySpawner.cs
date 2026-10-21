using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave System")]
    [SerializeField] private List<WaveData> waves = new List<WaveData>();
    private int currentWaveIndex = 0;
    private WaveData currentWave;

    private float currentSpawnInterval; // 현재 적용중인 스폰 주기
    private float waveTimer = 0f;       // 웨이브 진행 시간 타이머

    [Header("References")]
    [SerializeField] private ProtectHealth protect;

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
        if (protect == null)
        {
            protect = FindFirstObjectByType<ProtectHealth>();
        }

        // 웨이브가 등록되어 있다면 첫 번째 웨이브 시작
        if (waves != null && waves.Count > 0)
        {
            StartWave(0);
            StartCoroutine(SpawnRoutine());
        }
        else
        {
            Debug.LogWarning("EnemySpawner에 웨이브가 설정되어 있지 않습니다!");
        }
    }

    private void Update()
    {
        if (protect == null || protect.IsDead || currentWave == null) return;

        if (currentWave.isFinalWave)
        {
            // 마지막 웨이브: 시간이 지날수록 스폰 주기를 감소시킴 (속도 증가)
            currentSpawnInterval -= currentWave.intervalDecreaseRate * Time.deltaTime;
            // 한계치 밑으로 떨어지지 않게 고정
            currentSpawnInterval = Mathf.Max(currentSpawnInterval, currentWave.minSpawnIntervalLimit);
        }
        else
        {
            // 일반 웨이브: 정해진 시간이 지나면 다음 웨이브로 넘어감
            waveTimer += Time.deltaTime;
            if (waveTimer >= currentWave.waveDuration)
            {
                StartNextWave();
            }
        }
    }

    private void StartWave(int index)
    {
        if (index >= waves.Count) return;

        currentWaveIndex = index;
        currentWave = waves[currentWaveIndex];

        // 웨이브 초기 설정 적용
        currentSpawnInterval = currentWave.spawnInterval;
        waveTimer = 0f;

        Debug.Log($"웨이브 {currentWaveIndex + 1} 시작! (마지막 웨이브 여부: {currentWave.isFinalWave})");
    }

    private void StartNextWave()
    {
        if (currentWaveIndex + 1 < waves.Count)
        {
            StartWave(currentWaveIndex + 1);
        }
        else
        {
            Debug.Log("모든 웨이브가 끝났습니다.");
            // 필요시 승리 처리 로직 등을 여기에 추가
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (protect != null && !protect.IsDead)
        {
            if (currentWave != null)
            {
                SpawnEnemy();

                // 동적으로 변하는 currentSpawnInterval 만큼 대기
                yield return new WaitForSeconds(currentSpawnInterval);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void SpawnEnemy()
    {
        if (currentWave.enemies == null || currentWave.enemies.Count == 0)
        {
            Debug.LogWarning("현재 웨이브에 Enemy Prefab이 등록되어 있지 않습니다.");
            return;
        }

        if (protect == null) return;

        if (!TryGetSpawnPosition(out Vector3 spawnPosition)) return;

        // 가중치 기반 랜덤 Enemy 선택
        GameObject selectedEnemy = GetRandomEnemyPrefabByWeight();

        if (selectedEnemy == null) return;

        Vector3 lookDirection = protect.transform.position - spawnPosition;
        lookDirection.y = 0f;

        Quaternion spawnRotation = Quaternion.identity;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            spawnRotation = Quaternion.LookRotation(lookDirection);
        }

        Instantiate(selectedEnemy, spawnPosition, spawnRotation, enemyParent);
    }

    // ★ 가중치(확률) 기반 랜덤 선택 알고리즘 ★
    private GameObject GetRandomEnemyPrefabByWeight()
    {
        int totalWeight = 0;

        // 1. 모든 적의 가중치 합을 구함
        foreach (var enemyData in currentWave.enemies)
        {
            if (enemyData.enemyPrefab != null)
            {
                totalWeight += enemyData.spawnWeight;
            }
        }

        if (totalWeight == 0) return null;

        // 2. 0 ~ 총 가중치 사이의 랜덤 값 추출
        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // 3. 랜덤 값이 어느 구간에 속하는지 판별
        foreach (var enemyData in currentWave.enemies)
        {
            if (enemyData.enemyPrefab == null) continue;

            cumulativeWeight += enemyData.spawnWeight;
            if (randomValue < cumulativeWeight)
            {
                return enemyData.enemyPrefab;
            }
        }

        return null;
    }

    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        for (int i = 0; i < spawnSearchAttempts; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(minSpawnRadius, maxSpawnRadius);

            Vector3 randomDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 randomPosition = protect.transform.position + randomDirection * distance;

            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
                return true;
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }
}