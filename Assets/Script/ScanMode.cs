using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;
using System.Linq;

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
        private GameObject[] arObjectsNumbersLettersToPlace;

        [SerializeField]
        private GameObject[] arObjectsSymbolsToPlace;

        private ARTrackedImageManager m_TrackedImageManager;
        
        private Dictionary<string, List<GameObject>> arObjects2 = new Dictionary<string, List<GameObject>>();
        public HashSet<Card> cards = new HashSet<Card>();

        public void Start()
        {
            GM = FindObjectOfType<GameManager>();
        }

        void Awake(){
            dismissButton.onClick.AddListener(Dismiss);
            gameMode.onClick.AddListener(GameMode);
            m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

            // setup all game objects in dictionary
            foreach (GameObject arObject in arObjectsSymbolsToPlace){
                GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
                var numberAndSymbol = new List<GameObject>();              
                foreach(GameObject arObjectNumber in arObjectsNumbersLettersToPlace){
                    GameObject newARNumberObject = Instantiate(arObjectNumber, Vector3.zero, Quaternion.identity);             
                    var numberSymbolString = arObjectNumber.name + "-" + arObject.name;
                    newARObject.name = numberSymbolString;
                    newARNumberObject.name = numberSymbolString;
                    numberAndSymbol.Add(newARObject);
                    numberAndSymbol.Add(newARNumberObject);
                    arObjects2.Add(numberSymbolString, numberAndSymbol);
                }        
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
            SceneManager.LoadScene("GameMode");
        }

        void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs){
            //On vérifie qu'on a bien scanné toutes les cartes et pas une de plus.
            if (GM.maxCards != cards.Count){
                foreach (ARTrackedImage trackedImage in eventArgs.added){
                    UpdateARImageAdded(trackedImage);
                }
            }
            foreach (ARTrackedImage trackedImage in eventArgs.updated)
            {
                if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    UpdateARImageUpdated(trackedImage);
                }
            }

            foreach (ARTrackedImage trackedImage in eventArgs.removed)
            {
                foreach(GameObject go in arObjects2[trackedImage.referenceImage.name]){
                    go.SetActive(false);
                }
            }
        }

        private void UpdateARImageAdded(ARTrackedImage trackedImage){
            // Display the name of the tracked image in the canvas
            imageTrackedText.text = trackedImage.referenceImage.name;

            string[] subs = trackedImage.referenceImage.name.Split('-');

            Card card = new Card(subs[0], subs[1]);

            cards.Add(card);

            Vector3 offset = new Vector3((float)0.03, 0, 0);
            arObjects2[trackedImage.referenceImage.name].ElementAt(0).transform.position = trackedImage.transform.position;
            arObjects2[trackedImage.referenceImage.name].ElementAt(0).SetActive(true);     
            offset.x = (float)-0.03;
            arObjects2[trackedImage.referenceImage.name].ElementAt(1).transform.position = trackedImage.transform.position + offset;
            arObjects2[trackedImage.referenceImage.name].ElementAt(1).SetActive(true);
            
              foreach (List<GameObject> listGo in arObjects2.Values){
                  foreach(GameObject go in listGo){
                      Debug.Log($"Go in arObjects.Values: {go.name}");
                      if (go.name != trackedImage.referenceImage.name){
                          Debug.Log($"Go in arObjects.Values DESACTIVATED: {go.name}");
                          go.SetActive(false);
                      }
                  }     
              }

            Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
            imageCardsText.text = "Cards " + cards.Count + "/"+ GM.maxCards;
            GM.cards = cards;
        }
        private void UpdateARImageUpdated(ARTrackedImage trackedImage){
            // Display the name of the tracked image in the canvas
            imageTrackedText.text = trackedImage.referenceImage.name;

            Vector3 offset = new Vector3((float)0.03, 0, 0);
            arObjects2[trackedImage.referenceImage.name].ElementAt(0).transform.position = trackedImage.transform.position;
            arObjects2[trackedImage.referenceImage.name].ElementAt(0).SetActive(true);
            offset.x = (float)-0.03;
            arObjects2[trackedImage.referenceImage.name].ElementAt(1).transform.position = trackedImage.transform.position + offset;
            arObjects2[trackedImage.referenceImage.name].ElementAt(1).SetActive(true);        

            foreach (List<GameObject> listGo in arObjects2.Values){
                  foreach(GameObject go in listGo){
                      Debug.Log($"Go in arObjects.Values: {go.name}");
                      if (go.name != trackedImage.referenceImage.name){
                          Debug.Log($"Go in arObjects.Values DESACTIVATED: {go.name}");
                          go.SetActive(false);
                      }
                  }     
              }
            Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
        }
    }
}