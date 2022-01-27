using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Dalle : MonoBehaviour
{
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material requiredMaterial;

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

    public bool IsFilled
    {
        get => _isFilled;
        set
        {
            if (value)
                GameManager.instance.onDalleFilled(this);
            else
                GameManager.instance.onDalleEmpty(this);

            _isFilled = value;
        }
    }

    private bool _isFilled;
    
    
    public Transform cubeTransformPosition;

    // Components
    private MeshRenderer _meshRenderer;
    
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        mode = Mode.Normal;
    }
}
