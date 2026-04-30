using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CyrusRank { E, D, C, B, A, S, SS }
public class CyrusPassiveManager : PassiveSkillManager {

    #region Parameters

    public static CyrusPassiveManager Instance;
    StatusManager _statusManager;

    // Components
    CyrusPassiveSO _info;

    // Atributes
    CyrusRank _currentRank = CyrusRank.E;
    Dictionary<SkillSlot, int> _skillLevel = new() {
        { SkillSlot.SkillOne, 0 },
        { SkillSlot.SkillTwo, 0 },
        { SkillSlot.Ultimate, 0 },
    };
    Dictionary<SkillSlot, int> _skillUses = new()
    {
        { SkillSlot.SkillOne, 0 },
        { SkillSlot.SkillTwo, 0 },
        { SkillSlot.Ultimate, 0 },
    };

    // Actions
    public event Action<SkillSlot> OnSkillLevelUp;
    public event Action<float, float> OnRankLevelUp;


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

    }

    void Initialize(PassiveSO passive, GameObject parent) {
        _info = passive as CyrusPassiveSO;
        _statusManager = parent.GetComponent<StatusManager>();
    }

    #endregion

    #region ExpGain
    private bool HasReachedMaxRank => _currentRank >= CyrusRank.SS;

    public void AddUseSkill(SkillSlot slot, int amountOfUsesToUpgrade, List<Sprite> listOfSprites)
    {
        if (HasReachedMaxRank) return;
        if (!_skillUses.ContainsKey(slot)) return;
        if (_skillLevel[slot] >= 3) return;

        int uses = _skillUses[slot];

        if (uses + 1 >= amountOfUsesToUpgrade) UpgradeSkill(slot, listOfSprites);
        else _skillUses[slot]++;
    }

    void UpgradeSkill(SkillSlot slot, List<Sprite> listOfSprites) {
        if (HasReachedMaxRank) return;

        if (!_skillLevel.ContainsKey(slot)) return;

        if (_skillLevel[slot] >= 3) return;

        Debug.Log(parent.name);
        _info.RankUpSound.Post(parent);

        _skillLevel[slot]++;
        _skillUses[slot] = 0;

        Sprite newSkillSprite = listOfSprites[_skillLevel[slot]];
        PlayerSkillUI.Instance.ChangeSkillImage(newSkillSprite, slot);

        _currentRank++;

        int maxEnum = Enum.GetValues(typeof(CyrusRank)).Length;

        OnSkillLevelUp?.Invoke(slot);
        OnRankLevelUp?.Invoke((int)_currentRank, maxEnum - 1);

        if (_currentRank == CyrusRank.SS) {
            ReachRankSS();
        }
    }

    void ReachRankSS() {
        foreach(var status in _info.ListOfStatusToBuff.Keys) {
            float percent = _info.ListOfStatusToBuff[status] / 100;
            _statusManager.ChangeStatus(status, percent, true);
        }
    }
    #region Getters
    public int ReturnSkillLevel(SkillSlot slot) => _skillLevel[slot];

    public CyrusRank ReturnCyrusRank() => _currentRank;

    #endregion

    #endregion

    #endregion
}
