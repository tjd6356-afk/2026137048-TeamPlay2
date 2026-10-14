using UnityEngine;
using UnityEngine.AI;

public class ChaserEnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private PlayerBulletHealth player;

    [Header("Attack")]
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float attackRange = 0.1f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;

    private Collider playerCollider;

    private float nextAttackTime;
    private bool wasInAttackRange = false;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Start()
    {
        // Player를 Inspector에서 등록하지 않았다면 자동 검색
        if (player == null)
        {
            player =
                FindFirstObjectByType<PlayerBulletHealth>();
        }

        if (player != null)
        {
            playerCollider =
                player.GetComponent<Collider>();

            // Player Root에 Collider가 없으면 자식에서도 검색
            if (playerCollider == null)
            {
                playerCollider =
                    player.GetComponentInChildren<Collider>();
            }

            SetupStoppingDistance();
        }
    }

    private void Update()
    {
        // Player가 이미 삭제되었거나 존재하지 않음
        if (player == null)
        {
            StopAgent();
            return;
        }

        // Player 사망
        if (player.IsDead)
        {
            StopAgent();
            return;
        }

        if (agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        float distanceToPlayer =
            GetDistanceToPlayerSurface();

        float attackDistance =
            agent.radius + attackRange;

        if (distanceToPlayer <= attackDistance)
        {
            if (!wasInAttackRange)
            {
                // 처음 닿은 순간 바로 공격 가능
                nextAttackTime = 0f;
            }

            wasInAttackRange = true;

            StopAndAttack();
        }
        else
        {
            wasInAttackRange = false;

            ChasePlayer();
        }
    }

    // --------------------------------------------------
    // Player 추적
    // --------------------------------------------------

    private void ChasePlayer()
    {
        if (player == null)
            return;

        agent.isStopped = false;

        agent.SetDestination(
            player.transform.position
        );
    }

    // --------------------------------------------------
    // Player 공격
    // --------------------------------------------------

    private void StopAndAttack()
    {
        agent.isStopped = true;

        FacePlayer();

        if (Time.time < nextAttackTime)
            return;

        player.TakeDamage(
            bulletDamage
        );

        nextAttackTime =
            Time.time + attackInterval;
    }

    // --------------------------------------------------
    // Player 방향 바라보기
    // --------------------------------------------------

    private void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 direction =
            player.transform.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    // --------------------------------------------------
    // Player 표면까지 거리 계산
    // --------------------------------------------------

    private float GetDistanceToPlayerSurface()
    {
        if (player == null)
        {
            return float.MaxValue;
        }

        Vector3 enemyPosition =
            transform.position;

        enemyPosition.y = 0f;

        // Collider가 없으면 Transform 거리 사용
        if (playerCollider == null)
        {
            Vector3 playerPosition =
                player.transform.position;

            playerPosition.y = 0f;

            return Vector3.Distance(
                enemyPosition,
                playerPosition
            );
        }

        // Player Collider에서
        // Enemy와 가장 가까운 지점
        Vector3 closestPoint =
            playerCollider.ClosestPoint(
                transform.position
            );

        closestPoint.y = 0f;

        return Vector3.Distance(
            enemyPosition,
            closestPoint
        );
    }

    // --------------------------------------------------
    // NavMeshAgent가 Player 안으로 들어가지 않도록 설정
    // --------------------------------------------------

    private void SetupStoppingDistance()
    {
        if (agent == null)
            return;

        float playerRadius = 0.5f;

        if (playerCollider != null)
        {
            Bounds bounds =
                playerCollider.bounds;

            playerRadius =
                Mathf.Max(
                    bounds.extents.x,
                    bounds.extents.z
                );
        }

        agent.stoppingDistance =
            playerRadius +
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
    }
}