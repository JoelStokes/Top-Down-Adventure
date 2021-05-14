using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicMeter : MonoBehaviour
{
    public Image MagicBar;

    private float rechargeRate = 20;     //How fast the magic meter will recharge per second
    private float magicLim = 100;   //How much the magic meter holds
    private float magicAmount;      //How much is currently in the meter (Spells used subtract this number)

    void Start()
    {
        //Get Recharge Rate & Magic Lim since those can change based on items collected!
    }

    void Update()
    {
        if (magicAmount < magicLim)
        {
            magicAmount += (rechargeRate * Time.deltaTime);
            MagicBar.fillAmount = magicAmount / magicLim;
        } else if (magicAmount > magicLim)
        {
            magicAmount = magicLim;
        }
    }

    public void UseSpell(float spellAmount)
    {
        if (spellAmount < magicAmount)
        {
            magicAmount -= spellAmount;
        }
    }
}
