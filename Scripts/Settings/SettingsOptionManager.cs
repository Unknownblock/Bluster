using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsOptionManager : MonoBehaviour
{
    public List<CustomButton> options;
    public CustomButton chosenOption;
    public int chosenOptionNum;

    public float xDistance;
    public float yDistance;
    public Image chosenIcon;

    private void Update()
    {
        for (int i = 0; i < options.Count; i++) 
        {
            if (options[i].isPressed)
            {
                chosenOption = options[i];
                chosenOptionNum = i;
            }
        }

        chosenOption = options[chosenOptionNum];
        chosenIcon.transform.position = chosenOption.transform.position + new Vector3(xDistance, yDistance, 0f);
    }
}
