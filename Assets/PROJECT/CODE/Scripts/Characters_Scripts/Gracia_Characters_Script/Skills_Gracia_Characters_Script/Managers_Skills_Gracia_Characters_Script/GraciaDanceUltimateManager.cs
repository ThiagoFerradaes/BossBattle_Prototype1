using UnityEngine;

public class GraciaDanceUltimateManager : SkillObjectManager
{
    #region Paramethers

    // Components
    GraciaDanceUltimateSO _info;

    // Int
    int _skillLevel;

    // Coroutine
    Coroutine _skillDurationRoutine;
    #endregion

    #region Initialize

    public override void UseSkill(SkillSO skill)
    {
        Initialize(skill);

        animationCoroutine ??= StartCoroutine(AttackCoroutine(0, _info.attackAnimationParameter, _info.attackAnimationParameter, 0));
    }

    void Initialize(SkillSO skill)
    {
        if (_info == null) _info = skill as GraciaDanceUltimateSO;
        if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
    }

    #endregion

    #region Animation Methods Override

    public override void FirstFunc()
    {
        base.FirstFunc();

        energyManager.SetCanGainEnergy(false);
    }
    public override void ThirdFunc()
    {
        DecideBehaviour();
    }
    public override void FourthFunc()
    {
        base.FourthFunc();

        UnblockInputs();
    }

    #endregion

    #region Behaviours

    void DecideBehaviour()
    {
        GraciaAura currentAura = GraciaPassiveManager.Instance.ReturnCurrentAura();

        switch (currentAura)
        {
            case GraciaAura.Yellow: YellowBehaviour(); break;
            case GraciaAura.Green: GreenBehaviour();  break;
            case GraciaAura.Blue: BlueBehaviour(); break;
            case GraciaAura.Red: RedBehaviour(); break;
        }
    }

    void BlueBehaviour()
    {

    }
    void YellowBehaviour()
    {

    }
    void RedBehaviour()
    {

    }
    void GreenBehaviour()
    {

    }
    #endregion

    #region Instantiate



    #endregion
}
