using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXOpen.CAM;
using CimforceCaxTwPublic;

namespace ETableWork
{
    class Class002_MyClass
    {
    }
    // 基準面資訊
    public struct RefData
    {
        public string REF_A_HANDLE;
        public string REF_B_HANDLE;
        public string REF_C_HANDLE;
        public string REF_D_HANDLE;
        public string REF_X_HANDLE;
        public string REF_Y_HANDLE;
        public string REF_Z_HANDLE;
        public string REF_A_BOX;
        public string REF_B_BOX;
        public string REF_C_BOX;
        public string REF_D_BOX;
    }
    // 基準距離
    public struct BaseDist
    {
        public double MIN_X_POSITION;
        public double MIN_Y_POSITION;
        public double MIN_Z_POSITION;
        public double MAX_X_POSITION;
        public double MAX_Y_POSITION;
        public double MAX_Z_POSITION;
    }

    // 裝夾/校正之參數
    public struct ClampCalibrateParam
    {
        public string BASE_CORNER_QUADRANT;
        public string AXIS_ANGLE;
        public string Z_AXIS_BOTTOM_HEIGHT;
        public string BASE_CORNER_VECTOR;
    }
    // 基準面資訊
    public struct baseFace
    {
        public CaxPart.BaseCorner baseFaceA;
        public CaxPart.BaseCorner baseFaceB;
        public CaxPart.BaseCorner baseFaceC;
        public CaxPart.BaseCorner baseFaceD;
        public List<CaxPart.BaseCorner> baseFaceXLst;   // 20150617 基準面偏移
        public List<CaxPart.BaseCorner> baseFaceYLst;   // 20150617 基準面偏移
        public List<CaxPart.BaseCorner> baseFaceZLst;   // 20150617 基準面偏移
        public bool hasA;
        public bool hasB;
        public bool hasC;
        public bool hasD;
        public bool hasX;
        public bool hasY;
        public bool hasZ;
    }

    public struct ExportWorkTabel
    {
        public string Z_MOVE;   // 工件T面轉BO面Z軸偏移的距離 // 20150618 改成C面到中心距離(電極取正工件不取正)
        public string Z_BASE;   // 基準角底面到座標原點的距離 // 20150618 Zmin到中心距離(取正)
        public string X_OFFSET; // 基準角長面(距離原點較長的面)到座標原點的距離
        public string Y_OFFSET; // 基準角短面(距離原點較短的面)到座標原點的距離
    }

    public struct ListToolLengeh
    {
        public Operation oper;
        public bool isOK;
        public bool isOverToolLength;
        public string tool_name;
        public string tool_ext_length;
        public string oper_name;
        public double cutting_length;
        public double cutting_length_max;
        public double part_offset;
    }

    public struct ToolLengehStatus
    {
        //public bool isOverToolLength;
        public string tool_name;
        public string tool_ext_length;
        public double cutting_length_max;
    }

    public struct WorkPieceElecTaskKey
    {
        public string MOLD_NO;
        public string DES_VER_NO;
        public string WORK_NO;
        public string PART_NO;
        public string MFC_NO;
        public string MFC_TASK_NO;
        public string TASK_NO;
        public string MAC_MODEL_NO;
        public string TASK_SRNO;
    }
    // 配置檔ETable.txt
    public class ConfigData
    {
        public string companyName { get; set; }
        public string hasCMM { get; set; }
        public string hasMultiFixture { get; set; }
    }

    // cam2mes.dat
    public class Work
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Datum
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class ToolPath
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class Program
    {
        public List<Datum> data { get; set; }
        public List<ToolPath> tool_path { get; set; }
    }
    public class Cam2MesData
    {
        public List<Work> work { get; set; }
        public List<Program> programs { get; set; }
    }

}
