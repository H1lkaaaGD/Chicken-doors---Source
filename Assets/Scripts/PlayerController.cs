using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float Speed = 5f;
    public float SpeedCrouching = 2f;
    public bool IsCrouching = false;
    public bool Stunned = false;
    public bool Hiding = false;
    public bool InCutscene = false;
    public bool IsDied = false;

    [Header("References")]
    public Collider Collision;          // Обычный коллайдер
    public Collider CollisionCrouch;    // Коллайдер в присяди
    public FixedJoystick MovementJoystick; // Джойстик для передвижения
    public FloatingJoystick CameraJoystick; // Джойстик для управления камерой
    public Button JumpButton;           // Кнопка прыжка
    public Button CrouchButton;         // Кнопка присяда

    private Rigidbody rb;
    private bool canJump = true;
    private float cameraPitch = 0f;
    private Transform cameraTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
        
        // Инициализация коллайдеров
        UpdateColliders();
    }

    private void Start()
    {
        // Подписываемся на события кнопок
        if (JumpButton != null)
            JumpButton.onClick.AddListener(OnJump);
        
        if (CrouchButton != null)
            CrouchButton.onClick.AddListener(ToggleCrouch);
    }

    private void Update()
    {
        if (IsMovementBlocked()) return;

        HandleCameraRotation();
    }

    private void FixedUpdate()
    {
        if (IsMovementBlocked()) return;

        HandleMovement();
    }

    private bool IsMovementBlocked()
    {
        return Stunned || InCutscene || IsDied || Hiding;
    }

    private void HandleCameraRotation()
    {
        // Вращение камеры по вертикали
        cameraPitch -= CameraJoystick.Vertical * 2f;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        
        // Вращение персонажа по горизонтали
        float yaw = CameraJoystick.Horizontal * 2f;
        
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(0f, yaw, 0f);
    }

    private void HandleMovement()
    {
        Vector2 moveInput = new Vector2(
            MovementJoystick.Horizontal,
            MovementJoystick.Vertical
        );

        if (moveInput.magnitude > 0.1f)
        {
            Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
            float currentSpeed = IsCrouching ? SpeedCrouching : Speed;
            
            Vector3 targetVelocity = moveDirection.normalized * currentSpeed;
            targetVelocity.y = rb.velocity.y;
            
            rb.velocity = targetVelocity;
        }
        else
        {
            // Торможение при отсутствии ввода
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }

    private void OnJump()
    {
        if (CanPerformActions() && canJump && !IsCrouching)
        {
            // Логика прыжка
            Debug.Log("Jump performed");
            
            // Выход из присяди при прыжке
            if (IsCrouching)
            {
                SetCrouching(false);
            }
        }
    }

    private void ToggleCrouch()
    {
        if (CanPerformActions())
        {
            SetCrouching(!IsCrouching);
        }
    }

    private void SetCrouching(bool crouchState)
    {
        IsCrouching = crouchState;
        UpdateColliders();
    }

    private void UpdateColliders()
    {
        if (Collision != null) Collision.enabled = !IsCrouching;
        if (CollisionCrouch != null) CollisionCrouch.enabled = IsCrouching;
    }

    private bool CanPerformActions()
    {
        return !Stunned && !InCutscene && !IsDied && !Hiding;
    }

    public void SetHiding(bool hidingState)
    {
        Hiding = hidingState;
        if (hidingState)
        {
            // Запоминаем мог ли игрок прыгать до укрытия
            canJump = CanPerformActions() && canJump;
        }
    }

    public void SetStunned(bool stunnedState)
    {
        Stunned = stunnedState;
        if (stunnedState)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий кнопок
        if (JumpButton != null)
            JumpButton.onClick.RemoveListener(OnJump);
        
        if (CrouchButton != null)
            CrouchButton.onClick.RemoveListener(ToggleCrouch);
    }
}