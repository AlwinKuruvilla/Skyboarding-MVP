using Boids;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public AudioSource goalAudio;
    public AudioClip pointsGained;
    
    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out Boid boid)) return;
        
        if (boid.CheckCaptureStatus()) 
        {
            ScoreKeeper.IncreaseScore(other.gameObject.GetComponent<Boid>().pointValue);
            goalAudio.PlayOneShot(pointsGained);
            Destroy(other.gameObject);
        }
    }
    
}
