using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityLayoutTeamDisplay : MonoBehaviour {
    public Ability ability;
    public SpriteRenderer illustration;
    public TMP_Text textManaCost;
    public TMP_Text textCooldown;
    public TMP_Text textTooltip;
    public GameObject GO_ManaCost;
    public GameObject GO_Disable;
    public GameObject GO_DisableManaCost;

    public int cooldown; // Temps de rechargement de la capacité
    private int cooldownTemp;
    public int manaCostModif = 0; // Modificateur du cout en mana
    private int manaCostModifTemp;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (cooldown != cooldownTemp || manaCostModif != manaCostModifTemp) {
            cooldownTemp = cooldown;
            manaCostModifTemp = manaCostModif;

            refreshDisplayAbility();
        }
    }

    /// <summary>
    /// Actualise l'affichage de la capacité
    /// </summary>
    public void refreshDisplayAbility() {
        illustration.sprite = ability.illustration;
        textTooltip.text = ability.description;

        // On affiche le sprite du cout en mana
        if (ability.manaCost >= 0) {
            GO_ManaCost.SetActive(true);
        }
        textManaCost.text = (ability.manaCost + manaCostModif).ToString();
        textCooldown.text = cooldown.ToString();

        if (cooldown > 0) {
            GO_Disable.SetActive(true);

            if (ability.manaCost >= 0) {
                GO_DisableManaCost.SetActive(true);
            }
        }
    }
}
