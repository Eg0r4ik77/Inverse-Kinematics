using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpiderMovement : MonoBehaviour
{
    private const float speed = 2.2f;

    void Update()
    {
        if (transform.position.x >= 45)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        transform.position += new Vector3(Time.deltaTime * speed, 0, 0);
    }

}
