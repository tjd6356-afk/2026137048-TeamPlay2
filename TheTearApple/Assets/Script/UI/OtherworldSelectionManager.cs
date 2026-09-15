using System.Collections.Generic;
using UnityEngine;

public class OtherworldSelectionManager : MonoBehaviour
{
    [Header("Day 1 후보 - 3개")]
    public List<OtherworldData> day1Candidates =
        new List<OtherworldData>();

    [Header("격리실")]
    public IsolationRoom room1;
    public IsolationRoom room2;

    private List<OtherworldData> selectedWorlds =
        new List<OtherworldData>();

    public bool SelectionFinished
    {
        get;
        private set;
    }

    public int SelectedCount
    {
        get
        {
            return selectedWorlds.Count;
        }
    }


    private void Start()
    {
        StartDay1Selection();
    }


    public void StartDay1Selection()
    {
        selectedWorlds.Clear();

        SelectionFinished = false;

        if (room1 != null)
            room1.ClearRoom();

        if (room2 != null)
            room2.ClearRoom();

        Debug.Log("Day 1 이면세계 선택 시작");
    }


    public bool IsSelected(OtherworldData data)
    {
        return selectedWorlds.Contains(data);
    }


    public void SelectOtherworld(OtherworldData data)
    {
        if (SelectionFinished)
            return;

        if (data == null)
            return;

        if (selectedWorlds.Contains(data))
        {
            Debug.Log("이미 선택한 이면세계입니다.");
            return;
        }

        // 첫 번째 선택
        if (selectedWorlds.Count == 0)
        {
            if (room1 == null)
            {
                Debug.LogError("1번 격리실이 연결되지 않았습니다.");
                return;
            }

            selectedWorlds.Add(data);

            room1.AssignOtherworld(data);

            Debug.Log(
                $"첫 번째 선택 : {data.worldName} → 1번 격리실"
            );

            return;
        }

        // 두 번째 선택
        if (selectedWorlds.Count == 1)
        {
            if (room2 == null)
            {
                Debug.LogError("2번 격리실이 연결되지 않았습니다.");
                return;
            }

            selectedWorlds.Add(data);

            room2.AssignOtherworld(data);

            Debug.Log(
                $"두 번째 선택 : {data.worldName} → 2번 격리실"
            );

            FinishSelection();
        }
    }


    private void FinishSelection()
    {
        SelectionFinished = true;

        Debug.Log("Day 1 이면세계 선택 완료");

        Debug.Log(
            $"1번 격리실 : {room1.CurrentOtherworld.worldName}"
        );

        Debug.Log(
            $"2번 격리실 : {room2.CurrentOtherworld.worldName}"
        );
    }
}