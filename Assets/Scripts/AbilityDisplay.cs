using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AbilityDisplay : MonoBehaviour
{
    public Ability ability;
    public SpriteRenderer illustration;
    public TMP_Text textName;
    public TMP_Text textManaCost;
    public TMP_Text textCooldown;
    public TMP_Text textPriority;
    public TMP_Text textType;
    public TMP_Text textDescription;
    public GameObject GO_ManaCost;
    public GameObject GO_Disable;
    public GameObject GO_DisableManaCost;
    public GameObject GO_Tooltip;
    public GameObject GO_Tooltip2;
    public AbilityStatus abilityStatus = AbilityStatus.Board;

    public int cooldown = 0; // Temps de rechargement de la capacit�
    public int cooldownTemp;
    public int manaCostModif = 0; // Modificateur du cout en mana
    public int manaCostModifTemp;
    public bool activationLimited; // TRUE si les utilisations par tour sont limit�es
    public int remainingActivation; // Nombre d'activation restante pour le tour
    public int remainingActivationTemp;

    private bool init = true;

    // Start is called before the first frame update
    void Start() {
        if (abilityStatus == AbilityStatus.TeamLayout) return;

        illustration.sprite = ability.illustration;
        textName.text = ability.name;
        textPriority.text = ability.priority.ToString();
        textType.text = ability.abilityType.ToString();
        //textDescription.text = ability.description;

        // On affiche le sprite du cout en mana
        if (ability.manaCost >= 0) {
            GO_ManaCost.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (abilityStatus == AbilityStatus.TeamLayout) return;

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

        // Si le nombre d'utilisation est limit� et passe a 0 ou inf�rieur, on active le cooldown
        if (activationLimited && remainingActivation <= 0) {
            disableForXTurn(ability.cooldown);
        }
    }

    // Renvoi le cout r�el de la capacit�
    public int GetManaCost() {
        return ability.manaCost + manaCostModif;
    }

    ///// <summary>
    ///// Actualise l'affichage de la capacit�
    ///// </summary>
    //public void refreshDisplayAbility() {
    //    textManaCost.text = (ability.manaCost + manaCostModif).ToString();
    //    textCooldown.text = cooldown.ToString();

    //    if (cooldown > 0) {
    //        GO_Disable.SetActive(true);

    //        if (ability.manaCost >= 0) {
    //            GO_DisableManaCost.SetActive(true);
    //        }
    //    } else {
    //        GO_Disable.SetActive(false);
    //        GO_DisableManaCost.SetActive(false);
    //    }

    //    if (GameManager.fullDamageIntegred(ability.description, ability.elementalAffinity, GetComponent<OwnedByOppo>().monsterOwnThis) != null) {
    //        textDescription.text = GameManager.fullDamageIntegred(ability.description, ability.elementalAffinity, GetComponent<OwnedByOppo>().monsterOwnThis);
    //    }
    //}

    public void refreshDisplayAbility() {
        illustration.sprite = ability.illustration;
        textName.text = ability.name;
        textPriority.text = ability.priority.ToString();
        textType.text = ability.abilityType.ToString();
        textManaCost.text = (ability.manaCost + manaCostModif).ToString();
        textCooldown.text = cooldown.ToString();

        // On active l'affichage du cooldown si il est sup�rieur a 0
        if (cooldown > 0) {
            GO_Disable.SetActive(true);
            GO_DisableManaCost.SetActive(true);
        } else {
            GO_Disable.SetActive(false);
            GO_DisableManaCost.SetActive(false);
        }

        // Si c'est une capacit� passive, on cache le cout en mana
        if (ability.abilityType == AbilityType.Trigger || ability.abilityType == AbilityType.Global) {
            GO_ManaCost.SetActive(false);
            GO_DisableManaCost.SetActive(false);
        }

        // On modifie le texte de la capacit� pour afficher les d�g�ts r�els inflig�s
        if (GameManager.fullDamageIntegred(ability.description, ability.elementalAffinity, GetComponent<OwnedByOppo>().monsterOwnThis) != null) {
            textDescription.text = GameManager.fullDamageIntegred(ability.description, ability.elementalAffinity, GetComponent<OwnedByOppo>().monsterOwnThis);
        } else {
            textDescription.text = ability.description;
        }
    }

    /// <summary>
    /// Au d�but d'un nouveau tour
    /// </summary>
    public void newTurn() {
        cooldown--;
        remainingActivation = ability.activationPerTurn;
    }

    /// <summary>
    /// Desactive la capacit� pour X tour
    /// </summary>
    public void disableForXTurn(int XTurn) {
        cooldown = XTurn;
    }

    /// <summary>
    /// Incr�mente le nombre d'utilisation
    /// </summary>
    public void addRemainingActivation(int amountActivation) {
        remainingActivation += amountActivation;
    }

    /// <summary>
    /// Teste si la capacit� peut �tre activ�
    /// </summary>
    public bool canBeActivated() {
        if (cooldown <= 0 && (remainingActivation > 0 || !activationLimited))
            return true;

        return false;
    }

    /// <summary>
    /// On active les effets de la capacit�
    /// </summary>
    public void activeAbility(GameObject target) {
        // Si c'est une capacit� a activ�
        if (ability.abilityType == AbilityType.Active) {
            if (canBeActivated()) {
                ability.activeEffect(target, GetComponent<OwnedByOppo>().monsterOwnThis);
                if (activationLimited)
                    remainingActivation--;
            }
        }
    }

    /// <summary>
    /// On active les effets de la capacit� passive
    /// </summary>
    public void activePassiveAbility() {
        if (ability.abilityType == AbilityType.Trigger || ability.abilityType == AbilityType.Global) {
            ability.activeEffect(GetComponent<OwnedByOppo>().monsterOwnThis.gameObject, GetComponent<OwnedByOppo>().monsterOwnThis);
        }
    }

    /// <summary>
    /// On d�sactive les effets de la capacit� passive
    /// </summary>
    public void disablePassiveAbility() {
        if (ability.abilityType == AbilityType.Trigger || ability.abilityType == AbilityType.Global) {
            //ability.disableEffect(monsterOwnThis);
            ability.disableEffect(GetComponent<OwnedByOppo>().monsterOwnThis);
        }
    }

    // Boucle de v�rification de ciblage
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

    // Renverse sur x les textes et l'illustration
    public void ReverseText() {
        illustration.transform.localScale = new Vector3(-1, illustration.transform.localScale.y, illustration.transform.localScale.z);

        textName.transform.localScale = new Vector3(-1, textName.transform.localScale.y, textName.transform.localScale.z);
        textManaCost.transform.localScale = new Vector3(-1, textManaCost.transform.localScale.y, textManaCost.transform.localScale.z);
        textPriority.transform.localScale = new Vector3(-1, textPriority.transform.localScale.y, textPriority.transform.localScale.z);
        textType.transform.localScale = new Vector3(-1, textType.transform.localScale.y, textType.transform.localScale.z);
        textDescription.transform.localScale = new Vector3(-1, textDescription.transform.localScale.y, textDescription.transform.localScale.z);
    }

    // Affiche / cache le tooltip
    public void ShowHideTooltip(bool show) {
        GetComponent<AbilityDisplay>().GO_Tooltip.SetActive(show);
        GetComponent<AbilityDisplay>().GO_Tooltip2.SetActive(show);
    }
}
