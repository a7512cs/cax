using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NXOpen;
using NXOpen.Utilities;


namespace CaxUFLib
{
    public enum EdgeLoopType { Peripheral = 1, Hole = 2, Other = 3 };

    public class CaxUF_Lib
    {
        [DllImport("CaxUFLib.dll", CallingConvention = CallingConvention.Cdecl)]
        //[DllImport("CaxUFLib.dll")]
        private static extern void Init();
        [DllImport("CaxUFLib.dll")]
        public static extern void TouchArea(Tag product, Tag electrode, out TouchAreaData sTouchAreaData);

        [DllImport("CaxUFLib.dll")]
        public static extern void ElecFaceMap(Tag product, Tag electrode);

        [DllImport("CaxUFLib.dll")]
        private static extern void Terminate();

        [DllImport("CaxUFLib.dll")]
        private static extern int EdgeConnectivity(Tag edge);

        [DllImport("CaxUFLib.dll")]
        private static extern int EdgeConnectivityAngle(Tag edge, ref double outAngle);

         [DllImport("CaxUFLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetHoleLoopCount(Tag face);

        [DllImport("CaxUFLib.dll")]
        private static extern bool isEdgeForwardOnFace(Tag edge, Tag face);

        [DllImport("CaxUFLib.dll")]
        private static extern void GetFaceLoopItems(Tag face, int type, out IntPtr edges, ref int count);


        public struct PNT
        {
            public double x;
            public double y;
            public double z;
        }

        public struct TouchAreaData
        {
            public int status;           /* two objects interfere check result,   
                                   0 is interfere, 1 is touch, 2 is apart. */
            public int solid_num;        /* interfere solid number */
            //public Tag[] solid;         /* interfere solid pointer. Must be freed with UF_free after use. */
            public int face_num;         /* touch face number */
            //public Tag[] face;          /* touch face pointer. Must be freed with UF_free after use. */
            public int face_color_count; /* total color count on all touch faces */
            //public int[] face_color;      /* color value on all touch faces. Must be freed with UF_free after use. */
            //public Tag[] refer_face;       /* referrence face of electrode which is used to project area */
            public PNT csys_origin;  /* origin point of CSYS */
            public PNT x_vec;      /* x direction vector of CSYS */
            public PNT y_vec;      /* y direction vector of CSYS */
            public PNT z_vec;      /* z direction vector of CSYS */
            public double actual_area;   /* touch face actual area of two objects */
            public double project_area;  /* projected area of touch face */
            public double depth;         /* manufacturing depth of electrode */
            public double max_side_project_area;      /* the maximum project area on CSYS */
            public double angle_of_max_area_location; /* angle to the x axis of the 
                                              maximum side area location */
            public double actual_project_width_of_max_area; /* actual length in y direction,
                                                  not including gaps */
            public double min_side_project_area;            /* the minimum project area on CSYS */
            public double angle_of_min_area_location;       /* angle to the x axis of the 
                                                    minimum side area location */
            public double actual_project_width_of_min_area; /* actual length in y direction,
                                                    not including gaps */
            //public string electrode_name;  /* electrode name */
            //public string workpiece_name;  /* workpiece name */
        }

        public struct EW_project_area_info
        {
            public int status;           /* two objects interfere check result,   
                                   0 is interfere, 1 is touch, 2 is apart. */
            public int solid_num;        /* interfere solid number */
            public Tag[] solid;         /* interfere solid pointer. Must be freed with UF_free after use. */
            public int face_num;         /* touch face number */
            public Tag[] face;          /* touch face pointer. Must be freed with UF_free after use. */
            public int face_color_count; /* total color count on all touch faces */
            public int[] face_color;      /* color value on all touch faces. Must be freed with UF_free after use. */
            public Tag[] refer_face;       /* referrence face of electrode which is used to project area */
            public double[] csys_origin;  /* origin point of CSYS */
            public double[] x_vec;      /* x direction vector of CSYS */
            public double[] y_vec;      /* y direction vector of CSYS */
            public double[] z_vec;      /* z direction vector of CSYS */
            public double actual_area;   /* touch face actual area of two objects */
            public double project_area;  /* projected area of touch face */
            public double depth;         /* manufacturing depth of electrode */
            public double max_side_project_area;      /* the maximum project area on CSYS */
            public double angle_of_max_area_location; /* angle to the x axis of the 
                                              maximum side area location */
            public double actual_project_width_of_max_area; /* actual length in y direction,
                                                  not including gaps */
            public double min_side_project_area;            /* the minimum project area on CSYS */
            public double angle_of_min_area_location;       /* angle to the x axis of the 
                                                    minimum side area location */
            public double actual_project_width_of_min_area; /* actual length in y direction,
                                                    not including gaps */
            public string electrode_name;  /* electrode name */
            public string workpiece_name;  /* workpiece name */

        }


        // for external usage
        //
        public static void UfInit()
        {
            Init();
        }

        public static void UfTerminate()
        {
            Terminate();
        }

        //return:  0: smooth  1: convex  2: concave
        public static int GetEdgeConnectivity(Edge edge)
        {
            return EdgeConnectivity(edge.Tag);
        }

        //return:  0: smooth  1: convex  2: concave
        // angle is in degree
        public static int GetEdgeConnectivityAngle(Edge edge, out double angle)
        {
            angle = 0;
            int state = EdgeConnectivityAngle(edge.Tag, ref angle);
            angle = angle * 180 / Math.PI;

            return state;
        }

        public static bool HasHoleLoop(Face face)
        {
            int count = GetHoleLoopCount(face.Tag);

            return (count > 0) ? true : false;
        }

        public static bool IsEdgeForwardOnFace(Edge edge, Face face)
        {
            return isEdgeForwardOnFace(edge.Tag, face.Tag);
        }

        //type: 1 - Peripheral  2 - Hole  3 - Other
        public static Edge[] GetLoopEdges(Face face, EdgeLoopType type)
        {
            int[] objs;
            int count = 0;
            int loopType = (int)type;
            IntPtr buffer;

            GetFaceLoopItems(face.Tag, loopType, out buffer, ref count);
            if (count > 0)
            {
                objs = new int[count];
                Marshal.Copy(buffer, objs, 0, count);

                Edge[] result = new Edge[count];
                int i = 0;
                foreach (int obj in objs)
                {
                    result[i++] = (Edge)NXObjectManager.GetObjectFromUInt(Convert.ToUInt32(obj));
                }

                Marshal.FreeCoTaskMem(buffer); ////dispose of unneeded memory

                return result;
            }
            else
                return null;
        }
    }
}
