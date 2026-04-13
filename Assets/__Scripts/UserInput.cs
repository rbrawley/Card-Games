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
        CardKlondike kp = selected.gameObject.GetComponent<CardKlondike>();
        _tSRend = kp.GetComponent<SpriteRenderer>();

        print ("Clicked card");
        //flips card face up if it is unblocked when clicked

        //if card is in deck and unblocked by other cards in triple, select it


        //selects card if no card selected
        if (slot1 == this.gameObject)
        {
            
            slot1 = selected;
            _tSRend.color = Color.yellow;
        }

        //if a card is selected when slot1 != null, either stack them if eligable or select new card

        else if (slot1 != selected)
        {
            
        }
    }

    bool Stackable(GameObject selected)
    {
        CardKlondike S1 = slot1.GetComponent<CardKlondike>();
        CardKlondike S2 = selected.GetComponent<CardKlondike>();
        return false;
    }
}
