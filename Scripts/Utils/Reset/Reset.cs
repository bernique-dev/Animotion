using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour {

    public bool canResetWithKeyboard;

    public void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R) && canResetWithKeyboard) {
            ResetScene();
        }
    }

}
