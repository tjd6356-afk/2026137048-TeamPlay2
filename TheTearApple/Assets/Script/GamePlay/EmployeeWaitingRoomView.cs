using System.Collections.Generic;
using UnityEngine;

public class EmployeeWaitingRoomView : MonoBehaviour
{
    [Header("대기실")]
    public SpriteRenderer waitingRoomRenderer;

    [Header("직원 프리팹")]
    public EmployeeView employeePrefab;

    [Header("배치 설정")]
    public int columns = 5;

    public float horizontalPadding = 0.5f;
    public float verticalPadding = 0.5f;

    private Dictionary<int, EmployeeView> employeeViews
        = new Dictionary<int, EmployeeView>();


    private void Start()
    {
        if (PrototypeGameManager.Instance != null)
        {
            PrototypeGameManager.Instance.OnStateChanged += Refresh;
        }

        Refresh();
    }


    private void OnDestroy()
    {
        if (PrototypeGameManager.Instance != null)
        {
            PrototypeGameManager.Instance.OnStateChanged -= Refresh;
        }
    }


    public void Refresh()
    {
        PrototypeGameManager manager =
            PrototypeGameManager.Instance;

        if (manager == null)
            return;

        if (manager.state == null)
            return;

        CreateMissingEmployees(manager);

        RemoveMissingEmployees(manager);

        ArrangeEmployees(manager);

        UpdateSelection(manager);
    }


    // ============================================
    // 없는 직원 생성
    // ============================================

    private void CreateMissingEmployees(
        PrototypeGameManager manager)
    {
        foreach (EmployeeState employee
                 in manager.state.employees)
        {
            if (employeeViews.ContainsKey(employee.id))
                continue;

            EmployeeView newView =
                Instantiate(
                    employeePrefab,
                    transform
                );

            newView.Initialize(employee.id);

            employeeViews.Add(
                employee.id,
                newView
            );
        }
    }


    // ============================================
    // 죽거나 희생된 직원 제거
    // ============================================

    private void RemoveMissingEmployees(
        PrototypeGameManager manager)
    {
        List<int> removeIds =
            new List<int>();

        foreach (KeyValuePair<int, EmployeeView> pair
                 in employeeViews)
        {
            bool exists =
                manager.state.employees.Exists(
                    employee =>
                        employee.id == pair.Key
                );

            if (!exists)
            {
                Destroy(pair.Value.gameObject);

                removeIds.Add(pair.Key);
            }
        }


        foreach (int id in removeIds)
        {
            employeeViews.Remove(id);
        }
    }


    // ============================================
    // 대기실 안에 직원 정렬
    // ============================================

    private void ArrangeEmployees(
        PrototypeGameManager manager)
    {
        if (waitingRoomRenderer == null)
            return;

        int employeeCount =
            manager.state.employees.Count;

        if (employeeCount == 0)
            return;


        int rowCount =
            Mathf.CeilToInt(
                employeeCount /
                (float)columns
            );


        Bounds bounds =
            waitingRoomRenderer.bounds;


        float usableWidth =
            bounds.size.x -
            horizontalPadding * 2f;

        float usableHeight =
            bounds.size.y -
            verticalPadding * 2f;


        float cellWidth =
            usableWidth / columns;

        float cellHeight =
            usableHeight / rowCount;


        for (int i = 0;
             i < employeeCount;
             i++)
        {
            EmployeeState employee =
                manager.state.employees[i];

            if (!employeeViews.TryGetValue(
                    employee.id,
                    out EmployeeView view))
                continue;


            int column =
                i % columns;

            int row =
                i / columns;


            float x =
                bounds.min.x +
                horizontalPadding +
                cellWidth * column +
                cellWidth / 2f;


            float y =
                bounds.max.y -
                verticalPadding -
                cellHeight * row -
                cellHeight / 2f;


            view.transform.position =
                new Vector3(
                    x,
                    y,
                    0f
                );
        }
    }


    // ============================================
    // 현재 선택 표시
    // ============================================

    private void UpdateSelection(
        PrototypeGameManager manager)
    {
        foreach (KeyValuePair<int, EmployeeView> pair
                 in employeeViews)
        {
            bool selected =
                pair.Key ==
                manager.selectedEmployeeId;

            pair.Value.SetSelected(selected);
        }
    }
}