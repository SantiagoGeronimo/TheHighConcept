using UnityEngine;

public class CameraWobble : MonoBehaviour
{
    [Header("Base Wobble Settings")]
    [SerializeField] private float baseWobbleAmplitude = 0.05f;
    [SerializeField] private float baseRotationAmplitude = 1.5f;
    [SerializeField] private float wobbleFrequency = 1.5f;

    // Variables de control de caos progresivo
    [Header("Chaos Progression")]
    [SerializeField] private float chaosGrowthRate = 0.005f; // Cuánto aumenta la amplitud por segundo
    [SerializeField] private float maxWobbleAmplitude = 0.5f;  // Amplitud máxima a la que puede llegar
    [SerializeField] private float maxRotationAmplitude = 15f; // Amplitud máxima de rotación a la que puede llegar

    // Variables de suavizado y clamp de cámara (ya existentes)
    [Header("Camera Control Settings")]
    [SerializeField] private float cameraSensitivity = 50f;
    [SerializeField] private float verticalClamp = 70f;
    [SerializeField] private float cameraSmoothSpeed = 10f;

    // Variables internas del script
    private float currentWobbleAmplitude;   // Amplitud de wobble actual (dinámica)
    private float currentRotationAmplitude; // Amplitud de rotación actual (dinámica)
    private float currentPitch = 0f;        // Rotación vertical actual suavizada
    private float targetPitch = 0f;         // Rotación vertical instantánea del input

    void Start()
    {
        // Inicializar las amplitudes actuales con los valores base
        currentWobbleAmplitude = baseWobbleAmplitude;
        currentRotationAmplitude = baseRotationAmplitude;
    }

    void LateUpdate()
    {
        float time = Time.time * wobbleFrequency;

        // 1. Aumentar el caos con el tiempo
        // Incrementamos la amplitud actual del wobble y la rotación, clamped al máximo
        currentWobbleAmplitude = Mathf.Min(maxWobbleAmplitude, currentWobbleAmplitude + chaosGrowthRate * Time.deltaTime);
        currentRotationAmplitude = Mathf.Min(maxRotationAmplitude, currentRotationAmplitude + chaosGrowthRate * Time.deltaTime * 10f); // La rotación puede crecer más rápido

        // 2. Aplicar el Wobble de posición (ahora afectado por currentWobbleAmplitude)
        float xOffset = (Mathf.PerlinNoise(time, 0.5f) - 0.5f) * 2f * currentWobbleAmplitude; // <-- ¡USANDO currentWobbleAmplitude!
        float yOffset = (Mathf.PerlinNoise(0.5f, time) - 0.5f) * 2f * currentWobbleAmplitude; // <-- ¡USANDO currentWobbleAmplitude!
        transform.localPosition = new Vector3(xOffset, yOffset, 0f);

        // 3. Suavizado del Pitch (rotación vertical)
        float arrowY = Input.GetAxisRaw("VerticalArrow");
        targetPitch -= arrowY * cameraSensitivity * Time.deltaTime;
        targetPitch = Mathf.Clamp(targetPitch, -verticalClamp, verticalClamp);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, cameraSmoothSpeed * Time.deltaTime);

        // 4. Tambaleo de rotación (ahora afectado por currentRotationAmplitude)
        float wobbleX = Mathf.Sin(time * 1.2f) * currentRotationAmplitude; // <-- ¡USANDO currentRotationAmplitude!
        float wobbleZ = Mathf.Cos(time * 0.8f) * currentRotationAmplitude; // <-- ¡USANDO currentRotationAmplitude!
        Quaternion wobbleRot = Quaternion.Euler(wobbleX, 0f, wobbleZ);

        // 5. Aplicar la rotación vertical suavizada y el tambaleo
        Quaternion pitchRotation = Quaternion.Euler(currentPitch, 0f, 0f);
        transform.localRotation = pitchRotation * wobbleRot;
    }

    // Método público para resetear el caos (llamado desde PlayerMovement)
    public void ResetChaos()
    {
        currentWobbleAmplitude = baseWobbleAmplitude;
        currentRotationAmplitude = baseRotationAmplitude;
        Debug.Log("Camera chaos reset!");
    }
}
