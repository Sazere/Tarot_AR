using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;
using System.Threading;
using System;

namespace tARot
{
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

        public List<Card> playableCards = new List<Card>();

        public void Start(){
            GM = FindObjectOfType<GameManager>();
        }

        void Awake(){
            dismissButton.onClick.AddListener(Dismiss);
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        }

        void OnEnable(){
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged2;
        }

        void OnDisable(){
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged2;
        }

        private void Dismiss(){
            welcomePanel.SetActive(false);
            instructionsPanel.SetActive(true);
        }

        private void Exit(){
            SceneManager.LoadScene("Menu");
        }

        void OnTrackedImagesChanged2(ARTrackedImagesChangedEventArgs eventArgs){

            //Tant que tous les rounds ne sont pas jou?s, on va chercher ? scanner des cartes
            if (round != GM.nbRounds){
                foreach (ARTrackedImage trackedImage in eventArgs.added){
                    UpdateARImageAdded2(trackedImage);
                }

                foreach (ARTrackedImage trackedImage in eventArgs.updated){
                    if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking){
                        UpdateARImageUpdated2(trackedImage);
                    }
                }
                foreach (ARTrackedImage trackedImage in eventArgs.removed){
                    Debug.Log($"test");
                }

                var output = "";
                foreach (Card i in GM.cards)
                {
                    output += i.ToString() + " | ";
                }
                imageTrackedText.text = output;
            }
            else{
                //On actualise une derni?re fois l'affichage
                imageTrackedText.text = "";
                //et on clear toutes les variables
                cardsPlayedGame.Clear();

                exitPanel.SetActive(true);
                exitButton.onClick.AddListener(Exit);

            }
        }

        private void UpdateARImageAdded2(ARTrackedImage trackedImage){
            //Scan des cartes sur le plateau
            if (cardsPlayedRound.Count != (GM.nbPlayers - 1)){
                string[] subs = trackedImage.referenceImage.name.Split('-');
                Card card = new Card(subs[1], Convert.ToInt32(subs[0]));
                bool cardPlayer = GM.cards.Contains(card);
                ListCardPlayedBoard.text = "Add" + cardPlayer.ToString() + trackedImage.referenceImage.name;
            }
        }
        private void UpdateARImageUpdated2(ARTrackedImage trackedImage){
            var output = "";
            //Est-ce que la carte a d?j? ?t? scann? ?
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
            //On va scanner les cartes des autres joueurs
            if (cardsPlayedRound.Count != (GM.nbPlayers - 1)){
                //On v?rifie que ce n'est pas une carte du joueur
                if (cardPlayer == false){
                    //Et qu'elle n'a pas d?j? ?t? scann?
                    if (alreadyDisplayed == false){
                        //On l'ajoute ? la liste des cartes du round
                        cardsPlayedRound.Add(trackedImage.referenceImage.name);
                        cardsPlayedGame.Add(trackedImage.referenceImage.name);
                        foreach (string i in cardsPlayedRound)
                        {
                            output += i.ToString() + " | ";
                        }
                        // Display the name of the tracked image in the canvas
                         ListCardPlayedBoard.text = output;

                        // Check what card we can play
                        checkCardsToPlay(card);

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
            //Quand toutes les cartes du plateau ont ?t? scann?, on va scanner la carte du user.
            if (cardsPlayedRound.Count == (GM.nbPlayers - 1)){                
                //On v?rifie que c'est une carte du joueur
                if (cardPlayer == true && alreadyDisplayed == false){
                    round++;
                    //On efface tout
                    cardsPlayedRound.Clear();
                    cardsPlayedGame.Add(trackedImage.referenceImage.name);
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

        // function to trigger after scaning gameCard, it will set the HashSet of cards that we can play
        public void checkCardsToPlay(Card gameCard)
        {
            playableCards.Clear();
            bool haveSuit = checkSuit(gameCard);

            foreach (Card handCard in GM.cards)
            {
                if (haveSuit && (handCard.getSuit() == gameCard.getSuit()))
                {
                    Debug.Log($"-------JJJJJJAAAAAIIIIII------- {handCard}");
                    playableCards.Add(handCard);
                    //if (handCard.getValue() > gameCard.getValue()) handCard.setHighlight(true);
                }
                else if (!haveSuit && handCard.isAtout())
                {
                    Debug.Log($"-------CCCOOOOUUUUUPPPPPPEEE------- {handCard}");
                    playableCards.Add(handCard);
                    //handCard.setHighlight(true);
                }
                else if (!haveSuit && !handCard.isAtout())
                {
                    Debug.Log($"-------PPPPIIIIISSSSSSEEEEE------- {handCard}");
                    playableCards.Add(handCard);
                }
            }
        }

    }

}
