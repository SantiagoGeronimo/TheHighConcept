using UnityEngine;
using System.Collections.Generic; 

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float inputDelay = 0.2f;
    [SerializeField] private float arrowPushForce = 2f;
    [SerializeField] private float groundFriction = 4f; // Aseg�rate de haber quitado Time.fixedDeltaTime de su uso.
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private Transform cameraHolderTransform;

    // Nuevas variables para el sistema de hidrataci�n y v�mito
    [Header("Hydration & Vomit")]
    [SerializeField] private KeyCode vomitKey = KeyCode.V; // Tecla para vomitar
    [SerializeField] private float vomitCooldown = 5f; // Cooldown para no spamear el v�mito

    private Rigidbody rb;
    private Queue<(float timestamp, Vector3 input)> inputQueue = new();
    private Vector3 currentInput;

    private bool isHydrated = false; // Estado de hidrataci�n del jugador
    private CameraWobble cameraWobble; // Referencia al script CameraWobble
    private float lastVomitTime = -Mathf.Infinity; // Para el cooldown del v�mito

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (cameraHolderTransform == null)
        {
            Debug.LogError("cameraHolderTransform NO ha sido asignado en el Inspector para " + gameObject.name + ". �As�gnalo!");
            enabled = false;
            return;
        }

        // Buscar la instancia del script CameraWobble en la escena
        // Aseg�rate de que solo haya una c�mara principal o que este m�todo la encuentre correctamente.
        cameraWobble = FindFirstObjectByType<CameraWobble>();
        if (cameraWobble == null)
        {
            Debug.LogError("No se encontr� un script CameraWobble en la escena. Aseg�rate de que la c�mara principal lo tenga.");
        }
    }

    void Update()
    {
        // WASD input (sin cambios)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;
        inputQueue.Enqueue((Time.time, input));
        while (inputQueue.Count > 0)
        {
            var (timestamp, queuedInput) = inputQueue.Peek();
            if (Time.time - timestamp >= inputDelay)
            {
                currentInput = queuedInput;
                inputQueue.Dequeue();
            }
            else break;
        }

        // L�gica de v�mito
        if (Input.GetKeyDown(vomitKey))
        {
            TryVomit();
        }
    }

    void FixedUpdate()
    {
        // ... (resto del FixedUpdate sin cambios, usa cameraHolderTransform.forward, etc.)
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
            Vector3 friction = -horizontalVelocity * groundFriction; // Aseg�rate de este cambio!
            rb.AddForce(friction, ForceMode.VelocityChange);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    // M�todo p�blico para hidratar al jugador (llamado por el item de agua)
    public void Hydrate()
    {
        isHydrated = true;
        Debug.Log("�El jugador est� hidratado y listo para vomitar!");
        // Opcional: Podr�as resetear un poco el caos aqu� tambi�n si quisieras
    }

    // L�gica para intentar vomitar
    private void TryVomit()
    {
        // Solo puede vomitar si est� hidratado y no est� en cooldown
        if (isHydrated && Time.time >= lastVomitTime + vomitCooldown)
        {
            PerformVomit();
        }
        else if (!isHydrated)
        {
            Debug.Log("Necesitas beber agua para vomitar.");
        }
        else if (Time.time < lastVomitTime + vomitCooldown)
        {
            Debug.Log("Todav�a est�s en cooldown de v�mito. Espera " + (lastVomitTime + vomitCooldown - Time.time).ToString("F1") + " segundos.");
        }
    }

    // La acci�n de vomitar
    private void PerformVomit()
    {
        Debug.Log("�El jugador vomita!");
        isHydrated = false; // Ya no est� hidratado despu�s de vomitar
        lastVomitTime = Time.time; // Reiniciar el temporizador de cooldown

        // Resetear el caos de la c�mara
        if (cameraWobble != null)
        {
            cameraWobble.ResetChaos();
        }

        // Aqu� podr�as a�adir efectos de sonido, part�culas de v�mito, etc.
    }


}