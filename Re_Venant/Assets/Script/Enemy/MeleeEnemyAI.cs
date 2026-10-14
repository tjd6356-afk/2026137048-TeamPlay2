using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ProtectHealth protect;

    [Header("Attack")]
    [SerializeField] private float damage = 10f;

    // Protect 표면과 Enemy 사이에 허용할 추가 공격 거리
    [SerializeField] private float attackRange = 0.1f;

    [SerializeField] private float attackInterval = 1f;

    private Collider protectCollider;

    private float nextAttackTime;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Start()
    {
        // Inspector에서 Protect를 넣지 않았으면 자동 검색
        if (protect == null)
        {
            protect = FindFirstObjectByType<ProtectHealth>();
        }

        if (protect != null)
        {
            protectCollider =
                protect.GetComponentInChildren<Collider>();

            SetupStoppingDistance();
        }
    }

    private void Update()
    {
        if (protect == null)
        {
            StopAgent();
            return;
        }

        if (protect.IsDead)
        {
            StopAgent();
            return;
        }

        if (agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        float distanceToSurface =
            GetDistanceToProtectSurface();

        // Enemy 자신의 반지름까지 포함해서 접촉 여부 판단
        float attackDistance =
            agent.radius + attackRange;

        if (distanceToSurface <= attackDistance)
        {
            AttackProtect();
        }
        else
        {
            ChaseProtect();
        }
    }

    private void ChaseProtect()
    {
        agent.isStopped = false;

        agent.SetDestination(
            protect.transform.position
        );
    }

    private void AttackProtect()
    {
        // Protect 앞에서 멈춤
        agent.isStopped = true;

        agent.ResetPath();

        // 공격 쿨타임
        if (Time.time < nextAttackTime)
            return;

        protect.TakeDamage(damage);

        nextAttackTime =
            Time.time + attackInterval;
    }

    private float GetDistanceToProtectSurface()
    {
        // Collider가 없으면 Transform 중심거리 사용
        if (protectCollider == null)
        {
            Vector3 enemyPosition =
                transform.position;

            Vector3 targetPosition =
                protect.transform.position;

            enemyPosition.y = 0f;
            targetPosition.y = 0f;

            return Vector3.Distance(
                enemyPosition,
                targetPosition
            );
        }

        Vector3 enemyPos =
            transform.position;

        // Protect Collider 표면에서
        // Enemy와 가장 가까운 지점
        Vector3 closestPoint =
            protectCollider.ClosestPoint(enemyPos);

        // 탑다운 게임이므로 높이(Y)는 무시
        enemyPos.y = 0f;
        closestPoint.y = 0f;

        return Vector3.Distance(
            enemyPos,
            closestPoint
        );
    }

    private void SetupStoppingDistance()
    {
        if (agent == null)
            return;

        if (protectCollider == null)
            return;

        Bounds bounds =
            protectCollider.bounds;

        // Protect의 X/Z 방향 크기 중 큰 값을
        // 대략적인 반지름으로 사용
        float protectRadius =
            Mathf.Max(
                bounds.extents.x,
                bounds.extents.z
            );

        agent.stoppingDistance =
            protectRadius +
            agent.radius +
            attackRange;
    }

    private void StopAgent()
    {
        if (agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = true;
        agent.ResetPath();
    }
}