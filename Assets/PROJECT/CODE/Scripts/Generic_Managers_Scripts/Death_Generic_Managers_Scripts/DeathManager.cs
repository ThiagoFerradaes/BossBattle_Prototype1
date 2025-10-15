using System;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    HealthManager _healthManager;
    [SerializeField] DeathBehaviourSO deathBehaviour;

    Action _onDeath;

    private void Awake()
    {
        _healthManager = GetComponent<HealthManager>();
        _onDeath = HandleDeath;
    }

    private void OnEnable()
    {
        _healthManager.OnDeath += _onDeath;
    }

    private void OnDisable()
    {
        _healthManager.OnDeath -= _onDeath;
    }

    void HandleDeath()
    {
        deathBehaviour.Death(this.gameObject);
    }
}
