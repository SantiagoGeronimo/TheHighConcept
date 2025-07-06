using UnityEngine;

public class CameraWobble : MonoBehaviour
{
    [SerializeField] private float wobbleAmplitude = 0.05f;
    [SerializeField] private float wobbleFrequency = 1.5f;
    [SerializeField] private float rotationAmplitude = 1.5f;

    [SerializeField] private float cameraSensitivity = 50f;
    [SerializeField] private float verticalClamp = 70f;
    [SerializeField] private float cameraSmoothSpeed = 10f; // <-- �NUEVA VARIABLE! (Puedes usar el mismo valor que en CameraHolderController)

    private float currentPitch = 0f; // <-- �CAMBIADO: Esta es la rotaci�n actual suavizada!
    private float targetPitch = 0f;  // <-- �NUEVA VARIABLE: Esta es la rotaci�n a la que queremos llegar!

    void LateUpdate()
    {
        float time = Time.time * wobbleFrequency;

        // Wobble de posici�n (aplicado localmente)
        float xOffset = (Mathf.PerlinNoise(time, 0.5f) - 0.5f) * 2f * wobbleAmplitude;
        float yOffset = (Mathf.PerlinNoise(0.5f, time) - 0.5f) * 2f * wobbleAmplitude;
        transform.localPosition = new Vector3(xOffset, yOffset, 0f);

        // 1. Calcular el targetPitch (la rotaci�n instant�nea del input) y clamparla
        float arrowY = Input.GetAxisRaw("VerticalArrow");
        targetPitch -= arrowY * cameraSensitivity * Time.deltaTime;
        targetPitch = Mathf.Clamp(targetPitch, -verticalClamp, verticalClamp);

        // 2. Suavizar el currentPitch hacia el targetPitch
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, cameraSmoothSpeed * Time.deltaTime); // <-- �CAMBIO AQU�!

        // Tambaleo de rotaci�n
        float wobbleX = Mathf.Sin(time * 1.2f) * rotationAmplitude;
        float wobbleZ = Mathf.Cos(time * 0.8f) * rotationAmplitude;
        Quaternion wobbleRot = Quaternion.Euler(wobbleX, 0f, wobbleZ);

        // Aplica la rotaci�n vertical suavizada (currentPitch) y el tambaleo a la rotaci�n local de la c�mara
        Quaternion pitchRotation = Quaternion.Euler(currentPitch, 0f, 0f); // <-- �USANDO currentPitch!
        transform.localRotation = pitchRotation * wobbleRot;
    }
}
