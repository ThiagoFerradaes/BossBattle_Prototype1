using UnityEngine;

[CreateAssetMenu(menuName = "Skills / BaseAttack")]
public class BaseAttackSO : SkillSO {

    [Header("Animation")]
    public string FirstBaseAttackParameter;
    public string SecondBaseAttackParameter;
    public string FirstBaseAttackAnimationName;
    public string SecondtBaseAttackAnimationName;

    [Header("Atributes")]
    public Vector3 FirstBaseAttackHitBoxPosition;
    public Vector3 SecondtBaseAttackHitBoxPosition;
    public float FirstAttackDamage;
    public float SecondAttackDamage;
    public float CooldownBetweenAttacks;
    public float MaxTimeBetweenAttacks;
}
