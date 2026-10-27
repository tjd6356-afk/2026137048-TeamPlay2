using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIAnimator : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private Image backgroundImage;

    [Header("UI Elements")]
    [SerializeField] private RectTransform gameOverImage;
    [SerializeField] private RectTransform replayButton;
    [SerializeField] private RectTransform exitButton;
    [SerializeField] private RectTransform finalScoreText;

    [Header("Buttons")]
    [SerializeField] private Button replayButtonComponent;
    [SerializeField] private Button exitButtonComponent;

    [Header("Background Animation")]
    [SerializeField] private float backgroundFadeDuration = 1f;
    [SerializeField] private float afterBackgroundDelay = 0.3f;

    [Header("GameOver Image Animation")]
    [SerializeField] private float gameOverStartOffsetY = 700f;
    [SerializeField] private float gameOverMoveDuration = 0.7f;
    [SerializeField] private float afterGameOverDelay = 0.25f;

    [Header("Button Animation")]
    [SerializeField] private float buttonStartOffsetY = 500f;
    [SerializeField] private float buttonMoveDuration = 0.45f;
    [SerializeField] private float buttonInterval = 0.15f;

    [Header("Final Score Animation")]
    [SerializeField] private float scoreStartOffsetX = 700f;
    [SerializeField] private float scoreMoveDuration = 0.55f;

    private Vector2 gameOverTargetPosition;
    private Vector2 replayTargetPosition;
    private Vector2 exitTargetPosition;
    private Vector2 scoreTargetPosition;

    private float backgroundTargetAlpha = 1f;

    private Coroutine animationCoroutine;

    private void Awake()
    {
        // Unity에서 현재 배치한 위치를
        // 최종 도착 위치로 저장
        if (gameOverImage != null)
        {
            gameOverTargetPosition =
                gameOverImage.anchoredPosition;
        }

        if (replayButton != null)
        {
            replayTargetPosition =
                replayButton.anchoredPosition;
        }

        if (exitButton != null)
        {
            exitTargetPosition =
                exitButton.anchoredPosition;
        }

        if (finalScoreText != null)
        {
            scoreTargetPosition =
                finalScoreText.anchoredPosition;
        }

        if (backgroundImage != null)
        {
            backgroundTargetAlpha =
                backgroundImage.color.a;
        }
    }

    public void PlayGameOverAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        ResetAnimation();

        animationCoroutine =
            StartCoroutine(
                GameOverSequence()
            );
    }

    // =========================================
    // 처음 위치로 초기화
    // =========================================

    private void ResetAnimation()
    {
        // 배경 투명
        if (backgroundImage != null)
        {
            Color color =
                backgroundImage.color;

            color.a = 0f;

            backgroundImage.color =
                color;
        }

        // GameOver 이미지는 위쪽
        if (gameOverImage != null)
        {
            gameOverImage.anchoredPosition =
                gameOverTargetPosition +
                Vector2.up *
                gameOverStartOffsetY;
        }

        // Replay 버튼은 아래쪽
        if (replayButton != null)
        {
            replayButton.anchoredPosition =
                replayTargetPosition +
                Vector2.down *
                buttonStartOffsetY;
        }

        // Exit 버튼도 아래쪽
        if (exitButton != null)
        {
            exitButton.anchoredPosition =
                exitTargetPosition +
                Vector2.down *
                buttonStartOffsetY;
        }

        // Final Score는 왼쪽
        if (finalScoreText != null)
        {
            finalScoreText.anchoredPosition =
                scoreTargetPosition +
                Vector2.left *
                scoreStartOffsetX;
        }

        // 연출 중 버튼 클릭 방지
        if (replayButtonComponent != null)
        {
            replayButtonComponent.interactable =
                false;
        }

        if (exitButtonComponent != null)
        {
            exitButtonComponent.interactable =
                false;
        }
    }

    // =========================================
    // 전체 GameOver 연출
    // =========================================

    private IEnumerator GameOverSequence()
    {
        // -------------------------------------
        // 1. 배경 Fade
        // -------------------------------------

        yield return FadeBackground();

        yield return new WaitForSecondsRealtime(
            afterBackgroundDelay
        );

        // -------------------------------------
        // 2. GameOver 이미지
        // 위 → 아래
        // -------------------------------------

        if (gameOverImage != null)
        {
            yield return MoveUI(
                gameOverImage,
                gameOverImage.anchoredPosition,
                gameOverTargetPosition,
                gameOverMoveDuration
            );
        }

        yield return new WaitForSecondsRealtime(
            afterGameOverDelay
        );

        // -------------------------------------
        // 3. Replay Button
        // 아래 → 위
        // -------------------------------------

        if (replayButton != null)
        {
            yield return MoveUI(
                replayButton,
                replayButton.anchoredPosition,
                replayTargetPosition,
                buttonMoveDuration
            );
        }

        if (replayButtonComponent != null)
        {
            replayButtonComponent.interactable =
                true;
        }

        yield return new WaitForSecondsRealtime(
            buttonInterval
        );

        // -------------------------------------
        // 4. Exit Button
        // 아래 → 위
        // -------------------------------------

        if (exitButton != null)
        {
            yield return MoveUI(
                exitButton,
                exitButton.anchoredPosition,
                exitTargetPosition,
                buttonMoveDuration
            );
        }

        if (exitButtonComponent != null)
        {
            exitButtonComponent.interactable =
                true;
        }

        yield return new WaitForSecondsRealtime(
            buttonInterval
        );

        // -------------------------------------
        // 5. Final Score
        // 왼쪽 → 오른쪽
        // -------------------------------------

        if (finalScoreText != null)
        {
            yield return MoveUI(
                finalScoreText,
                finalScoreText.anchoredPosition,
                scoreTargetPosition,
                scoreMoveDuration
            );
        }

        animationCoroutine = null;
    }

    // =========================================
    // Background Fade
    // =========================================

    private IEnumerator FadeBackground()
    {
        if (backgroundImage == null)
            yield break;

        float timer = 0f;

        while (timer < backgroundFadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    backgroundFadeDuration
                );

            t = EaseOutCubic(t);

            Color color =
                backgroundImage.color;

            color.a =
                Mathf.Lerp(
                    0f,
                    backgroundTargetAlpha,
                    t
                );

            backgroundImage.color =
                color;

            yield return null;
        }

        Color finalColor =
            backgroundImage.color;

        finalColor.a =
            backgroundTargetAlpha;

        backgroundImage.color =
            finalColor;
    }

    // =========================================
    // UI 이동
    // =========================================

    private IEnumerator MoveUI(
        RectTransform target,
        Vector2 startPosition,
        Vector2 endPosition,
        float duration)
    {
        if (target == null)
            yield break;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    timer / duration
                );

            // 처음 빠르게, 마지막 부드럽게 감속
            t = EaseOutCubic(t);

            target.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    endPosition,
                    t
                );

            yield return null;
        }

        target.anchoredPosition =
            endPosition;
    }

    private float EaseOutCubic(float t)
    {
        return 1f -
               Mathf.Pow(
                   1f - t,
                   3f
               );
    }
}