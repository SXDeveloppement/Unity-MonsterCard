using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SbireDisplay : MonoBehaviour
{
    public GameObject sbireFeature;
    public TMP_Text sbireHealthPoint;
    public TMP_Text sbirePowerPoint;
    public GameObject attackAura;
    public GameObject actionAttackIcon;

    public bool sbireIsExhausted = true;
    public bool sbireIsExhaustedTemp = false;

    public int sbireHealthMax;
    public int sbireHealthAvailable;
    public int sbireHealthAvailableTemp;
    public int sbirePowerAvailable;
    public int sbirePowerAvailableTemp;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();

        Card card = GetComponent<CardDisplay>().card;
        if (card.type == CardType.Sbire && card.sbireHealthPoint > 0) {
            sbireHealthMax = card.sbireHealthPoint;
            sbireFeature.SetActive(true);
            sbireHealthAvailable = card.sbireHealthPoint;
            sbirePowerAvailable = card.sbirePowerPoint;
        } else {
            sbireFeature.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // On affiche l'aura d'attaque si il n'a pas attaqué ce tour
        if (sbireIsExhausted != sbireIsExhaustedTemp) {
            sbireIsExhaustedTemp = sbireIsExhausted;
            attackAura.SetActive(!sbireIsExhausted);
        }

        // On actualise la vie du sbire quand elle est modifié
        if (sbireHealthAvailable != sbireHealthAvailableTemp) {
            sbireHealthAvailableTemp = sbireHealthAvailable;
            refreshHealthPoint();
        }

        // On actualise la puissance du sbire quand elle est modifié
        if (sbirePowerAvailable != sbirePowerAvailableTemp) {
            sbirePowerAvailableTemp = sbirePowerAvailable;
            refreshPowerPoint();
        }

        // On met le sbire au cimetière si il n'a plus de vie
        if (sbireHealthAvailable <= 0 
        && GetComponent<CardDisplay>().status == CardStatus.SlotVisible
        && GetComponent<CardDisplay>().card.type == CardType.Sbire
        ) {
            gameManager.inGrave(gameObject);
        }
    }

    // Le sbire meurt
    public void sbireDies() {
        // On enlève le GO de la carte dans SlotDisplay de l'emplacement
        gameObject.transform.parent.parent.GetComponent<SlotDisplay>().cardOnSlot = null;

        // On le met au cimetière
        gameManager.inGrave(gameObject);
    }

    // Lors de l'invocation du sbire
    public void invokeSbire() {
        // On regarde si le sbire a la célérité
        bool haveHaste = false;
        foreach (SbirePassifEffect sbirePassifEffect in GetComponent<CardDisplay>().card.sbirePassifEffects) {
            if (sbirePassifEffect == SbirePassifEffect.Quickness) {
                haveHaste = true;
                break;
            }
        }

        sbireIsExhausted = !haveHaste;
        sbireIsExhaustedTemp = !sbireIsExhausted;
    }

    // Lors d'un nouveau tour
    public void newTurn() {
        sbireIsExhausted = false;
        sbireIsExhaustedTemp = !sbireIsExhausted;
    }

    // Prendre des dégâts ou soin (si amountDamage < 0)
    public void takeDamage(int amountDamage, bool attackerHasTrample = false) {
        sbireHealthAvailable -= amountDamage;

        // Si l'attaquant a le piétinement, on inflige le surplus de dégâts au monstre adverse
        if (attackerHasTrample && sbireHealthAvailable < 0) {
            //GetComponent<CardDisplay>().monsterOwnThis.takeDamage(Mathf.Abs(sbireHealthAvailable));
            GetComponent<OwnedByOppo>().monsterOwnThis.takeDamage(Mathf.Abs(sbireHealthAvailable));
        }

        if (sbireHealthAvailable < 0) {
            sbireHealthAvailable = 0;
        }else if (sbireHealthAvailable > sbireHealthMax) {
            sbireHealthAvailable = sbireHealthMax;
        }
    }

    // Renvoie TRUE si le sbire a le "Tank"
    public bool haveTank() {
        foreach (SbirePassifEffect sbirePassifEffect in GetComponent<CardDisplay>().card.sbirePassifEffects) {
            if (sbirePassifEffect == SbirePassifEffect.Tank) {
                return true;
            }
        }

        return false;
    }

    // Combat contre un autre sbire
    public IEnumerator fightVersus(SbireDisplay targetSbireDisplay) {
        // On regarde si le sbire attaquant a le piétinement
        bool haveTrample = false;
        foreach (SbirePassifEffect sbirePassifEffect in GetComponent<CardDisplay>().card.sbirePassifEffects) {
            if (sbirePassifEffect == SbirePassifEffect.Pierce) {
                haveTrample = true;
                break;
            }
        }

        // On vérifie l'initiative des deux sbires
        bool haveInitiative = false;
        foreach (SbirePassifEffect sbirePassifEffect in GetComponent<CardDisplay>().card.sbirePassifEffects) {
            if (sbirePassifEffect == SbirePassifEffect.Domination) {
                haveInitiative = true;
                break;
            }
        }
        bool targetHaveInitiative = false;
        foreach (SbirePassifEffect sbirePassifEffect in targetSbireDisplay.GetComponent<CardDisplay>().card.sbirePassifEffects) {
            if (sbirePassifEffect == SbirePassifEffect.Domination) {
                targetHaveInitiative = true;
                break;
            }
        }

        // Phase d'attaque d'initiative
        if (haveInitiative) {
            targetSbireDisplay.takeDamage(sbirePowerAvailable, haveTrample);
        }
        if (targetHaveInitiative) {
            takeDamage(targetSbireDisplay.sbirePowerAvailable, haveTrample);
        }

        yield return null;

        // Phase d'attaque normal
        if (!haveInitiative) {
            targetSbireDisplay.takeDamage(sbirePowerAvailable, haveTrample);
        }
        if (!targetHaveInitiative) {
            takeDamage(targetSbireDisplay.sbirePowerAvailable, haveTrample);
        }

        sbireIsExhausted = true;
    }

    // On actualise la vie du sbire
    public void refreshHealthPoint() {
        sbireHealthPoint.text = sbireHealthAvailable.ToString();
    }

    // On actualise la puissance du sbire
    public void refreshPowerPoint() {
        sbirePowerPoint.text = sbirePowerAvailable.ToString();
    }

    // Combat contre un monstre
    public IEnumerator fightMonster(MonsterDisplay targetMonster) {
        // On passe le monstre en épuisé
        sbireIsExhausted = true;

        // On inflige au monstre un nombre de dégât égal a la puissance du sbire
        targetMonster.takeDamage(sbirePowerAvailable);

        yield return null;
    }

    /// <summary>
    /// Affiche l'icon d'action d'attaque
    /// </summary>
    public void ShowActionAttckIcon() {
        actionAttackIcon.SetActive(true);
    }
}
