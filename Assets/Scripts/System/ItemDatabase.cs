using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

[XmlRoot("ItemCollection")]
public class ItemDatabase
{
    [XmlArray("Items"), XmlArrayItem("Item")]
    public List<Item> items = new List<Item>();

    public static ItemDatabase Load()
    {
        string path = "Item";
        TextAsset _xml = Resources.Load<TextAsset>(path);
        if (_xml != null)
        {
            Debugging.LogSystem("ItemDatabase is loading... " + path + ".xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ItemDatabase));
            var reader = new StringReader(_xml.text);
            ItemDatabase itemDB = serializer.Deserialize(reader) as ItemDatabase;
            reader.Close();
            return itemDB;
        }
        else
        {
            Debugging.LogSystemWarning("ItemDatabase wasn't loaded. >> " + path + " is null. >>");
            return null;
        }
    }
}
