using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public enum TargetType { none, enemy, position }

    public struct Target
    {
        public Target( TargetType type, RaycastHit hit)
        {
            Type = type;
            Hit = hit;
        }

        public TargetType Type;
        public RaycastHit Hit;
    }

    // Movement and Interaction
    NavMeshAgent agent;
    Target target = new Target(TargetType.none, new RaycastHit());
    [SerializeField] LayerMask mask;
    Vector2 MousePosition;

    // Inputs
    CustomActions input;
    InputAction m_interaction;
    [SerializeField] InputAction[] m_WeaponSwitch;

    [SerializeField] float AttackRange = 2f;
    [SerializeField] float Cooldown = 0.5f;
    bool CanShoot = true;
    [SerializeField] float InteractRange = 5f;

    // Weapons
    [SerializeField] GameObject[] Weapons = new GameObject[0];
    [SerializeField] GameObject CurrentWeapon;
    IWeapon CurrentWeaponController;
    
    
    [Header("Health Variables")]
    [SerializeField] int maxHealth = 100;
    int currentHealth = 100;

    public GameObject canva;
    public GameObject menu;

     public HealthUI healtUI;
     public TimerUI timerUI;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        input = new CustomActions();
        input.Main.Enable();
        
        m_interaction = input.Main.Move;

        m_WeaponSwitch = new InputAction[4] {input.Main.Weapon1, input.Main.Weapon2, input.Main.Weapon3, input.Main.Weapon4};
        
        healtUI.UpdateHealt(currentHealth, maxHealth);
        canva.SetActive(false);
        menu.SetActive(false);
        Time.timeScale = 1;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        
        // Recorremos la lista de Armas
        for (int i = 0; i < Weapons.Length; i++)
        {
            if ( i == 0)
            {
                CurrentWeapon = Weapons[i];  // Guardamos el primer arma como la actual
                CurrentWeaponController = CurrentWeapon.GetComponent<IWeapon>(); // Guardamos el controlador a parte para que sea más fácil de usar
            }
            else Weapons[i].gameObject.SetActive(false); // Desactivamos el resto de armas
        }

        Debug.Log("Start with " + CurrentWeapon.name);
    }

    // Update is called once per frame
    void Update()
    {

        Move();
        CheckTarget();
        SwitchWeapon();
        if(input.Main.Pause.WasPressedThisFrame())
        {
            OnEscPressed();
        }

    }

    private void OnEnable()
    {
        input.Main.Enable();
    }

    private void OnDisable()
    {
        input.Main.Disable();
    }

    void Move()
    {
        // Detecta si hemos pulsado un posible target y nos pone en movimiento

        MousePosition = Mouse.current.position.value;

        if (!m_interaction.WasPressedThisFrame()) { return; }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(MousePosition), out hit, 100, mask))
        {

            string LayerName = LayerMask.LayerToName(hit.transform.gameObject.layer);

            switch (LayerName)
           {
                case "Enemy":
                    target = new Target(TargetType.enemy, hit);
                    break;

                case "Floor":
                    target = new Target(TargetType.position, hit);
                    break;

                case "Interactable":
                    break;

                default: 
                    Debug.Log("???");
                    break;
            }

            agent.destination = target.Hit.point;
            agent.isStopped = false;
        }
    }

    void CheckTarget()
    {
        // Actualiza nuestro target y/o su posición
       
        
        switch (target.Type)
        {   
            case TargetType.enemy:  // Si tenemos un Target de tipo enemy
                
                if (target.Hit.transform == null) 
                {
                    target = new Target(TargetType.none, new RaycastHit());
                    break;  
                }

                agent.destination = target.Hit.transform.position; 
    
                // Calculamos la distancia real
                float distance = Vector3.Distance(transform.position, agent.destination);
    
                if (distance <= CurrentWeaponController.GetRange()) 
                {  
                    agent.isStopped = true; 
        
                    // CORRECCIÓN: Hacemos que mire al enemigo pero sin inclinarse hacia el suelo
                    Vector3 lookPos = target.Hit.transform.position;
                    lookPos.y = transform.position.y;
                    transform.LookAt(lookPos); 

                    Debug.Log("¡En rango! Intentando disparar..."); // Aviso para saber que llega aquí
                    CurrentWeaponController.Shoot(target.Hit.transform.GetComponent<EnemyController>()); 
                }
                break;
            case TargetType.position:
            default:
                break;
        }
    }

    void SwitchWeapon()
    {
        // Recorremos la lista de inputs para ver si se ha pulsado uno de los botones
        for (int i = 0; i < m_WeaponSwitch.Length; i++) 
        {
            // Si hay más botones que armas terminaremos la ejecución para evitar problemas
            if (i >= Weapons.Length) return;

            // Comprobamos si se ha pulsado el boton
            if (m_WeaponSwitch[i].WasPressedThisFrame())
            {   
                if (Weapons[i] != null && Weapons[i] != CurrentWeapon)
                {
                    CurrentWeapon.SetActive(false);
                    CurrentWeapon = Weapons[i];
                    CurrentWeaponController = CurrentWeapon.GetComponent<IWeapon>();
                    CurrentWeapon.SetActive(true);
                    CurrentWeaponController.SwitchWeapon();
                    CurrentWeapon.GetComponent<GunController>()?.ForceUIRefresh();
                    return;
                }
            }
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healtUI.UpdateHealt(currentHealth, maxHealth);
        Debug.Log("¡El jugador ha recibido " + damage + " de daño! Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("¡El jugador ha muerto!");
        // Aquí puedes reiniciar la escena, mostrar un menú de Game Over, o desactivar el control:
        agent.isStopped = true;
        this.enabled = false; // Desactiva este script para que no se pueda mover ni disparar
        canva.SetActive(true);
        timerUI.StopTimer();
    }

    void OnDrawGizmos()
    {
      // Si tenemos un target Dibujamos una pequeña esfera en su posicion
        if (target.Type != TargetType.none)
        {
            Gizmos.color = new Color (1f, 1f, 0f, 1f);
            Gizmos.DrawWireSphere(agent.destination, 0.25f);
        }

      if (CurrentWeaponController != null)
      {
          bool enemyInRange = target.Type == TargetType.enemy
              && target.Hit.transform != null
              && Vector3.Distance(transform.position, target.Hit.transform.position) <= CurrentWeaponController.GetRange();

          Gizmos.color = enemyInRange ? Color.red : Color.green;
          Gizmos.DrawWireSphere(transform.position, CurrentWeaponController.GetRange());
      }
    }

    public void OnEscPressed()
    {
        
        menu.SetActive(true);
        Time.timeScale = 0;
        timerUI.StopTimer();
    }
}