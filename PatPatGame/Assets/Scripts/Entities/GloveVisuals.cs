using System;
using UnityEngine;

public class GloveVisuals : MonoBehaviour
{
    [SerializeField] private PlayerBalance playerBalance;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private Texture gloveTexture;
    [SerializeField] private Color[] rarityColors;

    // assigns a texture and different colors on hand materials, for each rarity (1-5)
    private void HandleOnGloveRarityChanged(int rarity)
    {
        if (leftHand == null || rightHand == null || rarity < 1)
            return;
        Material leftHandMat = leftHand.GetComponent<SkinnedMeshRenderer>()?.material;
        Material rightHandMat = rightHand.GetComponent<SkinnedMeshRenderer>()?.material;
        if (rarity > 0)
        {
            // leftHandMat.SetTexture("_MainTex", gloveTexture);
            // rightHandMat.SetTexture("_MainTex", gloveTexture);
            leftHandMat.mainTexture = gloveTexture;
            rightHandMat.mainTexture = gloveTexture;
        }
        leftHandMat.SetColor("_BaseColor", rarityColors[rarity - 1]);
        rightHandMat.SetColor("_BaseColor", rarityColors[rarity - 1]);
    }

    void Awake()
    {
        playerBalance.OnGloveRarityChanged += HandleOnGloveRarityChanged;

        // sync in case playerBalance load method runs before this object's awake
        HandleOnGloveRarityChanged(playerBalance.gloveRarity);
    }
}
