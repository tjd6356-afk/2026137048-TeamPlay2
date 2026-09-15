using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OtherworldSelectionCard :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("UI")]
    public Image worldImage;
    public TMP_Text nameText;
    public GameObject selectedMark;

    private OtherworldData data;
    private OtherworldSelectionUI owner;

    private bool selected = false;


    public void Setup(
        OtherworldData newData,
        OtherworldSelectionUI newOwner)
    {
        data = newData;
        owner = newOwner;

        selected = false;

        if (selectedMark != null)
            selectedMark.SetActive(false);

        if (data == null)
            return;

        if (worldImage != null)
            worldImage.sprite = data.selectionImage;

        if (nameText != null)
            nameText.text = data.worldName;
    }


    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (data == null)
            return;

        owner.ShowHover(data);
    }


    public void OnPointerExit(
        PointerEventData eventData)
    {
        owner.HideHover();
    }


    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (selected)
            return;

        if (data == null)
            return;

        owner.TrySelect(
            data,
            this
        );
    }


    public void MarkSelected()
    {
        selected = true;

        if (selectedMark != null)
            selectedMark.SetActive(true);
    }
}