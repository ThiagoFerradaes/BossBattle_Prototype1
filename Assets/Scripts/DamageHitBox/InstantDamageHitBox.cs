using System.Collections;
using UnityEngine;

public class InstantDamageHitBox : MonoBehaviour
{
    float _damage;
    float _duration;
    public void Initialize(float damage, float duration) {
        _damage = damage; _duration = duration;
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
        if (!other.CompareTag("Enemy")) return;


    }
}
