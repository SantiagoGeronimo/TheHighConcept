using UnityEngine;

public class WaterBottle : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer; 

    void OnTriggerEnter(Collider other)
    {
        // Comprobar si el objeto que colision� est� en alguna de las capas especificadas en playerLayer
        // Usamos un bitwise AND (&) para verificar si la capa del 'other.gameObject' est� dentro de la 'playerLayer' mask.
        // (1 << other.gameObject.layer) crea una m�scara de bits con solo el bit de la capa del objeto.
        if (((1 << other.gameObject.layer) & playerLayer) != 0) 
        {
            // Intentar obtener el script PlayerMovement del objeto colisionado (la esfera)
            // Ya que PlayerMovement est� en el padre (GameObject "Player"), necesitamos ir un nivel arriba con .transform.parent
            PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

            if (player != null)
            {
                player.Hydrate(); // Llama al m�todo Hydrate del jugador
                Destroy(gameObject); // Destruye la botella de agua despu�s de ser recogida
            }
            else
            {
                Debug.LogWarning("WaterItem: El objeto en la capa especificada no tiene el script PlayerMovement en s� mismo o en su padre. Aseg�rate de que el GameObject 'Player' tenga el script PlayerMovement.");
            }
        }
    }
}
