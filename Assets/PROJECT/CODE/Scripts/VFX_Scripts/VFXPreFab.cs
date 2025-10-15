using UnityEngine;

public class VFXPreFab : MonoBehaviour
{
    public void Initialize(float preFabDuration) {
        gameObject.SetActive(true);
        Invoke(nameof(TurnOff), preFabDuration);
    }

    public void TurnOff() {
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.VFX);
    }
}
