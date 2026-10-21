using System.Collections.Generic;
using UnityEngine;

// 적 스폰 비율을 설정하기 위한 구조체
[System.Serializable]
public struct EnemySpawnData
{
    public GameObject enemyPrefab;
    [Range(1, 100)]
    public int spawnWeight; // 등장 가중치 (높을수록 자주 등장)
}

[CreateAssetMenu(fileName = "New Wave Data", menuName = "Wave System/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Wave Settings")]
    [Tooltip("이 웨이브가 지속되는 시간 (초)")]
    public float waveDuration = 60f;
    [Tooltip("기본 적 스폰 주기 (초)")]
    public float spawnInterval = 3f;

    [Header("Enemy Spawn Ratio")]
    [Tooltip("등장할 적과 가중치 목록")]
    public List<EnemySpawnData> enemies = new List<EnemySpawnData>();

    [Header("Final Wave Settings")]
    [Tooltip("이 웨이브가 마지막 무한 웨이브인지 여부")]
    public bool isFinalWave = false;

    [Tooltip("초당 스폰 주기가 얼마나 감소할 것인지 (스폰 속도 증가 비율)")]
    public float intervalDecreaseRate = 0.05f;

    [Tooltip("스폰 주기의 최소 한계치 (아무리 빨라져도 이 이하로는 안 내려감)")]
    public float minSpawnIntervalLimit = 0.5f;
}