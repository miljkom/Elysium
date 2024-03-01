using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


    public class PopupManager : MonoBehaviourSingletonPersistent<PopupManager>
    {
        private const string popupFilePath = "Popups/";

        public GameObject rootElement;
        public Image fader;
        [SerializeField] private Button faderButton;
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasTabletRescale tabletRescale;
        public RectTransform rectTransform;

        PopupStack popupStack = new PopupStack();

        public List<KeyValuePair<PopupType, GameObject>> openedPopups = new List<KeyValuePair<PopupType, GameObject>>();


        public Dictionary<PopupType, GameObject> allPopups = new Dictionary<PopupType, GameObject>();

        public static UnityAction<PopupType, bool, float> OnPopupFocusChanged;
        public static int TransitionsInProgress;
        
        public bool backButtonGloballyDisabled = false;

        private new void Awake()
        {
            base.Awake();
            Canvas newCanvas = Instance.canvas;
            newCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            newCanvas.worldCamera = GameObject.FindWithTag("UiCamera")?.GetComponent<Camera>()?? Camera.main; //ovo treba da se sredi pametnije
            newCanvas.planeDistance = 2;
            newCanvas.sortingLayerName = "Popup";
            newCanvas.sortingOrder = 1;
            OnPopupFocusChanged += OnPopupFocus;
        }

        private void Start()
        {
            Debug.LogError("Popup manager START!");
            SceneManager.sceneLoaded += (Scene scn, LoadSceneMode mode) => {tabletRescale.ReInit();};
        }

        private void OnPopupFocus(PopupType type, bool focus, float uiDelay)
        {
            fader.enabled = IsAnyPopupOpened();
            if (ElysiumSceneManager.Instance.IsActiveScene(Scenes.LevelSelect))
            {
                //todo pause game
                //GameManager.Instance.PauseGame(IsAnyPopupOpened());
            }

            if (type==PopupType.None)
            {
                faderButton.interactable = false;
            }
        }

        void Update()
        {
            //BackButtonControl();
        }

        private void OnDestroy()
        {
            OnPopupFocusChanged -= OnPopupFocus;
        }

        /*private void BackButtonControl()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (TransitionsInProgress>0 || backButtonGloballyDisabled)//backButtonGloballyDisabled)
                {
                    return;
                }
                if (CloseLastPopup())
                {
                    return;
                }
                else
                {
                    if (GameManager.Instance.overideBackButton)
                    {
                        DebugLogger.Log(LoggingTopic.UI, "BackButtonControl: overideBackButton");
                        return;
                    }
                    
                    switch (SceneManager.GetActiveScene().name)
                    {
                        //TODO - ne znam zasto nece da mi pristupi enumu "Scenes"...za sada sam preskocio
                        //case NonoSceneManager.Instance.GetSceneName(Scenes.LevelSelect):
                        case "GameScene":
                            DebugLogger.Log(LoggingTopic.UI, "BackButtonControl: GameScene");
                            if(GameManager.Instance.isIngameOptionsShowing)
                            {
                                InGameOptionsUi.Instance.BackButtonCloseOptions();
                            }
                            else if (!GameManager.Instance.IsTutorial && GameManager.Instance.boosterActive != Consumable.None)
                            {
                                BoosterMenuUi.Instance.ActivateBooster(Consumable.None);
                            }
                            else if (GameManager.Instance.IsEndlessMode)
                            {
                                PopupManager.Instance.ShowPopupLater(PopupType.EndlessGamePause);
                            }
                            else
                            {
                                PopupManager.Instance.ShowPopupLater(PopupType.GamePause);
                            }
                            return;
                        case "GameSceneNonodoku":
                            DebugLogger.Log(LoggingTopic.UI, "BackButtonControl: GameSceneNonodoku");
                            if (GameManager.Instance.isIngameOptionsShowing)
                            {
                                InGameOptionsUi.Instance.BackButtonCloseOptions();
                            }
                            else
                            {
                                ShowPopupNow(PopupType.NonodokuGamePause);
                            }
                            return;
                        case "IslandsLevels":
                            DebugLogger.Log(LoggingTopic.UI, "BackButtonControl: IslandsLevels");
                            ShowPopupNow(PopupType.GameQuit);
                            return;
                        default:
                            DebugLogger.Log(LoggingTopic.UI, "BackButtonControl: " + SceneManager.GetActiveScene().name);
                            return;
                    }
                }
            }
        }*/

        private GameObject GetPopupPrefab(PopupType popupname)
        {
            if (!allPopups.ContainsKey(popupname))
                allPopups.Add(popupname, Resources.Load<GameObject>(GetPopupResourcePath(popupname)));

            return allPopups[popupname];
        }

        private void LoadPopup(PopupType popupname)
        {
            if (!allPopups.ContainsKey(popupname))
                allPopups.Add(popupname, Resources.Load<GameObject>(GetPopupResourcePath(popupname)));
        }

        private string GetPopupResourcePath(PopupType popupname)
        {
            return "Popup/" + popupname.ToString() + "Popup";
        }

        bool CloseLastPopup()
        {
            PopupStackElement popupElement = popupStack.Peek();
            if (popupElement == null) return false;
            if (popupElement.gameObject == null) return false;
            PopupBase popup = popupElement.gameObject.GetComponent<PopupBase>();
            if (popup == null) return false;
            if (!popup.canBeClosed)
            {
                popup.BackButtonClose();
                return true;
            }
            //TODO - proveri da li moze Clode umesto Hide
            popup.Close();
            //popup.Hide();
            return true;
        }

        public void ClosePopupByType(PopupType popupType)
        {
            PopupStackElement elementToRemove = popupStack.Remove(popupType);
            elementToRemove?.DestroyElement();
            ShowPopupFromTop();
        }

        /// <summary>
        /// Destroy popup from stack
        /// </summary>
        /// <param name="type"></param>
        public void RemovePopupFromStack(PopupType type)
        {
            PopupStackElement popupElement = popupStack.Remove(type);
            if (popupElement == null)
                return;
            popupElement.DestroyElement();
        }

        public void HidePopup() //PopupType popupType)//uvek se zatvara poslednji popup!
            //popupType tu samo zbog debagovanja
        {
            PopupStackElement popupElement = popupStack.Pop();
            if (popupElement == null)
                return;
            popupElement.DestroyElement();
            ShowPopupFromTop();
        }

        /// <summary>
        /// A function that finds an active popup in the stack, by type, and destroys it
        /// </summary>
        public void DestroyPopup(PopupType popupType)
        {
            PopupStackElement popupElement = popupStack.Remove(popupType);
            if (popupElement != null)
            {
                Destroy(popupElement.gameObject);
            }
        }

        public void HideAllPopups()
        {
            //PopupStackElement pse = popupStack.Remove(PopupType.PopupLoading);
            popupStack.ClearAndDestroy();
            //if (pse != null) popupStack.Push(pse);
        }

        public void CloseAllPopups()
        {
            popupStack.CloseAll();
        }

        public void RemoveQueuedPopups()
        {
            popupStack.RemoveQueuedPopups();
        }

        public IEnumerator CloseAllPopups(Action callbackOnAllClosed)
        {
            yield return popupStack.CloseAllCoroutine();
            callbackOnAllClosed?.Invoke();
        }

        public void DeactivateAllPopups()
        {
            popupStack.DeactivateAllPopups();
        }

        public bool IsAnyPopupOpened()
        {
            if (popupStack.Count() > 0) return true;
            else return false;
        }
        
        public PopupType GetCurrentPopupType()
        {
            if (popupStack.Count() > 0)
            {
                PopupStackElement currentPopup = popupStack.Peek();
                if (currentPopup != null)
                {
                    return currentPopup.type;
                }
            }

            return PopupType.None;
        }

        public bool IsPopupOpened(PopupType popupType)
        {
            return popupStack.Contains(popupType);
        }
        
        public void ShowPopupNow(PopupType popupType, object[] param = null, Action onCloseCallback = null, Action onShowCallback = null, Action onHideCallback = null, bool showOverCurrentPopup = false)
        {
            PopupStackElement popupOnTopOfStack = popupStack.Peek(); //already shown popup if exists 
            PopupStackElement popupElement = popupStack.Remove(popupType) ?? new PopupStackElement(popupType, null, param, onCloseCallback, onShowCallback, onHideCallback); //new popup that we want to show

            if (!showOverCurrentPopup)
            {
                if (popupOnTopOfStack != null && popupOnTopOfStack.type != popupType && popupOnTopOfStack.state == PopupState.ACTIVE)
                    popupOnTopOfStack.Hide();
            }
            else
            {
                if (popupOnTopOfStack != null && popupOnTopOfStack.type != popupType &&
                    popupOnTopOfStack.state == PopupState.ACTIVE)
                {
                    popupElement.popupBeforeMe = popupOnTopOfStack;
                }
            }
           
            popupStack.Push(popupElement);

            switch (popupElement.state)
            {
                case PopupState.ACTIVE:
                    return;
                case PopupState.INACTIVE:
                case PopupState.HIDDEN:
                    popupElement.Show();
                    break;
            }
        }

        public void ShowPopupLater(PopupType popupType, object[] param = null, Action onCloseCallback = null, Action onShowCallback = null, Action onHideCallback = null, bool showOverCurrentPopup = false)
        {
            PopupStackElement popupElement = popupStack.Peek();
            if (popupElement == null)
            {
                ShowPopupNow(popupType, param, onCloseCallback, onShowCallback, onHideCallback, showOverCurrentPopup);
                return;
            }

            popupElement = new PopupStackElement(popupType, null, param, onCloseCallback, onShowCallback, onHideCallback);
            popupStack.Insert(popupElement);
        }
        private void ShowPopupFromTop()
        {
            PopupStackElement popupOnTopOfStack = popupStack.Peek();
            if (popupOnTopOfStack == null)
            {
                PopupManager.OnPopupFocusChanged?.Invoke(PopupType.None, true, 0f);
                return;
            }
            popupOnTopOfStack.Show();
        }

        public PopupBase InstantiatePopup(PopupType popupType, object[] param, bool newPopupType = false,
            Action onCloseCallback = null, Action onShowCallback = null, Action onHideCallback = null)
        {
            GameObject popupPrefab = GetPopupPrefab(popupType);
            GameObject tempPopup = Instantiate(popupPrefab, new Vector3(20f, 0f, 0f), Quaternion.identity  ,canvas.transform);
            tempPopup.transform.localPosition = new Vector3(tempPopup.transform.localPosition.x, tempPopup.transform.localPosition.y, 0f);
            RectTransform rTransform = tempPopup.GetComponent<RectTransform>();
            rTransform.anchorMin = new Vector2(0f, 0f);
            rTransform.anchorMax = new Vector2(1f, 1f);
            rTransform.sizeDelta = Vector3.zero;
            PopupBase basePop = tempPopup.GetComponent<PopupBase>();
            basePop.OnShow(param, onCloseCallback, onShowCallback, onHideCallback);

            return basePop;
        }
        public void AddButtonOnFader(PopupBase popup)
        {
            faderButton.interactable = false;
            faderButton.onClick.RemoveAllListeners();
            if (popup.closeButton != null)
            {
                faderButton.onClick.AddListener(delegate
                {
                    if (popup.closeButton.interactable)
                        popup.closeButton.onClick.Invoke();
                });
                faderButton.interactable = true;
            }
        }

        public void CancelAllQueues()
        {
            PopupManager.Instance.RemoveQueuedPopups();
        }

        private int disableCounter = 0;

        private Coroutine EnablingBackButton;

        public void DisableBackButton(float duration)
        {
            //if not already in progress, disable
            if (EnablingBackButton == null)
            {
                backButtonGloballyDisabled = true;
            
                EnablingBackButton = StartCoroutine(EnableBackButton(duration));
            }
        }

        public IEnumerator EnableBackButton(float duration)
        {
            yield return new WaitForSeconds(duration);
            backButtonGloballyDisabled = false;
            EnablingBackButton = null;
        }
    }