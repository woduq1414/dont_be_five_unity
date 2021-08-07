using System.Text;
using System.Collections.Generic;
// using JetBrains.Annotations;
using Object = UnityEngine.Object;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public static class StringUtils
{
    public static string ToString(List<Object> list, string delimiter = "\n")
    {
        if (list == null)
        {
            return "null";
        }

        int lastIndex = list.Count - 1;
        if (lastIndex == -1)
        {
            return "{}";
        }

        var builder = new StringBuilder(500);
        builder.Append('{');
        for (int n = 0; n < lastIndex; n++)
        {
            Append(list[n], builder);
            builder.Append(delimiter);
        }
        Append(list[lastIndex], builder);
        builder.Append('}');

        return builder.ToString();
    }

    public static void Append(Object target, StringBuilder toBuilder)
    {
        if (target == null)
        {
            toBuilder.Append("null");
        }
        else
        {
            toBuilder.Append("\"");
            toBuilder.Append(target.name);
            toBuilder.Append("\" (");
            toBuilder.Append(target.GetType().Name);
            toBuilder.Append(")");
        }
    }
}

public static class Utils
{

    public static GameObject FindChildren(GameObject targetObject, string name)
    {
        return targetObject.GetComponentsInChildren<Transform>()
                                 .FirstOrDefault(c => c.gameObject.name == name )?.gameObject;
    }


    public static void ShowToast(string message){
        SSTools.ShowMessage(message, SSTools.Position.top, SSTools.Time.twoSecond);
    }
}