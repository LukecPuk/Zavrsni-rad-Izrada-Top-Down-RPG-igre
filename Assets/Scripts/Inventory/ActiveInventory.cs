using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Make sure to include this namespace

public class ActiveInventory : MonoBehaviour
{
    private int activeSlotIndexNum = 0;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        playerControls.Inventory.Keyboard.performed += ctx => ToggleActiveSlot(ctx.control.name);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void ToggleActiveSlot(string keyName)
    {
        if (int.TryParse(keyName, out int numValue))
        {
            ToggleActiveHighlight(numValue - 1);
        }
        else
        {
            Debug.LogWarning("Invalid key pressed: " + keyName);
        }
    }

    private void ToggleActiveHighlight(int indexNum)
    {
        if (indexNum < 0 || indexNum >= this.transform.childCount)
        {
            Debug.LogWarning("Invalid slot index: " + indexNum);
            return;
        }

        activeSlotIndexNum = indexNum;

        foreach (Transform inventorySlot in this.transform)
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }

        this.transform.GetChild(indexNum).GetChild(0).gameObject.SetActive(true);
    }
}
