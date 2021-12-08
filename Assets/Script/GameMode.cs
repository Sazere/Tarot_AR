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


        public void Start(){
            GM = FindObjectOfType<GameManager>();
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
              var output ="";
               foreach (Card i in GM.cards)
               {
                   output += i.ToString()+ "|";
               }
               imageTrackedText.text = "test"+ output;



        }
    }

}
