using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //This line enables use of uGUI classes like Text


public class Score : MonoBehaviour
{
    [Header("Dynamic")]
    public int      finalScore = 0;

    private Text    uiText;
    void Start(){
       uiText = GetComponent<Text>(); 
       if (PlayerPrefs.HasKey("PlayerScore")){
           finalScore = PlayerPrefs.GetInt("PlayerScore");
        }

    }

    public int SCORE{
        get { return finalScore;}

        private set{
            // finalScore = value;
            PlayerPrefs.SetInt("PlayerScore", finalScore);

            if (uiText != null){
                uiText.text = "Your Score:\n " + finalScore.ToString("#,0");
            }
            
        }
    }

    void Update(){
        uiText.text = "Your Score:\n" + finalScore.ToString("#,0"); 
    }
}
