using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldCoin, healthGlobe, staminaGlobe;

    public void DropItems()
    {
        // Generira nasumični broj od 1 do 100
        int randomNum = Random.Range(1, 101);

        // 50% šanse za novčiće
        if (randomNum <= 50)
        {
            // Zagarantiran barem 1 novčić
            int coinsToDrop = 1;
            int additionalCoins = Random.Range(1, 101); // Generaraj random broj za dodatne novčiće

            if (additionalCoins <= 25) // 25% za 2
            {
                coinsToDrop = 2;
            }
            else if (additionalCoins <= 30) // 5% za 3
            {
                coinsToDrop = 3;
            }

            for (int i = 0; i < coinsToDrop; i++)
            {
                Instantiate(goldCoin, transform.position, Quaternion.identity);
            }
        }
        // 15% za health
        else if (randomNum > 50 && randomNum <= 65)
        {
            Instantiate(healthGlobe, transform.position, Quaternion.identity);
        }
        // 15% za staminu
        else if (randomNum > 65 && randomNum <= 80)
        {
            Instantiate(staminaGlobe, transform.position, Quaternion.identity);
        }
        // 20% za ništa
    }
}