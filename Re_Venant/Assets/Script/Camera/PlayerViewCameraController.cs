using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerViewCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform firstPersonPoint;

    [SerializeField]
    private Camera targetCamera;

    [Header("Top View")]
    [SerializeField]
    private float topHeight = 12f;

    [SerializeField]
    private float topLookAheadDistance = 4f;

    [SerializeField]
    private float topFollowSmoothTime = 0.12f;

    [SerializeField]
    private float topOrthographicSize = 7f;

    [Header("Third Person")]
    [SerializeField]
    private float thirdPersonDistance = 5f;

    [SerializeField]
    private float thirdPersonHeight = 1.5f;

    [SerializeField]
    private float shoulderOffset = 0.7f;

    [Header("Mouse Look")]
    [SerializeField]
    private float mouseSensitivity = 0.15f;

    [SerializeField]
    private float minPitch = -40f;

    [SerializeField]
    private float maxPitch = 70f;

    [Header("First Person")]
    [SerializeField]
    private float firstPersonFOV = 70f;

    [Header("Third Person Camera")]
    [SerializeField]
    private float thirdPersonFOV = 65f;

    private PlayerViewMode currentMode;

    private Vector3 topVelocity;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera =
                GetComponent<Camera>();
        }
    }

    public void SetViewMode(
        PlayerViewMode newMode)
    {
        currentMode = newMode;

        if (targetCamera != null)
        {
            if (!targetCamera.gameObject.activeSelf)
            {
                targetCamera.gameObject.SetActive(true);
            }

            targetCamera.enabled = true;
        }

        if (player != null)
        {
            yaw =
                player.eulerAngles.y;
        }

        if (newMode ==
            PlayerViewMode.TopDown)
        {
            targetCamera.orthographic =
                true;

            targetCamera.orthographicSize =
                topOrthographicSize;

            pitch = 0f;
        }
        else
        {
            targetCamera.orthographic =
                false;

            if (newMode ==
                PlayerViewMode.ThirdPerson)
            {
                targetCamera.fieldOfView =
                    thirdPersonFOV;

                pitch = 10f;
            }
            else
            {
                targetCamera.fieldOfView =
                    firstPersonFOV;

                pitch = 0f;
            }
        }
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        switch (currentMode)
        {
            case PlayerViewMode.TopDown:

                UpdateTopView();

                break;


            case PlayerViewMode.ThirdPerson:

                UpdateThirdPerson();

                break;


            case PlayerViewMode.FirstPerson:

                UpdateFirstPerson();

                break;
        }
    }

    // ==================================================
    // TOP VIEW
    // ==================================================

    private void UpdateTopView()
    {
        Vector3 lookAheadOffset =
            Vector3.zero;

        if (Mouse.current != null)
        {
            Vector2 mouse =
                Mouse.current.position.ReadValue();

            Vector2 center =
                new Vector2(
                    Screen.width * 0.5f,
                    Screen.height * 0.5f
                );

            Vector2 offset =
                mouse - center;

            Vector2 normalized =
                new Vector2(
                    offset.x /
                    (Screen.width * 0.5f),

                    offset.y /
                    (Screen.height * 0.5f)
                );

            normalized =
                Vector2.ClampMagnitude(
                    normalized,
                    1f
                );

            // Top View에서 화면 오른쪽 = World X
            // 화면 위쪽 = World Z
            lookAheadOffset =
                new Vector3(
                    normalized.x,
                    0f,
                    normalized.y
                ) *
                topLookAheadDistance;
        }

        Vector3 targetPosition =
            player.position +
            Vector3.up * topHeight +
            lookAheadOffset;

        transform.position =
            Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref topVelocity,
                topFollowSmoothTime
            );

        transform.rotation =
            Quaternion.Euler(
                90f,
                0f,
                0f
            );
    }

    // ==================================================
    // MOUSE LOOK
    // ==================================================

    private void ReadMouseLook()
    {
        if (Mouse.current == null)
            return;

        Vector2 delta =
            Mouse.current.delta.ReadValue();

        yaw +=
            delta.x *
            mouseSensitivity;

        pitch -=
            delta.y *
            mouseSensitivity;

        pitch =
            Mathf.Clamp(
                pitch,
                minPitch,
                maxPitch
            );

        // Player는 좌우 회전만
        player.rotation =
            Quaternion.Euler(
                0f,
                yaw,
                0f
            );
    }

    // ==================================================
    // THIRD PERSON
    // ==================================================

    private void UpdateThirdPerson()
    {
        ReadMouseLook();

        Quaternion cameraRotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );

        Vector3 pivot =
            player.position +
            Vector3.up *
            thirdPersonHeight;

        Vector3 backOffset =
            cameraRotation *
            Vector3.back *
            thirdPersonDistance;

        Vector3 sideOffset =
            cameraRotation *
            Vector3.right *
            shoulderOffset;

        transform.position =
            pivot +
            backOffset +
            sideOffset;

        transform.rotation =
            cameraRotation;
    }

    // ==================================================
    // FIRST PERSON
    // ==================================================

    private void UpdateFirstPerson()
    {
        ReadMouseLook();

        if (firstPersonPoint == null)
            return;

        transform.position =
            firstPersonPoint.position;

        transform.rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );
    }
}