using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform sphereTransform;

    void LateUpdate()
    {
       
        transform.position = sphereTransform.position;
        
    }
}
