using UnityEngine;

public class CameraHolderController : MonoBehaviour
{
    [SerializeField] private Transform sphereTransform;
    [SerializeField] private float cameraSensitivity = 50f;
    [SerializeField] private float cameraSmoothSpeed = 10f; 

    private float currentYaw = 0f;  
    private float targetYaw = 0f;   

    void LateUpdate()
    {
        
        if (sphereTransform != null)
        {
            transform.position = sphereTransform.position;
        }
        else
        {
            enabled = false;
            return;
        }

        // calculate targetYaw
        float arrowX = Input.GetAxisRaw("HorizontalArrow");
        targetYaw += arrowX * cameraSensitivity * Time.deltaTime;

        // Smooth currentYaw into targetYaw
        currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, cameraSmoothSpeed * Time.deltaTime); 

        // Apply smoothed rotation
        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f); 
    }
}
