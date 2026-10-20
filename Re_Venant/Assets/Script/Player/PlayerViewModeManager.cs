using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerViewModeManager : MonoBehaviour
{
    [Header("Current View")]
    [SerializeField]
    private PlayerViewMode currentMode =
        PlayerViewMode.TopDown;

    [Header("Systems")]
    [SerializeField]
    private PlayerViewCameraController cameraController;

    [SerializeField]
    private PlayerMouseAim mouseAim;

    [Header("Character")]
    [SerializeField]
    private Renderer[] characterRenderers;

    [Header("Weapon Visual")]
    [SerializeField]
    private Renderer[] worldWeaponRenderers;

    [SerializeField]
    private Renderer[] firstPersonWeaponRenderers;

    [Header("UI")]
    [SerializeField]
    private GameObject crosshair;

    private bool uiMode = false;

    public PlayerViewMode CurrentMode => currentMode;

    public bool CanJump =>
        currentMode == PlayerViewMode.ThirdPerson ||
        currentMode == PlayerViewMode.FirstPerson;

    private void Start()
    {
        ApplyViewMode();
    }

    private void Update()
    {
        // ==========================================
        // 게임이 정지된 상태
        // LevelUp / Setting / GameOver
        // ==========================================

        if (Time.timeScale <= 0f)
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;

            return;
        }

        // ==========================================
        // 게임이 정상 진행 중
        // ==========================================

        UpdateCursorState();

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            SwitchView();
        }
    }

    private void SwitchView()
    {
        switch (currentMode)
        {
            case PlayerViewMode.TopDown:

                currentMode =
                    PlayerViewMode.ThirdPerson;

                break;


            case PlayerViewMode.ThirdPerson:

                currentMode =
                    PlayerViewMode.FirstPerson;

                break;


            case PlayerViewMode.FirstPerson:

                currentMode =
                    PlayerViewMode.TopDown;

                break;
        }

        ApplyViewMode();
    }

    private void ApplyViewMode()
    {
        // ----------------------------
        // Camera
        // ----------------------------

        if (cameraController != null)
        {
            cameraController.SetViewMode(
                currentMode
            );
        }

        // ----------------------------
        // Top View Mouse Aim
        // ----------------------------

        if (mouseAim != null)
        {
            mouseAim.enabled =
                currentMode ==
                PlayerViewMode.TopDown;
        }

        // ----------------------------
        // Crosshair
        // ----------------------------

        if (crosshair != null)
        {
            crosshair.SetActive(
                currentMode !=
                PlayerViewMode.TopDown
            );
        }

        // ----------------------------
        // First Person Character Hide
        // ----------------------------

        bool showCharacter =
            currentMode !=
            PlayerViewMode.FirstPerson;

        if (characterRenderers != null)
        {
            foreach (Renderer rend
                     in characterRenderers)
            {
                if (rend != null)
                {
                    rend.enabled =
                        showCharacter;
                }
            }
        }

        bool isFirstPerson =
            currentMode == PlayerViewMode.FirstPerson;

        // 기존 월드 무기
        if (worldWeaponRenderers != null)
        {
            foreach (Renderer rend in worldWeaponRenderers)
            {
                if (rend != null)
                {
                    rend.enabled = !isFirstPerson;
                }
            }
        }

        // 1인칭 전용 무기
        if (firstPersonWeaponRenderers != null)
        {
            foreach (Renderer rend in firstPersonWeaponRenderers)
            {
                if (rend != null)
                {
                    rend.enabled = isFirstPerson;
                }
            }
        }
        UpdateCursorState();
    }
    public void SetUIMode(bool active)
    {
        uiMode = active;

        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        // 레벨업 같은 UI가 열려 있다면
        // 시점에 관계없이 마우스를 사용 가능하게 함
        if (uiMode)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        // Top View
        if (currentMode == PlayerViewMode.TopDown)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Third / First Person
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
}