using UnityEngine;

public class VictoryPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FindObjectOfType<EndScreen>().ShowWinScreen();
        }
    }
}