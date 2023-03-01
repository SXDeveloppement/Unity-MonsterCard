using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterLayoutTeamDisplay : MonoBehaviour
{
    public GameObject monsterLinked;

    public Image artworkImage;
    public TMP_Text powerText;
    public TMP_Text guardText;
    public TMP_Text speedText;
    public TMP_Text healthText;
    public TMP_Text manaText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Actualisation de toutes les infos du monstre dans le fenêtre de l'équipe
    public void refreshMonsterUI() {
        MonsterDisplay monsterDisplay = monsterLinked.GetComponent<MonsterDisplay>();
        artworkImage.sprite = monsterDisplay.artworkImage.sprite;
        powerText.text = monsterDisplay.powerText.text;
        guardText.text = monsterDisplay.guardText.text;
        speedText.text = monsterDisplay.speedText.text;
        healthText.text = monsterDisplay.healthText.text;
        manaText.text = monsterDisplay.manaText.text;

        // On modifie la taille de la barre
        healthText.transform.parent.Find("Health").transform.localScale = new Vector3((float)monsterDisplay.healthAvailable / monsterDisplay.healthMax, 1f, 1f);

        // On modifie la taille de la barre
        manaText.transform.parent.Find("Mana").transform.localScale = new Vector3((float)monsterDisplay.manaAvailable / monsterDisplay.manaMax, 1f, 1f);
    }

}
