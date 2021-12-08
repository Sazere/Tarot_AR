using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tARot{
    public class buttonPlayers : MonoBehaviour{
        void Start(){
            gameObject.GetComponent<Button>().interactable = false;
        }

    }
}
