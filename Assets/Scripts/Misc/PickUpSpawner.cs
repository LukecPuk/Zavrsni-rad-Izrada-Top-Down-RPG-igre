using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldCoin, healthGlobe, staminaGlobe;

    public void DropItems()
    {
        // Generate a random number between 1 and 100
        int randomNum = Random.Range(1, 101);

        // 50% chance for coins
        if (randomNum <= 50)
        {
            // Guaranteed at least 1 coin
            int coinsToDrop = 1;

            // Determine additional coins based on probabilities
            int additionalCoins = Random.Range(1, 101); // Generate a number to determine additional coins
            if (additionalCoins <= 25) // 25% chance for 2 coins
            {
                coinsToDrop = 2;
            }
            else if (additionalCoins <= 30) // 5% chance for 3 coins (cumulative probability of 30%)
            {
                coinsToDrop = 3;
            }

            // Instantiate the coins
            for (int i = 0; i < coinsToDrop; i++)
            {
                Instantiate(goldCoin, transform.position, Quaternion.identity);
            }
        }
        // 15% chance for health globe
        else if (randomNum > 50 && randomNum <= 65)
        {
            Instantiate(healthGlobe, transform.position, Quaternion.identity);
        }
        // 15% chance for stamina globe
        else if (randomNum > 65 && randomNum <= 80)
        {
            Instantiate(staminaGlobe, transform.position, Quaternion.identity);
        }
        // Remaining 20% chance results in no item drop
    }
}