using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 15f;      // 탄환 속도
    [SerializeField] private float maxDistance = 20f;// 최대 이동 거리

    private Vector3 startPosition;

    private void Start()
    {
        // 생성된 위치 저장
        startPosition = transform.position;

        // 임시로 노란색 구체로 만들기 위해 머티리얼 색상 변경
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
    }

    private void Update()
    {
        // 초당 speed만큼 앞(forward)으로 이동
        // PlayerShooter에서 Instantiate할 때 이미 목적지를 바라보게 회전시켰으므로 forward로 전진하면 됩니다.
        transform.position += transform.forward * speed * Time.deltaTime;

        // 시작 위치와 현재 위치의 거리를 계산
        float traveledDistance = Vector3.Distance(startPosition, transform.position);

        // 정해진 거리 이상 날아갔다면 파괴
        if (traveledDistance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    // (선택) 무언가에 부딪혔을 때 파괴되도록 하려면 아래 코드를 활용하세요.
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) 
        {
            Destroy(gameObject);
        }
    }
    */
}