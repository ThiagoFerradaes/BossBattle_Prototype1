using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillDescriptionMouseManager : MonoBehaviour {
    [SerializeField] Color hooverColor;
    [SerializeField] Color defaultColor;
    [SerializeField] Image arrowImage;
    [SerializeField] TextMeshProUGUI readMoreText;

    public void MouseEnterReadMore(bool isHovering) {
        arrowImage.color = isHovering ? hooverColor : defaultColor;
        readMoreText.color = isHovering ? hooverColor : defaultColor;
    }
}
