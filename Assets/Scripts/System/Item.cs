using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Item
{
    [XmlAttribute("id")]
    public int id;

    [XmlElement("Name")]
    public string name;

    [XmlElement("Description")]
    public string description;

    [XmlElement("ItemType")]
    public int itemtype;

    [XmlElement("WeaponType")]
    public int weapontype;

    [XmlElement("MinAttack")]
    public int minAttack;

    [XmlElement("MaxAttack")]
    public int maxAttack;

    [XmlElement("Value")]
    public int value;

}
