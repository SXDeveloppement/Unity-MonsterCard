using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterLayoutTeamDisplay : MonoBehaviour
{
    public SpriteRenderer illustration;
    public TMP_Text powerText;
    public TMP_Text guardText;
    public TMP_Text speedText;
    public TMP_Text healthText;
    public TMP_Text manaText;
    public GameObject affinityLayout;
    public GameObject buttonSwap;
    public SpriteRenderer illustrationAbility;
    public AbilityDisplay abilityDisplay;
    //public AbilityLayoutTeamDisplay abilityDisplay;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Actualisation de toutes les infos du monstre dans le fenêtre de l'équipe
    public void refreshMonsterUI() {
        //transform.localPosition = Vector3.zero;
        //transform.localScale = new Vector3(0.35f, 0.35f, 1);

        MonsterDisplay monsterDisplay = GetComponent<OwnedByOppo>().monsterOwnThis;
        illustration.sprite = monsterDisplay.illustration.sprite;
        powerText.text = monsterDisplay.getPowerPointString();
        guardText.text = monsterDisplay.getGuardPointString();
        speedText.text = monsterDisplay.getSpeedPointString();

        healthText.text = monsterDisplay.getHealthBarString();
        manaText.text = monsterDisplay.getManaBarString();
        // On modifie la taille de la barre
        healthText.transform.parent.Find("HealthBar").transform.localPosition = monsterDisplay.getHealthBarLocalPosition();
        // On modifie la taille de la barre
        manaText.transform.parent.Find("ManaBar").transform.localPosition = monsterDisplay.getManaBarLocalPosition();

        // Affichage des affinités élémentaires du monstre
        foreach (ElementalAffinity affinity in monsterDisplay.monster.elementalAffinity) {
            affinityLayout.transform.Find(affinity.ToString()).gameObject.SetActive(true);
        }

        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        // On modifie le bouton de changement de monstre en fonction des états du monstre sur le terrain
        //// Déjà sur le terrain
        if (monsterDisplay.gameObject == GameManager.GO_MonsterInvoked) {
            refreshButtonSwap(false, "In battle");
        }
        //// KO
        else if (monsterDisplay.isKO) {
            refreshButtonSwap(false, "K.O.");
        } else {
            // Si le joueur a swap ou a déjà fait une action ce tour, on desactive les boutons de swap
            if (GameManager.playerTakenSwap || GameManager.playerTakenAction) {
                refreshButtonSwap(false, "Swap");
            } else {
                refreshButtonSwap(true, "Swap");
            }
        }

        // On actualise l'affichage de la capacité
        abilityDisplay.ability = monsterDisplay.abilityDisplay.ability;
        abilityDisplay.cooldown = monsterDisplay.abilityDisplay.cooldown;
        abilityDisplay.manaCostModif = monsterDisplay.abilityDisplay.manaCostModif;
        abilityDisplay.GetComponent<OwnedByOppo>().monsterOwnThis = GetComponent<OwnedByOppo>().monsterOwnThis;
        abilityDisplay.refreshDisplayAbility();
    }

    // On actualise le bouton de changement de monstre
    public void refreshButtonSwap(bool active, string text) {
        buttonSwap.GetComponentInChildren<TMP_Text>().text = text;
        buttonSwap.transform.Find("Disable").gameObject.SetActive(!active);
        buttonSwap.GetComponent<BoxCollider2D>().enabled = active;
    }

    // On invoque le monstre si on clique sur le bouton swap
    public void swapMonster() {
        GameObject.FindObjectOfType<GameManager>().SwapAction(GetComponent<OwnedByOppo>().monsterOwnThis.gameObject);
    }
}
