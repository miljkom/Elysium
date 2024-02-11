using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    public class PopupStack : MonoBehaviour
    {
        List<PopupStackElement> popupList;

        public PopupStack()
        {
            popupList = new List<PopupStackElement>();
        }
        public bool IsEmpty()
        {
            return (popupList.Count > 0);
        }
        public int Count()
        {
            return popupList.Count;
        }
        public void Push(PopupStackElement popupElement)
        {
            popupList.Add(popupElement);
        }

        public void Insert(PopupStackElement popupElement)
        {
            popupList.Insert(0, popupElement);
        }

        public PopupStackElement Pop()
        {
            if (popupList.Count <= 0) return null;
            PopupStackElement el = popupList.Last();
            popupList.Remove(el);
            return el;
        }
        public PopupStackElement Peek()
        {
            if (popupList.Count <= 0) return null;
            PopupStackElement el = popupList.Last();
            return el;
        }
        public void ClearAndDestroy()
        {
            while (popupList.Count > 0)
                Destroy(Pop().gameObject);
        }
        
        public void RemoveQueuedPopups()
        {
            if (popupList.Count > 0)
            {
                for (int i = popupList.Count - 1; i >= 0; i--)
                {
                    if (popupList[i].state != PopupState.ACTIVE)
                    {
                        PopupStackElement el = popupList[i];
                        Destroy(el.gameObject);
                        popupList.Remove(el);
                    }
                }
            }
        }
        public bool Contains(PopupType type)
        {
            foreach (PopupStackElement element in popupList)
                if (element.type == type) return true;
            return false;
        }
        public PopupStackElement GetIfExists(PopupType type)
        {
            foreach (PopupStackElement element in popupList)
                if (element.type == type) return element;
            return null;
        }
        public PopupStackElement Remove(PopupType type)
        {
            PopupStackElement popupElement = GetIfExists(type);
            if (popupElement == null) return null;
            popupList.Remove(popupElement);
            return popupElement;
        }
        public void DeactivateAllPopups()
        {
            foreach (PopupStackElement element in popupList)
                if (element.state == PopupState.ACTIVE)
                {
                    element.Hide();
                }
        }

        public void CloseAll()
        {
            for (int i = 0; i < popupList.Count; i++)
            {
                popupList[i].Close();
            }
        }

        public IEnumerator CloseAllCoroutine()
        {
            foreach (PopupStackElement element in popupList)
            {
                element.Close();
            }
            int i = 1;
            while (popupList.Count > 0 && i < 1000)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
