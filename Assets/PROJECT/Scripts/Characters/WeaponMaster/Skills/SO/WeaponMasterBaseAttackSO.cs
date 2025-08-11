using UnityEngine;

[CreateAssetMenu(menuName = "Skills / BaseAttack")]
public class WeaponMasterBaseAttackSO : SkillSO {

    [Header("Animation")]
    public string FirstBaseAttackParameter;
    public string FirstBaseAttackAnimationName;
    public string SecondBaseAttackParameter;
    public string SecondtBaseAttackAnimationName;

    [Header("Atributes")]
    public float FirstAttackDamage;
    public float SecondAttackDamage;
    public float FirstAttackHitBoxDuration;
    public float SecondAttackHitBoxDuration;
    public float CooldownBetweenAttacks;
    public float MaxTimeBetweenAttacks;
    public float PenetrationFirstAttack;
    public float PenetrationSecondAttack;   
    public string SwordName;
    public bool HitShield;
    public Tags EnemyTag;
    public DamageType DamageType;
    public GameObject SwordPrefab;
    public Vector3 FirstBaseAttackHitBoxPosition;
    public Vector3 SecondtBaseAttackHitBoxPosition;
    public Vector3 WeaponPosition;
}
