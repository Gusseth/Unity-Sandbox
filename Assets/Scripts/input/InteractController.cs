using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractController : MonoBehaviour
{
    [SerializeField] GameObject indicatorObject;
    [SerializeField] TextMeshProUGUI indicatorText;
    [SerializeField] Camera cam;
    [SerializeField] LayerMask raycastMask;
    [SerializeField] AbstractActorBase actor;
    [SerializeField] bool probeForInteractable;

    IInteractable target;


    // Start is called before the first frame update
    void Start()
    {
        indicatorText ??= indicatorObject.GetComponent<TextMeshProUGUI>();
        cam ??= Camera.main;
        actor ??= GetComponent<AbstractActorBase>();

        indicatorObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (probeForInteractable)
            Probe();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.canceled) return;

        if (target != null)
        {
            target.OnInteract(actor);
        }
    }

    void Probe()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward,
            out RaycastHit hit, actor.Reach, raycastMask))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (target == null)
                {
                    target = interactable;
                    ProcessHoverData(interactable.OnHover(hit));
                }
                return;
            }
        }

        if (target != null)
            NoInteractable();
    }

    void ProcessHoverData(HoverData data)
    {
        indicatorObject.SetActive(true);
        indicatorText.text = data.name;
    }

    void NoInteractable()
    {
        indicatorObject.SetActive(false);
        if (target != null)
            target = null;
    }
}
