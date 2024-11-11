using UnityEngine;
using UnityEngine.UI;

public class ValuesManager : MonoBehaviour
{
    public GameObject valuesBG;
    public ValueHolder sferaValue;
    public ValueHolder cylinderValue;
    public ValueHolder osValue;
    public ValueHolder angleFactorValue;
    public ValueHolder distantFactorValue;
    public Button exitButton;

    public static ValuesManager instance;
    public static GameObject ValuesBG => instance.valuesBG;
    public static ValueHolder SferaValue => instance.sferaValue;
    public static ValueHolder CylinderValue => instance.cylinderValue;
    public static ValueHolder OsValue => instance.osValue;
    public static ValueHolder AngleFactorValue => instance.angleFactorValue;
    public static ValueHolder DistantFactorValue => instance.distantFactorValue;

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
