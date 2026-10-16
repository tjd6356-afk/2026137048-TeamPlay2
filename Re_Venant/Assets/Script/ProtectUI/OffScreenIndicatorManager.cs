using System.Collections.Generic;
using UnityEngine;

public class OffScreenIndicatorManager : MonoBehaviour
{
    // 싱글톤 패턴으로 어디서든 쉽게 접근 가능하게 설정
    public static OffScreenIndicatorManager Instance { get; private set; }

    [Header("UI Settings")]
    [SerializeField, Tooltip("화면 가장자리에 표시될 화살표/아이콘 UI 프리팹")]
    private GameObject indicatorPrefab;
    [SerializeField, Tooltip("지시표시기가 생성될 캔버스(Canvas)")]
    private RectTransform canvasRect;

    [Header("Padding")]
    [SerializeField, Tooltip("화면 끝에서 안쪽으로 얼마나 띄울지 (픽셀 단위)")]
    private float edgePadding = 50f;

    private Camera mainCamera;

    // 활성화된 인디케이터들을 보관하는 딕셔너리 (탄창 타겟 : 생성된 UI)
    private Dictionary<Transform, RectTransform> activeIndicators = new Dictionary<Transform, RectTransform>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        if (canvasRect == null)
            canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    // 타겟(탄창)이 스폰될 때 인디케이터 UI 생성
    public void RegisterTarget(Transform target)
    {
        if (!activeIndicators.ContainsKey(target) && indicatorPrefab != null)
        {
            GameObject newIndicator = Instantiate(indicatorPrefab, canvasRect);


            newIndicator.transform.SetAsFirstSibling();

            activeIndicators.Add(target, newIndicator.GetComponent<RectTransform>());
        }
    }

    // 타겟(탄창)이 파괴될 때 인디케이터 UI 제거
    public void UnregisterTarget(Transform target)
    {
        if (activeIndicators.ContainsKey(target))
        {
            if (activeIndicators[target] != null)
                Destroy(activeIndicators[target].gameObject);

            activeIndicators.Remove(target);
        }
    }

    private void LateUpdate()
    {
        // 등록된 모든 타겟의 위치를 매 프레임 계산
        foreach (var pair in activeIndicators)
        {
            Transform target = pair.Key;
            RectTransform indicatorUI = pair.Value;

            if (target == null) continue; // 삭제 진행 중인 객체 무시

            UpdateIndicatorPosition(target, indicatorUI);
        }
    }

    private void UpdateIndicatorPosition(Transform target, RectTransform indicatorUI)
    {
        // 1. 타겟의 위치를 월드 좌표에서 화면(스크린) 좌표로 변환
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // 카메라의 Z값이 0보다 작으면 타겟이 카메라 뒤에 있는 것
        bool isBehind = screenPos.z < 0;

        // 만약 타겟이 카메라 뒤에 있다면, 화면 좌표를 반전시켜 화면 가장자리에 꽂히도록 유도
        if (isBehind)
        {
            screenPos *= -1;
        }

        // 2. 화면 안에 있는지 확인 (0 ~ 화면 가로/세로 길이)
        bool isOffScreen = screenPos.x <= 0 || screenPos.x >= Screen.width ||
                           screenPos.y <= 0 || screenPos.y >= Screen.height || isBehind;

        if (isOffScreen)
        {
            // 화면 밖이면 UI 활성화
            if (!indicatorUI.gameObject.activeSelf)
                indicatorUI.gameObject.SetActive(true);

            // 3. 화면 중심을 (0,0)으로 맞추기 위해 변환
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 direction = (new Vector2(screenPos.x, screenPos.y) - screenCenter).normalized;

            // 4. 화면 밖으로 나가지 않도록 경계선 계산 (y = mx 공식을 이용한 교점 계산)
            float m = direction.y / direction.x;

            Vector2 boundsPos = Vector2.zero;

            // 화면 끝에서 여백(padding)을 뺀 실제 UI 이동 가능 한계선
            float boundsX = screenCenter.x - edgePadding;
            float boundsY = screenCenter.y - edgePadding;

            // X축 경계에 닿는지, Y축 경계에 닿는지 계산
            if (direction.x > 0) // 우측
            {
                boundsPos.x = boundsX;
                boundsPos.y = boundsX * m;
            }
            else // 좌측
            {
                boundsPos.x = -boundsX;
                boundsPos.y = -boundsX * m;
            }

            // 계산된 Y값이 화면 위아래 경계를 벗어나면 위/아래 경계로 다시 맞춤
            if (boundsPos.y > boundsY) // 상단
            {
                boundsPos.y = boundsY;
                boundsPos.x = boundsY / m;
            }
            else if (boundsPos.y < -boundsY) // 하단
            {
                boundsPos.y = -boundsY;
                boundsPos.x = -boundsY / m;
            }

            // 5. 다시 화면 좌측 하단을 (0,0)으로 하는 스크린 좌표로 원복
            boundsPos += screenCenter;

            // 6. UI 위치 적용
            indicatorUI.position = boundsPos;

            // 7. (선택) 화살표 아이콘이라면 타겟 방향으로 회전시키기
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // 화살표 이미지의 기본 방향에 따라 offset 추가 (보통 오른쪽을 바라보는 이미지면 offset 0)
            indicatorUI.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // 화면 안에 타겟이 보인다면 UI 숨기기
            if (indicatorUI.gameObject.activeSelf)
                indicatorUI.gameObject.SetActive(false);
        }
    }
}