using Boids;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Boid boid)) return;
        
        if (boid.CheckCaptureStatus()) 
        {
            ScoreKeeper.IncreaseScore(other.gameObject.GetComponent<Boid>().pointValue);
            Destroy(other.gameObject);
        }
    }
    
}
