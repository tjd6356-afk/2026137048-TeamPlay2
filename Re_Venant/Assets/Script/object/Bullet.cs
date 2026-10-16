using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 15f;

    [SerializeField]
    private float maxDistance = 20f;

    [Header("Damage")]
    [SerializeField] private float damage = 10f;

    private Vector3 startPosition;

    private void Start()
    {
        // 생성된 위치 저장
        startPosition = transform.position;

        // 테스트용 총알 색상
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
    }

    private void Update()
    {
        // 앞으로 이동
        transform.position +=
            transform.forward *
            speed *
            Time.deltaTime;

        // 이동 거리 계산
        float traveledDistance =
            Vector3.Distance(
                startPosition,
                transform.position
            );

        // 최대 사거리 초과 시 삭제
        if (traveledDistance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Protect에는 플레이어 총알 데미지를 주지 않음
        ProtectHealth protect =
            other.GetComponentInParent<ProtectHealth>();

        if (protect != null)
        {
            return;
        }

        // 1. 맞은 오브젝트에서 EnemyHealth 찾기
        EnemyHealth enemy =
            other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            // Enemy 데미지
            enemy.TakeDamage(damage);

            // 총알 삭제
            Destroy(gameObject);

            return;
        }

        // 2. 맞은 오브젝트에서 탄창 상자(MagazineObject) 찾기 (추가된 부분)
        MagazineObject box =
            other.GetComponentInParent<MagazineObject>();

        if (box != null)
        {
            // 탄창 상자 데미지
            box.TakeDamage(damage);

            // 타격 확인용 로그
            Debug.Log($"<color=yellow>[Bullet] 총알이 상자를 타격했습니다! 데미지: {damage}</color>");

            // 총알 삭제
            Destroy(gameObject);

            return;
        }
    }
}