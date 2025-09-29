using System.Collections;
using UnityEngine;

public class CrabManager : EnemyBehaviourManager {

    [Header("Components")]
    public StatusManager StatusManager;

    GameObject _player;

    public override IEnumerator Start() {

        _player = PlayerManager.Instance.Player;

        return base.Start();
    }

    //public IEnumerator WalkToPlayer() {

    //}
}
