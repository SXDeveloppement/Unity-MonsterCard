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
    public SpriteRenderer illustration;

    public int slotId;
    public bool ownedByOppo;
    public GameObject slotCard;
    public GameObject cardOnSlot;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        slotCard = transform.parent.parent.GetChild(1).gameObject;

        nameText.text = equipment.name;
        powerText.text = equipment.powerPoint.ToString();
        guardText.text = equipment.guardPoint.ToString();
        speedText.text = equipment.speedPoint.ToString();
        hpText.text = equipment.healthPoint.ToString();
        if (artworkImage != null)
            artworkImage.sprite = equipment.artwork;
        if (illustration != null)
            illustration.sprite = equipment.artwork;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool onDrop(GameObject cardPlayed) {
        bool isPutOnBoard = false;
        if (GameManager.dragged) {
            GameObject targetSlot = this.gameObject;
            Debug.Log("Drop Equipment");

            // Si l'emplacement est vide et que la carte dans la main ou dans la zone de contre attaque
            if (cardOnSlot == null && (cardPlayed.GetComponent<CardDisplay>().status == Status.Hand || cardPlayed.GetComponent<CardDisplay>().status == Status.SlotHidden)) {
                // On vérifie les conditions de ciblage pour pouvoir placer la carte
                bool targetCondition = false;
                TargetType[] cardPlayedTargetType = cardPlayed.GetComponent<CardDisplay>().card.targetType;
                foreach (TargetType cardTargetType in cardPlayedTargetType) {
                    if (cardTargetType == TargetType.PlayerEquipment && !ownedByOppo) {
                        isPutOnBoard = gameManager.tryToPutOnBoard(cardPlayed, targetSlot, true);
                        targetCondition = true;
                        break;
                    }                   
                }

                // On place la carte si les conditions de ciblages sont respectées
                if (!targetCondition) {
                    Debug.Log("ERR : bad target [" + targetSlot.name + "] / ownByOppo = " + ownedByOppo.ToString());
                }
            }
        }

        if (isPutOnBoard)
            cardOnSlot = cardPlayed;

        return isPutOnBoard;
    }

    // On détruit l'enchantement de l'équipement
    public void destroyEnchantment() {
        Destroy(cardOnSlot);
        cardOnSlot = null;
    }
}
