using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private static readonly int Land = Animator.StringToHash("Land");
    private static readonly int WallHold = Animator.StringToHash("WallHold");
    private Vector3 _startingScale;

    private void Start()
    {
        _startingScale = transform.localScale;
    }

    public void PlayJumpAnimation()
    {
        animator.SetTrigger(Jump);
    }
    
    public void PlayFallAnimation()
    {
        animator.SetTrigger(Fall);
    }
    
    public void PlayLandAnimation()
    {
        animator.SetTrigger(Land);
    }
    
    public void PlayWallHoldAnimation()
    {
        animator.SetTrigger(WallHold);
    }

    public void RotatePlayer(bool direction)
    {
        var newScaleX = direction ? -_startingScale.x : _startingScale.x;
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
    }

    public void ResetAllTriggers()
    {
        animator.ResetTrigger(Jump);
        animator.ResetTrigger(Fall);
        animator.ResetTrigger(Land);
        animator.ResetTrigger(WallHold);
    }
}
