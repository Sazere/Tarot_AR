using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;
using System.Threading;
using System;

namespace tARot{
    /// <summary>
    /// The GameMode class is a class that handles all events (add,refresh and delete) during a game round.
    /// The first cards scanned correspond to the opponents' cards, depending on the number of players previously decided. The last card scanned is necessarily a player's card.
    /// We will therefore always go to the update event.
    /// When all the player's cards have been scanned, a popup appears to signify the end of the game. This refers to the MainMenu scene.
    /// </summary>
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class GameMode : MonoBehaviour
    {
        private GameManager GM;

        [SerializeField]
        private GameObject welcomePanel;

        [SerializeField]
        private GameObject exitPanel;

        [SerializeField]
        private Button dismissButton;

        [SerializeField]
        private Button exitButton;

        [SerializeField]
        private Text imageTrackedText;

        [SerializeField]
        private Text ListCardPlayedBoard;

        [SerializeField]
        private Text CardPlayer;

        [SerializeField]
        private GameObject instructionsPanel;

        private ARTrackedImageManager m_TrackedImageManager;
        public string cardsPlayed = "";
        public List<string> cardsPlayedRound = new List<string>();
        public List<string> cardsPlayedGame = new List<string>();
        public int round = 0;

        public bool haveSuit;
        public List<Card> playableCards = new List<Card>();
        public List<Card> greatestAtout = new List<Card>();

        public void Start(){
            GM = FindObjectOfType<GameManager>();
        }

        void Awake(){
            dismissButton.onClick.AddListener(Dismiss);
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        void OnEnable(){
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChangedGameMode;
        }

        void OnDisable(){
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChangedGameMode;
        }

        private void Dismiss(){
            welcomePanel.SetActive(false);
            instructionsPanel.SetActive(true);
        }

        private void Exit(){
            GM.firstGame = false;
            SceneManager.LoadScene("Menu");
        }

        void OnTrackedImagesChangedGameMode(ARTrackedImagesChangedEventArgs eventArgs){
            // As long as all rounds are not played, we will try to scan cards
            if (round != GM.nbRounds){
                foreach (ARTrackedImage trackedImage in eventArgs.added){
                    UpdateARImageAddedGameMode(trackedImage);
                }

                foreach (ARTrackedImage trackedImage in eventArgs.updated){
                    if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking){
                        UpdateARImageUpdatedGameMode(trackedImage);
                    }
                }
                foreach (ARTrackedImage trackedImage in eventArgs.removed){}

                var output = "";
                foreach (Card i in GM.cards){
                    output += i.ToString() + " | ";
                }
                imageTrackedText.text = output;
            }
            else{
                //Refresh the display one last time
                imageTrackedText.text = "";
                //and clear all variables
                cardsPlayedGame.Clear();

                exitPanel.SetActive(true);
                exitButton.onClick.AddListener(Exit);

            }
        }

        private void UpdateARImageAddedGameMode(ARTrackedImage trackedImage){
            //Scan of the cards on the board
            if (cardsPlayedRound.Count != (GM.nbPlayers - 1)){
                string[] subs = trackedImage.referenceImage.name.Split('-');
                Card card = new Card(subs[1], Convert.ToInt32(subs[0]));
                bool cardPlayer = GM.cards.Contains(card);
                ListCardPlayedBoard.text = "Add" + cardPlayer.ToString() + trackedImage.referenceImage.name;
            }
        }
        private void UpdateARImageUpdatedGameMode(ARTrackedImage trackedImage){
            var output = "";
            // Was the card already scanned?
            bool alreadyDisplayed = cardsPlayedGame.Contains(trackedImage.referenceImage.name);
            string[] subs = trackedImage.referenceImage.name.Split('-');
            Card card = new Card(subs[1], Convert.ToInt32(subs[0]));

            bool cardPlayer = false;
            foreach (Card i in GM.cards)
            {
                if (i.getSuit().Equals(card.getSuit()))
                {
                    if (i.getValue().Equals(card.getValue()))
                    {
                        cardPlayer = true;
                        break;
                    }
                }
            }
            Debug.Log($"test{cardsPlayedRound.Count}+GM.nbPlayers - 1 :{GM.nbPlayers - 1}+ cardPlayer{cardPlayer}");
            //We will scan the cards of the other players
            if (cardsPlayedRound.Count != (GM.nbPlayers - 1)){
                //We check that it is not a card of the player
                if (cardPlayer == false){
                    //And that it has not already been scanned
                    if (alreadyDisplayed == false){
                        //We add it to the list of cards of the round
                        cardsPlayedRound.Add(trackedImage.referenceImage.name);
                        cardsPlayedGame.Add(trackedImage.referenceImage.name);
                        if (card.isAtout())
                        {
                            // If the card scanned is an atout we check if it is greater than the last one
                            keepGreaterAtout(card);
                        }
                        
                        foreach (string i in cardsPlayedRound)
                        {
                            output += i.ToString() + " | ";
                        }
                        // Display the name of the tracked image in the canvas
                         ListCardPlayedBoard.text = output;


                        // We check if we have a card of the suit asked (the first card to be played)and what card we can play
                        if (cardsPlayedRound.Count == 1)
                        {
                            haveSuit = checkSuit(card);
                            checkCardsToPlay(card);
                        // We check again what card to play if we do not have the suit asked and that an atout is played
                        } else if (!haveSuit && card.isAtout())
                        {
                            checkCardsToPlay(card);
                        }

                        // Display playing recomendation
                        var recommendation = "";
                        foreach (Card i in playableCards)
                        {
                            recommendation += i.ToString() + " | ";
                        }
                        CardPlayer.text = recommendation;

                    }
                }
            }
            //When all the cards on the board have been scanned, we will scan the user's card.
            if (cardsPlayedRound.Count == (GM.nbPlayers - 1)){
                //We check that it's a player's card
                if (cardPlayer == true && alreadyDisplayed == false){
                    round++;
                    //Delete everything
                    cardsPlayedRound.Clear();
                    greatestAtout.Clear();
                    cardsPlayedGame.Add(trackedImage.referenceImage.name);
                    CardPlayer.text = "";
                    ListCardPlayedBoard.text = "";
                    var imageTrackedTextVariable = "";
                    foreach (Card i in GM.cards)
                    {
                        if (i.getSuit().Equals(card.getSuit()))
                        {
                            if (i.getValue().Equals(card.getValue()))
                            {
                                GM.cards.Remove(i);
                            }
                        }
                        imageTrackedTextVariable += i.ToString() + " | ";
                    }
                    imageTrackedText.text = imageTrackedTextVariable;
                }
            }

        }

        // check if we have at least one card of the suit asked
        private bool checkSuit(Card gameCard)
        {
            foreach (Card handCard in GM.cards)
            {
                Debug.Log($"-------ISAAAAATTTTTOOOOUUUUTTTT------- {handCard.getSuit()}   {gameCard.getSuit()} == {handCard.getSuit() == handCard.getSuit()}");
                if (handCard.getSuit() == gameCard.getSuit())
                {
                    return true;
                }
            }
            return false;
        }

        // check if we have at least one atout 
        private bool checkAtout()
        {
            foreach (Card handCard in GM.cards)
            {
                if (handCard.getSuit() == "atout")
                {
                    return true;
                }
            }
            return false;
        }

        private bool checkGreaterAtout(Card card)
        {
            if (card.isAtout())
            {
                foreach (Card handCard in GM.cards)
                {
                    if (handCard.isAtout())
                    {
                        if (handCard.getValue() > card.getValue())
                        {
                            return true;
                        }

                    }
                }
            }
            return false;
        }

        private void keepGreaterAtout(Card atout)
        {

            if (greatestAtout.Count == 0)
            {
                greatestAtout.Add(atout);
            }
                else if(greatestAtout[0].getValue() < atout.getValue())
            {
                greatestAtout.Clear();
                greatestAtout.Add(atout);
            }
        }

        // function to trigger after scaning gameCard, it will set the HashSet of cards that we can play
        public void checkCardsToPlay(Card gameCard)//14 trefle -- 13 carreau 16 atout
        {
            playableCards.Clear();
            bool haveAtout = checkAtout();
            Debug.Log($"-------haveAtout???????------- {haveAtout}");

            foreach (Card handCard in GM.cards)
            {
                if (haveSuit && (handCard.getSuit() == gameCard.getSuit()))
                {
                    Debug.Log($"-------JJJJJJAAAAAIIIIII------- {handCard}");
                    
                    if (gameCard.isAtout() && checkGreaterAtout(gameCard))
                    {
                        foreach (Card card in greatestAtout)
                        {
                            if (handCard.getValue() > card.getValue())
                            {
                                playableCards.Add(handCard);
                            }
                        }
                    } else
                    {
                        playableCards.Add(handCard);
                    }
                    
                }
                else if (!haveSuit && handCard.isAtout())
                {
                    Debug.Log($"-------CCCOOOOUUUUUPPPPPPEEE------- {handCard}");
                    if (checkGreaterAtout(gameCard))
                    {
                        foreach (Card card in greatestAtout)
                        {
                            if (handCard.getValue() > card.getValue())
                            {
                                playableCards.Add(handCard);
                            }
                        }
                    }
                    else
                    {
                        playableCards.Add(handCard);
                    }
                }
                else if (!haveSuit && !handCard.isAtout() && !haveAtout)
                {
                    Debug.Log($"-------PPPPIIIIISSSSSSEEEEE------- {handCard}");
                    playableCards.Add(handCard);
                }
            }
        }

    }

}
