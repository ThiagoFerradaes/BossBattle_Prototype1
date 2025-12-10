using System.Collections;
using UnityEngine;

public class VFXPreFabProjectile : MonoBehaviour
{
    VFXAtributes _vfxAtributes;
    Coroutine _moveRoutine, _collisionRoutine;

    bool _hasCollided;
    public void Initialize(VFXAtributes atributes)
    {
        _vfxAtributes = atributes;
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
        yield return new WaitForSeconds(_vfxAtributes.VFXPosCollisionDuration);
        TurnOff();
    }

    IEnumerator ProjectileMoveRoutine()
    {
        float duration =
            _vfxAtributes.Distance / _vfxAtributes.VFXSpeed;
        float timer = 0;

        while (timer < duration && !_hasCollided)
        {
            transform.position += _vfxAtributes.VFXSpeed * Time.deltaTime * transform.forward;
            timer += Time.deltaTime;
            yield return null;
        }

        TurnOff();
    }

    // N�O SEI O QUE FAZER SOBRE ISSO AQUI
    private void OnTriggerEnter(Collider other)
    {
        if (!_vfxAtributes.UnitsToHit.ContainsLayer(other.gameObject.layer)) return;

        if (!_vfxAtributes.CrossEnemy) _collisionRoutine ??= StartCoroutine(ColisionTimer());
    }

    // N�o funcionou
    private void OnParticleCollision(GameObject other) {
        Debug.Log("Collision");
        if (!_vfxAtributes.UnitsToHit.ContainsLayer(other.layer)) return;

        if (!_vfxAtributes.CrossEnemy) _collisionRoutine ??= StartCoroutine(ColisionTimer());
    }
}
