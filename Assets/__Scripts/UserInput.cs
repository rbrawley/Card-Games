using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    private Klondike klondike;
    public GameObject slot1;
    public GameObject PrefabCardKS;
    private SpriteRenderer _tSRend;
    void Awake()
    {
        // Transform cardTrans = transform.Find("PrefabCardKS");
        // PrefabCardKS = cardTrans.gameObject;
        // PrefabCardKS.SetActive(false);
    }

    void Start()
    {
        klondike = FindObjectOfType<Klondike>();
        slot1 = this.gameObject;
    }
    void Update()
    {
        GetMouseClick();
    }
    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                if (hit.collider.CompareTag("DeckPile"))
                {
                    DeckPile();
                }
                if (hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject);
                }
                if (hit.collider.CompareTag("Goal"))
                {
                    Goal();
                }
                if (hit.collider.CompareTag("Play"))
                {
                    Play();
                }
            }
        }
    }

void DeckPile()
    {
        //print ("Clicked Deck");
        klondike.DealFromDeck();
    }
void Play()
    {
         print ("Clicked play");
    }
void Goal()
    {
         print ("Clicked goal");
    }
void Card(GameObject selected)
    {   
        if(Selectable(selected)){
            CardKlondike kp = selected.gameObject.GetComponent<CardKlondike>();

            //flips card face up if it is unblocked when clicked

            //if card is in deck and unblocked by other cards in triple, select it


            //selects card if no card selected
            if (slot1 == this.gameObject)
            {
                
                slot1 = selected;
                _tSRend = kp.GetComponent<SpriteRenderer>();
                _tSRend.color = Color.yellow;
            }

            //if a card is selected when slot1 != null, either stack them if eligable or select new card

            else if (slot1 != selected)
            {
                if (Stackable(selected)){
                    _tSRend = slot1.GetComponent<SpriteRenderer>();
                    _tSRend.color = Color.white;
                    Stack(selected);
                    
                }
                else
                {
                    _tSRend = slot1.GetComponent<SpriteRenderer>();
                    _tSRend.color = Color.white;
                    _tSRend = kp.GetComponent<SpriteRenderer>();
                    slot1 = selected;
                    _tSRend.color = Color.yellow;
                }
            }
        }
    }


    bool Stackable(GameObject selected)
    {
        CardKlondike s1 = slot1.GetComponent<CardKlondike>();
        CardKlondike s2 = selected.GetComponent<CardKlondike>();


        if(s2.state == eCardStateKS.play){
            if (!s1.faceUp || !s2.faceUp) return false;
            if (!s1.AdjacentTo(s2, false)) return false;
            if (s1.rank > s2.rank) return false;
            if (s1.colS == s2.colS) return false;
        }
        else if (s2.state == eCardStateKS.goal)
        {
            print("test");
            if (!s1.faceUp) return false;
            if (s1.suit != s2.suit) return false;
            if (!s1.AdjacentTo(s2, false)) return false;
            if (s1.rank < s2.rank) return false;
        }
        else return false;

        return true;
    }

    bool Selectable(GameObject selected)
    {
        CardKlondike kp = selected.GetComponent<CardKlondike>();

        if (!kp.faceUp) return false;

        if (kp.transform.parent == klondike.kpDiscard)
        {
          if( kp != klondike.discardPile[klondike.discardPile.Count-1])  return false;
        }

        return true;
    }

    void MoveCard(GameObject selected)
    {   
        CardKlondike kp = slot1.GetComponent<CardKlondike>();
        CardKlondike target = selected.GetComponent<CardKlondike>();

        if (target.state == eCardStateKS.goal)
        {
            //TODO move to goal
        }
        else
        {
            
        }
    }

    void Stack(GameObject selected)
    {
        CardKlondike s1 = slot1.GetComponent<CardKlondike>();
        CardKlondike s2 = selected.GetComponent<CardKlondike>();
        float yOffset = 0.5f;

        if (s2.state == eCardStateKS.goal || (s2.state != eCardStateKS.goal && s1.rank == 13))
        {
            yOffset = 0;
        }

        slot1.transform.position = new Vector3(selected.transform.position.x,selected.transform.position.y - yOffset, selected.transform.position.z + 1.0f);
        slot1.transform.parent = selected.transform;  //used to make children move with parents

        if(s1.state == eCardStateKS.discard)
        {
            klondike.triplesOnDisplay.Remove(s1);
            klondike.discardPile.Remove(s1);
        }
        else if (s1.state == eCardStateKS.goal && s2.state == eCardStateKS.goal && s1.rank == 1)
        {
            //int test = klondike.goalPos.IndexOf(klondike.goalPos, s1.parent.transform);
            klondike.goalPos[s1.row] = null;
        }
        else if (s1.state == eCardStateKS.goal)
        {
            klondike.goalPos[s1.row].GetComponent<CardKlondike>().rank = s1.rank - 1;
        }
        else // removes the card string from the appropriate list
        {
            klondike.play[s1.row].Remove(s1);
        }
//
        s1.row = s2.row;
        s1.transform.SetParent(s2.transform.parent);

        if (s2.state == eCardStateKS.goal) 
        {
            klondike.goalPos[s1.row] = slot1;
            s1.state  = eCardStateKS.goal;
        }
        

        // after completing move reset slot1 to be null as being null will break the logic
        slot1 = this.gameObject;

    }
}
