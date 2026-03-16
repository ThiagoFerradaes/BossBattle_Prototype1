using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public event Action<GraciaAura> OnCurrentAuraChanged;

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

        SetInitialAuras();
    }

    void SetInitialAuras() {
        // Pegando a aura esquerda
        SkillSO leftSkill = CurrentSelectedCharacterWhiteBoard.Instance.ReturnCurrentSkillBySlot(SkillSlot.SkillOne);
        if (leftSkill is IGraciaSkill leftGraciaSkill) _leftAura = leftGraciaSkill.ReturnSkillAura();

        // Pegando a aura direita
        SkillSO rightSkill = CurrentSelectedCharacterWhiteBoard.Instance.ReturnCurrentSkillBySlot(SkillSlot.SkillTwo);
        if (rightSkill is IGraciaSkill rightGraciaSkill) _rightAura = rightGraciaSkill.ReturnSkillAura();

        _currentAura = Random.value > 0.5f ? _leftAura : _rightAura;

    }

    #endregion

    #region Bar Value Area

    /// <summary>
    /// Função chamada para alterar o valor da barra da gracia - Se o valor for negativo ela reduz se for positivo ela aumenta
    /// </summary>
    /// <param name="amountToChange"></param>
    /// <param name="type"></param>
    public void ChangeBarValue(float amountToChange, GraciaAura aura) {

        bool isLeftBar = aura == GraciaAura.Red || aura == GraciaAura.Blue;

        if (isLeftBar) {
            // Alterando o valor da barra
            _currentLeftBarValue = Mathf.Clamp(_currentLeftBarValue + amountToChange, 0f, 100f);

            // Alterando a area atual da area
            if (_currentLeftBarValue >= _info.ValueToEnterArea3) _currentLeftBarArea = 2;
            else if (_currentLeftBarValue >= _info.ValueToEnterArea2) _currentLeftBarArea = 1;
            else _currentLeftBarArea = 0;

            // Alterando a aura atual
            if (_currentLeftBarValue > _currentRightBarValue) {
                _currentAura = _leftAura;
                OnCurrentAuraChanged?.Invoke(_currentAura);
            }
            else if (_currentLeftBarValue < _currentRightBarValue) {
                _currentAura = _rightAura;
                OnCurrentAuraChanged?.Invoke(_currentAura);
            }

            // Avisando que o valor da barra alterou
            OnGraciaBarValueChanged?.Invoke(_currentLeftBarValue, GraciaTypeOfSkill.Left);
            OnGraciaBarAreaChanged?.Invoke(_currentLeftBarArea, GraciaTypeOfSkill.Left);
        }
        else {
            // Alterando o valor da barra
            _currentRightBarValue = Mathf.Clamp(_currentRightBarValue + amountToChange, 0f, 100f);

            // Alterando a area atual da area
            if (_currentRightBarValue >= _info.ValueToEnterArea3) _currentRightBarArea = 2;
            else if (_currentRightBarValue >= _info.ValueToEnterArea2) _currentRightBarArea = 1;
            else _currentRightBarArea = 0;

            // Alterando a aura atual
            if (_currentRightBarValue > _currentLeftBarValue) {
                _currentAura = _rightAura;
                OnCurrentAuraChanged?.Invoke(_currentAura);
            }
            else if (_currentRightBarValue < _currentLeftBarValue) {
                _currentAura = _leftAura;
                OnCurrentAuraChanged?.Invoke(_currentAura);
            }

            // Avisando que o valor da barra alterou
            OnGraciaBarValueChanged?.Invoke(_currentRightBarValue, GraciaTypeOfSkill.Right);
            OnGraciaBarAreaChanged?.Invoke(_currentRightBarArea, GraciaTypeOfSkill.Right);
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
    public int ReturnCurrentSkillArea(GraciaAura aura) {

        bool isLeftBar = aura == GraciaAura.Red || aura == GraciaAura.Blue;

        if (isLeftBar) return _currentLeftBarArea;
        else return _currentRightBarArea;
    }
    public GraciaAura ReturnCurrentAura() => _currentAura;

    public GraciaAura ReturnLeftAura() => _leftAura;
    public GraciaAura ReturnRighttAura() => _rightAura;
    public float ReturnBarAmount(GraciaAura aura) {
        bool isLeft = aura == GraciaAura.Blue || aura == GraciaAura.Red;

        if (isLeft) return _currentLeftBarValue;
        else return _currentRightBarValue;
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

public interface IGraciaSkill {
    public GraciaAura ReturnSkillAura() { return GraciaAura.Null; }
}
