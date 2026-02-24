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
    private Dictionary<SkillSlot, Image> cooldownImages;

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
        SetCooldownImagesOff();

    }

    public void SetImage()
    {
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
        
        Image cooldownImage = cooldownImages[slot];
        float timer = cooldown;
        cooldownImage.fillAmount = slot == SkillSlot.Dash? 0 : 1;

        while (timer > 0) {
            timer -= Time.deltaTime;
            if (slot != SkillSlot.Dash) cooldownImage.fillAmount = timer / cooldown;
            else cooldownImage.fillAmount = 1 - (timer / cooldown);
            yield return null;
        }

        cooldownImage.fillAmount = slot == SkillSlot.Dash ? 1 : 0;
        cooldownCoroutines[slot] = null;
    }

    private void SetSkillsImage() {
        Debug.Log("Skills have image");

        CurrentSelectedCharacterWhiteBoard whiteboard = CurrentSelectedCharacterWhiteBoard.Instance;
        Character selectedCharacter = whiteboard.ReturnSelectedCharacter();

        if (whiteboard.ReturnSkillOne(selectedCharacter).UISkillSpriteIcon)
            skillOneImage.sprite = whiteboard.ReturnSkillOne(selectedCharacter).UISkillSpriteIcon;
        if (whiteboard.ReturnSkillTwo(selectedCharacter).UISkillSpriteIcon)
            skillTwoImage.sprite = whiteboard.ReturnSkillTwo(selectedCharacter).UISkillSpriteIcon;
        if (whiteboard.ReturnUltimate(selectedCharacter).UISkillSpriteIcon)
            ultimateImage.sprite = whiteboard.ReturnUltimate(selectedCharacter).UISkillSpriteIcon;
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
        if (cooldownImages == null || cooldownImages.Count == 0)
        {
            StartDictionary();
        }
        
        foreach (var image in cooldownImages) {
            if (image.Key == SkillSlot.Dash || image.Key == SkillSlot.Ultimate) {
                image.Value.fillAmount = 1f;
            }
            else image.Value.fillAmount = 0f;
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

