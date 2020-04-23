using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthStaminaBar : MonoBehaviour
{
    private Slider healthSlider;
    private Slider staminaSlider;
    public Sprite stamina;
    public Sprite depletedStamina;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        healthSlider = transform.GetChild(0).GetComponent<Slider>();
        staminaSlider = transform.GetChild(1).GetComponent<Slider>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.maxValue = player.GetComponent<PlayerControl>().maxHealth;
        healthSlider.value = player.GetComponent<PlayerControl>().health;
        staminaSlider.maxValue = player.GetComponent<PlayerControl>().maxStamina;
        staminaSlider.value = player.GetComponent<PlayerControl>().stamina;

        if (player.GetComponent<PlayerControl>().stamina < 30f)
        {
            transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = depletedStamina;
        }
        else
        {
            transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = stamina;
        }
    }

    /* Not really needed
    public void AddStamina()
    {
        staminaSlider.value++;
        if (staminaSlider.value > staminaSlider.maxValue)
        {
            staminaSlider.value = staminaSlider.maxValue;
        }
    }
    public void DecreaseStamina()
    {
        staminaSlider.value--;
        if (staminaSlider.value < 0)
        {
            staminaSlider.value = 0;
        }
    }
    public void AddHealth()
    {
        healthSlider.value++;
        if (healthSlider.value > healthSlider.maxValue)
        {
            healthSlider.value = healthSlider.maxValue;
        }
    }
    public void DecreaseHealth()
    {
        healthSlider.value--;
        if (healthSlider.value < 0)
        {
            healthSlider.value = 0;
        }
    }
    */


}
