using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ButtonFollow : MonoBehaviour
{
    [SerializeField] private Spot spot;

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
            if (!spot.isBought) return;
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;

            isFollowing = true;
            freeze = false;

            pokeAttachTransform = interactor.attachTransform;
            // Record where the finger was when it first entered
            pokeStartLocalPosition = visualTarget.parent.InverseTransformPoint(pokeAttachTransform.position);
        }
    }

    public void Reset(BaseInteractionEventArgs hover)
    {
        if(hover.interactorObject is XRPokeInteractor)
        {
            isFollowing = false;
            freeze = false;
            hasActivated = false;
        }
    }

    public void Freeze(BaseInteractionEventArgs hover)
    {
        if(hover.interactorObject is XRPokeInteractor)
        {
            freeze = true;
        }
    }

    private void OnButtonPressed()
    {
        spot.TryRemoveAnimal();
    }


    // Update is called once per frame
    void Update()
    {
        if(freeze) return;

        if (isFollowing)
        {
            Vector3 pokeLocalPosition = visualTarget.parent.InverseTransformPoint(pokeAttachTransform.position);
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
            visualTarget.localPosition = Vector3.Lerp(visualTarget.localPosition, initialLocalPosition, Time.deltaTime * resetSpeed);
        }
    }

}
