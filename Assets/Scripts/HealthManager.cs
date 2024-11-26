using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{

    public float totalHealth;

    public float currenthealth;
    public float resistancePercentage;


    // Start is called before the first frame update
    void Start()
    {
        currenthealth = totalHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TakeDamage(float damage)
    {
        float adjustedDmg = damage * ((100 - resistancePercentage)/100);

        currenthealth -= adjustedDmg;

        Death();

        Debug.Log(gameObject.name + " was just hit taking: " + adjustedDmg + " damage");
    }

    public void Death()
    {
        if (currenthealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

}
