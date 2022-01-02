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
            NbPlayers4();
        }

        public void PlayGame(){
            Debug.Log("nb joueurs :" + GM.nbPlayers);
            SceneManager.LoadScene("ScanMode");
        }

        public void NbPlayers5()
        {
            Debug.Log("5 joueurs");
            GM.nbPlayers = 5;
            GM.maxCards = 4; //(78-6)/4=14
            GM.nbRounds = 4; //(78-6)/4=14
        }

        public void NbPlayers4(){
            Debug.Log("4 joueurs");
            GM.nbPlayers = 4;
            GM.maxCards = 2; //(78-6)/4=18
            GM.nbRounds = 2; //(78-6)/4=18
        }

        public void NbPlayers3(){
            Debug.Log("3 joueurs");
            GM.nbPlayers = 3;
            GM.maxCards = 3; //(78-6)/3=24
            GM.nbRounds = 3; //(78-6)/3=24
        }

        public void QuitGame(){
            Application.Quit();
        }
    }
}
