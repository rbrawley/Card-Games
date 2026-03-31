using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "CardSprites",
                 menuName = "ScriptableObjects/CardSpritesSo")]

public class CardSpritesSO : ScriptableObject
{
    [Header("Card Stock")]
    public Sprite cardBack;
    public Sprite CardBackGold;
    public Sprite cardFront;
    public Sprite cardFrontGold;
    [Header("Suits")]
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;

    [Header("Pip Sprites")]
    public Sprite[] faceSprites;
    public Sprite[] rankSprites;

    private static CardSpritesSO S;
    public static Dictionary<char, Sprite> SUITS {get; private set;}
    public void Init()
    {
        INIT_STATICS(this);
    }


/// <summary>
/// Initializes static elements of CardSpriteSo
/// </summary>
/// <param name="cSSO">CardSpriteSo to be assigned to the Singleton S</param>
    static void INIT_STATICS(CardSpritesSO cSSO)
    {
        if (S != null)
        {
            Debug.LogError("CardSpritesSO.S can't be set up a second time!");
            return;
        }
        S = cSSO;  //initiatlizes the Singleton

        SUITS = new Dictionary<char, Sprite>()
        {
            {'C', S.suitClub},
            {'D', S.suitDiamond},
            {'H', S.suitHeart},
            {'S', S.suitSpade}
        };
    }

    public static Sprite[] RANKS
    {
        get{return S.rankSprites;}
    }

/// <summary>
/// Searches S.faceSprites for the  one with the right name
/// </summary>
/// <param name="name">The name to search for</param>
/// <returns>A face Sprite</returns>
    public static Sprite GET_FACE(string name)
    {
        foreach (Sprite spr in S.faceSprites)
        {
            if (spr.name == name) return spr;
        }
        return null;
    }

/// <summary>
/// This makes S.cardBack accessible to other classes
/// </summary>
    public static Sprite BACK
    {
        get {return S.cardBack;}
    }

/// <summary>
/// Call to Reset S to Null at end of game
/// </summary>
    public static void RESET()
    {
        S = null;
    }

}