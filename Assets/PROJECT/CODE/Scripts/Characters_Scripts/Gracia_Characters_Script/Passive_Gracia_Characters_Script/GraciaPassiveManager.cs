using System;
using UnityEngine;

public enum GraciaTypeOfSkill { Left, Right };
public enum GraciaAura { Blue, Yellow, Red, Green, Null };
public class GraciaPassiveManager : PassiveSkillManager {
    #region Paramethers

    public static GraciaPassiveManager Instance;
    GraciaPassiveSO _info;

    float _currentLeftBarValue, _currentRightBarValue;
    int _currentLeftBarArea, _currentRightBarArea;
    GraciaAura _currentAura, _leftAura, _rightAura;

    public Action<float, GraciaTypeOfSkill> OnGraciaBarValueChanged;
    public Action<int, GraciaTypeOfSkill> OnGraciaBarAreaChanged;
    #endregion

    #region Initialize 

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public override void OnStart(PassiveSO passive, GameObject parent) {

        Initialize(passive);

        gameObject.SetActive(true);

        AditionalUIManager.Instance.InstantiateUI(_info.UI);

    }

    void Initialize(PassiveSO passive) {
        _info = passive as GraciaPassiveSO;
    }

    #endregion

    #region Bar Value Area

    /// <summary>
    /// Função chamada para alterar o valor da barra da gracia - Se o valor for negativo ela reduz se for positivo ela aumenta
    /// </summary>
    /// <param name="amountToChange"></param>
    /// <param name="type"></param>
    public void ChangeBarValue(float amountToChange, GraciaTypeOfSkill type, GraciaAura aura) {
        switch (type) {
            case GraciaTypeOfSkill.Left:
                // Alterando o valor da barra
                _currentLeftBarValue = Mathf.Clamp(_currentLeftBarValue + amountToChange, 0f, 100f);

                // Alterando a area atual da area
                if (_currentLeftBarValue >= _info.ValueToEnterArea3) _currentLeftBarArea = 2;
                else if (_currentLeftBarValue >= _info.ValueToEnterArea2) _currentLeftBarArea = 1;
                else _currentLeftBarArea = 0;

                // Alterando a aura atual
                if (_currentLeftBarValue > _currentRightBarValue) _currentAura = _leftAura;

                // Avisando que o valor da barra alterou
                OnGraciaBarValueChanged?.Invoke(_currentLeftBarValue, GraciaTypeOfSkill.Left);
                OnGraciaBarAreaChanged?.Invoke(_currentLeftBarArea, GraciaTypeOfSkill.Left);
                break;
            case GraciaTypeOfSkill.Right:
                // Alterando o valor da barra
                _currentRightBarValue = Mathf.Clamp(_currentRightBarValue + amountToChange, 0f, 100f);

                // Alterando a area atual da area
                if (_currentRightBarValue >= _info.ValueToEnterArea3) _currentRightBarArea = 2;
                else if (_currentRightBarValue >= _info.ValueToEnterArea2) _currentRightBarArea = 1;
                else _currentRightBarArea = 0;

                // Alterando a aura atual
                if (_currentRightBarValue > _currentLeftBarValue) _currentAura = _rightAura;

                // Avisando que o valor da barra alterou
                OnGraciaBarValueChanged?.Invoke(_currentRightBarValue, GraciaTypeOfSkill.Right);
                OnGraciaBarAreaChanged?.Invoke(_currentRightBarArea, GraciaTypeOfSkill.Right);
                break;
            default: break;
        }
       
    }

    #endregion

    #region Getters
    public int ReturnCurrentSkillArea(GraciaTypeOfSkill type) {
        switch (type) {
            case GraciaTypeOfSkill.Left: return _currentLeftBarArea;
            case GraciaTypeOfSkill.Right: return _currentRightBarArea;
            default: break;
        }
        return 0;
    }

    public GraciaAura ReturnCurrentAura() => _currentAura;
    #endregion

    #region Setters

    public void SetAura(GraciaTypeOfSkill type, GraciaAura aura) {
        switch (type) {
            case GraciaTypeOfSkill.Left:
                _leftAura = aura;
                break;
            case GraciaTypeOfSkill.Right:
                _rightAura = aura;
                break;
        }
    }

    #endregion

    #region Red Aura 

    CritRatePerAttackIndex _critValues;
    float _critDamage;
    public void SetCritRate(CritRatePerAttackIndex critValues) => _critValues = critValues;
    public void SetCritDamage(float critDamage) => _critDamage = critDamage;
    public CritRatePerAttackIndex ReturnCriValues() => _critValues;
    public float ReturnCritDamage() => _critDamage;

    #endregion
}
