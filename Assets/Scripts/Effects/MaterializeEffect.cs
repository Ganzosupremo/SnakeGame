using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    /// <summary>
    /// This Coroutine is used to apply an effect to any object, so
    /// the object appears to be materializing into existance.
    /// </summary>
    /// <param name="materializeShader">The shader that will apply the materialize effect.</param>
    /// <param name="materializeColor">The desired color for the effect.</param>
    /// <param name="materializeTime">The time the effect will take to complete.</param>
    /// <param name="spriteRendererArray">The sprites that the effect will be applied to</param>
    /// <param name="defaultMaterial">The sprite-lit default material to apply to the object after
    /// it has been materialized.</param>
    /// <returns></returns>
    public IEnumerator MaterializeRoutine(Shader materializeShader, Color materializeColor, float materializeTime,
        SpriteRenderer[] spriteRendererArray, Material defaultMaterial)
    {
        Material materializeMaterial = new(materializeShader);

        materializeMaterial.SetColor("_EmissionColor", materializeColor);

        //Set the material for each sprite renderer
        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = materializeMaterial;
        }

        float dissolveAmount = 0f;

        //While the dissolve amount is less than one, but once it is greather than one, we set the default lit material again
        while (dissolveAmount < 1f)
        {
            dissolveAmount += Time.deltaTime / materializeTime;
            materializeMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;
        }

        //Set again the default lit material for the sprites renderes
        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }
}
