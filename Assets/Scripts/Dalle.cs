using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshRenderer))]
public class Dalle : MonoBehaviour
{
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material requiredMaterial;

    [SerializeField] private float checkRayMaxDistance = 8;
    
    public enum Mode { Normal, Required }

    public Mode mode
    {
        set
        {
            switch (value)
            {
                case Mode.Normal: _meshRenderer.material = normalMaterial;
                    break;
                case Mode.Required: _meshRenderer.material = requiredMaterial;
                    break;
            }
            
            _mode = value;
        }

        get => _mode;
    }

    private Mode _mode;
    public bool isFilled;
    
    public Transform cubeTransformPosition;

    // Components
    private MeshRenderer _meshRenderer;
    
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        mode = Mode.Normal;
    }

    private void Update()
    {
        if(mode == Mode.Required) CheckIsFilled();
    }

    private void CheckIsFilled()
    {
        Collider[] hitterColliders = Physics.OverlapSphere(transform.position, 1, LayerMask.NameToLayer("Moving Block"));
        
        if (hitterColliders[0])
        {
            isFilled = true;
        }
    }
}
