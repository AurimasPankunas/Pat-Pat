using Unity.VisualScripting;
using UnityEngine;

public class SpotVisuals : MonoBehaviour
{
    [SerializeField] private Spot spot;

    void Awake()
    {
        spot.OnSpotBoughtUpdated += HandleOnSpotBought;
        spot.OnSpotBought += HandleOnSpotBought;
    }

    void Start()
    {
        HandleOnSpotBought(spot.isBought);
    }

    private void ChangeMaterial(bool isBought)
    {
        if (this == null || gameObject == null) return;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Ensure the renderer and materials are not null
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null || renderer.materials.Length == 0) continue;
            // Loop through each material in the mesh
            foreach (Material material in renderer.materials)
            {
                if (!isBought)
                {
                    // Set the material to transparent
                    material.SetInt("_Surface", 1);  // Use transparency surface type
                    material.SetInt("_Blend", 1);    // Set blend mode to transparent
                    material.SetInt("_ZWrite", 0);   // Disable depth writing
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.renderQueue = 3000; // Transparent render queue

                    // Adjust the alpha value to 0.5 (semi-transparent)
                    Color color = material.color;
                    color.a = 0.5f;
                    material.color = color;
                }
                else
                {
                    // Revert the material to opaque
                    material.SetInt("_Surface", 0);  // Use opaque surface type
                    material.SetInt("_Blend", 0);    // Set blend mode to opaque
                    material.SetInt("_ZWrite", 1);   // Enable depth writing
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.renderQueue = -1; // Default render queue for opaque objects

                    // Set the alpha back to 1 (fully opaque)
                    Color color = material.color;
                    color.a = 1f;
                    material.color = color;
                }
            }
        }
    }

    private void HandleOnSpotBought(bool value)
    {
        ChangeMaterial(value);
    }


    private void OnDestroy()
    {
        // Unsubscribe to prevent the destroyed object from receiving events
        if (spot != null)
        {
            spot.OnSpotBoughtUpdated -= HandleOnSpotBought;
            spot.OnSpotBought -= HandleOnSpotBought;
        }
    }
}
