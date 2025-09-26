using System.Collections;
using TMPro;
using UnityEngine;

public class CrabMarineAnimal : MonoBehaviour {

    #region Parameters

    [Header("Components")]
    [SerializeField] CrabMarineAnimalSO MarineAnimalInfo;

    // Coroutines
    Coroutine _durationCoroutine;

    #endregion

    #region Methods
    public void OnStart() {
        gameObject.SetActive(true);

        _durationCoroutine ??= StartCoroutine(Duration());
    }

    IEnumerator Duration() {

        yield return new WaitForSeconds(MarineAnimalInfo.duration);

        _durationCoroutine = null;

        End();
    }

    void End() {
        if (_durationCoroutine != null) StopCoroutine(_durationCoroutine);

        MarineAnimalInfo.OnEnd();

        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.Hitbox);
    }

    private void OnTriggerEnter(Collider other) {

        MarineAnimalInfo.OnTrigger(other, this);

        End();
    }

    #endregion
}
