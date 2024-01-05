using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public enum EAxis
{
    X,
    Y,
    Z,
    W,
    XY,
    XZ,
    XW,
    YZ,
    YW,
    ZW,
    XYZ,
    XYW,
    XZW,
    YZW
}

public static class Utilities
{
    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T RandomElement<T>(this IList<T> list)
    {
        return list.Count > 0 ? list[UnityEngine.Random.Range(0, list.Count)] : default;
    }

    /// <summary>
    /// Get's a random number between min and max.
    /// </summary>
    /// <param name="minInclusive">min(inclusive)</param>
    /// <param name="maxExclusive">max(exclusive)</param>
    /// <returns>Random int between two numbers.</returns>
    public static int GetRandomNumber(int minInclusive, int maxExclusive)
    {
        using (RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider())
        {
            byte[] rno = new byte[3];
            rg.GetBytes(rno);
            return minInclusive + (BitConverter.ToUInt16(rno, 0) % (maxExclusive - minInclusive));
        }
    }

    public static float RemapRange(this float s, float a1, float a2, float b1, float b2)
    {
        // Remap a value from one range (a1 to a2) to another (b1 to b2)
        // Example, I have 15(s) / 240(a2) health (a1 = 0), I want to remap that to a scale of 0 to 1
        // (s - a1) | 15 - 0 = 15
        // (b2 - b1) | 1 - 0 = 1
        // (a2 - a1) | 240 - 0 = 240
        // 15 * 1 = 15
        // 15 / 240 = 0.0625
        // 0 + 0.0625 = 0.0625

        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    public static float RemapClamped(this float s, float a1, float a2, float b1, float b2)
    {
        return Mathf.Clamp(RemapRange(s, a1, a2, b1, b2), b1, b2);
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static bool AddUnique<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
            return true;
        }
        return false;
    }

    public static bool RemoveIfContains<T>(this List<T> list, T item)
    {
        if (list.Contains(item))
        {
            list.Remove(item);
            return true;
        }
        return false;
    }

    public static void ExecuteNextFrame(this MonoBehaviour behaviour, Action action)
    {
        behaviour.StartCoroutine(NextFrame(action));
    }

    private static IEnumerator NextFrame(Action action)
    {
        yield return 0;
        action();
    }

    public static bool IsValidIndex(this Array array, int index)
    {
        return array.Length > index && index >= 0;
    }

    public static bool IsValidIndex<T>(this List<T> list, int index)
    {
        return list.Count > index && index >= 0;
    }

    public static Vector2 IgnoreAxis(this Vector2 vector, EAxis ignoredAxis, float ignoredAxisValue = 0)
    {
        switch (ignoredAxis)
        {
            case EAxis.X:
                return new Vector2(ignoredAxisValue, vector.y);
            case EAxis.Y:
                return new Vector2(vector.x, ignoredAxisValue);
        }
        Debug.Log("Unsupported ignored axis given");
        return vector;
    }

    public static Vector3 IgnoreAxis(this Vector3 vector, EAxis ignoredAxis, float ignoredAxisValue = 0)
    {
        switch (ignoredAxis)
        {
            case EAxis.X:
                return new Vector3(ignoredAxisValue, vector.y, vector.z);
            case EAxis.Y:
                return new Vector3(vector.x, ignoredAxisValue, vector.z);
            case EAxis.Z:
                return new Vector3(vector.x, vector.y, ignoredAxisValue);
            case EAxis.XY:
                return new Vector3(ignoredAxisValue, ignoredAxisValue, vector.z);
            case EAxis.XZ:
                return new Vector3(ignoredAxisValue, vector.y, ignoredAxisValue);
            case EAxis.YZ:
                return new Vector3(vector.x, ignoredAxisValue, ignoredAxisValue);
        }
        Debug.Log("Unsupported ignored axis given");
        return vector;
    }

