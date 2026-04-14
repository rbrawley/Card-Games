using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Start : MonoBehaviour{

    public void PlayButton()
    {
        SceneManager.LoadScene("__Prospector_Scene_0");
    }

    public void PlayButtonK()
    {
        SceneManager.LoadScene("__Klondike_Scene_");
    }
    
}
