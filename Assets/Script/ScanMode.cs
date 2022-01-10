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
    public class ScanMode : MonoBehaviour{
        private GameManager GM;

        [SerializeField]
        private GameObject welcomePanel;

        [SerializeField]
        private Button dismissButton;

        [SerializeField]
        private Button gameMode;

        [SerializeField]
        private Text imageTrackedText;
        [SerializeField]
        private Text imageCardsText;

        [SerializeField]
        private GameObject[] arObjectsToPlace;

        private ARTrackedImageManager m_TrackedImageManager;

        private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();
        public HashSet<Card> cards = new HashSet<Card>();

        public void Start()
        {
            GM = FindObjectOfType<GameManager>();
            imageCardsText.text = "Cards 0/" + GM.maxCards;
            cards.Clear();
        }

        void Awake(){
            dismissButton.onClick.AddListener(Dismiss);
            gameMode.onClick.AddListener(GameMode);
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

            //Simpler version
            foreach (GameObject arObject in arObjectsToPlace){
                GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
                newARObject.name = arObject.name;   
                arObjects.Add(newARObject.name, newARObject);                
            }
        }

        void OnEnable(){
            m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }

        void OnDisable(){
            m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }

        private void Dismiss() => welcomePanel.SetActive(false);

        private void GameMode() {
            Thread.Sleep(2000);
            SceneManager.LoadScene("GameMode");
        }

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs){
            //On vérifie qu'on a bien scanné toutes les cartes et pas une de plus.
            if (GM.maxCards != cards.Count){
                foreach (ARTrackedImage trackedImage in eventArgs.added){
                    UpdateARImageAdded(trackedImage);
                }
            }
            else
            {
                GameMode();
            }
            foreach (ARTrackedImage trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {

                    if (GM.firstGame == true){
                        UpdateARImageUpdated(trackedImage);

                    }
                    else{

                        //On va vérifier si on a déjà scanné la carte dans notre jeu ou non
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

                        //On a déjà scanné la carte dans notre jeu donc on affiche que son animation
                        if (cardPlayer == true)
                        {
                            UpdateARImageUpdated(trackedImage);

                        }
                        else
                        {
                            UpdateARImageAdded(trackedImage);
                        }
                    }
                }
            }

            foreach (ARTrackedImage trackedImage in eventArgs.removed)
            {
                arObjects[trackedImage.referenceImage.name].SetActive(false);
            }
        }

        private void UpdateARImageAdded(ARTrackedImage trackedImage){
            // Display the name of the tracked image in the canvas
            imageTrackedText.text = trackedImage.referenceImage.name;

            string[] subs = trackedImage.referenceImage.name.Split('-');

            Card card = new Card(subs[1], Convert.ToInt32(subs[0]));

            cards.Add(card);

            imageCardsText.text = "Cards " + cards.Count + "/" + GM.maxCards;
            GM.cards = cards;


            arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
            arObjects[trackedImage.referenceImage.name].transform.rotation = Quaternion.Euler(0, -90, 0);
            arObjects[trackedImage.referenceImage.name].SetActive(true);

            foreach (GameObject go in arObjects.Values){           
                    Debug.Log($"Go in arObjects.Values: {go.name}");
                    if (go.name != trackedImage.referenceImage.name){
                        Debug.Log($"Go in arObjects.Values DESACTIVATED: {go.name}");
                        go.SetActive(false);
                    }                    
            }
            Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
        }
        private void UpdateARImageUpdated(ARTrackedImage trackedImage){
            // Display the name of the tracked image in the canvas
            imageTrackedText.text = trackedImage.referenceImage.name;

            arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
            arObjects[trackedImage.referenceImage.name].transform.rotation = Quaternion.Euler(0, -90, 0);
            arObjects[trackedImage.referenceImage.name].SetActive(true);

            foreach (GameObject go in arObjects.Values){           
                    Debug.Log($"Go in arObjects.Values: {go.name}");
                    if (go.name != trackedImage.referenceImage.name){
                        Debug.Log($"Go in arObjects.Values DESACTIVATED: {go.name}");
                        go.SetActive(false);
                    }                    
            }
            Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
        }
    }
}