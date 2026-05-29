using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent agent;

   public GameObject target;

    [Header("Health Variables")]
    [SerializeField] int maxHealth;
    int currentHealth;

    [Header("Attack Variables")]
    [SerializeField] int damage = 10;            // Cuánto daño hace al jugador
    [SerializeField] float attackRange = 1.5f;   // A qué distancia ataca
    [SerializeField] float attackRate = 1.0f;    // Tiempo de espera entre ataques (en segundos)
    float nextAttackTime = 0f;                   // Temporizador interno

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (currentHealth <= 0)
            currentHealth = maxHealth;
    }

    /// <summary>
    /// Aplica vida según la oleada (llamar justo después de Instantiate).
    /// </summary>
    public void InitializeForWave(int health)
    {
        maxHealth = Mathf.Max(1, health);
        currentHealth = maxHealth;
    }
    
    void Update()
    {
        NavMeshHit navHit;
        NavMesh.SamplePosition(target.transform.position, out navHit, 20, NavMesh.AllAreas);
        agent.SetDestination(navHit.position);
        if (target)
        {
            // Calculamos la distancia entre el enemigo y el jugador
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            // Si está dentro del rango de ataque
            if (distanceToTarget <= attackRange)
            {
                agent.isStopped = true; // El enemigo se detiene para atacar
                
                // Comprobamos si ha pasado suficiente tiempo para volver a atacar
                if (Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + attackRate; // Reiniciamos el temporizador
                }
            }
            else
            {
                // Si el jugador se aleja, vuelve a perseguirlo
                agent.isStopped = false;
                Move();
            }
        } 
    }

    void Move()
    {
        agent.destination = target.transform.position;
    }

    void Attack()
    {
        // Intentamos obtener el script PlayerController del target
        PlayerController player = target.GetComponent<PlayerController>();
        
        if (player != null)
        {
            player.TakeDamage(damage); // Le hacemos daño
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void GetDamaged(int Damage)
    {
        Debug.Log("OUCH!!");    
        currentHealth -= Damage;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}