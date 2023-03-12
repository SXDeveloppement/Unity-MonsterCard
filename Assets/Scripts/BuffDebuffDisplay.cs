using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuffDebuffDisplay : MonoBehaviour
{
    public GameObject iconPower;
    public GameObject iconGuard;
    public GameObject iconSpeed;
    public GameObject buffBackground;
    public GameObject debuffBackground;
    public TMP_Text buffAmountText;
    public TMP_Text debuffAmountText;
    public TMP_Text timeText;

    // Start is called before the first frame update
    void Start()
    {
        refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Actualise l'affichage du buff / debuff
    public void refresh() {
        foreach(Transform child in iconPower.transform.parent) {
            child.gameObject.SetActive(false);
        }

        // Changement de l'icon en fonction du type de buff/debuff
        if (GetComponent<BuffDebuff>().buffDebuffType == BuffDebuffType.Power) {
            iconPower.SetActive(true);
        } else if (GetComponent<BuffDebuff>().buffDebuffType == BuffDebuffType.Guard) {
            iconGuard.SetActive(true);
        } else if (GetComponent<BuffDebuff>().buffDebuffType == BuffDebuffType.Speed) {
            iconSpeed.SetActive(true);
        } else if (GetComponent<BuffDebuff>().buffDebuffType == BuffDebuffType.Mana) {
            Debug.Log("refresh Mana buff/debuff");
        } else if (GetComponent<BuffDebuff>().buffDebuffType == BuffDebuffType.DamageRaw) {
            Debug.Log("refresh damageRaw buff/debuff");
        } else if (GetComponent<BuffDebuff>().buffDebuffType == BuffDebuffType.DamagePercent) {
            Debug.Log("refresh damagePercent buff/debuff");
        } else {
            Debug.Log("Buff / debuff type not found");
        }

        // Changement du background
        buffBackground.SetActive(!(GetComponent<BuffDebuff>().amount < 0));
        debuffBackground.SetActive(GetComponent<BuffDebuff>().amount < 0);

        // Changement du texte du montant du buff/debuff
        buffAmountText.text = Mathf.Abs(GetComponent<BuffDebuff>().amount).ToString();
        debuffAmountText.text = Mathf.Abs(GetComponent<BuffDebuff>().amount).ToString();
        buffAmountText.gameObject.SetActive(!(GetComponent<BuffDebuff>().amount < 0));
        debuffAmountText.gameObject.SetActive(GetComponent<BuffDebuff>().amount < 0);

        // Changement du texte des tours restant
        timeText.text = GetComponent<BuffDebuff>().turn.ToString();
    }
}
