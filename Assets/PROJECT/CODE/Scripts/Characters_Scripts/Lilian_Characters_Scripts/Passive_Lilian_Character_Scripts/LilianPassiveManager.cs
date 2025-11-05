using System;
using System.Collections;
using UnityEngine;

public class LilianPassiveManager : PassiveSkillManager {
    public static LilianPassiveManager Instance;

    // Components
    LilianPassiveSO _info;
    HealthManager _healthManager;

    // Atributes
    float _currentAmountOfTributes = 0f;

    // Actions
    Action _onHit;

    // Events
    public event Action<float, float> OnTributesChange;

    #region Initialize
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _onHit = Hit;
    }
    public override void OnStart(PassiveSO skill, GameObject parent) {
        base.OnStart(skill, parent);

        _info = skill as LilianPassiveSO;
        _healthManager = parent.GetComponent<HealthManager>();

        gameObject.SetActive(true);

        AditionalUIManager.Instance.InstantiateUI(_info.LilianUI);

        _healthManager.OnHit += _onHit;
    }

    #endregion

    #region New Passive

    public void Hit()
    {
        _currentAmountOfTributes++; 
        OnTributesChange?.Invoke(_currentAmountOfTributes, _info.MaxAmountOfTributes);

        if (_currentAmountOfTributes >= _info.MaxAmountOfTributes)
        {
            Blessing();
        }
    }

    void Blessing()
    {
        _currentAmountOfTributes = 0;
        OnTributesChange?.Invoke(_currentAmountOfTributes, _info.MaxAmountOfTributes);

        _healthManager.Heal(_info.BlessingHealing);
    }

    #endregion
}
