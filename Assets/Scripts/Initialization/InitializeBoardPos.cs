using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeBoardPos : MonoBehaviour
{
    [SerializeField] private Transform _xrCamPos;
    [SerializeField] private float _playerHeightOffset = 1.36144f;
    [SerializeField] private Transform _xRRig;
    
    private Rigidbody _rb = null;
    
    // takes the xr rig position and apply it so that the board is in the center
    // Parent the rig to this board object
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        Invoke(nameof(InitializePositions), 2f);
    }
    
    private void InitializePositions()
    {
        transform.position = new Vector3(_xrCamPos.position.x, _xrCamPos.position.y - _playerHeightOffset, _xrCamPos.position.z);
        _xRRig.transform.parent = transform;
        
        _rb.constraints = RigidbodyConstraints.None;

        
    }
}
