using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityDisplay : MonoBehaviour
{
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
    private bool activationLimited; // TRUE si les utilisations par tour sont limitées
    private int remainingActivation; // Nombre d'activation restante pour le tour
    private int remainingActivationTemp;

    // Start is called before the first frame update
    void Start()
    {
        illustration.sprite = ability.illustration;
        textTooltip.text = ability.description;
        cooldown = ability.cooldown;
        remainingActivation = ability.activationPerTurn;

        // Il n'y a pas de limite d'utilisation si le nombre d'utilisation par tour et le cooldown sont inférieur ou égal a 0
        if (ability.activationPerTurn > 0 && cooldown > 0) {
            activationLimited = true;
        } else {
            activationLimited = false;
        }

        // On affiche le sprite du cout en mana
        if (ability.manaCost >= 0) {
            GO_ManaCost.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown != cooldownTemp || manaCostModif != manaCostModifTemp) {
            cooldownTemp = cooldown;
            manaCostModifTemp = manaCostModif;

            refreshDisplayAbility();
        }

        // Si le nombre d'utilisation est limité et passe a 0 ou inférieur, on active le cooldown
        if (activationLimited && remainingActivation <= 0) {
            disableForXTurn(ability.cooldown);
        }
    }

    /// <summary>
    /// Actualise l'affichage de la capacité
    /// </summary>
    public void refreshDisplayAbility() {
        textManaCost.text = (ability.manaCost + manaCostModif).ToString();
        textCooldown.text = cooldown.ToString();

        if (cooldown > 0) {
            GO_Disable.SetActive(true);

            if (ability.manaCost >= 0) {
                GO_DisableManaCost.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Au début d'un nouveau tour
    /// </summary>
    public void newTurn() {
        cooldown--;
        remainingActivation = ability.activationPerTurn;
    }

    /// <summary>
    /// Desactive la capacité pour X tour
    /// </summary>
    public void disableForXTurn(int XTurn) {
        cooldown = XTurn;
    }

    /// <summary>
    /// Incrémente le nombre d'utilisation
    /// </summary>
    public void addRemainingActivation(int amountActivation) {
        remainingActivation += amountActivation;
    }

    /// <summary>
    /// Teste si la capacité peut être activé
    /// </summary>
    public bool canBeActivated() {
        if (cooldown <= 0 && (remainingActivation > 0 || !activationLimited))
            return true;

        return false;
    }

    /// <summary>
    /// On active les effets de la capacité
    /// </summary>
    public void activeAbility(GameObject target) {
        if (canBeActivated()) {
            ability.activeEffect(target);
            if (activationLimited)
                remainingActivation--;
        }
    }
}
