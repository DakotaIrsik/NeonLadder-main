using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class OverrideControllerAssigner : MonoBehaviour
{
    private Animator animator;
    private AnimatorOverrideController overrideController;
    private List<KeyValuePair<AnimationClip, AnimationClip>> originalOverrides; // To keep the original overrides
    private bool? lastUnlockedSwordState;

    public bool isUsingOverride { get; private set; } // Public boolean to check the status

    void Start()
    {
        animator = GetComponent<Animator>();
        // Ensure the runtimeAnimatorController is actually an AnimatorOverrideController
        if (animator.runtimeAnimatorController is AnimatorOverrideController)
        {
            overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            originalOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
            overrideController.GetOverrides(originalOverrides);
            isUsingOverride = false; // Assume starting with base animations
            lastUnlockedSwordState = null; // Set to null to ensure the initial check in Update triggers
        }
    }

    private void Update()
    {
        // Check if the state has changed since the last frame
        if (Constants.UnlockedSword != lastUnlockedSwordState)
        {
            if (Constants.UnlockedSword)
            {
                UseOverrideAnimations();
            }
            else
            {
                UseBaseAnimations();
            }
            lastUnlockedSwordState = Constants.UnlockedSword; // Update the last state
        }
    }

    public void UseBaseAnimations()
    {
        if (overrideController != null)
        {
            // Create a list of pairs with null overrides to revert to the base controller's animations
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
            foreach (var pair in originalOverrides)
            {
                overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(pair.Key, null));
            }
            overrideController.ApplyOverrides(overrides);
            isUsingOverride = false; // Update the boolean
        }
    }

    public void UseOverrideAnimations()
    {
        // Reapply the original overrides to use the override animations
        if (overrideController != null)
        {
            overrideController.ApplyOverrides(originalOverrides);
            isUsingOverride = true; // Update the boolean
        }
    }

    private void OnDestroy()
    {
        if (overrideController != null)
        {
            UseOverrideAnimations();
        }
    }
}
