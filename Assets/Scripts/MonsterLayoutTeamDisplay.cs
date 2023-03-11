using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterLayoutTeamDisplay : MonoBehaviour
{
    public GameObject monsterLinked;

    public SpriteRenderer illustration;
    public TMP_Text powerText;
    public TMP_Text guardText;
    public TMP_Text speedText;
    public TMP_Text healthText;
    public TMP_Text manaText;
    public GameObject affinityLayout;
    public GameObject buttonSwap;
    public SpriteRenderer illustrationAbility;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Actualisation de toutes les infos du monstre dans le fenêtre de l'équipe
    public void refreshMonsterUI() {
        transform.localPosition = Vector3.zero;
        transform.localScale = new Vector3(0.35f, 0.35f, 1);

        MonsterDisplay monsterDisplay = monsterLinked.GetComponent<MonsterDisplay>();
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
        if (monsterDisplay.gameObject == gameManager.GO_MonsterInvoked) {
            refreshButtonSwap(false, "In battle");
        }
        //// KO
        else if (monsterDisplay.isKO) {
            refreshButtonSwap(false, "K.O.");
        } else {
            refreshButtonSwap(true, "Swap");
        }
    }

    // On actualise le bouton de changement de monstre
    public void refreshButtonSwap(bool active, string text) {
        buttonSwap.GetComponentInChildren<TMP_Text>().text = text;
        buttonSwap.transform.Find("Disable").gameObject.SetActive(!active);
        buttonSwap.GetComponent<BoxCollider2D>().enabled = active;
    }

    // On invoque le monstre si on clique sur le bouton swap
    public void swapMonster() {
        int index = gameObject.transform.GetSiblingIndex();
        GameObject.FindObjectOfType<GameManager>().swapMonster(index);
    }
}
