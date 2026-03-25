using UnityEngine;
using UnityEditor.VersionControl;
using System.Threading;


#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Very rough image generator.
/// Can be used to generate images for ShopItems
/// Put script anywhere into the scene 
/// Put some GameObjects into the list
/// Upon starting the game images will be saved to folder:
/// Assets/UI/Resources/Textures
/// 
/// !!! Script does not work in a built game
/// Generates images from prefab thumbnails, because
/// of this there is a grey background
/// </summary>
public class SaveObjectThumbnailImage : MonoBehaviour
{
    [Header("Note: Might take a few tries")]
    [SerializeField] public GameObject[] objectsToSave;
    [Tooltip("Will remove background colored pixels on the item")]
    [SerializeField] public bool removeBackground = true;
    [Header("Optional:")]
    [SerializeField] public Color backgroundColor = new Color(0.322f, 0.322f, 0.322f, 1f);
    void Start()
    {
        /*if (backgroundColor == null)
        {
            backgroundColor = new Color(0.322f, 0.322f, 0.322f, 1f);
        }*/
#if UNITY_EDITOR
        if (objectsToSave != null) {
            foreach (GameObject obj in objectsToSave)
            {
                if (obj != null) {
                    Texture2D texture = AssetPreview.GetAssetPreview(obj);
                    EntityId entityId = obj.GetEntityId();
                    // wait till asset preview loads:
                    int tries = 5000;
                    while (AssetPreview.IsLoadingAssetPreview(entityId) && tries > 0)
                    {
                        tries--;
                        Thread.Sleep(50);
                    }

                    if (tries != 0)
                    {
                        if (texture != null){
                            if (removeBackground) {
                                Color32[] colors = texture.GetPixels32();
                                for (int i = 0; i < colors.Length; i++)
                                {
                                    if (colors[i].Equals(backgroundColor))
                                    {
                                        colors[i].a = 0;
                                    }
                                }
                                texture.SetPixels32(colors);
                            }
                            Debug.Log($"Saved image:{obj.name}Icon.png");
                            SaveTextureToFileUtility.SaveTexture2DToFile(texture,
                            $"Assets/UI/Resources/Textures/{obj.name}Icon", SaveTextureToFileUtility.SaveTextureFileFormat.PNG);
                            AssetDatabase.ImportAsset($"Assets/UI/Resources/Textures/{obj.name}Icon.png");
                        }
                        else
                        {
                            Debug.Log("Null texture");
                        }
                    }
                    else
                    {
                        Debug.Log($"Took too long to load preview image for {obj.name}, try again");
                    }
                }
            }
        }
        else
        {
            Debug.Log("Object list is empty");
        }
        #endif
    }


}
