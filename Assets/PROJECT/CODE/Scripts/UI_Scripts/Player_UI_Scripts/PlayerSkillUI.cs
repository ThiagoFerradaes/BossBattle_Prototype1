using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillUI : MonoBehaviour {

    #region Parameters

    public static PlayerSkillUI Instance;

    [Header("Skill Image")]
    [SerializeField] private Image dashSkillImage;
    [SerializeField] private Image skillOneImage;
    [SerializeField] private Image skillTwoImage;
    [SerializeField] private Image ultimateImage;

    [Header("Cooldown Image")]
    [SerializeField] private Image dashCooldown;
    [SerializeField] private Image skillOneCooldown;
    [SerializeField] private Image skillTwoCooldown;
    [SerializeField] private Image ultimateEnergy;

    [Header("Charges")]
    [SerializeField] private TextMeshProUGUI baseAttackCharge;
    [SerializeField] private TextMeshProUGUI dashCharge;
    [SerializeField] private TextMeshProUGUI skillOneCharge;
    [SerializeField] private TextMeshProUGUI skillTwoCharge;

    // Actions
    Action<float, float> _energyGainAction;
    Action<SkillSlot, int> _setChargeNumber;
    Action<SkillSlot, int> _changeChargeNumber;

    // Lists
    private Dictionary<SkillSlot, Coroutine> cooldownCoroutines;
    private Dictionary<SkillSlot, Image> cooldownImages;

    #endregion

    #region Methods
    private void Awake() {

        if (Instance == null) Instance = this;
        else Destroy(this);

        _setChargeNumber = (SkillSlot slot, int charge) => SetInitialChargeNumbers(slot, charge);
        _changeChargeNumber = (SkillSlot slot, int currentCharge) => ChangeCharge(slot, currentCharge);
        _energyGainAction = (currentEnergy, maxEnergy) => UpdateUltimateEnergyCost(currentEnergy, maxEnergy);

        SubscribeEvents();
    }
    private void Start() {

        StartDictionary();
        SetSkillsImage();
        SetCooldownImagesOff();

    }

    void StartDictionary() {
        cooldownCoroutines = new Dictionary<SkillSlot, Coroutine>();
        cooldownImages = new Dictionary<SkillSlot, Image>
        {
            { SkillSlot.Dash, dashCooldown },
            { SkillSlot.SkillOne, skillOneCooldown },
            { SkillSlot.SkillTwo, skillTwoCooldown },
            { SkillSlot.Ultimate, ultimateEnergy }
        };
    }
    private void SubscribeEvents() {
        PlayerSkillCooldownManager.OnCooldownSet -= StartCooldownUI;
        PlayerSkillCooldownManager.OnCooldownSet += StartCooldownUI;

        EnergyManager.OnEnergyValueChanged -= _energyGainAction;
        EnergyManager.OnEnergyValueChanged += _energyGainAction;

        PlayerSkillCooldownManager.OnChargesSet -= _setChargeNumber;
        PlayerSkillCooldownManager.OnChargesSet += _setChargeNumber;

        PlayerSkillCooldownManager.OnChargesChange -= _changeChargeNumber;
        PlayerSkillCooldownManager.OnChargesChange += _changeChargeNumber;

    }

    private void StartCooldownUI(SkillSlot slot, float cooldown) {
        if (slot == SkillSlot.BaseAttack) return;

        if (cooldownCoroutines.TryGetValue(slot, out Coroutine currentRoutine) && currentRoutine != null) {
            StopCoroutine(currentRoutine);
        }

        Coroutine newRoutine = StartCoroutine(CooldownRoutine(slot, cooldown));
        cooldownCoroutines[slot] = newRoutine;
    }

    void UpdateUltimateEnergyCost(float currentEnergy, float maxEnergy) {
        cooldownImages[SkillSlot.Ultimate].fillAmount = 1 - (currentEnergy / maxEnergy);
    }

    private IEnumerator CooldownRoutine(SkillSlot slot, float cooldown) {
        Image cooldownImage = cooldownImages[slot];
        float timer = cooldown;
        cooldownImage.fillAmount = 1f;

        while (timer > 0) {
            timer -= Time.deltaTime;
            cooldownImage.fillAmount = timer / cooldown;
            yield return null;
        }

        cooldownImage.fillAmount = 0f;
        cooldownCoroutines[slot] = null;
    }

    private void SetSkillsImage() {
        Debug.Log("Skills have image");

        PlayerWhiteBoard whiteboard = PlayerWhiteBoard.Instance;
        Character selectedCharacter = whiteboard.ReturnSelectedCharacter();

        if (whiteboard.ReturnSkillOne(selectedCharacter).SkillSpriteIcon)
            skillOneImage.sprite = whiteboard.ReturnSkillOne(selectedCharacter).SkillSpriteIcon;
        if (whiteboard.ReturnSkillTwo(selectedCharacter).SkillSpriteIcon)
            skillTwoImage.sprite = whiteboard.ReturnSkillTwo(selectedCharacter).SkillSpriteIcon;
        if (whiteboard.ReturnUltimate(selectedCharacter).SkillSpriteIcon)
            ultimateImage.sprite = whiteboard.ReturnUltimate(selectedCharacter).SkillSpriteIcon;

    }

    private void SetCooldownImagesOff() {
        foreach (var image in cooldownImages.Values) {
            image.fillAmount = 0f;
        }

        cooldownImages[SkillSlot.Ultimate].fillAmount = 1;
    }

    private void OnDisable() {
        PlayerSkillCooldownManager.OnCooldownSet -= StartCooldownUI;
        EnergyManager.OnEnergyValueChanged -= _energyGainAction;
        PlayerSkillCooldownManager.OnChargesSet -= _setChargeNumber;
        PlayerSkillCooldownManager.OnChargesChange -= _changeChargeNumber;

    }

    void SetInitialChargeNumbers(SkillSlot slot, int charges) {
        switch (slot) {
            case SkillSlot.Dash:
                if (charges < 2)
                    dashCharge.gameObject.SetActive(false);
                else {
                    dashCharge.gameObject.SetActive(true);
                    dashCharge.text = charges.ToString();
                }
                break;
            case SkillSlot.SkillOne:
                if (charges < 2)
                    skillOneCharge.gameObject.SetActive(false);
                else {
                    skillOneCharge.gameObject.SetActive(true);
                    skillOneCharge.text = charges.ToString();
                }
                break;
            case SkillSlot.SkillTwo:
                if (charges < 2)
                    skillTwoCharge.gameObject.SetActive(false);
                else {
                    skillTwoCharge.gameObject.SetActive(true);
                    skillTwoCharge.text = charges.ToString();
                }
                break;
        }
    }

    void ChangeCharge(SkillSlot slot, int currentCharges) {
        switch (slot) {
            case SkillSlot.Dash:
                dashCharge.text = currentCharges.ToString();
                break;
            case SkillSlot.SkillOne:
                skillOneCharge.text = currentCharges.ToString();
                break;
            case SkillSlot.SkillTwo:
                skillTwoCharge.text = currentCharges.ToString();
                break;
        }
    }

    #endregion
}

