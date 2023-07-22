using UnityEngine;
using Sirenix.OdinInspector;
using QFSW.QC;

public class HealthBar : MonoBehaviour
{
    public static HealthBar Instance;

    [Title("Inspector References")]
    [SerializeField] private MeshRenderer r_Renderer;

    private float HealthPercentage
    {
        get
        {
            return r_Renderer.sharedMaterial.GetFloat("_Progress") * 100;
        }
        set 
        {
            r_Renderer.sharedMaterial.SetFloat("_Progress", value / 100);
        }
    }

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region Public Methods
    [Command]
    /// <summary>
    /// Drops the health of this object.
    /// </summary>
    public void OnHit(int amount)
    {
        HealthPercentage = Mathf.Clamp(HealthPercentage - amount, 0, 100);

        if (HealthPercentage <= 0)
            Dbug.Instance.Log("DEAD!");
    }
    #endregion
}