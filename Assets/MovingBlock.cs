using System;
using StarterAssets;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    [SerializeField] private float pushRayMaxDistance = 10;
    
    private StarterAssetsInputs _input;
    private Dalle _currentDalle;
    private Dalle _nextDalle;

    private void Start()
    {
        _input = StarterAssetsInputs.instance;
        _currentDalle = GetCurrentDalleManually();
        
        if(_currentDalle == null)
            Debug.LogError($"Moving Block \"{this}\" is not on a Dalle !");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PushTargetDetection();
        }
    }
    
    private void PushTargetDetection()
    {
        // prevent diagonal deplacement
        if (_input.move.x != 0 && _input.move.y != 0)
            return;

        // 1, 0 -> x+
        // -1, 0 -> x-
        // 0, 1 -> z+
        // 0, -1 -> z-
        
        var pushDirection = new Vector3(_input.move.x, 0, _input.move.y);
        Ray pushRay = new Ray(transform.position, pushDirection);

        Physics.Raycast(pushRay, out RaycastHit hit, pushRayMaxDistance);
        Debug.DrawRay(pushRay.origin, pushRay.direction * pushRayMaxDistance, Color.red, 30);

        if (hit.collider != null)
        {
            GameObject dalle = hit.collider.gameObject;
            Dalle dalleTarget = dalle.GetComponent<Dalle>();
            
            if(!dalleTarget)
                return;

            _nextDalle = dalleTarget;
            PushBox(_nextDalle, 0);
        }
    }

    private void PushBox(Dalle target, float interpolationPercentage)
    {
        transform.position = target.cubeTransformPosition.position;

        // TEMP
        _currentDalle = _nextDalle;
        _nextDalle = null;
        // TEMP

        Debug.Log(GameManager.instance);
        GameObject player = GameManager.instance.player;

        var rotation = player.transform.rotation;
        var position = player.transform.position;
        
        GameManager.instance.playerActions.Add(new PlayerAction(
            _currentDalle, 
            _nextDalle, 
            this, 
            new Vector3(position.x, position.y, position.z), 
            new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w))
        );
    }

    private Dalle GetCurrentDalleManually()
    {
        Collider[] hitterColliders = Physics.OverlapSphere(transform.position, 1);

        foreach (var hitterCollider in hitterColliders)
        {
            Debug.Log(hitterCollider.gameObject);
            
            Dalle dalle = hitterCollider.gameObject.GetComponent<Dalle>();
            if (dalle != null)
            {
                return dalle;
            }
        }
        
        return null;
    }
}
