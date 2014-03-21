using UnityEngine;
using System.Collections;

//todo: add auto indent
//2013.3.5  ken  initial version
public class KZCodeBuilder {
    public string tabStr="    ";
    public string ws=" ";
    public string commentStart="//";
    private System.Text.StringBuilder sb=new System.Text.StringBuilder();
    //private int indent=0; //>>>
    
    public KZCodeBuilder p(string text) { sb.Append(text); return this; }
    public KZCodeBuilder pn() { sb.Append(System.Environment.NewLine); return this; }
    public KZCodeBuilder pn(string text) { sb.Append(text).Append(System.Environment.NewLine); return this; } //print line
    public KZCodeBuilder ps(string text) { sb.Append(text).Append(ws); return this; } //print space
    public KZCodeBuilder tab() {sb.Append(tabStr); return this; }
    public KZCodeBuilder enter() { sb.Append(System.Environment.NewLine); return this; }
    public KZCodeBuilder space() {sb.Append(ws); return this; }
    public KZCodeBuilder pc(string text) { return ps(commentStart).p(text); } //print comment
    public KZCodeBuilder pcn(string text) { return ps(commentStart).pn(text); }
    public KZCodeBuilder pf(string sig) { //print function
        return ps(sig).pn("{}");
    }
    public KZCodeBuilder pf(string sig, string body) {
        return ps(sig).pn("{").tab().tab().pn (body).tab().pn("}");
    }
//      private CodeBuilder AutoIndent() { //>>>
//          for(int i=0; i<indent; i++) {
//              sb.AppendLine(tabStr);
//          }
//      }
    public override string ToString() {
        return sb.ToString();
    }
}
