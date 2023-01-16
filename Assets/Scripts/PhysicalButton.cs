using UnityEngine;
using UnityEngine.Events;

public class PhysicalButton : MonoBehaviour
{
    public UnityEvent action;

    private void OnMouseUp()
    {
        action.Invoke();
    }
}
