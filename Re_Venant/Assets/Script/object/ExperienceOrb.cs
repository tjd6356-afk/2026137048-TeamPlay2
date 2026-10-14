using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] private int experienceAmount = 1;

    [Header("Magnet")]
    [SerializeField] private float attractionRange = 4f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;

    [Header("Pickup")]
    [SerializeField] private float pickupDistance = 0.4f;

    private PlayerExperience player;

    private float currentSpeed = 0f;
    private bool isAttracted = false;

    private void Start()
    {
        player =
            FindFirstObjectByType<PlayerExperience>();
    }

    private void Update()
    {
        if (player == null)
            return;

        Vector3 playerPosition =
            player.transform.position;

        Vector3 direction =
            playerPosition - transform.position;

        float distance =
            direction.magnitude;

        // 플레이어가 일정 거리 안에 들어옴
        if (distance <= attractionRange)
        {
            isAttracted = true;
        }

        if (!isAttracted)
            return;

        MoveToPlayer(direction);

        // 충분히 가까우면 획득
        if (distance <= pickupDistance)
        {
            Collect();
        }
    }

    private void MoveToPlayer(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        currentSpeed =
            Mathf.MoveTowards(
                currentSpeed,
                moveSpeed,
                acceleration * Time.deltaTime
            );

        transform.position +=
            direction.normalized *
            currentSpeed *
            Time.deltaTime;
    }

    private void Collect()
    {
        if (player == null)
            return;

        player.AddExperience(
            experienceAmount
        );

        Destroy(gameObject);
    }

    public void SetExperience(int amount)
    {
        experienceAmount =
            Mathf.Max(1, amount);
    }
}