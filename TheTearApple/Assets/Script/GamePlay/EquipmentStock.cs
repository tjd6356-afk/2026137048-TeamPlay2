using System;

[Serializable]
public class EquipmentStock
{
    public EquipmentType type;
    public int amount;

    public EquipmentStock(EquipmentType type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
}