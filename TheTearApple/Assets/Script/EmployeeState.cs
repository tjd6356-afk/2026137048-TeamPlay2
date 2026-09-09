using System;

[Serializable]
public class EmployeeState
{
    public int id;
    public int power = 1;

    public EquipmentType equipment = EquipmentType.None;

    // 오늘 이미 격리실에서 작업했는지
    public bool workedToday = false;

    public EmployeeState(int id)
    {
        this.id = id;
        power = 1;
        equipment = EquipmentType.None;
        workedToday = false;
    }
}