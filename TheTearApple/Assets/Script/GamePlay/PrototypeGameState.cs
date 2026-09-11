using System;
using System.Collections.Generic;

[Serializable]
public class PrototypeGameState
{
    public int day = 1;

    public int quotaTarget = 10;
    public int quotaCurrent = 0;

    public int nextEmployeeId = 1;

    public List<EmployeeState> employees =
        new List<EmployeeState>();

    public List<EquipmentStock> equipments =
        new List<EquipmentStock>();
}