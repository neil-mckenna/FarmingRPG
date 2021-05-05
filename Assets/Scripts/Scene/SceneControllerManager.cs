using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading = false;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;

    private IEnumerator Fade(float finalAlpha)
    {
        // Set the fading to flag to true so the FadeAndSwitchScenes coroutine won't be called again.
        isFading = true;

        // Make sure the CanvasGroup blocks raycasts into the scene so no more input can be accpeted.
        faderCanvasGroup.blocksRaycasts = true;

        // Calculate how fast the CanvaGroup shoudl fade based on it's current alpha, it's final alpha and how long it has to change between teh two.
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // While the cnavas grouphasn't reached the final alpha yet ...
        while(!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            // move the apha towards it's target alpha
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            // wait for a frame then continue
            yield return null;
        }

        // Set the flag false since the fade has finished
        isFading = false;

        // Stop the CanvasGroup from blocking raycasts so input is no longer ignored
        faderCanvasGroup.blocksRaycasts = false;
    }

    // this is the coroutine where the 'building blocks' of teh script are put together.
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        // call before scene unloads fade out event
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // Start fading to black and wait for it to finish before continuing
        yield return StartCoroutine(Fade(1f));

        // Set player position
        Player.Instance.gameObject.transform.position = spawnPosition;

        // Call before scene unload event
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // Start loading the given scene and wait for it to finish
        yield return StartCoroutine(LoadSceneAndActive(sceneName));

        // Call after scene load event
        EventHandler.CallAfterSceneLoadEvent();

        // Start fading back in and wait for it to finish before exiting the function.
        yield return StartCoroutine(Fade(0f));

        // Call after scene load fade event
        EventHandler.CallAfterSceneLoadFadeInEvent();

    } 

    private IEnumerator LoadSceneAndActive(string sceneName)
    {
        // Allow the given scene to load over several frames and add it to the already loaded scenes (just the Persistent scene at this point).
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Find the scene that was most recently loaded (the one at the last index)
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // Set the newly loaded scene as the active scene (this marks it as the one to be unloaded next).
        SceneManager.SetActiveScene(newlyLoadedScene);
    }


    // This is the main method external point point of contact and influence from the rest of the project.
    // This will be called when the player wants to switch scenes.

    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        // if fade isn't happeningthen start fading and switching scenes
        if(!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }

    }

    // loads first scene of the game
    private IEnumerator Start() 
    {
        // Set the initial alpha to start off with a black screen.
        faderImage.color = new Color(0f, 0f, 0f, 1f);
        faderCanvasGroup.alpha = 1f;

        // Start the first scene loading and wait for it to finish
        yield return StartCoroutine(LoadSceneAndActive(startingSceneName.ToString()));

        // If eventhandler has any subscribers call it.
        EventHandler.CallAfterSceneLoadEvent();

        // Once the scene is finished loading, start fading in
        StartCoroutine(Fade(0f));

    }

    
}
