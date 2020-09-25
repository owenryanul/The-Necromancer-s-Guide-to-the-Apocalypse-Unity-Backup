using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Transition_Script : MonoBehaviour
{

    public string sceneToMoveTo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onSceneTransitionPanelAnimationDone()
    {
        SceneManager.LoadSceneAsync(sceneToMoveTo, LoadSceneMode.Single);
    }
}
