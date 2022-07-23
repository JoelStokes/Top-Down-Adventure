using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicMeter : MonoBehaviour
{
    public Image MagicBar;
    public GameObject Fireball;
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

    public void UseSpell(float spellAmount, int dir, Vector3 playerPos)
    {
        if (spellAmount < magicAmount)
        {
            magicAmount -= spellAmount;

            Quaternion rotation;
            switch (dir)
            {
                case 1:
                    rotation = Quaternion.Euler(0,0,270);
                    break;
                case 2:
                    rotation = Quaternion.Euler(0,0,180);
                    break;
                case 3:
                    rotation = Quaternion.Euler(0,0,90);
                    break;
                default:
                    rotation = Quaternion.Euler(0,0,0);
                    break;
            }

            Instantiate(Fireball, playerPos, rotation);
        }
    }
}
