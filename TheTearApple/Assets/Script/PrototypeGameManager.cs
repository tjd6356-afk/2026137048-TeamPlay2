using System;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeGameManager : MonoBehaviour
{
    public static PrototypeGameManager Instance { get; private set; }

    [Header("시작 설정")]
    public int startingEmployeeCount = 5;

    public int startingQuota = 10;

    [Header("직원 보충")]
    public int recruitRequiredPower = 3;
    public int recruitEmployeeCount = 4;

    [Header("현재 상태")]
    public PrototypeGameState state;

    [Header("선택된 직원")]
    public int selectedEmployeeId = -1;

    public string LastMessage { get; private set; }

    public event Action OnStateChanged;

    private PrototypeGameState dayStartSnapshot;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartNewGame();
    }

    // =========================================
    // 게임 시작
    // =========================================

    private void StartNewGame()
    {
        state = new PrototypeGameState();

        state.day = 1;
        state.quotaTarget = startingQuota;
        state.quotaCurrent = 0;
        state.nextEmployeeId = 1;

        state.equipments = new List<EquipmentStock>()
        {
            new EquipmentStock(EquipmentType.Mask, 2),
            new EquipmentStock(EquipmentType.Suit, 2),
            new EquipmentStock(EquipmentType.Weapon, 2)
        };

        for (int i = 0; i < startingEmployeeCount; i++)
        {
            AddEmployee(false);
        }

        selectedEmployeeId = -1;

        SaveDayStart();

        SetMessage("Day 1 시작");
    }

    // =========================================
    // 직원
    // =========================================

    private void AddEmployee(bool alreadyWorkedToday)
    {
        EmployeeState employee =
            new EmployeeState(state.nextEmployeeId);

        employee.workedToday = alreadyWorkedToday;

        state.nextEmployeeId++;

        state.employees.Add(employee);
    }

    public void SelectEmployee(int id)
    {
        EmployeeState employee =
            GetEmployee(id);

        if (employee == null)
            return;

        selectedEmployeeId = id;

        SetMessage(
            $"직원 {id} 선택 / 성장도 {employee.power}"
        );
    }

    public EmployeeState GetSelectedEmployee()
    {
        return GetEmployee(selectedEmployeeId);
    }

    private EmployeeState GetEmployee(int id)
    {
        return state.employees.Find(
            employee => employee.id == id
        );
    }

    // =========================================
    // 장비
    // =========================================

    public void EquipMask()
    {
        EquipSelectedEmployee(EquipmentType.Mask);
    }

    public void EquipSuit()
    {
        EquipSelectedEmployee(EquipmentType.Suit);
    }

    public void EquipWeapon()
    {
        EquipSelectedEmployee(EquipmentType.Weapon);
    }

    public void Unequip()
    {
        EmployeeState employee =
            GetSelectedEmployee();

        if (employee == null)
        {
            SetMessage("직원을 먼저 선택하세요.");
            return;
        }

        if (employee.equipment == EquipmentType.None)
            return;

        AddEquipment(
            employee.equipment,
            1
        );

        employee.equipment =
            EquipmentType.None;

        SetMessage("장비를 해제했습니다.");
    }

    private void EquipSelectedEmployee(
        EquipmentType equipmentType)
    {
        EmployeeState employee =
            GetSelectedEmployee();

        if (employee == null)
        {
            SetMessage("직원을 먼저 선택하세요.");
            return;
        }

        if (employee.equipment == equipmentType)
        {
            SetMessage("이미 해당 장비를 착용 중입니다.");
            return;
        }

        if (GetEquipmentAmount(equipmentType) <= 0)
        {
            SetMessage("해당 장비가 없습니다.");
            return;
        }

        // 기존 장비 반환
        if (employee.equipment !=
            EquipmentType.None)
        {
            AddEquipment(
                employee.equipment,
                1
            );
        }

        AddEquipment(
            equipmentType,
            -1
        );

        employee.equipment =
            equipmentType;

        SetMessage(
            $"직원 {employee.id}에게 {equipmentType} 장착"
        );
    }

    public int GetEquipmentAmount(
        EquipmentType type)
    {
        EquipmentStock stock =
            state.equipments.Find(
                item => item.type == type
            );

        if (stock == null)
            return 0;

        return stock.amount;
    }

    private void AddEquipment(
        EquipmentType type,
        int amount)
    {
        EquipmentStock stock =
            state.equipments.Find(
                item => item.type == type
            );

        if (stock == null)
            return;

        stock.amount += amount;
    }

    // =========================================
    // 격리실
    // =========================================

    public void SendSelectedEmployeeToRoom(
        IsolationRoom room)
    {
        EmployeeState employee =
            GetSelectedEmployee();

        if (employee == null)
        {
            SetMessage("직원을 먼저 선택하세요.");
            return;
        }

        if (employee.workedToday)
        {
            SetMessage(
                "이 직원은 오늘 이미 작업했습니다."
            );

            return;
        }

        // 장비가 맞지 않음
        if (employee.equipment !=
            room.requiredEquipment)
        {
            int deadId = employee.id;

            // 직원 사망 시 장비도 손실
            state.employees.Remove(employee);

            selectedEmployeeId = -1;

            SetMessage(
                $"직원 {deadId} 사망! " +
                $"{room.roomName}에는 " +
                $"{room.requiredEquipment} 장비가 필요합니다."
            );

            return;
        }

        // 작업 성공
        employee.power += room.powerGain;

        employee.workedToday = true;

        SetMessage(
            $"직원 {employee.id} 작업 성공! " +
            $"성장도 +{room.powerGain} " +
            $"현재 성장도 {employee.power}"
        );
    }

    // =========================================
    // 할당량 희생
    // =========================================

    public void SacrificeForQuota()
    {
        EmployeeState employee =
            GetSelectedEmployee();

        if (employee == null)
        {
            SetMessage("희생할 직원을 선택하세요.");
            return;
        }

        int quotaValue = employee.power;
        int id = employee.id;

        ReturnEmployeeEquipment(employee);

        state.employees.Remove(employee);

        selectedEmployeeId = -1;

        state.quotaCurrent += quotaValue;

        SetMessage(
            $"직원 {id} 희생. " +
            $"할당량 +{quotaValue}"
        );
    }

    // =========================================
    // 직원 보충
    // =========================================

    public void SacrificeForRecruit()
    {
        EmployeeState employee =
            GetSelectedEmployee();

        if (employee == null)
        {
            SetMessage("직원을 선택하세요.");
            return;
        }

        if (employee.power <
            recruitRequiredPower)
        {
            SetMessage(
                $"성장도 {recruitRequiredPower} 이상인 직원이 필요합니다."
            );

            return;
        }

        int id = employee.id;

        ReturnEmployeeEquipment(employee);

        state.employees.Remove(employee);

        selectedEmployeeId = -1;

        for (int i = 0;
             i < recruitEmployeeCount;
             i++)
        {
            // 오늘은 작업할 수 없음
            AddEmployee(true);
        }

        SetMessage(
            $"직원 {id} 희생. " +
            $"신규 직원 {recruitEmployeeCount}명 보충."
        );
    }

    private void ReturnEmployeeEquipment(
        EmployeeState employee)
    {
        if (employee.equipment ==
            EquipmentType.None)
            return;

        AddEquipment(
            employee.equipment,
            1
        );

        employee.equipment =
            EquipmentType.None;
    }

    // =========================================
    // 하루 종료
    // =========================================

    public void EndDay()
    {
        if (state.quotaCurrent <
            state.quotaTarget)
        {
            SetMessage(
                $"할당량 부족! " +
                $"{state.quotaCurrent}/{state.quotaTarget}"
            );

            return;
        }

        state.day++;

        state.quotaCurrent = 0;

        // 5일마다 +15
        if (state.day % 5 == 0)
        {
            state.quotaTarget += 15;
        }
        else
        {
            state.quotaTarget += 5;
        }

        foreach (
            EmployeeState employee
            in state.employees)
        {
            employee.workedToday = false;
        }

        selectedEmployeeId = -1;

        SaveDayStart();

        SetMessage(
            $"Day {state.day} 시작"
        );
    }

    // =========================================
    // 하루 재시작
    // =========================================

    private void SaveDayStart()
    {
        string json =
            JsonUtility.ToJson(state);

        dayStartSnapshot =
            JsonUtility.FromJson
            <PrototypeGameState>(json);
    }

    public void RestartDay()
    {
        if (dayStartSnapshot == null)
            return;

        string json =
            JsonUtility.ToJson(
                dayStartSnapshot
            );

        state =
            JsonUtility.FromJson
            <PrototypeGameState>(json);

        selectedEmployeeId = -1;

        SetMessage(
            $"Day {state.day}을 처음부터 다시 시작합니다."
        );
    }

    // =========================================

    private void SetMessage(string message)
    {
        LastMessage = message;

        Debug.Log(message);

        OnStateChanged?.Invoke();
    }
}