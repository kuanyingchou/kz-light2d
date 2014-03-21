using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System;

//>>> xmlserializer is expensive
public class KZXML<T> {
    private static XmlSerializerNamespaces ns = 
        new XmlSerializerNamespaces(); 

    static KZXML() {
        ns.Add("", "");
    }

    //[ throws exception if IO exception occurrs
    public void Save(T target, string path) {
        XmlSerializer serializer=new XmlSerializer(typeof(T));
        StreamWriter writer = new StreamWriter(path);
        serializer.Serialize(writer, target, ns);
        writer.Close();
    }

    //[ throws exception if 
    //1. IO exception occurrs, or
    //2. <input> is not well-formed representation of <T>
    public T Load(string path) {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        FileStream fileStream = new FileStream(path, FileMode.Open);
        T res=(T)serializer.Deserialize(fileStream);
        fileStream.Close();
        return res;
    }

    public string SaveString(T target) {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringWriter writer = new StringWriter();
        serializer.Serialize(writer, target, ns);
        writer.Close();
        return writer.ToString();
    }

    //[ throws exception if <input> is not a well-formed 
    //  representation of <T>
    public T LoadString(string input) {
        StringReader reader=new StringReader(input);
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        T res = (T)serializer.Deserialize(reader);
        reader.Close();
        return res;
    }

}
