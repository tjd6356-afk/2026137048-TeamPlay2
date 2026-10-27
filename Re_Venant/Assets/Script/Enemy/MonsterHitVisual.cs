using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHitVisual : MonoBehaviour
{
    [Header("Monster Image")]
    [SerializeField] private Image monsterImage;

    [Header("Sprites")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hitSprite;

    [Header("Hit Image")]
    [Tooltip("피격 이미지를 유지하는 시간")]
    [SerializeField] private float hitSpriteDuration = 3f;

    [Header("Transparency Flash")]
    [Tooltip("피격 순간 가장 투명해지는 정도")]
    [Range(0f, 1f)]
    [SerializeField] private float hitAlpha = 0.25f;

    [Tooltip("투명해지는 시간")]
    [SerializeField] private float fadeOutDuration = 0.08f;

    [Tooltip("다시 불투명해지는 시간")]
    [SerializeField] private float fadeInDuration = 0.12f;

    private Coroutine hitCoroutine;

    private Color originalColor;

    private void Awake()
    {
        if (monsterImage == null)
        {
            monsterImage =
                GetComponent<Image>();
        }

        if (monsterImage != null)
        {
            originalColor =
                monsterImage.color;

            if (defaultSprite == null)
            {
                defaultSprite =
                    monsterImage.sprite;
            }
        }
    }

    public void PlayHitEffect()
    {
        if (monsterImage == null)
            return;

        // 이미 피격 연출 중이면
        // 기존 연출을 끊고 처음부터 다시 시작
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        // 혹시 이전 연출 도중 멈췄다면
        // 알파값 정상화
        SetAlpha(originalColor.a);

        hitCoroutine =
            StartCoroutine(
                HitEffectRoutine()
            );
    }

    private IEnumerator HitEffectRoutine()
    {
        // ============================
        // 피격 이미지로 변경
        // ============================

        if (hitSprite != null)
        {
            monsterImage.sprite =
                hitSprite;
        }

        // ============================
        // 잠깐 투명해짐
        // ============================

        yield return FadeAlpha(
            originalColor.a,
            hitAlpha,
            fadeOutDuration
        );

        // ============================
        // 다시 원래 불투명 상태
        // ============================

        yield return FadeAlpha(
            hitAlpha,
            originalColor.a,
            fadeInDuration
        );

        // ============================
        // 피격 이미지 유지
        // ============================

        float remainingTime =
            hitSpriteDuration -
            fadeOutDuration -
            fadeInDuration;

        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(
                remainingTime
            );
        }

        // ============================
        // 기본 이미지 복귀
        // ============================

        if (defaultSprite != null)
        {
            monsterImage.sprite =
                defaultSprite;
        }

        SetAlpha(originalColor.a);

        hitCoroutine = null;
    }

    private IEnumerator FadeAlpha(
        float startAlpha,
        float targetAlpha,
        float duration)
    {
        if (duration <= 0f)
        {
            SetAlpha(targetAlpha);
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / duration
                );

            float alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color color =
            monsterImage.color;

        color.a = alpha;

        monsterImage.color =
            color;
    }

    private void OnDisable()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }

        if (monsterImage != null)
        {
            if (defaultSprite != null)
            {
                monsterImage.sprite =
                    defaultSprite;
            }

            SetAlpha(originalColor.a);
        }
    }
}