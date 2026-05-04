using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Deck))]
[RequireComponent(typeof(JsonParseLayout))]
public class Klondike : MonoBehaviour
{
    private static Klondike S;

    [Header("Dynamic")]
    public List<CardKlondike>              drawPile;
    public List<CardKlondike>             discardPile;
    public List<CardKlondike>             table;
    public CardKlondike                   target;

    public GameObject[] playPos;
    public GameObject[] goalPos;

    public GameObject   drawPileButton;

    public List<CardKlondike>[] play;
    public List<CardKlondike>[] goal;
    public List<CardKlondike> triplesOnDisplay = new List<CardKlondike>();
    public List<List<CardKlondike>> deckTriples = new List<List<CardKlondike>>();

    public Transform kpDiscard;
    private Deck deck;
    private JsonLayout jsonLayout;


    public List<CardKlondike> Play0 = new List<CardKlondike>();
    public List<CardKlondike> Play1 = new List<CardKlondike>();

    public List<CardKlondike> Play2 = new List<CardKlondike>();

    public List<CardKlondike> Play3 = new List<CardKlondike>();

    public List<CardKlondike> Play4 = new List<CardKlondike>();

    public List<CardKlondike> Play5 = new List<CardKlondike>();

    public List<CardKlondike> Play6 = new List<CardKlondike>();

    public int triple;
    public int tripleRemainder;
    public int deckLocation;

    

    //a dictionary to pair table layout IDs and actual cards
    private Dictionary<int, CardKlondike> tableIdToCardDict;
    
    void Start()
    {
        if (S != null) Debug.LogError("Attempted to set S more than Once");
        S = this;

        jsonLayout = GetComponent<JsonParseLayout>().layout;

        deck = GetComponent<Deck>();

        deck.InitDeck();
        Deck.Shuffle(ref deck.cards);
        drawPile = ConvertCardsToCardKlondike(deck.cards);

        play = new List<CardKlondike>[] {Play0, Play1, Play2, Play3, Play4, Play5, Play6};
        StartCoroutine(LayoutTable());
        

        //set up initial target
        //MoveToTarget(Draw());

        //set up drawPile
        
    }

/// <summary>
/// Converts each Card in List(Card into a list (CardKlondike) to be used in game)
/// </summary>
/// <param name="listCard">A List(Card to be converted</param>
/// <returns>A list(CardKlondike) of the converted cards</returns>
    List<CardKlondike> ConvertCardsToCardKlondike(List<Card> listCard)
    {
        List<CardKlondike> listkp = new List<CardKlondike>();
        CardKlondike kp;
        foreach (Card card in listCard)
        {
            kp = card as CardKlondike;
            listkp.Add(kp);
        }
        return(listkp);
    }

/// <summary>
/// pulls a single card from the drawpile and returns it
/// </summary>
/// <returns>The top card of drawPile</returns>
    CardKlondike Draw()
    {

        CardKlondike kp = drawPile[0];
        drawPile.RemoveAt(0);
        return(kp);
    }

/// <summary>
/// Positions the initial tableau of cards
/// </summary>
    IEnumerator LayoutTable()
    {
        
        //creates empty gameobject to serve as an anchor
        if(kpDiscard == null)
        {
            GameObject tGO = new GameObject("_kpDiscard");
            kpDiscard = tGO.transform;
        }

        CardKlondike kp;
        float yOffset = 0.0f;

        tableIdToCardDict = new Dictionary<int, CardKlondike>();

        //iteration through the JsonLayoutSlots pulled from JsonLayout
        foreach(JsonLayoutSlot slot in jsonLayout.slots)
        {            
            yield return new WaitForSeconds(0.01f);
            kp = S.Draw();  //pull a card from top of draw pile
            kp.faceUp = slot.faceUp;
            
            
            switch (slot.id)
            {
                case 0:
                    yOffset = 0.3f*Play0.Count;
                    Play0.Add(kp);
                    kp.transform.SetParent(playPos[0].transform);
                    kp.row = 0;
                    break;
                case 1:
                case 7:
                    yOffset = 0.3f*Play1.Count;
                    Play1.Add(kp);
                    kp.transform.SetParent(playPos[1].transform);
                    kp.row = 1;
                    break;
                case 2:
                case 8:
                case 13:
                    yOffset = 0.3f*Play2.Count;
                    Play2.Add(kp);
                    kp.transform.SetParent(playPos[2].transform);
                    kp.row = 2;
                    break;
                case 3:
                case 9:
                case 14:
                case 18:
                    yOffset = 0.3f*Play3.Count;
                    Play3.Add(kp);
                    kp.transform.SetParent(playPos[3].transform);
                    kp.row = 3;
                    break;
                case 4:
                case 10:
                case 15:
                case 19:
                case 22:
                    yOffset = 0.3f*Play4.Count;
                    Play4.Add(kp);
                    kp.transform.SetParent(playPos[4].transform);
                    kp.row = 4;
                    break;
                case 5:
                case 11:
                case 16:
                case 20:
                case 23:
                case 25:
                    yOffset = 0.3f*Play5.Count;
                    Play5.Add(kp);
                    kp.transform.SetParent(playPos[5].transform);
                    kp.row = 5;
                    break;
                default:
                    yOffset = 0.3f*Play6.Count;
                    Play6.Add(kp);
                    kp.transform.SetParent(playPos[6].transform);
                    kp.row = 6;
                    break;
            }
            
            //convert the last char of the layer string to an int
            int z = int.Parse(slot.layer[slot.layer.Length - 1].ToString());

            //set the localPosition of the card based on slot information
            kp.SetLocalPos (new Vector3(
                transform.position.x,
                transform.position.y - yOffset,
                -z - 0.1f
                // jsonLayout.multiplier.x * slot.x,
                // jsonLayout.multiplier.y * slot.y,
                // -z
            ));

            

            kp.layoutID = slot.id;
            kp.layoutSlot = slot;
            kp.state = eCardStateKS.play;

            //set the sorting layer of all spriteRenderers on the card
            kp.SetSpriteSortingLayer(slot.layer);

            table.Add(kp);

            //add this CardKlondike to the tableIdtoCarddict
            tableIdToCardDict.Add(slot.id, kp);

            UpdateDrawPile();
            SortDeckIntoTriples();
        }
    }

