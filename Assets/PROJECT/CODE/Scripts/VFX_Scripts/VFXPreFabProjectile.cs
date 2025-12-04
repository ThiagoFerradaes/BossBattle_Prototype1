using System.Collections;
using UnityEngine;

public class VFXPreFabProjectile : MonoBehaviour
{
    VFXAtributes _damageAtributes;
    Coroutine _moveRoutine, _collisionRoutine;

    bool _hasCollided;
    public void Initialize(VFXAtributes atributes)
    {
        _damageAtributes = atributes;
        gameObject.SetActive(true);
        _moveRoutine ??= StartCoroutine(ProjectileMoveRoutine());
    }

    public void TurnOff()
    {
        _hasCollided = false;
        _moveRoutine = null;
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.VFX);
    }

    IEnumerator ColisionTimer()
    {
        _hasCollided = true;
        yield return new WaitForSeconds(_damageAtributes.VFXPosCollisionDuration);
        TurnOff();
    }

    IEnumerator ProjectileMoveRoutine()
    {
        float duration =
            _damageAtributes.Distance / _damageAtributes.VFXSpeed;
        float timer = 0;

        while (timer < duration && !_hasCollided)
        {
            transform.position += _damageAtributes.VFXSpeed * Time.deltaTime * transform.forward;
            timer += Time.deltaTime;
            yield return null;
        }

        TurnOff();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_damageAtributes.UnitsToHit.ContainsLayer(other.gameObject.layer)) return;

        if (!_damageAtributes.CrossEnemy) _collisionRoutine ??= StartCoroutine(ColisionTimer());
    }
}
