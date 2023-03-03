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
        powerText.text = monsterDisplay.getPowerPointString();
        guardText.text = monsterDisplay.getGuardPointString();
        speedText.text = monsterDisplay.getSpeedPointString();
        healthText.text = monsterDisplay.getHealthBarString();
        manaText.text = monsterDisplay.getManaBarString();

        // On modifie la taille de la barre
        healthText.transform.parent.Find("Health").transform.localScale = monsterDisplay.getHealthBarScale();

        // On modifie la taille de la barre
        manaText.transform.parent.Find("Mana").transform.localScale = monsterDisplay.getManaBarScale();
    }

}