    /// <summary>
    /// Moves current target card to discard
    /// </summary>
    /// <param name="kp">The CardKlondike to be moved</param>
    void MoveToDiscard(CardKlondike kp, float xOffset, float zOffset)
    {
        //sets state of card, adds it to discard pile, then updates parent
        kp.state = eCardStateKS.discard;
        discardPile.Add(kp);
        kp.transform.SetParent(kpDiscard);

        kp.SetLocalPos(new Vector3(
            (jsonLayout.multiplier.x * jsonLayout.discardPile.x)+xOffset,
            jsonLayout.multiplier.y * jsonLayout.discardPile.y,
            0.1f - zOffset
        ));

        kp.faceUp = true;

        kp.SetSpriteSortingLayer(jsonLayout.discardPile.layer);
        kp.SetSortingOrder(-200 + (discardPile.Count *3));
    }

// /// <summary>  TODO DELETE?
// /// Make kp the new target card
// /// </summary>
// /// <param name="kp">The card klondike to be moved</param>
//     void MoveToTarget(CardKlondike kp)
//     {
//         //if there is a target card, move it to discard
//         if (target != null) MoveToDiscard(target, 0, 0);

//         //move target card to correct location
//         MoveToDiscard(kp,0, 0);

//         target = kp;
//         kp.state = eCardStateKS.target;

//         kp.SetSpriteSortingLayer("Target");
//         kp.SetSortingOrder(0);
//     }

/// <summary>
/// Arranges cards of drawpile to show remaining
/// </summary>
    void UpdateDrawPile()
    {
        CardKlondike kp;

        for (int i = 0; i < drawPile.Count; i++)
        {
            kp = drawPile[i];
            kp.transform.SetParent(drawPileButton.transform);

            //position based on stagger level
            Vector3 kpPos = new Vector3();
            kpPos.x = transform.position.x * jsonLayout.drawPile.x;

            //kpPos.x += jsonLayout.drawPile.xStagger * i;
            kpPos.y = transform.position.y * jsonLayout.drawPile.y;
            kpPos.z = 0.1f * (i+1);
            kp.SetLocalPos(kpPos);

            kp.faceUp = false;
            kp.state = eCardStateKS.drawpile;

            kp.SetSpriteSortingLayer(jsonLayout.drawPile.layer);
            kp.SetSortingOrder(-10*i);
        }
    } 

/// <summary>
/// Turns card in the table faceup and facedown
/// </summary>
    public void SetTableFaceUps()
    {
        CardKlondike coverkp;
        foreach (CardKlondike kp in table)
        {
            bool faceUp = true;

            foreach (int coverID in kp.layoutSlot.hiddenBy)
            {
                coverkp = tableIdToCardDict[coverID];
                //if covering card is null or still in table, faceup = false
                if(coverkp == null || coverkp.state == eCardStateKS.play)
                {
                    faceUp = false;
                }
            }
            kp.faceUp = faceUp;
        }
    }

