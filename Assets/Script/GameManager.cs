using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tARot{
    /// <summary>
    /// The GameManager class is a class that keeps track of the state the game is in. 
    /// It keeps track of the number of players, turns, maximum cards of the player, his cards as well as whether the game has already been finished once or not.
    /// </summary>
    public class GameManager : MonoBehaviour{
        protected GameManager() { }
        public static GameManager instance = null;
        public static GameManager Instance{
            get
            {
                if (!instance){
                    instance = FindObjectOfType(typeof(GameManager)) as GameManager;
                    if (!instance){
                        Debug.LogError("There needs to be one active GameManager script on a GameObject in your scene.");
                    }
                }
                return instance;
            }
        }
        void Awake(){
            instance = this;
            DontDestroyOnLoad(this);
        }

        public void OnApplicationQuit(){
            GameManager.instance = null;
        }

        public int nbPlayers = 0;
        public int maxCards = 0;
        public int nbRounds = 0;
        public HashSet<Card> cards = new HashSet<Card>();
        public bool firstGame = true;
    }
}