using UnityEngine;

public class IsolationRoom : MonoBehaviour
{
    [Header("격리실 번호")]
    public int roomNumber;


    [Header("이면세계 이미지 표시")]
    public SpriteRenderer otherworldRenderer;


    [Header("이면세계 오브젝트 생성 위치")]
    public Transform otherworldSpawnPoint;


    public OtherworldData CurrentOtherworld
    {
        get;
        private set;
    }


    private GameObject spawnedOtherworldObject;


    // =========================================
    // 이면세계 배치
    // =========================================

    public void AssignOtherworld(
        OtherworldData data)
    {
        if (data == null)
            return;


        CurrentOtherworld = data;


        // 이면세계 이미지 적용
        if (otherworldRenderer != null)
        {
            otherworldRenderer.sprite =
                data.roomImage;

            otherworldRenderer.enabled =
                true;
        }


        // 기존 이면세계 오브젝트 제거
        if (spawnedOtherworldObject != null)
        {
            Destroy(
                spawnedOtherworldObject
            );
        }


        // 이면세계 Prefab 생성
        if (data.roomPrefab != null &&
            otherworldSpawnPoint != null)
        {
            spawnedOtherworldObject =
                Instantiate(
                    data.roomPrefab,
                    otherworldSpawnPoint.position,
                    Quaternion.identity,
                    otherworldSpawnPoint
                );
        }


        Debug.Log(
            $"{roomNumber}번 격리실에 " +
            $"{data.worldName} 배치"
        );
    }


    // =========================================
    // 격리실 초기화
    // =========================================

    public void ClearRoom()
    {
        CurrentOtherworld = null;


        if (otherworldRenderer != null)
        {
            otherworldRenderer.sprite =
                null;

            otherworldRenderer.enabled =
                false;
        }


        if (spawnedOtherworldObject != null)
        {
            Destroy(
                spawnedOtherworldObject
            );

            spawnedOtherworldObject =
                null;
        }
    }
}