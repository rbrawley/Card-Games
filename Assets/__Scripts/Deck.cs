using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JsonParseDeck))]
public class Deck : MonoBehaviour
{
    [Header("Inscribed")]
    public CardSpritesSO             cardSprites;
    public GameObject                prefabCard;
    public GameObject               prefabSprite;
    public bool                     startFaceUp = true;

    [Header("Dynamic")]
    public Transform                deckAnchor;
    public List<Card>               cards;

    private JsonParseDeck jsonDeck;

    static public GameObject SPRITE_PREFAB{ get; private set;}      
    void Start()
    {
        InitDeck();
        Shuffle(ref cards);
    }

/// <summary>
/// Card Games will call InitDeck to set deck and build all 52 objects from 
/// jsondeck and cardsprites information
/// </summary>
    public void InitDeck()
    {
        //create static reference to sprite prefab for card class to use
        SPRITE_PREFAB = prefabSprite;
        //call init method on cardspriteSo instance assigned to cardsprites
        cardSprites.Init();

        //get a reference to the JsonparseDeck component
        jsonDeck = GetComponent <JsonParseDeck>();

        //create an anchor for all Card gameobjects
        if (GameObject.Find("_Deck") == null)
        {
            GameObject anchorGO = new GameObject("_Deck");
            deckAnchor = anchorGO.transform;
        }

        MakeCards();
    }

/// <summary>
/// creates a gameObject for each card
/// </summary>
    void MakeCards()
    {
        cards = new List<Card>();
        Card c;

        //generate 13 cards for each suit
        string suits = "CDHS";
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <=13; j++)
            {
                c = MakeCard(suits[i], j);
                cards.Add(c);

                //aligns the cards in ros for testing
                c.transform.position = 
                new Vector3 ((j - 7) * 3, (i - 1.5f) * 4, 0);
            }
        }
    }



    Card MakeCard(char suit, int rank)
    {
        GameObject go = Instantiate<GameObject>( prefabCard, deckAnchor);
        Card card = go.GetComponent<Card>();

        card.Init(suit, rank, startFaceUp);
        return card;
    }

    static public void Shuffle(ref List<Card> refcards)
    {
        //create temp list to hold shuffle order
        List<Card> tCards = new List<Card>();

        int ndx; //holds index of card to be moved
        //repeat as long as there are cards in original list
        while (refcards.Count > 0)
        {
            //pick random card index
            ndx = Random.Range(0, refcards.Count);
            // add to temp list
            tCards.Add(refcards[ndx]);
            //remove from original list
            refcards.RemoveAt(ndx);
        }
        //replace original list with temp list
        refcards = tCards;
    }
}
