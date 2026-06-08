using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "DeathBehaviour/ CrabBarrier")]
public class CrabBarrierDeathBehaviour : DeathBehaviourSO {

    [SerializeField] float speed;
    [SerializeField] float finalHeight;
    public override void Death(GameObject parent) {
        parent.transform.DOMoveY(speed, finalHeight).OnComplete(() => {
            parent.SetActive(false);
        });
    }
}
