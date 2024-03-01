using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

    public class PopupStackElement
    {
        public PopupType type;
        public PopupState state;
        public GameObject gameObject;
        private PopupBase poupBase;
        public Action onCloseCallback;
        public Action onShowCallback;
        public Action onHideCallback;
        public object[] param;
        public PopupStackElement popupBeforeMe; //hack - only is used if we have 2 pops at the same time

        public PopupStackElement(PopupType t, GameObject gO, object[] p, Action onCloseCallback = null, Action onShowCallback = null, Action onHideCallback = null, PopupState s = PopupState.INACTIVE)
        {
            type = t;
            gameObject = gO;
            state = s;
            param = p;
            this.onCloseCallback = onCloseCallback;
            this.onHideCallback = onHideCallback;
            this.onShowCallback = onShowCallback;
        }

        public void Show(bool now = false)
        {
            switch (state)
            {
                case PopupState.INACTIVE:
                    poupBase = PopupManager.Instance.InstantiatePopup(type, param, false, onCloseCallback, onShowCallback, onHideCallback);
                    gameObject = poupBase.gameObject;
                    break;
                case PopupState.ACTIVE:
                    return;
                case PopupState.HIDDEN:
                    poupBase.gameObject.SetActive(true);
                    poupBase.Open();
                    break;
            }
            
            if (popupBeforeMe!=null)
            {
                PopupManager.OnPopupFocusChanged?.Invoke(popupBeforeMe.type, false, 0);
            }
            state = PopupState.ACTIVE;
            PopupManager.OnPopupFocusChanged?.Invoke(type, true, poupBase.uiInteractionDelay);

                
        }

        public void Hide() //fake hide
        {
            poupBase.Close(false);
            state = PopupState.HIDDEN;
            PopupManager.OnPopupFocusChanged?.Invoke(type, false,poupBase.uiInteractionDelay);
        }

        public void DestroyElement() //this closes the popup
        {
            if (poupBase != null)
            {
                PopupManager.OnPopupFocusChanged?.Invoke(type, false,poupBase.uiInteractionDelay);
                MonoBehaviour.Destroy(poupBase.gameObject);
            }
            
            if (popupBeforeMe!=null)
            {
                PopupManager.OnPopupFocusChanged?.Invoke(popupBeforeMe.type, true, 0);
            }
        }

        public void Close()
        {
            if (poupBase != null)
            {
                poupBase.Close();
            }
        }
    }
