using UnityEngine;
using UnityEngine.AI;

public class PowerUps : MonoBehaviour
{


    public interface IPowerUp
    {
        string Nombre { get; }
        float Cooldown { get; }
        void Activar(Transform mago);
    }

    public class FireballPowerUp : IPowerUp
    {
        public string Nombre => "Bola de Fuego";
        public float Cooldown => 5f;
        private GameObject fireballPrefab;
        private float radioExplosion = 3f;
        private float dano = 40f;

        public FireballPowerUp(GameObject prefab) { fireballPrefab = prefab; }

        public void Activar(Transform mago)
        {
            Vector3 puntoImpacto = mago.position + mago.forward * 5f;

            if (fireballPrefab != null)
            {
                GameObject fx = Object.Instantiate(fireballPrefab, puntoImpacto, Quaternion.identity);
                Object.Destroy(fx, 2f);
            }

            Collider[] enemigos = Physics.OverlapSphere(puntoImpacto, radioExplosion);
            foreach (var col in enemigos)
            {
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(dano);
                }
            }
        }
    }

    public class FreezePowerUp : IPowerUp
    {
        public string Nombre => "Congelar";
        public float Cooldown => 8f;
        private float radio = 4f;
        private float duracion = 3f;

        public void Activar(Transform mago)
        {
            Collider[] enemigos = Physics.OverlapSphere(mago.position, radio);
            foreach (var col in enemigos)
            {
                NavMeshAgent agent = col.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    mago.GetComponent<PowerUpInventory>().StartCoroutine(CongelarAgent(agent, duracion));
                }
            }
        }

        private System.Collections.IEnumerator CongelarAgent(NavMeshAgent agent, float duracion)
        {
            float velocidadOriginal = agent.speed;
            agent.speed = 0f;
            yield return new WaitForSeconds(duracion);
            if (agent != null) agent.speed = velocidadOriginal;
        }
    }
}
