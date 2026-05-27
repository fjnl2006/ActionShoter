using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class SmiteController : MonoBehaviour, IWeapon
{

    [SerializeField] SmiteData smiteData;   // El ScripteableObject con la info del tipo de Smite
    [SerializeField] TrailData trailData;   // La info de la representación de los misiles mágicos de este tipo de Smite

    bool CanShoot = true;   // Esta variable nos servirá para controlar cuando podemos disparar o no


    public void Shoot(EnemyController target)
    {
        if (!CanShoot) return; // Si no podemos disparar terminamos la ejecución de esta llamada al método

        /////////////////////////////////////////////////////////////////////////////////
        /// Podemos dibujar una serie de Debug.Lines para visualizar el área de daño 
        ///------------------------------------------------------------------------------
        // Por cada x angulos dibujamos una linea que vaya desde el centro del target hacia fuera
        // Que sean tan largas como el radio del ataque y duren al menos medio segundo para poder apreciar el área
        ///////////////////////////////////////////////////////////////////////////////
        

        int enemyLayer = LayerMask.GetMask("Enemy");
        Collider[] hits = Physics.OverlapSphere(target.transform.position, smiteData.AreaRadius, enemyLayer);

        foreach (Collider col in hits)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null) enemy.GetDamaged(smiteData.Damage);
        }

        Vector3 aboveTarget = target.transform.position + Vector3.up * 5f;
        StartCoroutine(trailData.PlayTrail(aboveTarget, target.transform.position));
        
        // Cada vez que disparemos tendremos que recargar
        Reload();
    }
    
    public void Reload()
    {
        /// Llamaremos a este método cuando queramos recargar
        // Solo iniciamos una coorrutina, pero podriamos rellenar este método 
        // con cualquier cosas que queremos que realize el player antes o durante la accion de recarga
        // (animaciones, ataques especiales, etc)

        StartCoroutine(Reloading());
    }

    public float GetRange() { return smiteData.Range; }
    public void SwitchWeapon() {}

    IEnumerator Reloading()
    {
        CanShoot = false;
        yield return new WaitForSeconds(smiteData.ReloadingTime);
        CanShoot = true;
    }
    void OnDrawGizmosSelected()
    {
       
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, smiteData.AreaRadius);
    }
}