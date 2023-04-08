using Sirenix.OdinInspector;
using UnityEngine;

public class Dbug : MonoBehaviour
{
    public static Dbug Instance;
    private void Awake()
    {
        if (Instance != null) LogInstanceAlreadyExists(this);
        Instance = this;
    }
    private enum LogLevel { OFF, LOGS, WARNINGS, ERRORS, ALL }

    [Title("Inspector References")]
    [SerializeField] private LogLevel logLevel;

    public void Log(string msg)
    {
        if (logLevel == LogLevel.LOGS || logLevel == LogLevel.ALL)
            Debug.Log(msg);
    }

    public void LogWarning(string msg)
    {
        if (logLevel == LogLevel.WARNINGS || logLevel == LogLevel.ALL)
            Debug.LogWarning(msg);
    }

    public void LogError(string msg)
    {
        if (logLevel == LogLevel.ERRORS || logLevel == LogLevel.ALL)
            Debug.LogError(msg);
    }

    public void LogInstanceAlreadyExists(object msg)
    {
        Debug.LogError($"{msg} :: Instance already exists!");
    }
}
