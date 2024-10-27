using UnityEngine;

public class HorseAnimator : MonoBehaviour
{
    private Animator animator;
    private Horse horse;

    void Start()
    {
        animator = GetComponent<Animator>();
        horse = GetComponent<Horse>();
    }

    void Update()
    {
        // Update animation based on the horse's movement
        switch (horse.currentDirection)
        {
            case Horse.Direction.Right:
                animator.Play("RunRight");
                horse.playerAnimationController.PlayRightDirectionAnimation();
                break;
            case Horse.Direction.Down:
                animator.Play("RunDown");
                horse.playerAnimationController.PlayDownDirectionAnimation();
                break;
            case Horse.Direction.Left:
                animator.Play("RunLeft");
                horse.playerAnimationController.PlayLeftDirectionAnimation();
                break;
            case Horse.Direction.Up:
                animator.Play("RunUp");
                horse.playerAnimationController.PlayUpDirectionAnimation();
                break;
            case Horse.Direction.Idle:
                animator.Play("Idle");
                horse.playerAnimationController.PlayIdleAnimation();
                break;
        }
    }
}
