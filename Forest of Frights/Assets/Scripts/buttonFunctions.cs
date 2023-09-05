using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    // This class contains all button functions for now it will contain the Pause Menu Functions

    // Quit
    // Restart
    // Resume


    //Quit - Quits the game
    public void Quit()
    {
        Application.Quit();
    }

    //Restart - reloads the game
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //loads active scene
        gameManager.instance.unPause(); // unPauses the game
    }

    //Resume - resume's the game
    public void resume()
    {
        gameManager.instance.unPause();
    }
}
