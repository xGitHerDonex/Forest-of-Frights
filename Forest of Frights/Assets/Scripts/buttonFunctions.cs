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
    // Respawn


    //Quit - Quits the game
    public void Quit()
    {
        Application.Quit();
    }

    //Restart - reloads the game
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //loads active scene
        gameManager.instance.unPause(); // unPausesthe game
    }

    //Resume - resume's the game
    public void resume()
    {
        gameManager.instance.unPause();
    }

    
    //Respawn - Respawns the player after they lose
    public void respawnPlayer()
    {
        gameManager.instance.unPause();
        gameManager.instance.playerScript.spawnPlayer();

    }

    //Runs startGame in Game manager
    public void start()
    {
        gameManager.instance.startGame();
    }
}