    public static Vector4 IgnoreAxis(this Vector4 vector, EAxis ignoredAxis, float ignoredAxisValue = 0)
    {
        switch (ignoredAxis)
        {
            case EAxis.X:
                return new Vector4(ignoredAxisValue, vector.y, vector.z, vector.w);
            case EAxis.Y:
                return new Vector4(vector.x, ignoredAxisValue, vector.z, vector.w);
            case EAxis.Z:
                return new Vector4(vector.x, vector.y, ignoredAxisValue, vector.w);
            case EAxis.W:
                return new Vector4(vector.x, vector.y, vector.z, ignoredAxisValue);
            case EAxis.XY:
                return new Vector4(ignoredAxisValue, ignoredAxisValue, vector.z, vector.w);
            case EAxis.XZ:
                return new Vector4(ignoredAxisValue, vector.y, ignoredAxisValue, vector.w);
            case EAxis.XW:
                return new Vector4(ignoredAxisValue, vector.y, vector.z, ignoredAxisValue);
            case EAxis.YZ:
                return new Vector4(vector.x, ignoredAxisValue, ignoredAxisValue, vector.w);
            case EAxis.YW:
                return new Vector4(vector.x, ignoredAxisValue, vector.z, ignoredAxisValue);
            case EAxis.ZW:
                return new Vector4(vector.x, vector.y, ignoredAxisValue, ignoredAxisValue);
            case EAxis.XYZ:
                return new Vector4(ignoredAxisValue, ignoredAxisValue, ignoredAxisValue, vector.w);
            case EAxis.XYW:
                return new Vector4(ignoredAxisValue, ignoredAxisValue, vector.z, ignoredAxisValue);
            case EAxis.XZW:
                return new Vector4(ignoredAxisValue, vector.y, ignoredAxisValue, ignoredAxisValue);
            case EAxis.YZW:
                return new Vector4(vector.x, ignoredAxisValue, ignoredAxisValue, ignoredAxisValue);
        }
        Debug.Log("Unsupported ignored axis given");
        return vector;
    }

