using UnityEngine;

public class EmployeeView : MonoBehaviour
{
    public int EmployeeId { get; private set; }

    private Vector3 normalScale;

    private void Awake()
    {
        normalScale = transform.localScale;
    }

    public void Initialize(int employeeId)
    {
        EmployeeId = employeeId;
        name = $"Employee_{employeeId}";
    }

    private void OnMouseDown()
    {
        if (PrototypeGameManager.Instance == null)
            return;

        PrototypeGameManager.Instance.SelectEmployee(EmployeeId);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            transform.localScale = normalScale * 1.25f;
        }
        else
        {
            transform.localScale = normalScale;
        }
    }
}