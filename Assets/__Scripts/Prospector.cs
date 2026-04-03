using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Deck))]
[RequireComponent(typeof(JsonParseLayout))]
public class Prospector : MonoBehaviour
{
    private static Prospector S;

    [Header("Dynamic")]
    public List<CardProspector>              drawPile;
    public List<CardProspector>             discardPile;
    public List<CardProspector>             mine;
    public CardProspector                   target;

    private Transform layoutAnchor;
    private Deck deck;
    private JsonLayout jsonLayout;

    //a dictionary to pair mine layout IDs and actual cards
    private Dictionary<int, CardProspector> mineIdToCardDict;
    void Start()
    {
        if (S != null) Debug.LogError("Attempted to set S more than Once");
        S = this;

        jsonLayout = GetComponent<JsonParseLayout>().layout;

        deck = GetComponent<Deck>();

        deck.InitDeck();
        Deck.Shuffle(ref deck.cards);
        drawPile = ConvertCardsToCardProspectors(deck.cards);

        LayoutMine();

        //set up initial target
        MoveToTarget(Draw());

        //set up drawPile
        UpdateDrawPile();
    }

/// <summary>
/// Converts each Card in List(Card into a list (CardProspector) to be used in game)
/// </summary>
/// <param name="listCard">A List(Card to be converted</param>
/// <returns>A list(CardProspector) of the converted cards</returns>
    List<CardProspector> ConvertCardsToCardProspectors(List<Card> listCard)
    {
        List<CardProspector> listCP = new List<CardProspector>();
        CardProspector cp;
        foreach (Card card in listCard)
        {
            cp = card as CardProspector;
            listCP.Add(cp);
        }
        return(listCP);
    }

/// <summary>
/// pulls a single card from the drawpile and returns it
/// </summary>
/// <returns>The top card of drawPile</returns>
    CardProspector Draw()
    {
        //Pull card at 0th location and remove it.  TODO currently no protection against over drawing
        CardProspector cp = drawPile[0];
        drawPile.RemoveAt(0);
        return(cp);
    }

/// <summary>
/// Positions the initial tableau of cards
/// </summary>
    void LayoutMine()
    {
        //creates empty gameobject to serve as an anchor
        if(layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
        }

        CardProspector cp;

        mineIdToCardDict = new Dictionary<int, CardProspector>();

        //iteration through the JsonLayoutSlots pulled from JsonLayout
        foreach(JsonLayoutSlot slot in jsonLayout.slots)
        {
            cp = Draw();  //pull a card from top of draw pile
            cp.faceUp = slot.faceUp;
            //make the CardProspector a child of layout Anchor
            cp.transform.SetParent(layoutAnchor);

            //convert the last char of the layer string to an int
            int z = int.Parse(slot.layer[slot.layer.Length - 1].ToString());

            //set the localPosition of the card based on slot information
            cp.SetLocalPos (new Vector3(
                jsonLayout.multiplier.x * slot.x,
                jsonLayout.multiplier.y * slot.y,
                -z
            ));

            cp.layoutID = slot.id;
            cp.layoutSlot = slot;
            cp.state = eCardState.mine;

            //set the sorting layer of all spriteRenderers on the card
            cp.SetSpriteSortingLayer(slot.layer);

            mine.Add(cp);

            //add this CardProspector to the mineIdtoCarddict
            mineIdToCardDict.Add(slot.id, cp);
        }
    }

    /// <summary>
    /// Moves current target card to discard
    /// </summary>
    /// <param name="cp">The CardProspector to be moved</param>
    void MoveToDiscard(CardProspector cp)
    {
        //sets state of card, adds it to discard pile, then updates parent
        cp.state = eCardState.discard;
        discardPile.Add(cp);
        cp.transform.SetParent(layoutAnchor);

        cp.SetLocalPos(new Vector3(
            jsonLayout.multiplier.x * jsonLayout.discardPile.x,
            jsonLayout.multiplier.y * jsonLayout.discardPile.y,
            0
        ));

        cp.faceUp = true;

        cp.SetSpriteSortingLayer(jsonLayout.discardPile.layer);
        cp.SetSortingOrder(-200 + (discardPile.Count *3));
    }

/// <summary>
/// Make cp the new target card
/// </summary>
/// <param name="cp">The card prospector to be moved</param>
    void MoveToTarget(CardProspector cp)
    {
        //if there is a target card, move it to discard
        if (target != null) MoveToDiscard(target);

        //move target card to correct location
        MoveToDiscard(cp);

        target = cp;
        cp.state = eCardState.target;

        cp.SetSpriteSortingLayer("Target");
        cp.SetSortingOrder(0);
    }

/// <summary>
/// Arranges cards of drawpile to show remaining
/// </summary>
    void UpdateDrawPile()
    {
        CardProspector cp;

        for (int i = 0; i < drawPile.Count; i++)
        {
            cp = drawPile[i];
            cp.transform.SetParent(layoutAnchor);

            //position based on stagger level
            Vector3 cpPos = new Vector3();
            cpPos.x = jsonLayout.multiplier.x * jsonLayout.drawPile.x;

            cpPos.x += jsonLayout.drawPile.xStagger * i;
            cpPos.y = jsonLayout.multiplier.y * jsonLayout.drawPile.y;
            cpPos.z = 0.1f * i;
            cp.SetLocalPos(cpPos);

            cp.faceUp = false;
            cp.state = eCardState.drawpile;

            cp.SetSpriteSortingLayer(jsonLayout.drawPile.layer);
            cp.SetSortingOrder(-10*i);
        }
    } 

/// <summary>
/// Turns card in the mine faceup and facedown
/// </summary>
    public void SetMineFaceUps()
    {
        CardProspector coverCP;
        foreach (CardProspector cp in mine)
        {
            bool faceUp = true;

            foreach (int coverID in cp.layoutSlot.hiddenBy)
            {
                coverCP = mineIdToCardDict[coverID];
                //if covering card is null or still in mine, faceup = false
                if(coverCP == null || coverCP.state == eCardState.mine)
                {
                    faceUp = false;
                }
            }
            cp.faceUp = faceUp;
        }
    }

    static public void CARD_CLICKED(CardProspector cp)
    {
        switch (cp.state)
        {
            case eCardState.target: //clicking target does nothing
                break;

            case eCardState.drawpile: //clicking drawpile draws the next card
                S.MoveToTarget(S.Draw());
                S.UpdateDrawPile();
                break;

                case eCardState.mine:
                    bool validMatch = true;

                    if (!cp.faceUp) validMatch = false; //facedown cards can't match

                    if(!cp.AdjacentTo(S.target)) validMatch = false; //must be an adjacent rank

                    if (validMatch)
                {
                    S.mine.Remove(cp); //if match, remove from mine
                    S.MoveToTarget(cp); //make target card
                }
                    S.SetMineFaceUps();
                    break;
        }
    }

}