    public static void SaveAsPNG(this Texture2D texture, string fullPath)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
    }

    public static Vector3 RandomUnitVectorInCone(this Quaternion targetDirection, float angle)
    {
        var angleInRad = UnityEngine.Random.Range(0.0f, angle) * Mathf.Deg2Rad;
        var PointOnCircle = (UnityEngine.Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        var V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
        return targetDirection * V;
    }

    public static Vector3 RandomUnitVectorInCone(this Vector3 targetDirection, float angle)
    {
        return RandomUnitVectorInCone(Quaternion.LookRotation(targetDirection), angle);
    }

    public static void AttachTo(this SkinnedMeshRenderer smr, SkinnedMeshRenderer parent)
    {
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in parent.bones)
            boneMap[bone.gameObject.name] = bone;

        SkinnedMeshRenderer myRenderer = smr;

        Transform[] newBones = new Transform[myRenderer.bones.Length];
        for (int i = 0; i < myRenderer.bones.Length; ++i)
        {
            GameObject bone = myRenderer.bones[i].gameObject;
            if (!boneMap.TryGetValue(bone.name, out newBones[i]))
            {
                Debug.Log("Unable to map bone \"" + bone.name + "\" to target skeleton.");
                continue;
            }
        }
        myRenderer.bones = newBones;

        smr.rootBone = parent.rootBone;
        smr.transform.parent = parent.transform.parent;
    }

    public static string SanitizeAndFormatTags(this string text)
    {
        return text.SanitizeText().FormatTags();
    }

    public static string SanitizeText(this string text)
    {
        string oldText;
        do
        {
            oldText = text;
            text = Regex.Replace(text, @"<\/noparse>", "", RegexOptions.IgnoreCase);
        } while (oldText != text);

        return text;
    }

    public static string SanitizeTextNoTMP(this string text)
    {
        string oldText;
        do
        {
            oldText = text;
            text = Regex.Replace(text, @"<\/noparse>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<noparse>", "", RegexOptions.IgnoreCase);
        } while (oldText != text);

        return text;
    }

    public static string FormatTags(this string text)
    {
        int characterIndex = -1;
        while (characterIndex < text.Length)
        {
            characterIndex++;
            characterIndex = text.IndexOf("$", characterIndex);
            if (characterIndex < 0) break;
            if (characterIndex < text.Length - 3) // Text has at least 3 more characters after this '$'
            {
                string str = text.Substring(characterIndex + 1, 3);
                if (str.OnlyHexInString())
                {
                    text = text.ReplaceSectionWithFormat(characterIndex, 4, $"</color><color={GetHexColorStringFrom3Chars(str)}>");
                }
            }
            if (characterIndex < text.Length - 2) // Text has at least 2 more characters after this '$'
            {
                if (text[characterIndex + 1] == 'l')
                {
                    int linkEndChar = text.IndexOf(' ', characterIndex);
                    if (linkEndChar == -1)
                        linkEndChar = text.Length;
                    string link = text.Substring(characterIndex + 2, linkEndChar - characterIndex - 2);
                    text = text.ReplaceSectionWithFormat(characterIndex, linkEndChar - characterIndex, $"<color=#0645AD><u><link=\"{link}\">{link}</link></u></color>");
                }
            }
        }
        return text;
    }

    public static bool OnlyHexInString(this string text)
    {
        return Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
    }

    public static string ReplaceSectionWithFormat(this string text, int startIndex, int length, string newSection)
    {
        string beforeText = text.Substring(0, startIndex);
        string afterText = text.Substring(startIndex + length);
        return beforeText + "</noparse>" + newSection + "<noparse>" + afterText;
    }

    public static string GetHexColorStringFrom3Chars(this string text)
    {
        if (text.Length == 3)
        {
            string r = text[0].ToString().ToUpper() + text[0].ToString().ToUpper();
            string g = text[1].ToString().ToUpper() + text[1].ToString().ToUpper();
            string b = text[2].ToString().ToUpper() + text[2].ToString().ToUpper();
            return "#" + r + g + b;
        }
        return "#000000";
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.TryGetComponent(out T component) ? component : gameObject.AddComponent<T>();
    }

    public static string Nicify(this string s)
    {
        string result = "";
        if (s.Length > 1)
            s = char.ToUpper(s[0]) + s[1..];
        for (int i = 0; i < s.Length; i++)
        {
            if (char.IsUpper(s[i]) == true && i != 0)
            {
                result += " ";
            }

            result += s[i];
        }
        return result;
    }

    public static void Materialify(this GameObject go, Material mat)
    {
        go.SetActive(true);
        MeshRenderer[] meshRenderers = go.GetComponents<MeshRenderer>();
        MeshRenderer[] childMeshRenderers = go.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var item in meshRenderers)
        {
            item.sharedMaterial = mat;
        }
        foreach (var item in childMeshRenderers)
        {
            item.sharedMaterial = mat;
        }

        SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponents<SkinnedMeshRenderer>();
        SkinnedMeshRenderer[] skinnedChildMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var item in skinnedMeshRenderers)
        {
            item.sharedMaterial = mat;
        }
        foreach (var item in skinnedChildMeshRenderers)
        {
            item.sharedMaterial = mat;
        }
    }

    public static void AddOrReplace<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
    {
        if (dictionary.ContainsKey(key))
            dictionary.Remove(key);
        dictionary.Add(key, value);
    }

    public static float DistanceSquared(this Vector3 vectorA, Vector3 vectorB)
    {
        return (vectorB - vectorA).sqrMagnitude;
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string result = string.Empty;
        for (int i = 0; i < length; i++)
        {
            result += chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        return result;
    }
}

public class IntStringDictionary<T>
{
    public Dictionary<int, T> intDictionary = new Dictionary<int, T>();
    public Dictionary<string, T> stringDictionary = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
    Dictionary<int, string> intStringPairing = new Dictionary<int, string>();
    Dictionary<string, int> stringIntPairing = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    public int Count { get { return intDictionary.Count; } }

