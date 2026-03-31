using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class stores information about each decorator or pip from JSON_Deck
[System.Serializable]
public class JsonPip
{
    public string           type = "pip"; //pip, letter, or suit
    public Vector3          loc;            //location of the sprite on the Card
    public bool             flip = false; //true to flip the sprite vertically
    public float            scale = 1;      // the scale of the Sprite
}

// Rank informaiton
[System.Serializable]
public class JsonCard
{
    public int              rank; //rank 1-13 of this card
    public string           face; //sprite to use for each face card
    public List<JsonPip> pips = new List<JsonPip>();  //the pips on this card
}

//Deck information
[System.Serializable]
public class JsonDeck
{
    public List<JsonPip> decorators = new List<JsonPip>();
    public List<JsonCard> cards = new List<JsonCard>();
}
public class JsonParseDeck : MonoBehaviour
{
    private static JsonParseDeck S {get; set;}
  [Header("Inscribed")]
  public TextAsset          jsonDeckFile;  

  [Header("Dynamic")]
  public JsonDeck           deck;

  void Awake()
    {
        if(S != null)
        {
            Debug.LogError("CJsonParseDeck.S can't be set up a second time!");
            return;
        }
        S = this;

        deck = JsonUtility.FromJson<JsonDeck>(jsonDeckFile.text);
    }
/// <summary>
/// Returns decorator layout info for all cards
/// </summary>
    static public List<JsonPip> DECORATORS
    {
        get{return S.deck.decorators;}
    }

/// <summary>
/// Returns json card mathcing rank passed in from 1-13
/// </summary>
/// <param name="rank">Must be an int in range 1-13</param>
/// <returns>JsonCard Information</returns>
    static public JsonCard GET_CARD_DEF(int rank)
    {
        if ((rank < 1) || (rank > S.deck.cards.Count))
        {
            Debug.LogWarning("Illegal rank argument: " + rank);
            return null;
        }
        return S.deck.cards[rank - 1];
    }
}
