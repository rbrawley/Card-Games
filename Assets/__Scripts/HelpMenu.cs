using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenu : MonoBehaviour
{
    public GameObject helpContainer;
        public bool       paused = false;
    
    void Update()
    {
        //When H is pressed, if unpaused, pause the game and open the help menu container
        if (Input.GetKeyDown(KeyCode.H) && Time.timeScale != 0)
        {
            helpContainer.SetActive(true);
            Time.timeScale = 0;
            //paused = true;
        }
        //When h is pressed, if paused, unpause the game and close the help menu container
        else if (Input.GetKeyDown(KeyCode.H) && Time.timeScale == 0)
        {
            helpContainer.SetActive(false);
            Time.timeScale = 1;
            //paused = false;
            
        }
    }
}
