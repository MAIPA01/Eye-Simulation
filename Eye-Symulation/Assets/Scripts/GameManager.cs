using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        if (ValuesManager.instance != null)
        {
            if (ValuesManager.instance.valuesBG.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (ValuesManager.instance != null)
            {
                ValuesManager.instance.valuesBG.SetActive(!ValuesManager.instance.valuesBG.activeSelf);

                if (ValuesManager.instance.valuesBG.activeSelf)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }
}
