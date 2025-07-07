using UnityEngine;
using System.Collections.Generic; 

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float arrowPushForce = 2f;
    [SerializeField] private float groundFriction = 4f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private Transform cameraHolderTransform;

    [Header("Input Delay Chaos")]
    [SerializeField] private float initialInputDelay = 0.2f; 
    [SerializeField] private float delayGrowthRate = 0.005f; 
    [SerializeField] private float maxInputDelay = 1.5f;     

    [Header("Hydration & Vomit")]
    [SerializeField] private KeyCode vomitKey = KeyCode.V;
    [SerializeField] private float vomitCooldown = 5f;

    private Rigidbody rb;
    private Queue<(float timestamp, Vector3 input)> inputQueue = new();
    private Vector3 currentInput;

    private bool isHydrated = false;
    private CameraWobble cameraWobble;
    private float lastVomitTime = -Mathf.Infinity;

    private float currentInputDelay;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentInputDelay = initialInputDelay; 

        if (cameraHolderTransform == null)
        {
            enabled = false;
            return;
        }

        cameraWobble = FindFirstObjectByType<CameraWobble>();
        if (cameraWobble == null)
        {
            Debug.LogError("No existe camera wobble");
        }
    }

    void Update()
    {
        // Increase input delay
        currentInputDelay = Mathf.Min(maxInputDelay, currentInputDelay + delayGrowthRate * Time.deltaTime); // <-- ¡INCREMENTO AQUÍ!

        // WASD input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;

        inputQueue.Enqueue((Time.time, input));

        // Update input based on delay
        while (inputQueue.Count > 0)
        {
            var (timestamp, queuedInput) = inputQueue.Peek();
            if (Time.time - timestamp >= currentInputDelay)
            {
                currentInput = queuedInput;
                inputQueue.Dequeue();
            }
            else break;
        }

        if (Input.GetKeyDown(vomitKey))
        {
            TryVomit();
        }
    }

    void FixedUpdate()
    {
        if (cameraHolderTransform == null) return;

        Vector3 camForward = cameraHolderTransform.forward;
        Vector3 camRight = cameraHolderTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * currentInput.z + camRight * currentInput.x;

        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(moveDir * moveForce, ForceMode.Acceleration);
        }

        float arrowH = Input.GetAxisRaw("HorizontalArrow");
        float arrowV = Input.GetAxisRaw("VerticalArrow");
        Vector3 arrowInput = new Vector3(arrowH, 0f, arrowV).normalized;

        Vector3 arrowDir = camForward * arrowInput.z + camRight * arrowInput.x;

        rb.AddForce(arrowDir * arrowPushForce, ForceMode.Acceleration);

        if (IsGrounded())
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            Vector3 friction = -horizontalVelocity * groundFriction;
            rb.AddForce(friction, ForceMode.VelocityChange);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    public void Hydrate()
    {
        isHydrated = true;
        Debug.Log("¡El jugador está hidratado y listo para vomitar!");
    }

    private void TryVomit()
    {
        if (isHydrated && Time.time >= lastVomitTime + vomitCooldown)
        {
            PerformVomit();
        }
        else if (!isHydrated)
        {
            Debug.Log("Tenes el estomago re vacio, no podes vomitar, tomate un agua");
        }
        else if (Time.time < lastVomitTime + vomitCooldown)
        {
            Debug.Log("Para loco, para quebrar de nuevo necesitas " + (lastVomitTime + vomitCooldown - Time.time).ToString("F1") + " segundos que sino te va a salir todo bilis");
        }
    }

    private void PerformVomit()
    {
        Debug.Log("Quebraste gil");
        isHydrated = false;
        lastVomitTime = Time.time;

      
        if (cameraWobble != null)
        {
            cameraWobble.ResetChaos();
        }

        ResetInputDelay();

        // At these point of the logic, we can play a sound / spawn a vomit object
    }
    public void ResetInputDelay()
    {
        currentInputDelay = initialInputDelay;
        Debug.Log("Input delay reset!");
    }
}