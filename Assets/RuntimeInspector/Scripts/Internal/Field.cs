using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace RI
{
    public interface IFieldAccesor
    {
        object GetValue(); //get self 
        void SetValue(object value); //set self
        SerializedObject[] GetNestedFields(); //Get nested field if self is a class or array
    }
    public class ArrayAccessor : IFieldAccesor
    {
        int index;
        string name;
        BindingFlags flags;
        object reference;
        Type type;
        public ArrayAccessor(int index, string name, Type type, object reference, BindingFlags flags)
        {
            this.index = index;
            this.type = type;
            this.reference = reference;
            this.name = name;
            this.flags = flags;
        }
        public object GetValue()
        {
            return GetArray().GetValue(index);
        }
        public void SetValue(object value)
        {
            GetArray().SetValue(value, index);
        }
        public SerializedObject[] GetNestedFields()
        {
            return type.GetFields(flags).Select(x => new SerializedObject(x.Name, x.FieldType, GetValue())).ToArray();
        }

        Array GetArray()
        {
            return reference.GetType().GetField(name, flags).GetValue(reference) as Array;
        }
    }
    public class DefaultAccessor : IFieldAccesor
    {
        string name;
        BindingFlags flags;
        object reference;
        Type type;
        public DefaultAccessor(string name, Type type, object reference, BindingFlags flags)
        {
            this.type = type;
            this.reference = reference;
            this.name = name;
            this.flags = flags;
        }
        public object GetValue()
        {
            return reference.GetType().GetField(name, flags).GetValue(reference);
        }
        public void SetValue(object value)
        {
            reference.GetType().GetField(name, flags).SetValue(reference, value);
        }
        public SerializedObject[] GetNestedFields()
        {
            return type.GetFields(flags).Where(w => w.GetType().GetConstructor(Type.EmptyTypes) != null).Select(x => new SerializedObject(x.Name, x.FieldType, GetValue())).ToArray(); ;
        }
    }
    public class SerializedObject
    {

        protected readonly BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        public object Reference { get; protected set; }
        public System.Type Type { get; protected set; }
        public string Name { get; protected set; }

        public IFieldAccesor Accessor;


        public SerializedObject(string name, Type type, object reference)
        {
            this.Name = name;
            this.Reference = reference;
            this.Type = type;
            this.Accessor = new DefaultAccessor(name, type, reference, flags);
            if (Accessor.GetValue() == null) Accessor.SetValue(CreateInstance());
        }
        public SerializedObject(string name, Type type, object reference, int index)
        {
            this.Name = name;
            this.Reference = reference;
            this.Type = type;
            this.Accessor = new ArrayAccessor(index, name, type, reference, flags);
            if (Accessor.GetValue() == null) Accessor.SetValue(CreateInstance());
        }


        public void SetField(object value)
        {
            Accessor.SetValue(value);
        }

        public object GetField()
        {
            return Accessor.GetValue();
        }

        public object CreateInstance()
        {
            if (this.Type.IsArray)
                return Array.CreateInstance(this.Type.GetElementType(), 0);
            else if (this.Type.GetConstructor(Type.EmptyTypes) != null)
            {
                return System.Activator.CreateInstance(this.Type);
            }
            return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(this.Type);
        }

        public SerializedObject[] GetAllNestedFields()
        {
            return Accessor.GetNestedFields();
        }
    }
    public abstract class BaseField
    {
        public SerializedObject serializedObject;
        public BaseField(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;
        }

        public abstract void Draw();

        public static BaseField CreateBaseField(SerializedObject serializedObject)
        {
            if (serializedObject != null)
            {
                if (serializedObject.Type.IsHideInInspector())
                {
                    if (serializedObject.Type.IsArrayOrList())
                    {
                        return new ArrayField(serializedObject);
                    }
                    else if (serializedObject.Type.IsClass())
                    {
                        return new ClassField(serializedObject);
                    }

                    return new TextField(serializedObject);
                }
            }
            return null;
        }

    }

    public class ClassField : BaseField
    {
        BaseField[] nestedFields;
        RI.UI.Foldout foldout = new RI.UI.Foldout(true);
        public ClassField(SerializedObject serializedObject) : base(serializedObject)
        {
            nestedFields = serializedObject.GetAllNestedFields().Select(x => BaseField.CreateBaseField(x)).Where(y => y.serializedObject.Reference != null).ToArray();
        }
        public override void Draw()
        {
            GUILayout.BeginHorizontal("Box");
            if (nestedFields.Length > 0) foldout.Draw();
            GUILayout.Label(serializedObject.Name, GUILayout.Width(80f));
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            if (foldout.open)
            {
                for (int i = 0; i < nestedFields.Length; i++)
                {
                    nestedFields[i].Draw();
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }



    public class ArrayField : BaseField
    {
        List<BaseField> elementFields = new List<BaseField>();
        RI.UI.Foldout foldout = new RI.UI.Foldout(true);

        bool fold = false;
        public ArrayField(SerializedObject instance) : base(instance)
        {
            Array array = (Array)serializedObject.GetField();

            for (int i = 0; i < array.Length; i++)
            {
                elementFields.Add(BaseField.CreateBaseField(new SerializedObject(serializedObject.Name, serializedObject.Type.GetElementType(), serializedObject.Reference, i)));
            }
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal("Box");
            if (elementFields.Count > 0) foldout.Draw();
            GUILayout.Label(serializedObject.Name, GUILayout.Width(80f));
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            if (foldout.open)
            {
                for (int i = 0; i < elementFields.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    elementFields[i].Draw();

                    GUI.enabled = (i != 0);
                    if (GUILayout.Button("⇑", GUILayout.MaxWidth(25)))
                    {
                        Swap(i, i - 1);
                    }
                    GUI.enabled = (i != elementFields.Count - 1);
                    {
                        if (GUILayout.Button("⇓", GUILayout.MaxWidth(25)))
                        {
                            Swap(i, i + 1);
                        }
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button("X", GUILayout.MaxWidth(25)))
                    {
                        Remove(i);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            if (GUILayout.Button("+"))
            {
                Add();
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

        }
        void Add()
        {
            Array array = (Array)serializedObject.GetField();
            array = array.Resize(array.Length + 1);
            serializedObject.SetField(array);
            elementFields.Add(BaseField.CreateBaseField(new SerializedObject(serializedObject.Name, serializedObject.Type.GetElementType(), serializedObject.Reference, array.Length - 1)));
        }
        void Remove(int index)
        {
            Array array = (Array)serializedObject.GetField();
            array = array.RemoveAt(index);
            serializedObject.SetField(array);
            elementFields.RemoveAt(index);
        }

        void Swap(int index1, int index2)
        {
            Array array = (Array)serializedObject.GetField();
            object temp = array.GetValue(index1);
            array.SetValue(array.GetValue(index2), index1);
            array.SetValue(temp, index2);

            BaseField tempSO = elementFields[index1];
            elementFields[index1] = elementFields[index2];
            elementFields[index2] = tempSO;
        }


    }

    public class TextField : BaseField
    {
        public string value = string.Empty;



        public TextField(SerializedObject serializedObject) : base(serializedObject)
        {
            if (serializedObject.Type == typeof(System.String))
            {
                value = serializedObject.GetField() as string;
            }
            else
            {
                value = serializedObject.GetField().ToString();
            }
        }


        public override void Draw()
        {
            GUILayout.BeginHorizontal("Box");

            GUILayout.Label(serializedObject.Name, GUILayout.Width(80f));
            GUILayout.Space(10);
            string newValue = GUILayout.TextArea(value, GUILayout.MinWidth(200));
            if (newValue != value)
            {

                if (serializedObject.Type == typeof(System.String))
                {
                    value = newValue;
                    serializedObject.SetField(value);
                }
                else if (serializedObject.Type == typeof(System.Int32))
                {
                    if (string.IsNullOrEmpty(newValue))
                    {
                        value = newValue;
                        serializedObject.SetField(0);
                    }
                    else
                    {
                        long parsed = System.Int64.Parse(System.Text.RegularExpressions.Regex.Replace(newValue, "[^0-9]+", string.Empty));

                        if (parsed < System.Int32.MaxValue && parsed >= System.Int32.MinValue)
                        {
                            value = newValue;
                            int newInt = (int)parsed;
                            value = newInt.ToString();
                            serializedObject.SetField(newInt);
                        }
                    }
                }
                else if (serializedObject.Type == typeof(float))
                {
                    if (string.IsNullOrEmpty(newValue))
                    {
                        value = newValue;
                        serializedObject.SetField(0);
                    }
                    else
                    {

                        var match = System.Text.RegularExpressions.Regex.Match(newValue, @"([-+]?[0-9]*\.?[0-9]+)");

                        if (match.Success)
                        {
                            float parsed = System.Convert.ToSingle(match.Groups[1].Value);
                            value = newValue;
                            value = parsed.ToString();
                            serializedObject.SetField(parsed);
                        }
                    }
                }
                else if (serializedObject.Type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    value = newValue;
                }
            }

            GUILayout.EndHorizontal();
        }



    }




}