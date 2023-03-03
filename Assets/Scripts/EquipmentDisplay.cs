using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquipmentDisplay : MonoBehaviour, IDropHandler 
{
    public Equipment equipment;
    public TMP_Text nameText;
    public TMP_Text powerText;
    public TMP_Text guardText;
    public TMP_Text speedText;
    public TMP_Text hpText;
    public Image artworkImage;

    public int slotId;
    public bool ownByOppo;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

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

    void IDropHandler.OnDrop(PointerEventData eventData) {
        GameObject cardPlayed = eventData.pointerDrag;
        GameObject targetSlot = eventData.pointerCurrentRaycast.gameObject;

        // On vérifie que la cible soit bien un emplacement d'equipement
        // Si l'emplacement n'est pas déjà enchanté
        if (targetSlot == gameObject && targetSlot.transform.GetComponentInChildren<CardDisplay>() == null) {
            // On vérifie les conditions de ciblage pour pouvoir placer la carte
            bool targetCondition = false;
            TargetType[] cardPlayedTargetType = cardPlayed.GetComponent<CardDisplay>().card.targetType;
            foreach (TargetType cardTargetType in cardPlayedTargetType) {
                if (cardTargetType == TargetType.PlayerEquipment && !ownByOppo) {
                    gameManager.tryToPutOnBoard(cardPlayed, gameObject, true);
                    targetCondition = true;
                    break;
                }
            }

            // On place la carte si les conditions de ciblages sont respectées
            if (!targetCondition) {
                Debug.Log("ERR : bad target");
            }
        }
    }
}
