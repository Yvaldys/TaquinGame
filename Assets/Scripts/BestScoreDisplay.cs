using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestScoreDisplay : MonoBehaviour
{
    #region Exposed
    
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        // update the score on the Home page
        BestTime bestTime = SaveSystem.LoadBestTime();
        if (bestTime != null) {
            this.GetComponent<Text>().text = "Current best score : " + Utils.FormatTime(bestTime.bestTime);
        } else {
            this.GetComponent<Text>().text = "No current best score";
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    #endregion

    #region Main methods
    
    #endregion

    #region Private & Protected
    
    #endregion
}
