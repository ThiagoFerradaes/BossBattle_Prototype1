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
    [SerializeField] private List<Image> dashCooldown;
    [SerializeField] private List<Image> skillOneCooldown;
    [SerializeField] private List<Image> skillTwoCooldown;

    [Header("Charges")]
    [SerializeField] private TextMeshProUGUI dashCharge;
    [SerializeField] private TextMeshProUGUI skillOneCharge;
    [SerializeField] private TextMeshProUGUI skillTwoCharge;

    // Actions
    Action<float, float> _energyGainAction;
    Action<SkillSlot, int> _setChargeNumber;
    Action<SkillSlot, int> _changeChargeNumber;

    // Lists
    private Dictionary<SkillSlot, Coroutine> cooldownCoroutines;
    private Dictionary<SkillSlot, List<Image>> cooldownImages;

    #endregion

    #region Methods
    private void Awake() {

        if (Instance == null) Instance = this;
        else Destroy(this);

        _setChargeNumber = (SkillSlot slot, int charge) => SetInitialChargeNumbers(slot, charge);
        _changeChargeNumber = (SkillSlot slot, int currentCharge) => ChangeCharge(slot, currentCharge);
        //_energyGainAction = (currentEnergy, maxEnergy) => UpdateUltimateEnergyCost(currentEnergy, maxEnergy);

        SubscribeEvents();
    }
    private void Start() {

        StartDictionary();
        SetSkillsImage();
        SetInnerCooldownImage();
        SetCooldownImagesOff();

    }

    public void SetImage() {
        SetSkillsImage();
        SetInnerCooldownImage();
        SetCooldownImagesOff();
    }

    void StartDictionary() {
        cooldownCoroutines = new Dictionary<SkillSlot, Coroutine>();
        cooldownImages = new Dictionary<SkillSlot, List<Image>>
        {
            { SkillSlot.Dash, dashCooldown },
            { SkillSlot.SkillOne, skillOneCooldown },
            { SkillSlot.SkillTwo, skillTwoCooldown },
        };
    }
    private void SubscribeEvents() {
        WhiteBoard.OnCooldownSet -= StartCooldownUI;
        WhiteBoard.OnCooldownSet += StartCooldownUI;

        EnergyManager.OnEnergyValueChanged -= _energyGainAction;
        EnergyManager.OnEnergyValueChanged += _energyGainAction;

        WhiteBoard.OnChargesSet -= _setChargeNumber;
        WhiteBoard.OnChargesSet += _setChargeNumber;

        WhiteBoard.OnChargesChange -= _changeChargeNumber;
        WhiteBoard.OnChargesChange += _changeChargeNumber;

    }

    private void StartCooldownUI(SkillSlot slot, float cooldown) {
        if (slot == SkillSlot.BaseAttack) return;

        if (cooldownCoroutines.TryGetValue(slot, out Coroutine currentRoutine) && currentRoutine != null) {
            StopCoroutine(currentRoutine);
        }

        Coroutine newRoutine = StartCoroutine(CooldownRoutine(slot, cooldown));
        cooldownCoroutines[slot] = newRoutine;
    }

    //void UpdateUltimateEnergyCost(float currentEnergy, float maxEnergy) {
    //    cooldownImages[SkillSlot.Ultimate].fillAmount = 1 - (currentEnergy / maxEnergy);
    //}

    private IEnumerator CooldownRoutine(SkillSlot slot, float cooldown) {

        List<Image> cooldownImage = cooldownImages[slot];
        float timer = cooldown;
        foreach (var image in cooldownImage) {
            image.fillAmount = slot == SkillSlot.Dash ? 0 : 1;
        }

        while (timer > 0) {
            timer -= Time.deltaTime;
            if (slot != SkillSlot.Dash) {
                foreach (var image in cooldownImage) {
                    image.fillAmount = timer / cooldown;
                }
            }
            else {
                foreach (var image in cooldownImage) {
                    image.fillAmount = 1 - (timer / cooldown);
                }
            }
            yield return null;
        }

        foreach (var image in cooldownImage) {
            image.fillAmount = slot == SkillSlot.Dash ? 1 : 0;
        }
        cooldownCoroutines[slot] = null;
    }

    private void SetSkillsImage() {

        CurrentSelectedCharacterWhiteBoard whiteboard = CurrentSelectedCharacterWhiteBoard.Instance;
        Character selectedCharacter = whiteboard.ReturnSelectedCharacter();

        if (whiteboard.ReturnSkillOne(selectedCharacter).MapDescriptionInfo.UISkillSpriteIcon)
            skillOneImage.sprite = whiteboard.ReturnSkillOne(selectedCharacter).MapDescriptionInfo.UISkillSpriteIcon;
        if (whiteboard.ReturnSkillTwo(selectedCharacter).MapDescriptionInfo.UISkillSpriteIcon)
            skillTwoImage.sprite = whiteboard.ReturnSkillTwo(selectedCharacter).MapDescriptionInfo.UISkillSpriteIcon;
        if (whiteboard.ReturnUltimate(selectedCharacter).MapDescriptionInfo.UISkillSpriteIcon)
            ultimateImage.sprite = whiteboard.ReturnUltimate(selectedCharacter).MapDescriptionInfo.UISkillSpriteIcon;
    }

    void SetInnerCooldownImage() {
        CurrentSelectedCharacterWhiteBoard whiteboard = CurrentSelectedCharacterWhiteBoard.Instance;
        Character selectedCharacter = whiteboard.ReturnSelectedCharacter();

        if (whiteboard.ReturnSkillOne(selectedCharacter).MapDescriptionInfo.UISkillSpriteIcon)
            skillOneCooldown[0].sprite = whiteboard.ReturnSkillOne(selectedCharacter).MapDescriptionInfo.UISkillSpriteIcon;
        if (whiteboard.ReturnSkillTwo(selectedCharacter).MapDescriptionInfo.UISkillSpriteIcon)
            skillTwoCooldown[0].sprite = whiteboard.ReturnSkillTwo(selectedCharacter).MapDescriptionInfo.UISkillSpriteIcon;
    }
    public void ChangeSkillImage(Sprite newSprite, SkillSlot typeOfSkill) {
        switch (typeOfSkill) {
            case SkillSlot.SkillOne:
                skillOneImage.sprite = newSprite;
                break;
            case SkillSlot.SkillTwo:
                skillTwoImage.sprite = newSprite;
                break;
            case SkillSlot.Ultimate:
                ultimateImage.sprite = newSprite;
                break;
        }
    }

    private void SetCooldownImagesOff() {
        if (cooldownImages == null || cooldownImages.Count == 0) {
            StartDictionary();
        }

        foreach (var pair in cooldownImages) {
            if (pair.Key == SkillSlot.Dash || pair.Key == SkillSlot.Ultimate) {
                foreach (var image in pair.Value) {
                    image.fillAmount = 1f;
                }
            }
            else {
                foreach (var image in pair.Value) {
                    image.fillAmount = 0f;
                }
            }
        }
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

    private void OnDestroy() {
        WhiteBoard.OnCooldownSet -= StartCooldownUI;
        EnergyManager.OnEnergyValueChanged -= _energyGainAction;
        WhiteBoard.OnChargesSet -= _setChargeNumber;
        WhiteBoard.OnChargesChange -= _changeChargeNumber;
    }
    #endregion
}

