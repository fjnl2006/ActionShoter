using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class DiabloInput : MonoBehaviour
{
    NavMeshAgent DiabloInputActions;
    DiabloInputActions inputActions;

    private InputAction m_moveAgent;
    
    Vector3 m_moveDirection = new Vector3();

    private void Awake()
    {
        DiabloInputActions = GetComponent<NavMeshAgent>();
        inputActions = new DiabloInputActions();
        inputActions.Hola.Enable();
        
        m_moveAgent = inputActions.Hola.Caracola;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_moveDirection = Mouse.current.position.value;
        
        if(m_moveAgent.WasPressedThisFrame())
        {
            MoveTo();
            Debug.Log("Pressed");
        }
    }

    void MoveTo()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(m_moveDirection), out hit, 100))
        {
            DiabloInputActions.destination = hit.point;    
        }
    }
}
