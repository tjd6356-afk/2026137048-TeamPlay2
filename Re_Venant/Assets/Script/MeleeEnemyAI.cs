using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform protectTarget;

    [Header("Attack")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackInterval = 1f;

    private ProtectHealth touchingProtect;
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
        // Protect를 직접 등록하지 않았으면 자동 검색
        if (protectTarget == null)
        {
            ProtectHealth protect =
                FindFirstObjectByType<ProtectHealth>();

            if (protect != null)
            {
                protectTarget = protect.transform;
            }
        }
    }

    private void Update()
    {
        if (protectTarget == null)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }

            return;
        }

        ChaseProtect();
        AttackProtect();
    }

    private void ChaseProtect()
    {
        if (agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        // Protect와 접촉 중이면 이동 정지
        if (touchingProtect != null)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(protectTarget.position);
    }

    private void AttackProtect()
    {
        if (touchingProtect == null)
            return;

        if (touchingProtect.IsDead)
            return;

        if (Time.time < nextAttackTime)
            return;

        touchingProtect.TakeDamage(damage);

        nextAttackTime =
            Time.time + attackInterval;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ProtectHealth protect =
            collision.collider.GetComponentInParent<ProtectHealth>();

        if (protect == null)
            return;

        touchingProtect = protect;

        // 닿은 순간 바로 첫 공격
        nextAttackTime = 0f;
    }

    private void OnCollisionExit(Collision collision)
    {
        ProtectHealth protect =
            collision.collider.GetComponentInParent<ProtectHealth>();

        if (protect == null)
            return;

        if (touchingProtect == protect)
        {
            touchingProtect = null;
        }
    }
}