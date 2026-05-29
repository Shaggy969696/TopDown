using UnityEngine;

public class PlayerAnima                                             : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("PlayerAnima: Animator no encontrado. Asegúrate de asignarlo en el Inspector o que el modelo con el Animator sea un hijo de este objeto.");
            }
        }
    }

    private bool lastRunningState = false;

    /// <summary>
    /// Activa o desactiva la animación de correr.
    /// </summary>
    public void SetRunning(bool isRunning)
    {
        if (animator != null)
        {
            if (isRunning != lastRunningState)
            {
                // Un pequeño mensaje en la consola para confirmar que la señal llega
                Debug.Log("Cambiando animación a correr: " + isRunning);
                lastRunningState = isRunning;
            }

            animator.SetBool("IsRunning", isRunning);
        }
    }
}
