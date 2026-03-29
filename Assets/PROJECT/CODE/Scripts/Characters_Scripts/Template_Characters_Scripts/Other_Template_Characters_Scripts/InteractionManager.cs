using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class InteractionManager : MonoBehaviour
{
    [SerializeField] LayerMask _interactableLayer;
    private List<GameObject> _listOfInteractableObjects = new();
    GameObject _closestInteractableObject;

    public void HandleInteraction(PlayerInputHandlerManager handler)
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
                interactable.Interact(handler);
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
    public void Interact(PlayerInputHandlerManager handler)
    {
    }
}
