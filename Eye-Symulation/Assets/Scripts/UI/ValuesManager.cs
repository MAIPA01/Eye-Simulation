using UnityEngine;
using UnityEngine.UI;

public class ValuesManager : MonoBehaviour
{
    public GameObject valuesBG;
    public ValueHolder sferaValue;
    public ValueHolder cylinderValue;
    public ValueHolder osValue;
    public Button exitButton;

    public static ValuesManager instance;
    public static GameObject ValuesBG => instance.valuesBG;
    public static ValueHolder SferaValue => instance.sferaValue;
    public static ValueHolder CylinderValue => instance.cylinderValue;
    public static ValueHolder OsValue => instance.osValue;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(() => { Application.Quit(); });
        }
    }
}
