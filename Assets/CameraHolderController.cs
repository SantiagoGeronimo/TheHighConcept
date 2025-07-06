using UnityEngine;

public class CameraHolderController : MonoBehaviour
{
    [SerializeField] private Transform sphereTransform;
    [SerializeField] private float cameraSensitivity = 50f;
    [SerializeField] private float cameraSmoothSpeed = 10f; // <-- ¡NUEVA VARIABLE!

    private float currentYaw = 0f;  // <-- ¡CAMBIADO: Esta es la rotación actual suavizada!
    private float targetYaw = 0f;   // <-- ¡NUEVA VARIABLE: Esta es la rotación a la que queremos llegar!

    void LateUpdate()
    {
        // El CameraHolder sigue la posición de la esfera
        if (sphereTransform != null)
        {
            transform.position = sphereTransform.position;
        }
        else
        {
            Debug.LogError("sphereTransform NO ha sido asignado en el Inspector para CameraHolderController en " + gameObject.name + ". ¡Asígnalo!");
            enabled = false;
            return;
        }

        // 1. Calcular el targetYaw (la rotación instantánea del input)
        float arrowX = Input.GetAxisRaw("HorizontalArrow");
        targetYaw += arrowX * cameraSensitivity * Time.deltaTime;

        // 2. Suavizar el currentYaw hacia el targetYaw
        // Mathf.LerpAngle es ideal para ángulos porque maneja el cruce de 360/0 grados correctamente.
        currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, cameraSmoothSpeed * Time.deltaTime); // <-- ¡CAMBIO AQUÍ!

        // 3. Aplicar la rotación suavizada al CameraHolder
        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f); // <-- ¡USANDO currentYaw!
    }
}
