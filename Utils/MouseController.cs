using System.Collections;
using UnityEngine;

public class MouseController : MonoBehaviour {

    public static Vector3 mousePosition;

    private void Update() {
        Vector3 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mousePosition.z = 0;
        mousePosition = _mousePosition;
    }
}