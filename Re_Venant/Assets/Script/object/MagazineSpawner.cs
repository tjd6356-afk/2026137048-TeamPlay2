using System.Collections.Generic;
using UnityEngine;

public class MagazineSpawner : MonoBehaviour
{
    [Header("스폰 설정 (Spawn Settings)")]
    [SerializeField, Tooltip("스폰할 탄창 프리팹")]
    private GameObject magazinePrefab;

    [SerializeField, Tooltip("탄창 생성 주기 (초)")]
    private float spawnInterval = 3f;

    [Header("거리 설정 (Distance Settings)")]
    [SerializeField, Tooltip("중심으로부터 최소 거리")]
    private float minSpawnRadius = 5f;

    [SerializeField, Tooltip("중심으로부터 최대 거리")]
    private float maxSpawnRadius = 15f;

    [Header("제한 설정 (Limit Settings)")]
    [SerializeField, Tooltip("맵에 존재할 수 있는 최대 탄창 수")]
    private int maxMagazines = 5;

    // 현재 맵에 존재하는 탄창들을 추적하는 리스트
    private List<GameObject> spawnedMagazines = new List<GameObject>();

    // 생성 주기 타이머
    private float timer = 0f;

    private void Update()
    {
        // 플레이어가 탄창을 먹어서 오브젝트가 파괴(null)되었다면 리스트에서 정리합니다.
        spawnedMagazines.RemoveAll(item => item == null);

        // 현재 스폰된 탄창 수가 최대치보다 적을 때만 타이머를 작동시킵니다.
        if (spawnedMagazines.Count < maxMagazines)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnMagazine();
                timer = 0f; // 스폰 후 타이머 초기화
            }
        }
    }

    private void SpawnMagazine()
    {
        if (magazinePrefab == null)
        {
            Debug.LogWarning("탄창 프리팹이 할당되지 않았습니다!");
            return;
        }

        // 1. 랜덤한 방향(원 테두리)을 구합니다.
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // 2. 최소 거리와 최대 거리 사이의 랜덤한 거리를 구합니다.
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);

        // 3. 방향과 거리를 곱해서 3D 공간의 X, Z 좌표를 구합니다.
        // (탑다운 뷰이므로 Y축 높이는 스포너의 Y축을 그대로 사용합니다)
        Vector3 spawnPosition = new Vector3(
            transform.position.x + randomDirection.x * randomDistance,
            transform.position.y, // Y축 고정
            transform.position.z + randomDirection.y * randomDistance
        );

        // 4. 해당 위치에 탄창 생성 후 리스트에 추가합니다.
        GameObject newMagazine = Instantiate(magazinePrefab, spawnPosition, Quaternion.identity);
        spawnedMagazines.Add(newMagazine);
    }

    // 유니티 에디터(씬 뷰)에서 스폰 범위를 눈으로 쉽게 확인할 수 있게 그려주는 기능입니다.
    private void OnDrawGizmosSelected()
    {
        // 스폰 불가능한 최소 거리 (빨간색 원)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minSpawnRadius);

        // 스폰 가능한 최대 거리 (초록색 원)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxSpawnRadius);
    }
}