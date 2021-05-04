using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] SO_AnimationTypeArray = null;

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    private AnimatorOverrideController aoc;

   
    void Start()
    {
        aoc = new AnimatorOverrideController();
        // Initilaise animation type dictionary keyed by clip
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();

        foreach(SO_AnimationType item in SO_AnimationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }

        // Initialise animation type dictionary by string
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        foreach(SO_AnimationType item in SO_AnimationTypeArray)
        {
            string key = item.characterPartAnimator.ToString() + item.PartVariantColour.ToString() + item.PartVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }         
    }

    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributesList)
    {
        // Stopwatch s1 = Stopwatch.StartNew();

        // Loop through all attributes and set the animation override controller for each
        foreach(CharacterAttribute characterAttribute in characterAttributesList)
        {
            
            Animator currentAnimator = null;
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairsList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            // arms e.g. 
            string animatorSOAssetName = characterAttribute.characterPart.ToString();

            // Find Animators in scene that match scriptable object animator
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();

            foreach(Animator animator in animatorsArray)
            {
                if(animator.name.ToLower() == animatorSOAssetName)
                {
                    
                    currentAnimator = animator;
                    break;
                }
                else
                {
                    //Debug.Log("animator name " + animator.name + " asset passed: " + animatorSOAssetName);

                }
            }

            //Debug.LogWarning("current anim is: " + currentAnimator.name);

            // Get base current animations for animator
            aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);

            foreach(AnimationClip animationClip in animationsList)
            {
                // find animation in directionary
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);

                if(foundAnimation)
                {
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColour.ToString() + characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();

                    SO_AnimationType swapSO_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    if(foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;

                        animsKeyValuePairsList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));

                    }
                }
            }

            // Apply animation updates to animation ovveride controller and then update with new controller
            aoc.ApplyOverrides(animsKeyValuePairsList);
            
            currentAnimator.runtimeAnimatorController = aoc;

        }

        // Stop


    }

}
