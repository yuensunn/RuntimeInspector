using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RI.SampleData
{

    [RIShow]
    public class TestData
    {
        public int intNum = 1;
        public float floatNum = 2.1f;
        public string text = "3text";
        public AudioClip audio;
        public Texture2D texture;
        public int[] arrayInt;
        public Data1[] data;
        public Data2 data2;

    }

    [RIShow]
    [System.Serializable]
    public class Data1
    {
        public int intNum = 1;
        public float floatNum = 3f;
        public string text = "Hello";
        public Data2 datadata;

        public Data3[] helloMotor;
    }

    [System.Serializable]
    public class Data2
    {
        public int intNum = 1;
        public float floatNum = 2.1f;
        public string text = "3text";

    }
    [System.Serializable]
    public class Data3
    {
        public int intNum = 1;
        public float floatNum = 2.1f;
        public string text = "3text";

    }
}