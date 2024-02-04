using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Managers
{
    public class ElysiumSceneManager : MonoBehaviourSingletonPersistent<ElysiumSceneManager>
    {
        [SerializeField] private Image blackFader;
        //[SerializeField] private GameObject loadingScreenBlocker;

        [SerializeField] private float fadeDuration = 0.25f;

        private EventSystem eventSystem = EventSystem.current;
        
        IEnumerator LoadSceneAsync(int sceneIndex)
        {

            yield return FadeBlack();

            SceneManager.LoadScene(sceneIndex);
        }

        public void ChangeSceneWhenLoaded(Scenes sceneName)
        {
            StartCoroutine(LoadSceneAsync((int)sceneName));
        }

        private IEnumerator FadeBlack()
        {
            blackFader.enabled = true;
            yield return null;
        }

        public void ResetFader()
        {
            blackFader.color = Color.clear;
            blackFader.enabled = false;
        }
        
        private IEnumerator FadeClear(UnityAction OnComplete=null)
        {
            blackFader.enabled = true;
            yield return null;
        }
        
        

        public string GetSceneName(Scenes scene)
        {
            return scene.ToString();
        }
        
        public string GetActiveSceneName()
        {
            return GetSceneName((Scenes)SceneManager.GetActiveScene().buildIndex);
        }

        public Scenes GetActiveScene()
        {
            return (Scenes)SceneManager.GetActiveScene().buildIndex;
        }

        public bool IsOutOfGameScenes()
        {
            return IsActiveScene(Scenes.LevelSelect) || IsActiveScene(Scenes.SplashScreen);
        }

        public bool IsActiveScene(Scenes scene)
        {
            return GetActiveScene() == scene;
        }

        public void DisableInputSystem(bool isDisable)
        {
            if (eventSystem == null)
            {
                eventSystem = EventSystem.current;
            }
            if (eventSystem != null)
            {
                eventSystem.enabled = !isDisable;
            }
            else
            {
                StartCoroutine(TryUnstucking());
            }
        }

        IEnumerator TryUnstucking()
        {
            while (eventSystem == null)
            {
                eventSystem = EventSystem.current;
                if (eventSystem != null)
                {
                    eventSystem.enabled = true;
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        // public void PopBlockLoader(bool isOn)
        // {
        //     loadingScreenBlocker.SetActive(isOn);
        // }
        //
        // public bool IsBlockLoaderOn()
        // {
        //     return loadingScreenBlocker.activeInHierarchy;
        // }
    }

    public enum Scenes
    {
        SplashScreen = 0,
        LevelSelect = 1,
    }
}