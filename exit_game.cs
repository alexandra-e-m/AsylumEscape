using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exit_game : MonoBehaviour
{
    // Start is called before the first frame update
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();

    }
}
