using UnityEngine;
using System.Collections.Generic; // Asegúrate de tener esto para Queue

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveForce;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float inputDelay;
    [SerializeField] private float arrowPushForce; 
    [SerializeField] private float groundFriction;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform cameraHolderTransform; 

    private Rigidbody rb;
    private Queue<(float timestamp, Vector3 input)> inputQueue = new();
    private Vector3 currentInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Verificar que cameraHolderTransform esté asignado al inicio
        if (cameraHolderTransform == null)
        {
            enabled = false; 
        }
    }

    void Update()
    {
        // WASD input
        float h = Input.GetAxisRaw("Horizontal"); // A/D
        float v = Input.GetAxisRaw("Vertical");    // W/S
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
    }

    void FixedUpdate()
    {
        if (cameraHolderTransform == null) return;

        // Ahora usamos la rotación del CameraHolder para el movimiento del jugador
        Vector3 camForward = cameraHolderTransform.forward; 
        Vector3 camRight = cameraHolderTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Dirección WASD relativa a la cámaraHolder
        Vector3 moveDir = camForward * currentInput.z + camRight * currentInput.x;

        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(moveDir * moveForce, ForceMode.Acceleration);
        }

        // Dirección flechas relativa a la cámaraHolder (para el empuje leve)
        float arrowH = Input.GetAxisRaw("HorizontalArrow");
        float arrowV = Input.GetAxisRaw("VerticalArrow");
        Vector3 arrowInput = new Vector3(arrowH, 0f, arrowV).normalized;

        Vector3 arrowDir = camForward * arrowInput.z + camRight * arrowInput.x;

        rb.AddForce(arrowDir * arrowPushForce, ForceMode.Acceleration);
    }

    
}