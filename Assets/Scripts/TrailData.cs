using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "TrailData", menuName = "Scriptable Objects/TrailData")]
public class TrailData : ScriptableObject
{
    /// Información visual del Trail
    public Material material;           // Material del Trial. Usaremos el "Sprite-Default" pero podemos crear nuestro propio material con shaders
    public Gradient color;              // Los colores en las distintas etapas del trail
    public AnimationCurve widthCurve;   // Forma del trial en sus distintas etapas


    public float duration = 0.5f;           // Cuanto dura el trail antes de desaparecer
    public float MinVertexDistance = 0.1f;  // Distancia entre veritces (como de suave sera la forma)
    public float SimulationSpeed = 100f;    // Velocidad del proyectil

    public TrailRenderer SpawnTrail()
    {
        // Creamos un nuevo GameObject y le asignamos un TrailRenderer component
        GameObject instance = new GameObject("Bullet");
        TrailRenderer trial = instance.AddComponent<TrailRenderer>();

        // Asignamos los datos al TrailRenderer
        trial.material = material;
        trial.colorGradient = color;
        trial.widthCurve = widthCurve;
        trial.time = duration;
        trial.minVertexDistance = MinVertexDistance;
        trial.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        
        // Devolvemos el Trail Renderer
        return trial; 
    }

    public IEnumerator PlayTrail(Vector3 origin, Vector3 end)
    {
        /// ScripteableObject no hereda de Monobehavior y por tanto no podremoso mover facilmente la "bala" desde aquí
        /// Pero podemos iniciar una corrutina del ScripteableObject desde un Script que si sea MonoBehaviour
        /// Esto nos permitirá crear balas y gestionar la lógica del movimiento desde el ScripteableObject

        // Recibiremos una posicion origen desde donde empieza el trail y una posición final a la que queremos llegar

        // Generamos un nuevo Trail y lo ponemos en la posicion de origen
        TrailRenderer newTrail = SpawnTrail();
        newTrail.transform.position = origin;

        // Calculamos la distancia inicial
        float distance = Vector3.Distance(origin, end);
        // La guardamos como distancia restante 
        float remainingDistance = distance;

        while(remainingDistance > 0)
        {
            // Este lerp nos permite avanzar a una velocidad estable
            newTrail.transform.position = Vector3.Lerp(origin, end, Mathf.Clamp01(1 - (remainingDistance/distance)));
            remainingDistance -= SimulationSpeed * Time.deltaTime; // Reducimos la distancia restante con la avanzada

            yield return null; // Esperamos al siguiente frame
        }

        // Al llegar a la posición deseada esperamos el tiempo de vida del trail
        // para que termine de desvanecerse naturalmente 
        yield return new WaitForSeconds(duration); 

        // Al terminar de desvanecerse el trail, destruimos la bala
        Destroy(newTrail.transform.gameObject);
    }
    
}
