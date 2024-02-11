using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
    public abstract class PopupBase : MonoBehaviour
    {
        [HideInInspector]
        public abstract PopupType popupType { get; }

        //protected bool isClosing;
        [HideInInspector]
        public Popuper popuper;

        public bool canBeClosed = true;

        public Button closeButton;
        public float uiInteractionDelay;
        
        public Action onCloseCallback;
        public Action onHideCallback;
        private bool destroyPopup = true;

        public abstract void AfterOnSnow(object[] param);

        private void Awake()
        {
            popuper = GetComponent<Popuper>();

            if (popuper)
            {
                popuper.OnExitTransition.AddListener(Hide);
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }

            PopupManager.OnPopupFocusChanged += OnFocusChanged;
        }

        public void OnShow(object[] param, Action onCloseCallback = null, Action onShowCallback = null, Action onHideCallback = null)
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(delegate {Close();});
            this.onCloseCallback = onCloseCallback;
            this.onHideCallback = onHideCallback;
            onShowCallback?.Invoke(); //should this go after AfterOnShow?
            AfterOnSnow(param);
            Open();
        }

        public virtual void Open()
        {
            if (popuper)
            {
                popuper.DoTransitionIn();
            }

            destroyPopup = true;//reset value here
        }
        
        public virtual void Close(bool shouldDestroy = true)
        {
            if (PopupManager.TransitionsInProgress > 0)
                return;

            destroyPopup = shouldDestroy;

            if (destroyPopup)
            {
                onCloseCallback?.Invoke();
                PlaySound("PopupClose");
            }
            
            if (popuper)
            {
                popuper.DoTransitionOut();
            }
            else
            {
                Hide();
            }
        }

        
        public void PlaySound(string clipName)
        {
            //TODO - add call for plaing sound
        }

        public void Hide()
        {
            if (destroyPopup)
            {
                PopupManager.Instance.ClosePopupByType(popupType);
                
                onHideCallback?.Invoke();
            }
            else
            {
                gameObject.SetActive(false);
            }

        }
        
        protected virtual void OnFocusChanged(PopupType type, bool isIn, float uiDelay)
        {
            if (popupType==type)
            {
                if (isIn)
                {
                    //focused
                    OnFocus();
                }
                else
                {
                    //defocused
                    OnDefocus();
                } 
            }

        }

        protected virtual void OnFocus()
        {
            //DebugLogger.LogError(LoggingTopic.Popup , $"On Focus {popupType}");
            PopupManager.Instance.AddButtonOnFader(this);
        }

        protected virtual void OnDefocus()
        {
            //DebugLogger.LogError(LoggingTopic.Popup ,$"On DeFocus {popupType}");
        }

        public virtual void BackButtonClose()
        {
            //DebugLogger.LogError(LoggingTopic.Popup ,$"Closed by BackButton {popupType}");
        }
        
        
        void OnDestroy()
        {
            Delegate[] subscribers;
            //remove all onCloseCallback delegates
            if (onCloseCallback != null)
            {
                subscribers = onCloseCallback.GetInvocationList();
                for(int i = 0; i < subscribers.Length; i++)
                {    
                    onCloseCallback -= subscribers[i] as Action;
                }
            }

            //remove all onHideCallback delegates
            if (onHideCallback != null)
            {
                subscribers = onHideCallback.GetInvocationList();
                for(int i = 0; i < subscribers.Length; i++)
                {    
                    onHideCallback -= subscribers[i] as Action; 
                }
            }
            
            PopupManager.OnPopupFocusChanged -= OnFocusChanged;
        }
    }
