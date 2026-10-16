using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    private float damage;
    private Transform owner;

    // 한 번의 공격에서 같은 적을 여러 번 때리지 않도록 저장
    private readonly HashSet<EnemyHealth> hitEnemies =
        new HashSet<EnemyHealth>();

    // [추가됨] 한 번의 공격에서 같은 상자를 여러 번 때리지 않도록 저장
    private readonly HashSet<MagazineObject> hitMagazines =
        new HashSet<MagazineObject>();

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
        TryDamageMagazine(other); // [추가됨] 상자 타격 시도
    }

    private void OnTriggerStay(Collider other)
    {
        // 이미 겹친 상태로 생성된 경우도 감지하기 위해 사용
        TryDamageEnemy(other);
        TryDamageMagazine(other); // [추가됨] 상자 타격 시도
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

    // [추가됨] 탄창 상자를 타격하기 위한 함수 (기존 구조와 동일하게 작성)
    private void TryDamageMagazine(Collider other)
    {
        // 자기 자신 공격 방지 (혹시 모를 상황 대비)
        if (owner != null &&
            other.transform.IsChildOf(owner))
        {
            return;
        }

        MagazineObject box =
            other.GetComponentInParent<MagazineObject>();

        if (box == null)
            return;

        // 이미 이번 공격에 맞은 상자면 무시
        if (hitMagazines.Contains(box))
            return;

        hitMagazines.Add(box);

        // 확인용 로그
        Debug.Log($"<color=cyan>[Hitbox] 상자를 타격했습니다! 데미지: {damage}</color>");

        box.TakeDamage(damage);
    }
}