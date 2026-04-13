using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState{drawpile, mine, target, discard}
public class CardProspector : Card
{
   [Header("Dynamic: CardProspector")]
   public eCardState            state = eCardState.drawpile;

   public List<CardProspector> hiddenBy = new List<CardProspector>();

   public int                   layoutID;

   public JsonLayoutSlot        layoutSlot;

   override public void OnMouseUpAsButton()
   {
       //base.OnMouseUpAsButton();  //uncomment to call base verson

      Prospector.CARD_CLICKED(this);
   }
}
