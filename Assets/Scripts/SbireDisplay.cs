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

    public bool sbireHasAttacked = false;
    public bool sbireHasAttackedTemp = false;

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
        if (card.type == Type.Sbire && card.sbireHealthPoint > 0) {
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
        if (sbireHasAttacked != sbireHasAttackedTemp) {
            sbireHasAttackedTemp = sbireHasAttacked;
            attackAura.SetActive(!sbireHasAttacked);
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
        && GetComponent<CardDisplay>().status == Status.SlotVisible
        && GetComponent<CardDisplay>().card.type == Type.Sbire
        ) {
            gameManager.inGrave(gameObject);
        }
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

        sbireHasAttacked = !haveHaste;
    }

    // Lors d'un nouveau tour
    public void newTurn() {
        sbireHasAttacked = false;
    }

    // Prendre des dégâts ou soin (si amountDamage < 0)
    public void takeDamage(int amountDamage, bool attackerHasTrample = false) {
        sbireHealthAvailable -= amountDamage;

        // Si l'attaquant a le piétinement, on inflige le surplus de dégâts au monstre adverse
        if (attackerHasTrample && sbireHealthAvailable < 0) {
            GetComponent<CardDisplay>().monsterOwnThis.GetComponent<MonsterDisplay>().takeDamage(Mathf.Abs(sbireHealthAvailable));
        }

        if (sbireHealthAvailable < 0) {
            sbireHealthAvailable = 0;
        }else if (sbireHealthAvailable > sbireHealthMax) {
            sbireHealthAvailable = sbireHealthMax;
        }
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

        sbireHasAttacked = true;
    }

    // On actualise la vie du sbire
    public void refreshHealthPoint() {
        sbireHealthPoint.text = sbireHealthAvailable.ToString();
    }

    // On actualise la puissance du sbire
    public void refreshPowerPoint() {
        sbirePowerPoint.text = sbirePowerAvailable.ToString();
    }
}
