using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerControlls playerControlls;
    private CharacterController m_Controller;

    private void Awake()
    {
        playerControlls = new PlayerControlls();
        playerControlls.Enable();
        m_Controller = GetComponent<CharacterController>();
    }
    private void OnDestroy()
    {
        playerControlls.Disable();
    }

    private void Update()
    {
        var input = playerControlls.Input.Movement.ReadValue<Vector2>();
        m_Controller.moveInput = input;
    }
}
