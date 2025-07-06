using UnityEngine;

public class CameraWobble : MonoBehaviour
{
    [SerializeField] private float wobbleAmplitude = 0.05f;
    [SerializeField] private float wobbleFrequency = 1.5f;
    [SerializeField] private float rotationAmplitude = 1.5f;

    [SerializeField] private float cameraSensitivity = 50f;
    [SerializeField] private float verticalClamp = 70f;
    [SerializeField] private float cameraSmoothSpeed = 10f; 

    private float currentPitch = 0f; 
    private float targetPitch = 0f;  

    void LateUpdate()
    {
        float time = Time.time * wobbleFrequency;

        // Wobble 
        float xOffset = (Mathf.PerlinNoise(time, 0.5f) - 0.5f) * 2f * wobbleAmplitude;
        float yOffset = (Mathf.PerlinNoise(0.5f, time) - 0.5f) * 2f * wobbleAmplitude;
        transform.localPosition = new Vector3(xOffset, yOffset, 0f);

        // Calculate and clamp target pitch
        float arrowY = Input.GetAxisRaw("VerticalArrow");
        targetPitch -= arrowY * cameraSensitivity * Time.deltaTime;
        targetPitch = Mathf.Clamp(targetPitch, -verticalClamp, verticalClamp);

        // Smooth current pitch
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, cameraSmoothSpeed * Time.deltaTime); 

        // Wobble rotation
        float wobbleX = Mathf.Sin(time * 1.2f) * rotationAmplitude;
        float wobbleZ = Mathf.Cos(time * 0.8f) * rotationAmplitude;
        Quaternion wobbleRot = Quaternion.Euler(wobbleX, 0f, wobbleZ);

        // Apply wobble and smoothness 
        Quaternion pitchRotation = Quaternion.Euler(currentPitch, 0f, 0f); 
        transform.localRotation = pitchRotation * wobbleRot;
    }
}
