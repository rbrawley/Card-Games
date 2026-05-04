using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Dynamic")]
    public char             suit;  //suit of card
    public int              rank; //rank of card 1-13
    public Color            color = Color.black;  // color to tint pips
    public string           colS = "Black";  // or "red"  name of color  
    public GameObject       back;  //back of card
    public JsonCard         def;  //card layout as defined in json_deck.json  
    private UserInput       userInput;
    public int              row;
    

    //list for decorators
    public List<GameObject> decoGOs = new List<GameObject>();
    //list for pips
    public List<GameObject> pipGOs = new List<GameObject>();


    /// <summary>
    /// creates this card's visuals based on suit and rank
    /// </summary>
    /// <param name="eSuit">The suit of the card</param>
    /// <param name="eRank">The rank from 1 to 13</param>
    public void Init(char eSuit, int eRank, bool startFaceUp = true)
        {
            gameObject.name = name = eSuit.ToString() + eRank;
            suit = eSuit;
            rank = eRank;

            //if diamond/heart, change default black to Red
            if( suit == 'D' || suit == 'H')
            {
                colS = "Red";
                color = Color.red;
            }

            def = JsonParseDeck.GET_CARD_DEF(rank);

            //build the card from sprites
            AddDecorators();
            AddPips();
            AddFace();
            AddBack();
            faceUp = startFaceUp;
        }

    /// <summary>
    /// shortcut for setting transform.local position
    /// </summary>
    /// <param name="v"></param>
    public virtual void SetLocalPos(Vector3 v)
        {
            transform.localPosition = v;
        }

        private Sprite          _tSprite = null;
        private GameObject      _tGO = null;
        private SpriteRenderer  _tSRend = null;

        //flip sprites upside down 180 degree
        private Quaternion      _flipRot = Quaternion.Euler(0,0,180);

        
        private void AddDecorators()
    {
        foreach (JsonPip pip in JsonParseDeck.DECORATORS)
        {
            if(pip.type == "suit"){
                _tGO = Instantiate<GameObject>(Deck.SPRITE_PREFAB, transform);
                _tSRend = _tGO.GetComponent<SpriteRenderer>();
                _tSRend.sprite = CardSpritesSO.SUITS[suit];  
                
            }
            else
            {
                _tGO = Instantiate<GameObject>(Deck.SPRITE_PREFAB, transform);
                _tSRend = _tGO.GetComponent<SpriteRenderer>();
                _tSRend.sprite = CardSpritesSO.RANKS[rank];
                _tSRend.color = color;
            }

            //make decorator sprites render above card
            _tSRend.sortingOrder = 1;
            //set localposition based on the location from deckxml
            _tGO.transform.localPosition = pip.loc;
            if (pip.flip) _tGO.transform.rotation = _flipRot;
            if (pip.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }
            //give gameObject name to find in hierarchy
            _tGO.name = pip.type;
            //add decorator gameobject to the list card.decoGos
            decoGOs.Add(_tGO); 
        }
    }

    private void AddPips()
    {
        int pipNum = 0;
        foreach (JsonPip pip in def.pips)
        {
            _tGO = Instantiate<GameObject>(Deck.SPRITE_PREFAB, transform);

            _tGO.transform.localPosition = pip.loc;

            if (pip.flip) _tGO.transform.rotation = _flipRot;

            if(pip.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }

            _tGO.name = "pip_"+pipNum++;
            _tSRend = _tGO.GetComponent<SpriteRenderer>();
            _tSRend.sprite = CardSpritesSO.SUITS[suit];
            _tSRend.sortingOrder = 1;
            pipGOs.Add(_tGO);
        }
    }

    private void AddFace()
    {
        if(def.face == "") return;

        string faceName = def.face + suit;
        _tSprite = CardSpritesSO.GET_FACE(faceName);
        if(_tSprite == null)
        {
            Debug.LogError("Face sprite " + faceName + " not found.");
            return;
        }

        _tGO = Instantiate<GameObject>(Deck.SPRITE_PREFAB, transform);
        _tSRend = _tGO.GetComponent<SpriteRenderer>();
        _tSRend.sprite = _tSprite;
        _tSRend.sortingOrder = 1;
        _tGO.transform.localPosition = Vector3.zero;
        _tGO.name = faceName;


    }

    public bool faceUp
    {
        get {return (!back.activeSelf);}
        set {back.SetActive(!value);}
    }
    private void AddBack()
    {
         _tGO = Instantiate<GameObject>(Deck.SPRITE_PREFAB, transform);
        _tSRend = _tGO.GetComponent<SpriteRenderer>();
        _tSRend.sprite = CardSpritesSO.BACK;
        _tSRend.sortingOrder = 2;
        _tGO.name = "back";
        back = _tGO;
    }

    private SpriteRenderer[] spriteRenderers;

    void PopulateSpriteRenderers()
    {
        if (spriteRenderers != null) return;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

/// <summary>
/// moves the sprites of this card to a specified sorting layer
/// </summary>
/// <param name="layerName">Name of the Layer to move to</param>
    public void SetSpriteSortingLayer(string layerName){
        PopulateSpriteRenderers();

        foreach(SpriteRenderer srend in spriteRenderers)
        {
            srend.sortingLayerName = layerName;
        }
    }

    public void SetSortingOrder(int sOrd)
    {
        PopulateSpriteRenderers();

        foreach (SpriteRenderer srend in spriteRenderers)
        {
            //if the gameObject is this gameobject, set card to bottom layer
            if (srend.gameObject == this.gameObject)
            {
                srend.sortingOrder = sOrd;
            
            }
            //if its the back, set to the highest layer
            else if (srend.gameObject.name == "back")
            {
                srend.sortingOrder = sOrd + 2;
            }
            //put inbetween if anything else
            else
            {
                srend.sortingOrder = sOrd + 1;
            }
        }
    }

    virtual public void OnMouseUpAsButton()
    {
        print (name);
    }

    public bool AdjacentTo(Card otherCard, bool wrap = true)
    {
        if (!faceUp || !otherCard.faceUp) return false;  //if either are facedown, not a match

        if (Mathf.Abs(rank - otherCard.rank) == 1) return true;

        if (wrap)  //if cards are ace and king, cards are adjacent
        {
            if (rank == 1 && otherCard.rank == 13) return true;
            if (rank == 13 && otherCard.rank == 1) return true;
        }

        return false;
    }

 

    
}
