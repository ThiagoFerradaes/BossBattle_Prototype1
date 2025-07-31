using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDamageContext {
    public float Damage;
    public float Duration;
    public bool IsTrueDamage;
    public Tags UnitToHitTag;
    public List<Modifiers> ListOfModifiers;

    public InstantDamageContext(float damage, float hitBoxDuration
        , bool isTrueDamage, Tags tag,  List<Modifiers> listOfModifiers = null) {
        this.Damage = damage;
        this.Duration = hitBoxDuration;
        this.IsTrueDamage = isTrueDamage;
        this.UnitToHitTag = tag;
        this.ListOfModifiers = listOfModifiers ?? new List<Modifiers>();
    }
}
public class InstantDamageHitBox : MonoBehaviour
{
    #region Parameters

    float _damage;
    float _duration;
    string _tag;
    bool _isTrueDamage;

    #endregion

    #region Methods
    public void Initialize(InstantDamageContext context) {
        _damage = context.Damage; _duration = context.Duration; _isTrueDamage = context.IsTrueDamage;
        _tag = context.UnitToHitTag.ToString();
        gameObject.SetActive(true);
        StartCoroutine(AttackDuration());
    }

    IEnumerator AttackDuration() {
        float timer = 0;
        while (timer < _duration) {
            timer += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag(_tag)) return;

        if (!other.TryGetComponent<HealthManager>(out HealthManager health)) return;

        health.TakeDamage(_damage, _isTrueDamage);
    }
    #endregion
}
