using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuffDebuffDisplay : MonoBehaviour
{
    public GameObject buffPower;
    public GameObject buffPowerSup;
    public GameObject buffGuard;
    public GameObject buffGuardSup;
    public GameObject buffSpeed;
    public GameObject buffSpeedSup;
    public GameObject buffMana;
    public GameObject debuffPower;
    public GameObject debuffPowerSup;
    public GameObject debuffGuard;
    public GameObject debuffGuardSup;
    public GameObject debuffSpeed;
    public GameObject debuffSpeedSup;

    public GameObject buffBackground;
    public GameObject debuffBackground;
    public TMP_Text buffText;
    public TMP_Text debuffText;

    public TMP_Text activeText;

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
        foreach(Transform child in buffPower.transform.parent) {
            child.gameObject.SetActive(false);
        }

        switch (GetComponent<BuffDebuff>().buffDebuffType) {
            case BuffDebuffType.Power:
                if (GetComponent<BuffDebuff>().amount < -100) {
                    debuffPowerSup.SetActive(true);
                } else if (GetComponent<BuffDebuff>().amount < 0 && GetComponent<BuffDebuff>().amount >= -100) {
                    debuffPower.SetActive(true);
                } else if (GetComponent<BuffDebuff>().amount >= 0 && GetComponent<BuffDebuff>().amount <= 100) {
                    buffPower.SetActive(true);
                } else if (GetComponent<BuffDebuff>().amount > 100) {
                    buffPowerSup.SetActive(true);
                }
                break;
            case BuffDebuffType.Guard:
                if (GetComponent<BuffDebuff>().amount < -100) {
                    debuffGuardSup.SetActive(true);
                } else if (GetComponent<BuffDebuff>().amount < 0 && GetComponent<BuffDebuff>().amount >= -100) {
                    debuffGuard.SetActive(true);
                } else if (GetComponent<BuffDebuff>().amount >= 0 && GetComponent<BuffDebuff>().amount <= 100) {
                    buffGuard.SetActive(true);
                } else if (GetComponent<BuffDebuff>().amount > 100) {
                    buffGuardSup.SetActive(true);
                }
                break;
            case BuffDebuffType.Speed:
                if (GetComponent<BuffDebuff>().amount < -100) {
                    debuffSpeedSup.SetActive(true);
                } else if (GetComponent<BuffDebuff>().amount < 0 && GetComponent<BuffDebuff>().amount >= -100) {
                    debuffSpeed.SetActive(true);
                } else if (GetComponent<BuffDebuff>().amount >= 0 && GetComponent<BuffDebuff>().amount <= 100) {
                    buffSpeed.SetActive(true);
                } else if (GetComponent<BuffDebuff>().amount > 100) {
                    buffSpeedSup.SetActive(true);
                }
                break;
            case BuffDebuffType.Mana:
                buffPowerSup.SetActive(true);
                break;
            case BuffDebuffType.DamageRaw:
                Debug.Log("refresh damageRaw");
                break;
            case BuffDebuffType.DamagePercent:
                Debug.Log("refresh damagePercent");
                break;
            default:
                Debug.Log("Buff / debuff type not found");
                break;
        }

        buffBackground.SetActive(!(GetComponent<BuffDebuff>().amount < 0));
        debuffBackground.SetActive(GetComponent<BuffDebuff>().amount < 0);

        if (GetComponent<BuffDebuff>().amount < 0) {
            activeText = debuffText;
            buffText.gameObject.SetActive(false);
            debuffText.gameObject.SetActive(true);
        } else {
            activeText = buffText;
            buffText.gameObject.SetActive(true);
            debuffText.gameObject.SetActive(false);
        }
        activeText.text = GetComponent<BuffDebuff>().turn.ToString();
    }
}
