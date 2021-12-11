using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tARot
{
    public class Card : MonoBehaviour
    {
        protected string suit;
        protected string cardvalue;
        public Card()
        {
        }
        public Card(string suit2, string cardvalue2)
        {
            suit = suit2;
            cardvalue = cardvalue2;
        }

        public string getSuit()
        {
            return suit;
        }

        public string getCardvalue()
        {
            return cardvalue;
        }

        public override string ToString()
        {
            return string.Format("{0} of {1}", cardvalue, suit);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
