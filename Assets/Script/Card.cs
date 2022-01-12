using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tARot{    
     /// <summary>
     /// The Card class is a class that represents a classic card game card. 
     /// It has a number as well as a family. It has a variable (isHighlighted) to know if the card is recommended or not
     /// </summary>
    public class Card : MonoBehaviour{
        protected string suit;
        protected int cardvalue;
        protected bool isHighlighted = false;

        public Card(string suit2, int cardvalue2){
            suit = suit2;
            cardvalue = cardvalue2;
        }

        public string getSuit(){
            return suit.ToLower();
        }

        public int getValue(){
            return cardvalue;
        }

        public bool isAtout(){
            return suit.ToLower() == "atout";
        }

        public override string ToString(){
            return string.Format("{0} of {1}", cardvalue, suit);
        }

        // Start is called before the first frame update
        void Start(){}

        // Update is called once per frame
        void Update(){}
    }
}
