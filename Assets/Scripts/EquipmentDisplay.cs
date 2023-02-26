using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquipmentDisplay : MonoBehaviour
{
    public Equipment equipment;
    public TMP_Text nameText;
    public TMP_Text powerText;
    public TMP_Text guardText;
    public TMP_Text speedText;
    public TMP_Text hpText;
    public Image artworkImage;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = equipment.name;
        powerText.text = equipment.powerPoint.ToString();
        guardText.text = equipment.guardPoint.ToString();
        speedText.text = equipment.speedPoint.ToString();
        hpText.text = equipment.healthPoint.ToString();
        artworkImage.sprite = equipment.artwork;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
