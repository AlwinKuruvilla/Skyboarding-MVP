using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine;

public class Moving3D : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [SerializeField]
    [Tooltip("The Input System Action that will be used to read Move data from the left hand controller. Must be a Value Vector2 Control.")]
    InputActionProperty m_LeftHandMoveAction;
    /// <summary>
    /// The Input System Action that will be used to read Move data from the left hand controller. Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.
    /// </summary>
    public InputActionProperty leftHandMoveAction
    {
        get => m_LeftHandMoveAction;
        set => SetInputActionProperty(ref m_LeftHandMoveAction, value);
    }

    [SerializeField]
    [Tooltip("The Input System Action that will be used to read Move data from the right hand controller. Must be a Value Vector2 Control.")]
    InputActionProperty m_RightHandMoveAction;
    /// <summary>
    /// The Input System Action that will be used to read Move data from the right hand controller. Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.
    /// </summary>
    public InputActionProperty rightHandMoveAction
    {
        get => m_RightHandMoveAction;
        set => SetInputActionProperty(ref m_RightHandMoveAction, value);
    }
    
    [SerializeField]
    [Tooltip("The speed, in units per second, to move forward.")]
    float m_MoveSpeed = 1f;
    /// <summary>
    /// The speed, in units per second, to move forward.
    /// </summary>
    public float moveSpeed
    {
        get => m_MoveSpeed;
        set => m_MoveSpeed = value;
    }
    
    [SerializeField]
    [Tooltip("The source Transform to define the forward direction.")]
    Transform m_ForwardSource;
    /// <summary>
    /// The source <see cref="Transform"/> to define the forward direction.
    /// </summary>
    public Transform forwardSource
    {
        get => m_ForwardSource;
        set => m_ForwardSource = value;
    }
    
    /// <summary>
    /// See <see cref="MonoBehaviour"/>.
    /// </summary>
    protected void OnEnable()
    {
        m_LeftHandMoveAction.EnableDirectAction();
        m_RightHandMoveAction.EnableDirectAction();
    }

    /// <summary>
    /// See <see cref="MonoBehaviour"/>.
    /// </summary>
    protected void OnDisable()
    {
        m_LeftHandMoveAction.DisableDirectAction();
        m_RightHandMoveAction.DisableDirectAction();
    }
    
    void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
    {
        if (Application.isPlaying)
            property.DisableDirectAction();

        property = value;

        if (Application.isPlaying && isActiveAndEnabled)
            property.EnableDirectAction();
    }
}
