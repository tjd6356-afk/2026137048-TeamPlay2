using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    private float damage;
    private Transform owner;

    // 한 번의 공격에서 같은 적을 여러 번 때리지 않도록 저장
    private readonly HashSet<EnemyHealth> hitEnemies =
        new HashSet<EnemyHealth>();

    public void Initialize(
        float damageAmount,
        float lifeTime,
        Transform attackOwner)
    {
        damage = damageAmount;
        owner = attackOwner;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamageEnemy(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // 이미 겹친 상태로 생성된 경우도 감지하기 위해 사용
        TryDamageEnemy(other);
    }

    private void TryDamageEnemy(Collider other)
    {
        // 자기 자신 공격 방지
        if (owner != null &&
            other.transform.IsChildOf(owner))
        {
            return;
        }

        EnemyHealth enemy =
            other.GetComponentInParent<EnemyHealth>();

        if (enemy == null)
            return;

        // 이미 이번 공격에 맞은 적이면 무시
        if (hitEnemies.Contains(enemy))
            return;

        hitEnemies.Add(enemy);

        enemy.TakeDamage(damage);
    }
}