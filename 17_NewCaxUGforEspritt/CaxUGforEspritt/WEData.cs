using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen;
using NXOpen.Assemblies;

namespace WeData
{
    public struct TolValue
    {
        public string Tol_Region;
        public string Tol_Upper;
        public string Tol_Lower;
    }

    public struct WeFaceGroup
    {
        public  NXOpen.Assemblies.Component comp;
        public  string faceGroup;
        public  List<FaceGroupPnt> sFaceGroupPnt;
        public  int isAnalyzeSucess;
        public  string WE_FIX;
        public  string IS_FIX;
        public  string WE_FIX_PATH;
    }

    public struct WeGroupFacePnt
    {
        public WeListKey sWeListKey;
        public WeFaceGroup sWeFace;
        public System.Windows.Forms.Form mainForm;
    }

    public struct WeListKey
    {
        public string compName;
        public string section;
        public string wkface;
    }

    public struct FaceGroupPnt
    {
        public List<Face> faceOccAry;
        public string pnt_x;
        public string pnt_y;
    }

    public struct WorkPiece
    {
        public double WP_Length;
        public double WP_Wide;
        public double WP_Height;
    }

    public struct skey
    {
        public Component comp;
        public string section;
        public string wkface;
    }

    public struct skeyFailed
    {
        public Component comp;
        public string compName;
        public string section;
        public string wkface;
    }

    public class COMPANY
    {
        public string COMPANYNAME { get; set; }
        public string WENAME { get; set; }
    }

    public class COMPANY_ARY
    {
        public List<COMPANY> COMPANYARY { get; set; }
    }
}