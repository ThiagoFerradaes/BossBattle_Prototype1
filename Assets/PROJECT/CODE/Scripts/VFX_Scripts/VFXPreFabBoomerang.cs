using System.Collections;
using UnityEngine;

public class VFXPreFabBoomerang : MonoBehaviour
{
    VFXAtributes _vfxAtributes;
    GameObject _parent;

    Coroutine _moveRoutine;

    public void Initialize(VFXAtributes atributes, GameObject parent) {
        _vfxAtributes = atributes;
        _parent = parent;
        gameObject.SetActive(true);
        _moveRoutine ??= StartCoroutine(BoomerangMoveRoutine());
    }

    IEnumerator BoomerangMoveRoutine() {
        float duration = _vfxAtributes.Distance / _vfxAtributes.VFXSpeed;
        float timer = 0f;

        while (timer < duration) {
            transform.position += _vfxAtributes.VFXSpeed * Time.deltaTime * transform.forward;
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(_vfxAtributes.TimeStopped);

        timer = 0f;
        float distanceToTarget = Vector3.Distance(transform.position, _parent.transform.position);
        while (distanceToTarget > _vfxAtributes.MinDistanceBack) {

            // Calculo da direção 
            Vector3 direction = _parent.transform.position - transform.position;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);

            // Calculo da velocidade
            distanceToTarget = Vector3.Distance(transform.position, _parent.transform.position);
            timer += Time.deltaTime;
            float speed = distanceToTarget / (duration - timer);

            Vector3 nextPos = transform.position + speed * Time.deltaTime * transform.forward;

            float newDist = Vector3.Distance(nextPos, _parent.transform.position);

            if (newDist > distanceToTarget) {
                transform.position = _parent.transform.position;
                End();
                yield break;
            }

            transform.position = nextPos;

            yield return null;
        }

        End();
    }

    void End() {
        _moveRoutine = null;
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.VFX);
    }
}
