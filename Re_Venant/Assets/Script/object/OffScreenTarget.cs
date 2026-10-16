using UnityEngine;

public class OffScreenTarget : MonoBehaviour
{
    private void Start()
    {
        // 자신이 스폰되면 매니저에게 나를 등록해달라고 요청
        if (OffScreenIndicatorManager.Instance != null)
        {
            OffScreenIndicatorManager.Instance.RegisterTarget(transform);
        }
    }

    private void OnDestroy()
    {
        // 플레이어가 먹거나 파괴되어 사라질 때 매니저에게 나를 지워달라고 요청
        if (OffScreenIndicatorManager.Instance != null)
        {
            OffScreenIndicatorManager.Instance.UnregisterTarget(transform);
        }
    }
}