using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text lvlText;
    public Slider hpSlider;

    public void setHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        lvlText.text = "Lvl" + unit.level;
        hpSlider.maxValue = unit.maxHealth;
        hpSlider.value = unit.currentHealth;
    }
    public void setHP(int hp)
    {
        hpSlider.value = hp;
    }
}
