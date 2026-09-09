using UnityEngine;

public class IsolationRoom : MonoBehaviour
{
    [Header("격리실 설정")]
    public string roomName;

    [Header("필요 장비")]
    public EquipmentType requiredEquipment;

    [Header("성공 시 성장량")]
    public int powerGain = 1;

    public void EnterSelectedEmployee()
    {
        if (PrototypeGameManager.Instance == null)
            return;

        PrototypeGameManager.Instance.SendSelectedEmployeeToRoom(this);
    }
}