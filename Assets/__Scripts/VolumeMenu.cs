using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VolumeMenu : MonoBehaviour
{

        [SerializeField] private AudioClip[] testClips;
        public GameObject container;
        public bool       paused = false;
    
    void Update()
    {
        //When V is pressed, if unpaused, pause the game and open the volume menu container
        if (Input.GetKeyDown(KeyCode.V) && Time.timeScale != 0)
        {
            container.SetActive(true);
            Time.timeScale = 0;
            //paused = true;
        }
        //When V is pressed, if paused, unpause the game and close the volume menu container
        else if (Input.GetKeyDown(KeyCode.V) && Time.timeScale == 0)
        {
            container.SetActive(false);
            Time.timeScale = 1;
            //paused = false;
            
        }
    }

    public void TestSFXButton()
    {
        SFXManager.S.PlayRandomSFX(testClips, transform, 1f);
    }

    public void QuitButton()
    {
        SceneManager.LoadScene("_Start");
    }

}
