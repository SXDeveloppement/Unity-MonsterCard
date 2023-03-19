using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityLayoutTeamDisplay : MonoBehaviour
{
    public Ability ability;
    public SpriteRenderer illustration;
    public TMP_Text textManaCost;
    public TMP_Text textCooldown;
    public TMP_Text textTooltip;
    public GameObject GO_ManaCost;
    public GameObject GO_Disable;
    public GameObject GO_DisableManaCost;
    public GameObject GO_Tooltip;
    public MonsterDisplay monsterOwnThis;

    public int cooldown; // Temps de rechargement de la capacité
    private int cooldownTemp;
    public int manaCostModif = 0; // Modificateur du cout en mana
    private int manaCostModifTemp;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// Actualise l'affichage de la capacité
    /// </summary>
    public void refreshDisplayAbility() {
        illustration.sprite = ability.illustration;
        textManaCost.text = (ability.manaCost + manaCostModif).ToString();
        textCooldown.text = cooldown.ToString();

        // On active l'affichage du cooldown si il est supèrieur a 0
        if (cooldown > 0) {
            GO_Disable.SetActive(true);
            GO_DisableManaCost.SetActive(true);
        } else {
            GO_Disable.SetActive(false);
            GO_DisableManaCost.SetActive(false);
        }

        // Si c'est une capacité passive, on cache le cout en mana
        if (ability.abilityType == AbilityType.Trigger || ability.abilityType == AbilityType.Global) {
            GO_ManaCost.SetActive(false);
            GO_DisableManaCost.SetActive(false);
        }

        // On modifie le texte de la capacité pour afficher les dégâts réels infligés
        if (GameManager.fullDamageIntegred(ability.description, ability.elementalAffinity, monsterOwnThis) != null) {
            textTooltip.text = GameManager.fullDamageIntegred(ability.description, ability.elementalAffinity, monsterOwnThis);
        } else {
            textTooltip.text = ability.description;
        }
    }
}
