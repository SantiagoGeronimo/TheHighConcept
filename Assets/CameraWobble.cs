using UnityEngine;

public class CameraWobble : MonoBehaviour
{
    [Header("Base Wobble Settings")]
    [SerializeField] private float baseWobbleAmplitude = 0.05f;
    [SerializeField] private float baseRotationAmplitude = 1.5f;
    [SerializeField] private float wobbleFrequency = 1.5f;

    // Variables de control de caos progresivo
    [Header("Chaos Progression")]
    [SerializeField] private float chaosGrowthRate = 0.005f; // Cu�nto aumenta la amplitud por segundo
    [SerializeField] private float maxWobbleAmplitude = 0.5f;  // Amplitud m�xima a la que puede llegar
    [SerializeField] private float maxRotationAmplitude = 15f; // Amplitud m�xima de rotaci�n a la que puede llegar

    // Variables de suavizado y clamp de c�mara (ya existentes)
    [Header("Camera Control Settings")]
    [SerializeField] private float cameraSensitivity = 50f;
    [SerializeField] private float verticalClamp = 70f;
    [SerializeField] private float cameraSmoothSpeed = 10f;

    // Variables internas del script
    private float currentWobbleAmplitude;   // Amplitud de wobble actual (din�mica)
    private float currentRotationAmplitude; // Amplitud de rotaci�n actual (din�mica)
    private float currentPitch = 0f;        // Rotaci�n vertical actual suavizada
    private float targetPitch = 0f;         // Rotaci�n vertical instant�nea del input

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
        // Incrementamos la amplitud actual del wobble y la rotaci�n, clamped al m�ximo
        currentWobbleAmplitude = Mathf.Min(maxWobbleAmplitude, currentWobbleAmplitude + chaosGrowthRate * Time.deltaTime);
        currentRotationAmplitude = Mathf.Min(maxRotationAmplitude, currentRotationAmplitude + chaosGrowthRate * Time.deltaTime * 10f); // La rotaci�n puede crecer m�s r�pido

        // 2. Aplicar el Wobble de posici�n (ahora afectado por currentWobbleAmplitude)
        float xOffset = (Mathf.PerlinNoise(time, 0.5f) - 0.5f) * 2f * currentWobbleAmplitude; // <-- �USANDO currentWobbleAmplitude!
        float yOffset = (Mathf.PerlinNoise(0.5f, time) - 0.5f) * 2f * currentWobbleAmplitude; // <-- �USANDO currentWobbleAmplitude!
        transform.localPosition = new Vector3(xOffset, yOffset, 0f);

        // 3. Suavizado del Pitch (rotaci�n vertical)
        float arrowY = Input.GetAxisRaw("VerticalArrow");
        targetPitch -= arrowY * cameraSensitivity * Time.deltaTime;
        targetPitch = Mathf.Clamp(targetPitch, -verticalClamp, verticalClamp);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, cameraSmoothSpeed * Time.deltaTime);

        // 4. Tambaleo de rotaci�n (ahora afectado por currentRotationAmplitude)
        float wobbleX = Mathf.Sin(time * 1.2f) * currentRotationAmplitude; // <-- �USANDO currentRotationAmplitude!
        float wobbleZ = Mathf.Cos(time * 0.8f) * currentRotationAmplitude; // <-- �USANDO currentRotationAmplitude!
        Quaternion wobbleRot = Quaternion.Euler(wobbleX, 0f, wobbleZ);

        // 5. Aplicar la rotaci�n vertical suavizada y el tambaleo
        Quaternion pitchRotation = Quaternion.Euler(currentPitch, 0f, 0f);
        transform.localRotation = pitchRotation * wobbleRot;
    }

    // M�todo p�blico para resetear el caos (llamado desde PlayerMovement)
    public void ResetChaos()
    {
        currentWobbleAmplitude = baseWobbleAmplitude;
        currentRotationAmplitude = baseRotationAmplitude;
        Debug.Log("Camera chaos reset!");
    }
}
