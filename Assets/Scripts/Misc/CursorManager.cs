using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = false;

        if(Application.isPlaying )
        {
            Cursor.lockState = CursorLockMode.None;
        } 
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;

        //if(!Application.isPlaying ) { return; }
        //Cursor.visible = false;
    }
}