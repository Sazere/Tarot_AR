using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace tARot{
    public class MainMenu : MonoBehaviour{
        private GameManager GM;

        public void Start(){
            GM = FindObjectOfType<GameManager>();
            GM.maxCards = 3;
        }

        public void PlayGame(){
            Debug.Log("nb joueurs :" + GM.nbPlayers);
            SceneManager.LoadScene("ScanMode");
        }

        public void NbPlayers4(){
            Debug.Log("4 joueurs");
            GM.nbPlayers = 4;
            GM.maxCards = 3;
        }

        public void NbPlayers3(){
            Debug.Log("3 joueurs");
            GM.nbPlayers = 3;
            GM.maxCards = 3;
        }

        public void QuitGame(){
            Application.Quit();
        }
    }
}
