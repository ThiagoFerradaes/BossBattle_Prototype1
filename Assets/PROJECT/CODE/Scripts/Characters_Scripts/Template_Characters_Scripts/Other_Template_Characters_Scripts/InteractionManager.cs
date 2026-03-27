using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    private List<GameObject> _listOfInteractableObjects = new();
    GameObject _closestInteractableObject;
    [SerializeField] LayerMask _interactableLayer;
    public void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (Time.timeScale == 0) return;
        if (ctx.performed)
        {
            HandleInteraction();
        }
    }

    void HandleInteraction()
    {
        if (_listOfInteractableObjects.Count > 0)
        {
            _closestInteractableObject = GetClosestInteractableObject();
            if (_closestInteractableObject != null)
            {
                if (!_closestInteractableObject.TryGetComponent<IInteractable>(out var interactable))
                {
                    return;
                }
                interactable.Interact();
            }
        }
    }

    private GameObject GetClosestInteractableObject()
    {
        float closestDistance = Mathf.Infinity;
        GameObject closestObject = null;

        foreach (var interactableObject in _listOfInteractableObjects)
        {
            float distance = Vector3.Distance(transform.position, interactableObject.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = interactableObject;
            }
        }

        return closestObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_interactableLayer.ContainsLayer(other.gameObject.layer))
        {
            _listOfInteractableObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactableLayer.ContainsLayer(other.gameObject.layer) && _listOfInteractableObjects.Contains(other.gameObject))
        {
            _listOfInteractableObjects.Remove(other.gameObject);
        }
    }
}

public interface IInteractable
{
    public void Interact()
    {
    }
}
