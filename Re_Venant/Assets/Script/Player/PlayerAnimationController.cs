using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    private static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    private static readonly int ThrowHash =
        Animator.StringToHash("Throw");

    public void SetMoving(bool isMoving)
    {
        if (animator == null)
            return;

        animator.SetBool(
            IsMovingHash,
            isMoving
        );
    }

    public void PlayThrow()
    {
        if (animator == null)
            return;

        // 이전 Trigger가 남아 있을 가능성 방지
        animator.ResetTrigger(ThrowHash);
        animator.SetTrigger(ThrowHash);
    }
}