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
    public GameObject GO_Tooltip;
    //public MonsterDisplay monsterOwnThis; // Le MonsterDisplay qui possède cette capacité

    public int cooldown = 0; // Temps de rechargement de la capacité
    public int cooldownTemp;
    public int manaCostModif = 0; // Modificateur du cout en mana
    public int manaCostModifTemp;
    public bool activationLimited; // TRUE si les utilisations par tour sont limitées
    public int remainingActivation; // Nombre d'activation restante pour le tour
    public int remainingActivationTemp;

    private bool init = true;

    // Start is called before the first frame update
    void Start() {
        illustration.sprite = ability.illustration;
        textTooltip.text = ability.description;

        // On affiche le sprite du cout en mana
        if (ability.manaCost >= 0) {
            GO_ManaCost.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Initialisation
        if (init) {
            init = false;
            remainingActivation = ability.activationPerTurn;
            if (ability.activationPerTurn > 0 && ability.cooldown > 0) {
                activationLimited = true;
            } else {
                activationLimited = false;
            }

            if (ability.abilityType == AbilityType.Trigger || ability.abilityType == AbilityType.Global) {
                GO_ManaCost.SetActive(false);
            }
        }

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

    // Renvoi le cout réel de la capacité
    public int GetManaCost() {
        return ability.manaCost + manaCostModif;
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
        } else {
            GO_Disable.SetActive(false);
            GO_DisableManaCost.SetActive(false);
        }

        //if (GameManager.fullDamageIntegred(ability.description, ability.elementalAffinity, monsterOwnThis) != null) {
        //    textTooltip.text = GameManager.fullDamageIntegred(ability.description, ability.elementalAffinity, monsterOwnThis);
        //}

        if (GameManager.fullDamageIntegred(ability.description, ability.elementalAffinity, GetComponent<OwnedByOppo>().monsterOwnThis) != null) {
            textTooltip.text = GameManager.fullDamageIntegred(ability.description, ability.elementalAffinity, GetComponent<OwnedByOppo>().monsterOwnThis);
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
        // Si c'est une capacité a activé
        if (ability.abilityType == AbilityType.Active) {
            if (canBeActivated()) {
                ability.activeEffect(target, GetComponent<OwnedByOppo>().monsterOwnThis);
                if (activationLimited)
                    remainingActivation--;
            }
        }
    }

    /// <summary>
    /// On active les effets de la capacité passive
    /// </summary>
    public void activePassiveAbility() {
        if (ability.abilityType == AbilityType.Trigger || ability.abilityType == AbilityType.Global) {
            //ability.activeEffect(monsterOwnThis.gameObject, monsterOwnThis);
            ability.activeEffect(GetComponent<OwnedByOppo>().monsterOwnThis.gameObject, GetComponent<OwnedByOppo>().monsterOwnThis);
        }
    }

    /// <summary>
    /// On désactive les effets de la capacité passive
    /// </summary>
    public void disablePassiveAbility() {
        if (ability.abilityType == AbilityType.Trigger || ability.abilityType == AbilityType.Global) {
            //ability.disableEffect(monsterOwnThis);
            ability.disableEffect(GetComponent<OwnedByOppo>().monsterOwnThis);
        }
    }

    // Boucle de vérification de ciblage
    public bool loopTargetAllowed(GameObject target) {
        foreach (TargetType targetType in ability.targetType) {
            TargetType targetType2 = targetType;
            if (targetType2 == TargetType.SlotHidden)
                targetType2 = TargetType.SlotVisible;

            if (targetType2 == GameManager.typeTarget(target))
                return true;
        }
        return false;
    }
}
