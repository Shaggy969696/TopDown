using UnityEngine;



  

public class PowerUpPickup : MonoBehaviour
{
    public TipoPowerUp tipo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerUpInventory inventario = other.GetComponent<PowerUpInventory>();
            if (inventario != null)
            {
                inventario.Agregar(tipo);
                Destroy(gameObject);
            }
        }
    }
}
