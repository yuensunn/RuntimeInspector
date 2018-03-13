using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RI.SampleData
{
    [System.Serializable]
    [RIShow]
    public class SampleQuestion
    {
        public string question;

        [DataType(typeof(Sprite))]
        public string[] correctAnswer;

        [DataType(typeof(Sprite))]
        public string[] wrongAnswer;



    }
}