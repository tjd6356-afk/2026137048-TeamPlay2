using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 반드시 추가해야 합니다.

public class TitleUIManager : MonoBehaviour
{
    // 인스펙터에서 연결할 팝업 패널 변수
    public GameObject popupPanel;

    // 팝업 열기 함수
    public void OpenPopup()
    {
        popupPanel.SetActive(true); // 패널 활성화
    }

    // 팝업 닫기 함수
    public void ClosePopup()
    {
        popupPanel.SetActive(false); // 패널 비활성화
    }

    // 씬 이동 함수 (이동할 씬 이름 쓰면 됨)
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}