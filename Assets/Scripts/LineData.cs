using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LineData", menuName = "Scriptable Objects/LineData")]
public class LineData : ScriptableObject
{
    public Material material;           // Material del Line. Usaremos el "Sprite-Default" pero podemos crear nuestro propio material con su sprite, shaders...
    public AnimationCurve widthCurve;   // Los colores en las distintas etapas del line
    public Gradient color;              // Forma del line en sus distintas etapas

    public IEnumerator DrawLineRenderer(Transform[] points, float duration)
    {
        /// ScripteableObject no hereda de Monobehavior y por tanto no podremoso mover facilmente el laser desde aquí
        /// Pero podemos iniciar una corrutina del ScripteableObject desde un Script que si sea MonoBehaviour
        /// Esto nos permitirá crear el láser, gestionar el tiempo que se muestra y su destrucción

        /// LineRenderer necesita una serie de puntos para dibujar la lína, por eso tenemos que pasar un Array de Transforms cuando llamemos a esta coorutina
        /// la forma del laser dependerá en cierta forma de la lista de puntos (linea recta, zigzag, espiral...)
        /// También recibiremos un float que determina cuanto durará el láser

        if (points.Length < 2) yield break; // Necesitamos al menos dos puntos, si no los tenemos terminamos la ejecucion

        // Vamos a crear un nuevo GameObject al que le añadiremos un line renderer 
        GameObject instance = new GameObject("newLaser");
        LineRenderer newLine =  instance.AddComponent<LineRenderer>();

        // Asignamos los datos de este ScriptableObject a las variables del nuevo LineRenderer
        newLine.positionCount = points.Length;
        newLine.material = material;
        newLine.colorGradient = color;
        newLine.widthCurve = widthCurve;
        newLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Esto desactivará las sombras


        /// Necesitamos que el láser se mantenga dibujado tanto tiempo como nos indica "duration"
        /// Vamos a aprovechar para hacer que a cada frame revise el array de puntos que nos han pasado
        /// esto nos permitirá que el laser se mueva mientras este esté activo
        /// así podriamos hacer que el laser siga al jugador, crear un laser flexible o incluso proyectar el laser lejos del Player
        
        // Creamos un contador donde almacenaremos el tiempo trascurrido desde que creamos el laser a 0
        float timePassed = 0f;
        while (timePassed < duration)
        {   
            // Mientras no haya pasado mas tiempo de lo que debe durar el laser
            // Convertimos la lista de puntos que nos han pasado en forma de Transfomrs a una nueva de Vector3
            Vector3[] newPoints =  new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                newPoints[i] = points[i].position;
            }

            // Asignamos los nuevos puntos al Line renderer
            // Como los Transform normalmente se pasan POR REFERENCIA, cada vez que hagamos esto estaremos accediendo a la posición actualizada del Transform
            // Por lo que si el objeto que contiene este tranform se ha movido, el punto del láser también lo hará
            newLine.SetPositions(newPoints);

            yield return null;            // Esperamos al siguiente frame
            timePassed += Time.deltaTime; // Actualizamos contador con el tiempo que ha transcurrido desde el anterior frame
   
        }

        // Cuando haya transcurrido el tiempo indicado Destruimos el láser
        Destroy(instance);

    }
}