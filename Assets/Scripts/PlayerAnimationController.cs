using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    public GameObject idleCharacter;
    public GameObject rightCharacter;
    public GameObject downCharacter;
    public GameObject leftCharacter;
    public GameObject upCharacter;

    public void PlayIdleAnimation()
    {
        rightCharacter.SetActive(false);
        idleCharacter.SetActive(true);
        animator.Play("Idle");
    }

    public void PlayRightDirectionAnimation()
    {
        idleCharacter.SetActive(false);
        leftCharacter.SetActive(false);
        upCharacter.SetActive(false);
        rightCharacter.SetActive(true);
        animator.Play("Right");
    }
    public void PlayDownDirectionAnimation()
    {
        rightCharacter.SetActive(false);
        downCharacter.SetActive(true);
        animator.Play("Down");
    }

    public void PlayLeftDirectionAnimation()
    {
        downCharacter.SetActive(false);
        leftCharacter.SetActive(true);
        animator.Play("Left");
    }


    public void PlayUpDirectionAnimation()
    {
        leftCharacter.SetActive(false);
        upCharacter.SetActive(true);
        animator.Play("Up");
    }
}
