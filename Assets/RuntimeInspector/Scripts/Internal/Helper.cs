using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
namespace RI
{
    public class Undo
    {

        public static Undo current = new Undo();
        public Stack<System.Action> actions = new Stack<System.Action>();
        public void Register(System.Action action)
        {
            Debug.Log(actions.Count);
            actions.Push(action);
        }
        public void Use()
        {
            actions.Pop()();
        }
        public void UnRegister()
        {
            actions.Pop();
        }
    }
    public static class Helper
    {
        public static IEnumerable<System.Type> GetTypesWithHelpAttribute()
        {
            foreach (System.Reflection.Assembly a in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in a.GetTypes())
                {
                    if (type.GetCustomAttributes(typeof(RIShow), true).Length > 0)
                    {
                        yield return type;
                    }
                }
            }
        }
        public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }
            return result.ToArray();
        }


        public static bool IsHideInInspector(this System.Type Type)
        {
            return Type.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(HideInInspector)) == null;
        }

        public static bool IsArrayOrList(this System.Type Type)
        {
            return Type.IsArray || typeof(IList).IsAssignableFrom(Type);
        }

        public static bool IsClass(this System.Type Type)
        {
            return !(Type.IsPrimitive || Type.IsValueType || (Type == typeof(string))) && !Type.IsSubclassOf(typeof(UnityEngine.Object));
        }


        public static System.Array Resize(this System.Array source, int newSize)
        {
            System.Type elementType = source.GetType().GetElementType();
            System.Array destination = System.Array.CreateInstance(elementType, newSize);
            System.Array.Copy(source, destination, System.Math.Min(source.Length, destination.Length));
            return destination;
        }

        public static System.Array RemoveAt(this System.Array source, int index)
        {
            System.Type elementType = source.GetType().GetElementType();
            System.Array destination = System.Array.CreateInstance(elementType, source.Length - 1);
            if (index > 0)
                System.Array.Copy(source, 0, destination, 0, index);

            if (index < source.Length - 1)
                System.Array.Copy(source, index + 1, destination, index, source.Length - index - 1);

            return destination;
        }


        public static Texture2D SetColor(this Texture2D texture, Color color)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();

            return texture;
        }
    }
}