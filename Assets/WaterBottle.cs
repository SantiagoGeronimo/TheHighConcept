using UnityEngine;

public class WaterBottle : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer; 

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0) 
        {
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

            if (player != null)
            {
                player.Hydrate(); 
                Destroy(gameObject); 
            }
            else
            {
                Debug.LogWarning("Hiciste algo mal flaco");
            }
        }
    }
}
