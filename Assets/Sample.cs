using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Sample : MonoBehaviour
{
    public TextAsset asset;

    public GameObject go;

    public AssetBundle bundle;




    void Start()
    {
        RI.SampleData.SampleQuestion question = JsonUtility.FromJson<RI.SampleData.SampleQuestion>(asset.text);


        AssetBundle bundle = AssetBundle.LoadFromFile(Application.dataPath + "/icons");



        foreach (var str in question.correctAnswer.Union(question.wrongAnswer).ToArray())
        {
            UnityEngine.UI.Image temp = Instantiate(go, go.transform.parent).GetComponent<UnityEngine.UI.Image>();

            temp.sprite = bundle.LoadAsset<Sprite>(str);
        }


    }
}