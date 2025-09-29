using System.Collections;
using UnityEngine;

public class CrabManager : EnemyBehaviourManager {

    [Header("Components")]
    public StatusManager StatusManager;

    [HideInInspector] public GameObject Player;

    public override IEnumerator Start() {

        Player = PlayerManager.Instance.Player;

        return base.Start();
    }

    //public IEnumerator WalkToPlayer() {

    //}
}
