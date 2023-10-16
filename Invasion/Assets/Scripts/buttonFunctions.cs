using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
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

    public void mainMenu()
    {
        SceneManager.LoadScene(0); //loads active scene
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

    public void respawn()
    {
        checkPoint midBossCP = gameManager.instance.midBossCP;

        //if mid boss is dead
        if (gameManager.instance.midBoss)
        {

            gameManager.instance.restartEndBoss();
  
        }

        else if (midBossCP != null && midBossCP.isTriggered) 
        {         
            gameManager.instance.restartMidBoss();

        }

        respawnPlayer();

    }

    public void inventoryScreen()
    {
        gameManager.instance.pause();


    }

    public void controlsMenuActive()
    {

            gameManager.instance.setControlsMenuActive();


    }
    public void controlsMenuDisabled()
    {

        gameManager.instance.setControlsMenuActive(false);


    }

}

