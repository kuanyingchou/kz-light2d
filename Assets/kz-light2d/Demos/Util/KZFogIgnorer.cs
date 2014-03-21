using UnityEngine;
using System.Collections;

//Used to ignore fog settings for a camera

//2013.7.25  ken  initial version, modified from 
//                http://wiki.unity3d.com/index.php?title=Fog_Layer
[AddComponentMenu ("Rendering/Fog Layer")]
[RequireComponent (typeof(Camera))]
public class KZFogIgnorer : MonoBehaviour {
    private bool revertFogState = false;
    public bool ignoreFog = true;
 
    public void  OnPreRender () {
        revertFogState = RenderSettings.fog;
        RenderSettings.fog = !ignoreFog;
    }
 
    public void  OnPostRender () {
        RenderSettings.fog = revertFogState;
    }
}
