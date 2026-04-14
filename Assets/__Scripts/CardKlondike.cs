using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardStateKS{drawpile, goal, play, discard}
public class CardKlondike : Card
{
   [Header("Dynamic: CardKlondike")]
   public eCardStateKS            state = eCardStateKS.drawpile;

   public List<CardKlondike> hiddenBy = new List<CardKlondike>();

   public int                   layoutID;

   public JsonLayoutSlot        layoutSlot;
   

   override public void OnMouseUpAsButton()
   {
       //base.OnMouseUpAsButton();  //uncomment to call base verson

      //Klondike.CARD_CLICKED(this);
   }

   
}
