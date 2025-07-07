using UnityEngine;

public class CameraWobble : MonoBehaviour
{
    [Header("Base Wobble Settings")]
    [SerializeField] private float baseWobbleAmplitude = 0.05f;
    [SerializeField] private float baseRotationAmplitude = 1.5f;
    [SerializeField] private float wobbleFrequency = 1.5f;

    
    [Header("Chaos Progression")]
    [SerializeField] private float chaosGrowthRate = 0.005f; 
    [SerializeField] private float maxWobbleAmplitude = 0.5f;  
    [SerializeField] private float maxRotationAmplitude = 15f; 

    [Header("Camera Control Settings")]
    [SerializeField] private float cameraSensitivity = 50f;
    [SerializeField] private float verticalClamp = 70f;
    [SerializeField] private float cameraSmoothSpeed = 10f;

   
    private float currentWobbleAmplitude;   
    private float currentRotationAmplitude; 
    private float currentPitch = 0f;        
    private float targetPitch = 0f;         

    void Start()
    {
        currentWobbleAmplitude = baseWobbleAmplitude;
        currentRotationAmplitude = baseRotationAmplitude;
    }

    void LateUpdate()
    {
        float time = Time.time * wobbleFrequency;

        
        // Increase wobble and rotation
        currentWobbleAmplitude = Mathf.Min(maxWobbleAmplitude, currentWobbleAmplitude + chaosGrowthRate * Time.deltaTime);
        currentRotationAmplitude = Mathf.Min(maxRotationAmplitude, currentRotationAmplitude + chaosGrowthRate * Time.deltaTime * 10f);

        // Apply wobble to position
        float xOffset = (Mathf.PerlinNoise(time, 0.5f) - 0.5f) * 2f * currentWobbleAmplitude; 
        float yOffset = (Mathf.PerlinNoise(0.5f, time) - 0.5f) * 2f * currentWobbleAmplitude; 
        transform.localPosition = new Vector3(xOffset, yOffset, 0f);

        // Vertical smoothness
        float arrowY = Input.GetAxisRaw("VerticalArrow");
        targetPitch -= arrowY * cameraSensitivity * Time.deltaTime;
        targetPitch = Mathf.Clamp(targetPitch, -verticalClamp, verticalClamp);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, cameraSmoothSpeed * Time.deltaTime);

        // Wobble rotation
        float wobbleX = Mathf.Sin(time * 1.2f) * currentRotationAmplitude; 
        float wobbleZ = Mathf.Cos(time * 0.8f) * currentRotationAmplitude; 
        Quaternion wobbleRot = Quaternion.Euler(wobbleX, 0f, wobbleZ);

        // Apply wobble and vertical smoothness
        Quaternion pitchRotation = Quaternion.Euler(currentPitch, 0f, 0f);
        transform.localRotation = pitchRotation * wobbleRot;
    }

    public void ResetChaos()
    {
        currentWobbleAmplitude = baseWobbleAmplitude;
        currentRotationAmplitude = baseRotationAmplitude;
        Debug.Log("Camera chaos reset!");
    }
}
