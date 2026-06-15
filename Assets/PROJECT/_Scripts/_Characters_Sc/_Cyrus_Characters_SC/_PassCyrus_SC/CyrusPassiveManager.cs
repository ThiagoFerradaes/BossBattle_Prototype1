using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyrusPassiveManager : PassiveSkillManager {

    #region Parameters

    public static CyrusPassiveManager Instance;
    StatusManager _statusManager;

    // Components
    CyrusPassiveSO _info;


    #endregion

    #region Methods

    #region Initialize
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

    }
    public override void OnStart(PassiveSO passive, GameObject parent) {

        base.OnStart(passive, parent);

        Initialize(passive, parent);

        gameObject.SetActive(true);

        AditionalUIManager.Instance.InstantiateUI(_info.CyrusUI);

        BattleRankManager.Instance.IncreasePointsMultiplier(_info.PercentOfRankPointsToIncrease);

    }

    void Initialize(PassiveSO passive, GameObject parent) {
        _info = passive as CyrusPassiveSO;
        _statusManager = parent.GetComponent<StatusManager>();

        BattleRankManager.OnRankChanged += SetBaseAttackAndDefenseBasedOnRank;
    }

    private void OnDestroy() {
        BattleRankManager.OnRankChanged -= SetBaseAttackAndDefenseBasedOnRank;
    }
    #endregion

    #region ExpGain

    void SetBaseAttackAndDefenseBasedOnRank(BattleRank currentRank) {

        // defesa
        float rankLevel = (float)currentRank;
        float maxRanks = Enum.GetValues(typeof(BattleRank)).Length;
        float division = rankLevel / (maxRanks - 1);

        float defense = _info.DefenseAtRankSS * Mathf.Pow(division, 2);
        _statusManager.SetBaseStatus(StatusType.Defense , defense);


        // ataque
        float atk = 1 + _info.ExtraAttackAtRankSS * Mathf.Pow(division, 2);
        _statusManager.SetBaseStatus(StatusType.Attack, atk);
    }


    public void AddUseSkill(SkillSlot slot, int amountOfUsesToUpgrade, List<Sprite> listOfSprites) {

    }


    #region Getters
    public int ReturnSkillLevel(SkillSlot slot) => 0;


    #endregion

    #endregion

    #endregion
}