    // static public void CARD_CLICKED(CardKlondike kp)  TODO REMOVE?
    // {
    //     switch (kp.state)
    //     {
    //         // case eCardStateKS.target: //clicking target does nothing  TODO DELETE?
    //         //     break;

    //         case eCardStateKS.drawpile: //clicking drawpile draws the next card
    //             S.MoveToTarget(S.Draw());
    //             S.UpdateDrawPile();
    //             break;

    //             case eCardStateKS.table:
    //                 bool validMatch = true;

    //                 if (!kp.faceUp) validMatch = false; //facedown cards can't match

    //                 if(!kp.AdjacentTo(S.target)) validMatch = false; //must be an adjacent rank

    //                 if (validMatch)
    //             {
    //                 S.table.Remove(kp); //if match, remove from table
    //                 S.MoveToTarget(kp); //make target card
    //             }
    //                 S.SetTableFaceUps();
    //                 break;
    //     }
    // }

    public void SortDeckIntoTriples()
    {
        
        triple = drawPile.Count / 3;
        tripleRemainder = drawPile.Count % 3;
        deckTriples.Clear();

        int modifier = 0;
        for (int i = 0; i < triple; i++)
        {
            List<CardKlondike> curTriples = new List<CardKlondike>();
            for ( int j = 0; j < 3; j++)
            {
                curTriples.Add(drawPile[j+modifier]);
            }
            deckTriples.Add(curTriples);
            modifier +=3;
        }

        if (tripleRemainder != 0)
        {
            List<CardKlondike> curRemainders = new List<CardKlondike>();
            modifier = 0;
        for (int k = 0; k < tripleRemainder; k++)
            {
                curRemainders.Add(drawPile[drawPile.Count - tripleRemainder + modifier]);
                modifier++;
            }
        deckTriples.Add(curRemainders);
        triple++;
        }
        deckLocation = 0;
    }

    public void DealFromDeck()
    {
        // foreach(Transform child in _deck.transform)
        // {
        //     drawPile.Remove(child.name);
        //     discardPile.Add(child.name);
        // }
        
        if (deckLocation < triple)
        {
            CardKlondike kp;
            triplesOnDisplay.Clear();
            float xOffset = 2.5f;
            float zOffset = -0.2f;

            if(deckLocation != 0){
                foreach(CardKlondike card in discardPile)  //moves current triples into one discard stack
                {
                    //kp.transform.position(0, 0, zOffset); TODO fix and add
                }
            }
            

            foreach(CardKlondike card in deckTriples[deckLocation])
            {
                kp = Draw();
                MoveToDiscard(kp, xOffset, zOffset);
                kp.state = eCardStateKS.discard;
                xOffset += 0.5f;
                zOffset += 0.2f;
                triplesOnDisplay.Add(kp);
            }
            deckLocation++;
        }
        else
        {
            //restack deck
            RestackDeck();
        }
    }

    void RestackDeck()
    {
        foreach (CardKlondike card in discardPile)
        {
            drawPile.Add(card);
            card.state = eCardStateKS.drawpile;
            UpdateDrawPile();
        }
        discardPile.Clear();
        SortDeckIntoTriples();
    }


}
