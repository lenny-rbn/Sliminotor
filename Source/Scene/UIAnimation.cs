
using UnityEngine;
using UnityEngine.InputSystem;

public class UIAnimation : MonoBehaviour
{
    public bool isGameStarted = false;

    private InputAction _startGame;
    private PlayerInput input;

    Animator animator;

    private void Start()
    {
        input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        _startGame = input.actions.FindAction("Start");
        _startGame.performed += _ => StartGame();
    }

    public void StartGame()
    {
        if (isGameStarted) return;

        isGameStarted = true;
        animator.SetTrigger("GameStart");
    }
    
}
