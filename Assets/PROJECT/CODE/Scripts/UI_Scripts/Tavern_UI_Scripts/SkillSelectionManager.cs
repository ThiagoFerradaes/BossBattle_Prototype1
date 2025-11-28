using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SkillSelectionIndex {
    public GameObject IndexParent;
    public List<Image> ListOfSkillsIcons;
    public List<Image> ListOfSkillsBackGround;
    public List<Image> ListOfSkillsLocks;
}
public class SkillSelectionManager : MonoBehaviour {

    [Header("Components")]
    [SerializeField] GameObject SkillSelectionScreen;
    [SerializeField] TextMeshProUGUI SkillName;
    [SerializeField] TextMeshProUGUI SkillLongDescription;
    [SerializedDictionary("Slot", "Conexion"), SerializeField]
    SerializedDictionary<SkillSlot, Image> dictionaryOfConexions = new();
    [SerializedDictionary("Slot", "Index"), SerializeField]
    SerializedDictionary<SkillSlot, SkillSelectionIndex> dictionaryOfTypeOfIndexers = new();

    public void Initialize(SkillSlot slotInitialized) {

    }

    void SetPassive() {
        Character selectedCharacter = PlayerWhiteBoard.Instance.ReturnSelectedCharacter();
        PassiveSO passive = PlayerWhiteBoard.Instance.ReturnPassive(selectedCharacter);

        SkillSelectionIndex index = dictionaryOfTypeOfIndexers[SkillSlot.Passive];

        // Textos
        SkillName.text = passive.PassiveName;
        SkillLongDescription.text = passive.LongDescription;

        // Conexão
        foreach (var conexion in dictionaryOfConexions.Keys) {
            bool slotSelected = conexion == SkillSlot.Passive;
            dictionaryOfConexions[conexion].gameObject.SetActive(slotSelected);
        }
        // Icones
        index.ListOfSkillsIcons[0].sprite = passive.PassiveIcon;
        index.ListOfSkillsLocks[0].gameObject.SetActive(false);
        index.ListOfSkillsBackGround[0].gameObject.SetActive(false);
        index.IndexParent.SetActive(true);
    }
    void SetUltimates() {
        CharacterSO selectedCharacterInfo = PlayerWhiteBoard.Instance.ReturnSelectedCharacterSO();
        List<UltimateSkillSO> listOfUltimate = selectedCharacterInfo.CharacterListOfUltimates;
        
        SkillSelectionIndex index = dictionaryOfTypeOfIndexers[SkillSlot.Ultimate];

        // Conexão
        foreach (var conexion in dictionaryOfConexions.Keys) {
            bool slotSelected = conexion == SkillSlot.Ultimate;
            dictionaryOfConexions[conexion].gameObject.SetActive(slotSelected);
        }

        // Icones
        for (int i = 0; i < listOfUltimate.Count; i++) {
            index.ListOfSkillsIcons[i].sprite = listOfUltimate[i].SkillSpriteIcon;
        }
        
        index.ListOfSkillsLocks[0].gameObject.SetActive(false);
        index.ListOfSkillsBackGround[0].gameObject.SetActive(false);
        index.IndexParent.SetActive(true);
    }

    void ChangeUltimatesIcon() {
        Character selectedCharacter = PlayerWhiteBoard.Instance.ReturnSelectedCharacter();
        UltimateSkillSO currentUltimate = PlayerWhiteBoard.Instance.ReturnUltimate(selectedCharacter);

        // Textos
        SkillName.text = currentUltimate.SkillName;
        SkillLongDescription.text = currentUltimate.SkillLongDescription;
    }
    void ChangeSelectedSkill(SkillSlot slot) {

    }
}
