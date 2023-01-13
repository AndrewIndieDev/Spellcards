using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TimerController : MonoBehaviour
{

    public GameObject LevelComplete;
    public Image timer_linear_image;
    public Image timer_radial_image;
    float time_remaining;
    public float max_time = 120.0f;
    public GameObject timer_radial_textholder;








    // Start is called before the first frame update
    void Start()
    {
        time_remaining = max_time;


    }





    // Update is called once per frame
    void Update()
    {
        if (time_remaining > 0)
        {

            time_remaining -= Time.deltaTime;
            timer_linear_image.fillAmount = time_remaining / max_time;
            timer_radial_image.fillAmount = time_remaining / max_time;

        }
        else
        {

            LevelComplete.SetActive(true);
            timer_radial_image.gameObject.SetActive(false);

        }

    }
}
