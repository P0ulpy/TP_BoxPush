using System;
using StarterAssets;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    [SerializeField] private float pushRayMaxDistance = 10;
    [SerializeField] private float moveLerpTime = 2;
    
    private StarterAssetsInputs _input;
    private Dalle _currentDalle;
    private Dalle _nextDalle;
    private float _moveLerpElapsedTime = 0f;
    
    public enum Status { Static, Moving }
    public Status status = Status.Static;
    
    private void Start()
    {
        _input = StarterAssetsInputs.instance;
        _currentDalle = GetCurrentDalleManually();
        
        if(_currentDalle == null)
            Debug.LogError($"Moving Block \"{this}\" is not on a Dalle !");
    }

    private void OnTriggerStay(Collider other)
    {
        if (status != Status.Moving && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PushTargetDetection();
        }
    }

    private void Update()
    {
        if (_nextDalle)
        {
            PushBox();
        }
    }

    private void PushTargetDetection()
    {
        // prevent diagonal deplacement
        if (_input.move.x != 0 && _input.move.y != 0)
            return;
        
        var pushDirection = new Vector3(_input.move.x, 0, _input.move.y);
        Ray pushRay = new Ray(transform.position, pushDirection);

        Physics.Raycast(pushRay, out RaycastHit hit, pushRayMaxDistance);
        Debug.DrawRay(pushRay.origin, pushRay.direction * pushRayMaxDistance, Color.red, 1);

        if (hit.collider != null)
        {
            GameObject dalle = hit.collider.gameObject;
            Dalle dalleTarget = dalle.GetComponent<Dalle>();
            
            if(!dalleTarget)
                return;

            _nextDalle = dalleTarget;
            status = Status.Moving;
        }
    }

    private void PushBox()
    {
        _moveLerpElapsedTime += Time.deltaTime;
        
        float lerpPercentage = _moveLerpElapsedTime % moveLerpTime;
        transform.position = Vector3.Lerp(transform.position, _nextDalle.cubeTransformPosition.position, lerpPercentage);

        Debug.Log(lerpPercentage);
        
        if (lerpPercentage >= 1f)
        {
            _currentDalle.IsFilled = false;
            _nextDalle.IsFilled = true;

            GameObject player = GameManager.instance.player;

            GameManager.instance.playerActions.Add(new PlayerAction(
                _currentDalle, 
                _nextDalle, 
                this, 
                player.transform.position,
                player.transform.rotation
            ));

            _currentDalle = _nextDalle;
            _nextDalle = null;
            _moveLerpElapsedTime = 0f;
            status = Status.Static;
        }
    }

    private Dalle GetCurrentDalleManually()
    {
        Collider[] hitterColliders = Physics.OverlapSphere(transform.position, 1/*, LayerMask.NameToLayer("Dalle") */);

        foreach (var hitterCollider in hitterColliders)
        {
            Dalle dalle = hitterCollider.gameObject.GetComponent<Dalle>();
            if (dalle != null)
            {
                return dalle;
            }
        }
        
        return null;
    }
}
