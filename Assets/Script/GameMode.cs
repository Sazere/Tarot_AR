using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

namespace tARot
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class GameMode : MonoBehaviour
    {
        private GameManager GM;

        [SerializeField]
        private GameObject welcomePanel;

        [SerializeField]
        private Button dismissButton;

        [SerializeField]
        private Text imageTrackedText;

        private ARTrackedImageManager m_TrackedImageManager;

        public HashSet<Card> cards = new HashSet<Card>();
        public HashSet<Card> gameCards = new HashSet<Card>();
        public List<Card> playableCards = new List<Card>();

        public string output = "";


        public void Start(){
            GM = FindObjectOfType<GameManager>();

            foreach (Card i in GM.gameCards)
            {
                //output += checkCardsToPlay(i).Count.ToString() + "|";
                checkCardsToPlay(i);

            }
            //imageTrackedText.text = "Cards: "+ playableCards[0].ToString(); //index out of bound

            foreach (Card i in playableCards)
            {
                output += i.ToString() + " | ";

            }
        }

        void Awake(){
            dismissButton.onClick.AddListener(Dismiss);
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

            
        }

        void OnEnable(){
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        void OnDisable(){
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        private void Dismiss() => welcomePanel.SetActive(false);

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs){
            //botCenterScreenSpace = new Vector2(Screen.width / 2, 0);
            //botCenterWorldSpace = Camera.main.ScreenToWorldPoint(new Vector3(botCenterScreenSpace.x, botCenterScreenSpace.y, Camera.main.nearClipPlane));
            //GameObject newCardUi = GameObject.Instantiate(cardPrefab, new Vector3(-5.750023f, -10.92712f, 0), Quaternion.identity);
            //newCardUi.SetActive(true);

            // GameObject newCardUi = GameObject.Instantiate(cardPrefab,botCenterWorldSpace, Quaternion.identity);


            imageTrackedText.text = "Cards: " + output; //index out of bound


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
            //playableCards.Clear();
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
