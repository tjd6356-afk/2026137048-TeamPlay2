using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProtectHPUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ProtectHealth protect;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas canvas;

    [Header("UI")]
    [SerializeField] private RectTransform indicatorRoot;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private TMP_Text hpText;

    [Header("Optional Direction Arrow")]
    [SerializeField] private RectTransform directionArrow;

    [Header("On Screen")]
    [SerializeField]
    private Vector2 onScreenOffset =
        new Vector2(0f, 80f);

    [Header("Off Screen")]
    [SerializeField] private float edgePadding = 30f;

    [Header("Arrow")]
    [SerializeField] private float arrowRotationOffset = -90f;

    private RectTransform canvasRect;
    private Camera uiCamera;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (indicatorRoot == null)
        {
            indicatorRoot = GetComponent<RectTransform>();
        }

        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                uiCamera = null;
            }
            else
            {
                uiCamera = canvas.worldCamera;
            }
        }
    }

    private void Start()
    {
        if (protect == null)
        {
            protect =
                FindFirstObjectByType<ProtectHealth>();
        }

        UpdateHealthUI();
    }

    private void LateUpdate()
    {
        if (protect == null)
        {
            gameObject.SetActive(false);
            return;
        }

        UpdateHealthUI();
        UpdateIndicatorPosition();
    }

    // --------------------------------------------------
    // HP 표시
    // --------------------------------------------------

    private void UpdateHealthUI()
    {
        if (protect == null)
            return;

        float currentHP =
            protect.CurrentHealth;

        float maxHP =
            protect.MaxHealth;

        float hpRatio = 0f;

        if (maxHP > 0f)
        {
            hpRatio =
                currentHP / maxHP;
        }

        if (hpFillImage != null)
        {
            hpFillImage.fillAmount =
                Mathf.Clamp01(hpRatio);
        }

        if (hpText != null)
        {
            hpText.text =
                $"{Mathf.CeilToInt(currentHP)} / {Mathf.CeilToInt(maxHP)}";
        }
    }

    // --------------------------------------------------
    // UI 위치
    // --------------------------------------------------

    private void UpdateIndicatorPosition()
    {
        if (mainCamera == null)
            return;

        if (canvasRect == null)
            return;

        if (indicatorRoot == null)
            return;

        Vector3 viewportPosition =
            mainCamera.WorldToViewportPoint(
                protect.transform.position
            );

        bool isInFront =
            viewportPosition.z > 0f;

        bool isInsideScreen =
            isInFront &&
            viewportPosition.x >= 0f &&
            viewportPosition.x <= 1f &&
            viewportPosition.y >= 0f &&
            viewportPosition.y <= 1f;

        if (isInsideScreen)
        {
            SetOnScreenPosition();
        }
        else
        {
            SetOffScreenPosition(
                viewportPosition
            );
        }
    }

    // --------------------------------------------------
    // Protect가 화면 안에 있을 때
    // --------------------------------------------------

    private void SetOnScreenPosition()
    {
        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(
                protect.transform.position
            );

        screenPosition.x +=
            onScreenOffset.x;

        screenPosition.y +=
            onScreenOffset.y;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            uiCamera,
            out Vector2 localPosition
        );

        indicatorRoot.anchoredPosition =
            localPosition;

        // 화면 안에서는 방향 화살표 숨김
        if (directionArrow != null)
        {
            directionArrow.gameObject.SetActive(false);
        }
    }

    // --------------------------------------------------
    // Protect가 화면 밖에 있을 때
    // --------------------------------------------------

    private void SetOffScreenPosition(
        Vector3 viewportPosition)
    {
        // 화면 중심 기준 Protect 방향
        Vector2 direction =
            new Vector2(
                viewportPosition.x - 0.5f,
                viewportPosition.y - 0.5f
            );

        // Protect가 카메라 뒤쪽에 있을 경우
        if (viewportPosition.z < 0f)
        {
            direction = -direction;
        }

        if (direction.sqrMagnitude < 0.001f)
        {
            direction = Vector2.up;
        }

        direction.Normalize();

        float halfWidth =
            canvasRect.rect.width * 0.5f;

        float halfHeight =
            canvasRect.rect.height * 0.5f;

        // UI 자체 크기까지 고려
        float indicatorHalfWidth =
            indicatorRoot.rect.width * 0.5f;

        float indicatorHalfHeight =
            indicatorRoot.rect.height * 0.5f;

        float usableWidth =
            halfWidth
            - indicatorHalfWidth
            - edgePadding;

        float usableHeight =
            halfHeight
            - indicatorHalfHeight
            - edgePadding;

        usableWidth =
            Mathf.Max(0f, usableWidth);

        usableHeight =
            Mathf.Max(0f, usableHeight);

        // 화면 중앙에서 Protect 방향으로 직선을 그어서
        // 화면 가장자리와 만나는 위치 계산
        float scaleX =
            Mathf.Abs(direction.x) > 0.001f
                ? usableWidth / Mathf.Abs(direction.x)
                : float.MaxValue;

        float scaleY =
            Mathf.Abs(direction.y) > 0.001f
                ? usableHeight / Mathf.Abs(direction.y)
                : float.MaxValue;

        float scale =
            Mathf.Min(scaleX, scaleY);

        Vector2 edgePosition =
            direction * scale;

        indicatorRoot.anchoredPosition =
            edgePosition;

        UpdateArrow(direction);
    }

    // --------------------------------------------------
    // 화살표 회전
    // --------------------------------------------------

    private void UpdateArrow(Vector2 direction)
    {
        if (directionArrow == null)
            return;

        directionArrow.gameObject.SetActive(true);

        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

        directionArrow.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                angle + arrowRotationOffset
            );
    }
}