using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class Popuper : MonoBehaviour
{
    public enum PopupDirection
    {
        None,
        Top, 
        Bottom,
        Left,
        Right,
        Fade
    }

    [Range(0,1)] [SerializeField] private float faderTransparency = 0.8f;
    public PopupDirection popupIn = PopupDirection.Bottom;
    public PopupDirection poupOut = PopupDirection.Bottom;
    //for curve style velocity cant be above 1
    public AnimationCurve transitionTimeCurve = new AnimationCurve(new Keyframe(0,0), new Keyframe(1,1));
    public UnityEvent OnEntryTransition;
    public UnityEvent OnExitTransition;
    private Vector2 screen =>PopupManager.Instance.rectTransform.sizeDelta;
    private bool inTransition; //this is internal check that this object uses to detect if a transition is active
    public void DoTransitionIn()
    {
        switch (popupIn)
        {
            case PopupDirection.None:
                Blip(true);
                break;
            case PopupDirection.Top:
            case PopupDirection.Bottom:
            case PopupDirection.Left:
            case PopupDirection.Right:
                StartCoroutine(Translation(true));
                break;
            case PopupDirection.Fade:
                StartCoroutine(Fade(true));
                break;
            default:
                Blip(true);
                break;
        }
    }

    public void DoTransitionOut()
    {

        switch (poupOut)
        {
            case PopupDirection.None:
                Blip(false);
                break;
            case PopupDirection.Top:
            case PopupDirection.Bottom:
            case PopupDirection.Left:
            case PopupDirection.Right:
                StartCoroutine(Translation(false));
                break;
            case PopupDirection.Fade:
                StartCoroutine(Fade(false));
                break;
            default:
                Blip(false);
                break;
        }
    }

    private IEnumerator Translation(bool isIn)
    {
        RegisterTransition();
        GetComponent<CanvasGroup>().alpha = 1;
        RectTransform rTransform = GetComponent<RectTransform>();
        rTransform.localScale = Vector3.one;
        float time = transitionTimeCurve[transitionTimeCurve.length-1].time;
        float elapsedTime = 0;
        
        // if (isIn)
        //     PopupManager.Instance.fader.enabled = true;
        
        float fadeAlphaFrom = isIn ? 0 : faderTransparency;
        float fadeAlphaTo = isIn ? faderTransparency : 0;
        
        Vector2 anchoredStart = isIn ? CalculateCoordinates(popupIn) : Vector2.zero;
        Vector2 anchoredEnd = isIn? Vector2.zero : CalculateCoordinates(poupOut);
        
        while (elapsedTime<=time)
        {
            float t = transitionTimeCurve.Evaluate(elapsedTime);
            rTransform.anchoredPosition = Vector2.LerpUnclamped(anchoredStart, anchoredEnd, t);
            PopupManager.Instance.fader.color = Color.Lerp(new Color(0, 0, 0, fadeAlphaFrom), new Color(0, 0, 0, fadeAlphaTo), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rTransform.anchoredPosition = anchoredEnd;
        PopupManager.Instance.fader.color = new Color(0,0,0,fadeAlphaTo);

        // if (!isIn)
        //     PopupManager.Instance.fader.enabled = false;
        
        UnregisterTransition();
        
        if (isIn)
            OnEntryTransition?.Invoke();
        else
            OnExitTransition?.Invoke();

    }

    private IEnumerator Fade(bool isIn)
    {
        RegisterTransition();
        CanvasGroup cGroup = GetComponent<CanvasGroup>();
        RectTransform rTransform = GetComponent<RectTransform>();
        rTransform.anchoredPosition = Vector2.zero;
        float time = transitionTimeCurve[transitionTimeCurve.length-1].time;
        float elapsedTime = 0;
        
        // if (isIn)
        //     PopupManager.Instance.fader.enabled = true;
        
        float fadeAlphaFrom = isIn ? 0 : faderTransparency;
        float fadeAlphaTo = isIn ? faderTransparency : 0;
        
        Vector3 scaleStart = isIn ? Vector3.one*0.8f : Vector2.one;
        Vector3 scaleEnd = isIn? Vector2.one : Vector3.one*0.8f;

        float popTransparencyStart = isIn ? 0 : 1;
        float popTransparencyEnd = isIn ? 1 : 0;

        while (elapsedTime <= time)
        {
            float t = transitionTimeCurve.Evaluate(elapsedTime);
            rTransform.localScale = Vector3.LerpUnclamped(scaleStart, scaleEnd, t);
            cGroup.alpha = Mathf.Lerp(popTransparencyStart, popTransparencyEnd, t);
            PopupManager.Instance.fader.color = Color.Lerp(new Color(0, 0, 0, fadeAlphaFrom), new Color(0, 0, 0, fadeAlphaTo), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rTransform.localScale = scaleEnd;
        cGroup.alpha = popTransparencyEnd;
        PopupManager.Instance.fader.color = new Color(0,0,0,fadeAlphaTo);
        
        // if (!isIn)
        //     PopupManager.Instance.fader.enabled = false;
        
        UnregisterTransition();
        
        if (isIn)
            OnEntryTransition?.Invoke();
        else
            OnExitTransition?.Invoke();


    }

    private void Blip(bool isIn)
    {
        CanvasGroup cGroup = GetComponent<CanvasGroup>();
        RectTransform rTransform = GetComponent<RectTransform>();
        rTransform.anchoredPosition = Vector2.zero;
        rTransform.localScale = Vector3.one;
        cGroup.alpha = isIn ? 1 : 0;

        PopupManager.Instance.fader.color = new Color(0, 0, 0, isIn ? faderTransparency : 0f);
        //PopupManager.Instance.fader.enabled = isIn;
        
        if (isIn)
            OnEntryTransition?.Invoke();
        else
            OnExitTransition?.Invoke();
        
    }


    private Vector2 CalculateCoordinates(PopupDirection direction)
    {
        Vector2 result = new Vector2();

        switch (direction)
        {
            case PopupDirection.Top:
                result = new Vector2(0, screen.y);
                break;
            case PopupDirection.Bottom:
                result = new Vector2(0, -screen.y);
                break;
            case PopupDirection.Left:
                result = new Vector2(-screen.x, 0);
                break;
            case PopupDirection.Right:
                result = new Vector2(screen.x, 0);
                break;
        }

        return result;
    }

    private void RegisterTransition()
    {
        inTransition = true;
        PopupManager.TransitionsInProgress++;
    }

    private void UnregisterTransition()
    {
        inTransition = false;
        PopupManager.TransitionsInProgress=PopupManager.TransitionsInProgress>0?  PopupManager.TransitionsInProgress-1 : 0;
    }

    private void OnDestroy()
    {
        if (inTransition)
        {
            //failsafe if unexpected destruction happens
            Debug.LogError("forced transition interuption");
            UnregisterTransition();
        }
    }
}
