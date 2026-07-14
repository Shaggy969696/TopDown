using UnityEngine;
using static PowerUps;




public enum TipoPowerUp { Fuego, Congelar }

public class PowerUpInventory : MonoBehaviour
{
    public GameObject fireballPrefab;
    private IPowerUp powerUpActual;
    private float cooldownRestante = 0f;

    public void Agregar(TipoPowerUp tipo)
    {
        switch (tipo)
        {
            case TipoPowerUp.Fuego:
                powerUpActual = new FireballPowerUp(fireballPrefab);
                break;
            case TipoPowerUp.Congelar:
                powerUpActual = new FreezePowerUp();
                break;
        }
        Debug.Log("Power-up equipado: " + powerUpActual.Nombre);
    }

    private void Update()
    {
        if (cooldownRestante > 0) cooldownRestante -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q) && powerUpActual != null && cooldownRestante <= 0)
        {
            powerUpActual.Activar(transform);
            cooldownRestante = powerUpActual.Cooldown;
        }
    }
}

