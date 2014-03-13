using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;

public enum Sex { Male, Female}

public class Student {
    public string name;
    public int age;
    public Sex sex;
    public Student() {}
    public Student(string n, int a, Sex s) {
        name=n;
        age=a;
        sex=s;
    }
    public override string ToString() {
        return name+" ("+age+"), "+sex;
    }
}

class School {
    //the generated xml will not contain null elements 
    [XmlElement(IsNullable = false)] 
    Student[] students;
}

public class TestKZXML: MonoBehaviour {
    public void Awake() {
        //TestString();
        //TestSingle();
        //TestList();
        TestHash();
    }
    public static void TestString() {
        string input="hello, <§A¦nworld";
        KZXML<string> xml=new KZXML<string>();
        try { 
            xml.Save(input, "hello.xml");
        } catch(Exception) {
            Debug.LogError("Failed to Save!");
        }
    }
    public static void TestSingle() {
        string FILE_NAME="student.xml";

        Student s=new Student("ken", 27, Sex.Male);

        KZXML<Student> xml=new KZXML<Student>();

        try { 
            xml.Save(s, FILE_NAME);
        } catch(Exception) {
            Debug.LogError("Failed to Save!");
        }

        Student t=null;
        try {
            t=xml.Load(FILE_NAME);
        } catch(Exception) {
            Debug.LogError("Failed to Load!");
        }

        if(t==null || t.ToString() != "ken (27), Male") {
            Debug.LogError(t);
        } else {
            Debug.Log("OK!");
        }
    }
    public static void TestList() {
        string FILE_NAME="student.xml";

        List<Student> students=new List<Student>();

        students.Add(new Student("ken", 27, Sex.Male));
        students.Add(new Student("tony", 27, Sex.Male));
        students.Add(new Student("albert", 37, Sex.Male));

        KZXML<List<Student>> xml=new KZXML<List<Student>>();

        try {
            xml.Save(students, FILE_NAME);
            List<Student> t=xml.Load(FILE_NAME);
            foreach(Student s in t) {
                Debug.Log(s);
            }
        } catch(Exception) {
            Debug.LogError("Failed to Save or Load!");
        }
    }
    
    //[ this will fail since .net does not support IDictionary!
    public static void TestHash() {
        Dictionary<string, Student> students=
            new Dictionary<string, Student>();
        string[] names= {"ken", "tony", "albert"};
        students.Add(names[0], new Student(names[0], 27, Sex.Male));
        students.Add(names[1], new Student(names[1], 27, Sex.Male));
        students.Add(names[2], new Student(names[2], 37, Sex.Male));

        KZXML<Dictionary<string, Student>> xml=
            new KZXML<Dictionary<string, Student>>();

        string res=xml.SaveString(students);
        Debug.Log(res);
    }
}
