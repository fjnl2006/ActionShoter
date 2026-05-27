using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class LaserController : MonoBehaviour, IWeapon
{

    [SerializeField] LaserData laserData;  // El Scriptable Object con la info de del láser
    [SerializeField] LineData lineData;    // La info de la representación del rayo de este tipo de laser

    [SerializeField] Transform Barrel;     // Posición desde la que dispararemos el laser
    [SerializeField] GameObject User;      // El GameObject que usa el arma, en nuestro caso el Player

    /// La info basica del arma
    int Damage;
    float Radius;
    float ReloadTime;
    float Range;
    float FireRating;

    int MaxAmo;                            // Municion máxima
    int AmoCost;                           // Coste de disparar un laser
    int CurrentAmo;                        // Municion actual

    
    bool CanShoot = true;                  // Esta variable nos servirá para controlar cuando se puede disparar o no

    Vector3 direction;                     // Direción en la que disparamos
    Vector3 origin;                        // Posición desde la que disparamos

    Transform[] laserPoints;               // Vector de puntos que determina la forma y longitud del laser

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Asignamos a nuestras variables la info del ScripteableObject
        Damage = laserData.Damage;
        Radius = laserData.Radius;
        Range = laserData.Range;
        
        FireRating = laserData.FireRating;
        ReloadTime = laserData.ReloadTime;

        MaxAmo = laserData.MaxAmo;
        AmoCost = laserData.AmoCost;

        // Empezamos con la municion al máximo
        CurrentAmo = MaxAmo;

        // Este laser va a tener solo dos puntos: inicio y final. Los creamos y se los asignamos al array de puntos
        laserPoints = new Transform[2]
        {
           new GameObject("LaserOrigin").transform,
           new GameObject("LaserEnd").transform 
        } ;
    }

    void Update()
    {
        direction = User.transform.forward.normalized;
        origin = Barrel.transform.position;

        float limit = Range;

        RaycastHit wallHit;
        int enemyLayer = LayerMask.GetMask("Enemy");
        int wallMask = ~enemyLayer; // Todo excepto enemigos

        if (Physics.Raycast(origin, direction, out wallHit, Range, wallMask))
        {
            limit = wallHit.distance;
        }

        laserPoints[0].position = origin;
        laserPoints[1].position = origin + direction * limit;
    }

    public void Shoot(EnemyController target)
    {
        if (!CanShoot) return;

        float distance = Vector3.Distance(laserPoints[0].position, laserPoints[1].position);

        int enemyLayer = LayerMask.GetMask("Enemy");
        RaycastHit[] hits = Physics.SphereCastAll(origin, Radius, direction, distance, enemyLayer);

        foreach (RaycastHit hit in hits)
        {
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();
            if (enemy != null) enemy.GetDamaged(Damage);
        }

        float duration = FireRating / 3f;
        StartCoroutine(lineData.DrawLineRenderer(laserPoints, duration));

        CurrentAmo -= AmoCost;

        if (CurrentAmo < AmoCost)
            Reload();
        else
            StartCoroutine(WaitFireRate());
    }

    public float GetRange() { return Range; }
    public void Reload() { StartCoroutine(Reloading()); }
    public void SwitchWeapon() {}


    IEnumerator Reloading()
    {
        CanShoot = false;
        yield return new WaitForSeconds(ReloadTime);
        CurrentAmo = MaxAmo;
        CanShoot = true;
    }
    
    IEnumerator WaitFireRate()
    {
        CanShoot = false;
        yield return new WaitForSeconds(FireRating);
        CanShoot = true;
    }

    void OnDrawGizmos()
    {
        
        if (laserPoints == null  || laserPoints.Length == 0 ) return;

        Gizmos.color = Color.lightBlue;

        for (int i = 0; i < laserPoints.Length; i++)
        {
            Gizmos.DrawWireSphere(laserPoints[i].position, Radius);
            if (i > 0) Gizmos.DrawLine(laserPoints[i-1].position, laserPoints[i].position);
        }
    }
    
    
}