    public void Add(int intValue, string stringValue, T value)
    {
        intDictionary.TryAdd(intValue, value);
        stringDictionary.TryAdd(stringValue, value);
        intStringPairing.TryAdd(intValue, stringValue);
        stringIntPairing.TryAdd(stringValue, intValue);
    }

    public void Remove(int key)
    {
        if (Find(key) == null) return;
        string stringKey = intStringPairing[key];
        intDictionary.Remove(key);
        stringDictionary.Remove(stringKey);
        intStringPairing.Remove(key);
        stringIntPairing.Remove(stringKey);
    }

    public void Remove(string key)
    {
        if (Find(key) == null) return;
        int intKey = stringIntPairing[key];
        intDictionary.Remove(intKey);
        stringDictionary.Remove(key);
        intStringPairing.Remove(intKey);
        stringIntPairing.Remove(key);
    }

    public bool ReplaceKey(string key, int newKey)
    {
        T find = Find(key);
        if (find == null) return false;
        Remove(key);
        Add(newKey, key, find);
        return true;
    }

    public bool ContainsKey(int key)
    {
        return Find(key) != null;
    }

    public bool ContainsKey(string key)
    {
        return Find(key) != null;
    }

    public T Find(int key)
    {
        T value;
        if (intDictionary.TryGetValue(key, out value))
        {
            return value;
        }
        return default(T);
    }

    public T Find(string key)
    {
        T value;
        if (stringDictionary.TryGetValue(key, out value))
        {
            return value;
        }
        return default(T);
    }

    public T this[int key]
    {
        get { return Find(key); }
    }

    public T this[string key]
    {
        get { return Find(key); }
    }

    public void Clear()
    {
        intDictionary.Clear();
        stringDictionary.Clear();
        intStringPairing.Clear();
        stringIntPairing.Clear();
    }
}

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data;

    public PriorityQueue()
    {
        this.data = new List<T>();
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        int ci = data.Count - 1; // child index; start at end
        while (ci > 0)
        {
            int pi = (ci - 1) / 2; // parent index
            if (data[ci].CompareTo(data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
            T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
            ci = pi;
        }
    }

    public T Dequeue()
    {
        // assumes pq is not empty; up to calling code
        int li = data.Count - 1; // last index (before removal)
        T frontItem = data[0];   // fetch the front
        data[0] = data[li];
        data.RemoveAt(li);

        --li; // last index (after removal)
        int pi = 0; // parent index. start at front of pq
        while (true)
        {
            int ci = pi * 2 + 1; // left child index of parent
            if (ci > li) break;  // no children so done
            int rc = ci + 1;     // right child
            if (rc <= li && data[rc].CompareTo(data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                ci = rc;
            if (data[pi].CompareTo(data[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
            T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
            pi = ci;
        }
        return frontItem;
    }

    public void RemoveElement(T element)
    {
        data.Remove(element);
    }

    public T Peek()
    {
        T frontItem = data[0];
        return frontItem;
    }

    public int Count()
    {
        return data.Count;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < data.Count; ++i)
            s += data[i].ToString() + " ";
        s += "\ncount = " + data.Count;
        return s;
    }

    public bool IsConsistent()
    {
        // is the heap property true for all data?
        if (data.Count == 0) return true;
        int li = data.Count - 1; // last index
        for (int pi = 0; pi < data.Count; ++pi) // each parent index
        {
            int lci = 2 * pi + 1; // left child index
            int rci = 2 * pi + 2; // right child index

            if (lci <= li && data[pi].CompareTo(data[lci]) > 0) return false; // if lc exists and it's greater than parent then bad.
            if (rci <= li && data[pi].CompareTo(data[rci]) > 0) return false; // check the right child too.
        }
        return true; // passed all checks
    } // IsConsistent
} // PriorityQueue
