using UnityEngine;
using System.Collections;
using UnityEditor;

public class GunController : MonoBehaviour, IWeapon
{

    [SerializeField] GunData Data;     // El ScripteableObject con la info del tipo de pistola
    [SerializeField] TrailData trail;  // La info de la representación de la balas de este tipo de pistola
    [SerializeField] Transform Barrel; // El GameObject que usa el arma, en nuestro caso el Player
    [SerializeField] GameObject User;  // El GameObject que usa el arma, en nuestro caso el Player
    [SerializeField] AmmoUI ammoUI;


    int Damage;             // Daño de cada disparo
    float ReloadTime;       // Tiempo de recarga de la munición
    float Range;            // Rango de alcance del arma
    float FireRating;       // Cadencia entre disparos

    int MaxAmo;             // Munición Máxima
    int CurrentAmo;         // Munición actual

    bool CanShoot = true;   // Esta variable nos servirá para controlar cuando la pistola puede o no disparar

    void Awake()
    {
        /// Rellenamos las variabloes con la info del ScripteableObject
    
        Damage = Data.Damage;
        ReloadTime = Data.ReloadTime;
        Range = Data.Range;
        FireRating = Data.FireRating;
        MaxAmo = Data.MaxAmo;

        // Ponemos la Municion al máximo para empezar
        CurrentAmo = MaxAmo;
        
        ammoUI.UpdateAmmo(CurrentAmo, MaxAmo);
    }

    public void Shoot(EnemyController target)
    {
        Debug.Log("Dado");
        // Llamaremos a esta función cuando queramos intentar disparar a un target

        if (!CanShoot) return;  // Si no tenemos balas terminamos la ejecución de este método
        

        // Usamos el forward porque el player se gira hacia el enemigo antes de intentar disparar
        // pero podriamos sacar la direccion a partir del Origin y la posicion del target
        //Vector3 direction = User.transform.forward; // Direccion en la que disparamos 

        Vector3 origin = Barrel.transform.position; // Origen del Rayo y la "bala"
        Vector3 direction = (target.transform.position - origin).normalized; 

        Vector3 endPoint;                           

        RaycastHit hit;     

        int enemyLayer = LayerMask.GetMask("Enemy");

        if (Physics.Raycast(origin, direction, out hit, Range, enemyLayer))
        {
            hit.transform.GetComponent<EnemyController>()?.GetDamaged(Damage);
            endPoint = hit.point;
            
        }
        else
        {
            endPoint = origin + direction * Range;
        }

        // Iniciamos la corrutina que dibuja y mueve las "balas" dandole el inicio y el final de la trayectoria 
        StartCoroutine(trail.PlayTrail(origin, endPoint));

        // Actualizamos la munición
        --CurrentAmo;
        ammoUI.UpdateAmmo(CurrentAmo, MaxAmo);
        Debug.Log(CurrentAmo);

        if (CurrentAmo == 0) Reload();          // Si no queda munición recargamos
        else StartCoroutine(WaitFireRate());    // De lo contrario iniciamos el tiempo de espera entre disparos
        ammoUI.UpdateAmmo(CurrentAmo, MaxAmo);

    }

    IEnumerator WaitFireRate()
    {   
        /// Bloqueamos el arma y esperamos el tiempo que hay entre disparos antes de poder volver a disparar
        CanShoot = false;
        yield return new WaitForSeconds(FireRating);
        CanShoot = true;
    }

    public void Reload()
    {
        /// Llamaremos a este método cuando queramos recargar
        // Solo iniciamos una coorrutina, pero podriamos rellenar este método 
        // con cualquier cosas que queremos que realize el player antes o durante la accion de recarga
        // (animaciones, ataques especiales, etc)

        StartCoroutine(Reloading());
        
    }

    IEnumerator Reloading()
    {
        /// Bloqueamos el arma, esperamos el tiempo de recarga antes de poner la munición al máximo y desbloquear el arma
        Debug.Log("Reloading");
        CanShoot = false;
        yield return new WaitForSeconds(ReloadTime);
        CurrentAmo = MaxAmo;
        CanShoot = true;
        Debug.Log("Reloaded!");
        UpdateAmmoReloading();
        
    }

    public void SwitchWeapon()
    {
        /// Llamaremos a este método cuando cambiemos a esta arma
        // Tan solo recargamos el arma para empezar a usarla con la municion al máximo, pero podriamos rellenar este método
        // con cualquier cosas que queremos que realize el player al cambiar de arma
        // (animaciones, ataques especiales, etc)

        Reload();
    }
    
    public float GetRange()
    {
        return Range;
    }
    void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }

    public void UpdateAmmoReloading()
    {
        ammoUI.UpdateAmmo(CurrentAmo, MaxAmo);
    }
    public void ForceUIRefresh()
    {
        ammoUI.UpdateAmmo(CurrentAmo, MaxAmo);
    }
}