using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InanitionBackground : MonoBehaviour
{
    public InanitionStageManager manager;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => 
        {
            if (manager) manager.OnMissClick();
        });

        if (!manager) manager = FindObjectOfType<InanitionStageManager>();
    }
}