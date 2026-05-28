using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WipeSaveFileButton : MonoBehaviour
{
    public Transform visualTarget;
    public Vector3 localAxis;
    public float resetSpeed = 5;
    private Vector3 pokeStartLocalPosition;

    private bool freeze = false;
    private bool hasActivated = false;

    private Vector3 initialLocalPosition;

    private Transform pokeAttachTransform;

    private XRBaseInteractable interactable;
    private bool isFollowing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialLocalPosition = visualTarget.localPosition;

        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(Follow);
        interactable.hoverExited.AddListener(Reset);
        interactable.selectEntered.AddListener(Freeze);
    }

    public void Follow(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;

            isFollowing = true;
            freeze = false;

            pokeAttachTransform = interactor.attachTransform;
            // Record where the finger was when it first entered
            pokeStartLocalPosition = visualTarget.parent.InverseTransformPoint(
                pokeAttachTransform.position
            );
        }
    }

    public void Reset(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            isFollowing = false;
            freeze = false;
            hasActivated = false;
        }
    }

    public void Freeze(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            freeze = true;
        }
    }

    private void OnButtonPressed()
    {
        string savePath = Application.persistentDataPath + "/save.json";

        if (SaveManager.SaveFileExists())
        {
            File.Delete(savePath);
        }

        // 2. Load the prepared (default) save from Resources and write it to disk
        TextAsset dummySave = Resources.Load<TextAsset>("DummySaveFileLocation/save");
        if (dummySave != null)
        {
            File.WriteAllText(savePath, dummySave.text);
            Debug.Log($"WipeSaveFileButton: default save written to {savePath}");
        }
        else
        {
            Debug.LogError(
                $"WipeSaveFileButton: default save not found at Resources/{"Assets/Resources/DummySaveFileLocation/save.json"}.json  " +
                "Make sure the file exists and the path in the Inspector is correct.");
            return; // Don't reload if we couldn't restore the default
        }

        // 3. Reload the gameplay scene so all managers reinitialise cleanly
        GameManager.Instance.saveManager.Load();
        StartCoroutine(ReloadScene());
    }

    private IEnumerator ReloadScene()
    {
        yield return null;
        SceneManager.sceneLoaded += ReloadEnvironment;
        SceneManager.LoadScene("MainScene");
    }

    private void ReloadEnvironment(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= ReloadEnvironment;
        SceneManager.LoadScene("Environment", LoadSceneMode.Additive);
    }



    // Update is called once per frame
    void Update()
    {
        if (freeze)
            return;

        if (isFollowing)
        {
            Vector3 pokeLocalPosition = visualTarget.parent.InverseTransformPoint(
                pokeAttachTransform.position
            );
            Vector3 delta = pokeLocalPosition - pokeStartLocalPosition;
            Vector3 constrainedDelta = Vector3.Project(delta, localAxis);

            float movement = Mathf.Clamp(constrainedDelta.y, -0.1f, 0f);
            visualTarget.localPosition = initialLocalPosition + new Vector3(0, movement, 0);

            // Trigger when fully pressed
            if (movement <= -0.1f && !hasActivated)
            {
                hasActivated = true;
                OnButtonPressed();
            }
        }
        else
        {
            visualTarget.localPosition = Vector3.Lerp(
                visualTarget.localPosition,
                initialLocalPosition,
                Time.deltaTime * resetSpeed
            );
        }
    }
}
