using TMPro;
using UnityEngine;

public class OtherworldSelectionUI : MonoBehaviour
{
    [Header("선택 시스템")]
    public OtherworldSelectionManager selectionManager;

    [Header("전체 선택 패널")]
    public GameObject selectionPanel;

    [Header("후보 카드")]
    public OtherworldSelectionCard card1;
    public OtherworldSelectionCard card2;
    public OtherworldSelectionCard card3;

    [Header("Hover UI")]
    public GameObject hoverPanel;
    public TMP_Text hoverNameText;
    public TMP_Text hoverDescriptionText;


    private void Start()
    {
        SetupDay1Cards();
    }


    private void SetupDay1Cards()
    {
        if (selectionManager == null)
        {
            Debug.LogError(
                "OtherworldSelectionManager가 연결되지 않았습니다."
            );

            return;
        }

        if (selectionManager.day1Candidates.Count < 3)
        {
            Debug.LogError(
                "Day 1 이면세계 후보가 3개 필요합니다."
            );

            return;
        }

        if (selectionPanel != null)
            selectionPanel.SetActive(true);

        if (hoverPanel != null)
            hoverPanel.SetActive(false);


        card1.Setup(
            selectionManager.day1Candidates[0],
            this
        );

        card2.Setup(
            selectionManager.day1Candidates[1],
            this
        );

        card3.Setup(
            selectionManager.day1Candidates[2],
            this
        );
    }


    public void ShowHover(
        OtherworldData data)
    {
        if (data == null)
            return;

        if (hoverPanel != null)
            hoverPanel.SetActive(true);

        if (hoverNameText != null)
        {
            hoverNameText.text =
                data.worldName;
        }

        if (hoverDescriptionText != null)
        {
            hoverDescriptionText.text =
                data.hoverDescription;
        }
    }


    public void HideHover()
    {
        if (hoverPanel != null)
            hoverPanel.SetActive(false);
    }


    public void TrySelect(
        OtherworldData data,
        OtherworldSelectionCard card)
    {
        if (selectionManager == null)
            return;

        if (selectionManager.SelectionFinished)
            return;

        if (selectionManager.IsSelected(data))
            return;


        selectionManager.SelectOtherworld(data);


        // 정상적으로 선택되었는지 다시 확인
        if (selectionManager.IsSelected(data))
        {
            card.MarkSelected();
        }


        HideHover();


        // 두 번째 선택까지 완료
        if (selectionManager.SelectionFinished)
        {
            FinishUI();
        }
    }


    private void FinishUI()
    {
        Debug.Log(
            "이면세계 선택 UI 종료"
        );

        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }
}