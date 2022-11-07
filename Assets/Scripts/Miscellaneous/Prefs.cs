using UnityEngine;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

public static class Prefs
{
    public static void Export<T>(this T instance, string path)
    {
        path = Application.dataPath + "/" + path + ".txt";
        DirectoryInfo directoryInfo = new FileInfo(path).Directory;
        if (directoryInfo == null) return;
        directoryInfo.Create();
        File.WriteAllText(path, instance.SaveRaw());
    }

    public static T Import<T>(this string path, T instance = default(T))
    {
        path = Application.dataPath + "/" + path + ".txt";
        return File.ReadAllText(path).LoadRaw<T>();
    }

    public static string[] GetFiles(this string directory, bool nameOnly = true, string extension = "txt")
    {
        directory = Application.dataPath + "/" + directory + "/";

        string[] files = Directory.GetFiles(@directory, "*." + extension);

        if (nameOnly)
            files = files.Select(s => s.Replace("." + extension, ""))
                .Select(s => s.Replace(directory, ""))
                .ToArray();

        return files;
    }

    public static void Save<T>(this T instance)
    {
        Save(instance, typeof(T).Name);
    }

    public static void Save<T>(this T instance, string name)
    {
        PlayerPrefs.SetString(name, instance.SaveRaw());
    }

    public static string SaveRaw<T>(this T instance)
    {
        //var sw = new StringWriter();
        //new XmlSerializer(instance.GetType()).Serialize(sw, instance);
        //return sw.ToString();
        return JsonUtility.ToJson(instance);
    }

    public static void Load<T>(ref T instance) where T : class
    {
        instance = typeof(T).Name.Load<T>() ?? instance;
    }

    public static T Load<T>(this string name, T instance = default(T))
    {
        return PlayerPrefs.GetString(name).LoadRaw<T>(instance);
    }

    public static T LoadRaw<T>(this string data, T instance = default(T))
    {
        /*return
            string.IsNullOrEmpty(data)
                ? instance
                : (T) new XmlSerializer(typeof(T)).Deserialize(new StringReader(data));*/
        return string.IsNullOrEmpty(data) ? instance : JsonUtility.FromJson<T>(data);
    }

    public static bool Exists(this string name)
    {
        return PlayerPrefs.HasKey(name);
    }

    public static bool Exists<T>(this T instance)
    {
        return Exists(typeof(T).Name);
    }

    public static void DeleteKey(this string name)
    {
        PlayerPrefs.DeleteKey(name);
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}