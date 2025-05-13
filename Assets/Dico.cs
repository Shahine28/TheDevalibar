using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Dico : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public string name;
        public Sprite sprite;
    }


    [SerializeField] List<Data> data;
    [SerializeField] Image _image;
    [SerializeField] Text _txt;

    public void ShowInput(string inputName)
    {

        var d = data.FirstOrDefault(i => i.name == inputName);



    }


}
