using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXOpen;
using NXOpen.UF;
using NXOpen.Utilities;

using System.IO;
using System.Data;
using NXOpen.Features;
using NXOpen.Assemblies;
using CaxUFLib;
using System.Runtime.InteropServices;

namespace NXCustomerComponent
{
    
//==============================================================
    public class FeatureAutoReg
    {
        // class members
        private Session theSession_;
        private UI theUI_;
        private UFSession theUfSession_;
        private CLog clog;

        private String Feat_type_attr = "FEATURE_TYPE";// 類型名稱
        private  String Feat_sub_type_attr ="CIM_FEAT_SUB_TYPE";//0~n
        private String Feat_Dia_attr = "CIM_FEAT_DIA_TYPE";// 最大直徑

        public class FeatureGroup
        {
            public string feat_type;
            public List<Tag> faceAry;
            public List<double> Hole_DiameterAry;
            public List<double> Hole_HightAry;
            //public List<String> opertionDirAry;
            //public bool isThroughHole;//是否通孔
            public double Main_diameter;
            public List<SubGroup> SubGroupAry;// sub Group Ary
        }

        public class SubGroup
        {
            public List<Tag> faceAry;
        }


        public class FaceGroup
        {
            public String feat_type;
            public Face face_cur;//自已
            public List<Tag> adjFace;//鄰面
            public String feat_sub_type;//群組id
            //public List<CFace.CFaceData> Faceinfo;//鄰面資料

        }

        public FeatureAutoReg()
        {
            try
            {
                theSession_ = Session.GetSession();
                theUI_ = UI.GetUI();
                theUfSession_ = UFSession.GetUFSession();
                clog = new CLog();
            }
            catch (NXOpen.NXException ex)
            {
                clog.showlog(ex.Message);

            }
        }

        public void showlog(String log)
        {
            //lwu_.WriteLine(log + "\n");
            clog.showlog(log + "\n");
        }

        public void showmsg(String msg)
        {
            clog.showmsg(msg);
        }


        public void getFeatureGroup(Part work_prt, ref List<String> f_typeAry)
        {
            CLog log = new CLog();
            List<FeatureGroup> HoleGroupAry = new List<FeatureGroup>();
            List<Face> cylindricalNoAttrAry = new List<Face>();

            // get all faces
            Body[] bodyArys = work_prt.Bodies.ToArray();
            if (bodyArys.Length == 0)
                return;
            Body bodyPrototype = bodyArys[0];

            try
            {
                Face[] facePrototypeAry = bodyPrototype.GetFaces();

                // 1. 取得所有 有屬性參數的 Face
                List<FaceGroup> FeaturetypeAry = new List<FaceGroup>();
                List<FaceGroup> Featuretype_NoneAry = new List<FaceGroup>();
                String attr_value = "";
                for (int i = 0; i < facePrototypeAry.Length; i++)
                {
                    try
                    {
                        attr_value = "";
                        attr_value = facePrototypeAry[i].GetStringAttribute(Feat_type_attr);
                        FaceGroup featG = new FaceGroup();
                        featG.feat_type = attr_value;
                        featG.face_cur = facePrototypeAry[i];
                        FeaturetypeAry.Add(featG);
                    }
                    catch (System.Exception ex)
                    {
                        FaceGroup featG = new FaceGroup();
                        featG.feat_type = "";
                        featG.face_cur = facePrototypeAry[i];
                        Featuretype_NoneAry.Add(featG);
                        continue;
                    }
                }

                /*List<FaceGroup> listCustomer3 = (from fg in FeaturetypeAry  
                                                orderby fg.feat_type descending //ascending  
                                                select fg).ToList<FaceGroup>(); 
                */
                // 排序
                List<FaceGroup> listSort = FeaturetypeAry.OrderBy(s => s.feat_type).ToList<FaceGroup>();
                // 計算不重複個數
                List<FaceGroup> list = listSort
                                .GroupBy(a => a.feat_type)
                                .Select(g => g.First())
                                .ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    log.showlog(" i=" + i + "==" + list[i].feat_type);
                    f_typeAry.Add(list[i].feat_type);
                }


            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }

            return;
        }


        // 尋找相鄰的相鄰面
        public void getFeature_SubGroup_adj(Tag selfFace_Tag ,              //in : 目前的面
                                            ref List<Tag> AllFaceeAry ,     //in : All face
                                            ref List<Tag> ContainsFaceAry   //in/out : 有在All face 中找到的
                                            )
        {

            bool isused = ContainsFaceAry.Contains(selfFace_Tag);
            if (!isused) // 沒被使用
            {               
                ContainsFaceAry.Add(selfFace_Tag);
                showlog("> Contain = " + ContainsFaceAry.Count);
                showlog("> Contain:" + ContainsFaceAry[ContainsFaceAry.Count-1]);
            }

            List<Tag> adj_current = new List<Tag>();
            CFace cface_ = new CFace();

            // 取相鄰面
            cface_.adjFaces(selfFace_Tag,out adj_current);
            // 先把自己加入
           
            showlog("> self :" + selfFace_Tag);
            String sss="";
            for (int i = 0; i < adj_current.Count;i++ )
            {
                sss += adj_current[i].ToString();
                sss += "\t";

            }
            showlog("\tadj :" + sss);


            String ss = "";
            String dd="\t";
            for (int i = 0; i < adj_current.Count;i++ )
            {

                bool isused1 = ContainsFaceAry.Contains(adj_current[i]);
                if (isused1)
                    continue;
                
                // 是否存在所有面中
                bool isfound = AllFaceeAry.Contains(adj_current[i]);
                if (isfound)
                {
                    showlog(" ==" + adj_current[i]);
                    getFeature_SubGroup_adj(adj_current[i], ref AllFaceeAry, ref ContainsFaceAry);
                    dd += "\t";
                    //ContainsFaceAry.Add(adj_current[i]);
                    ss = String.Format(dd + "adj1 : " + adj_current[i]);
                    showlog(ss);
                }

                // 當前相鄰面的相鄰面
                
            }



        }

        // 尋找面群 main ()  
        public void getFeature_SubGroup(Part work_prt, String id_name, 
                                        ref List<FeatureGroup> Out_FeatureGroupAary)
        {
           
            //List<Face> cylindricalNoAttrAry = new List<Face>();
            CFace cface_ = new CFace();
            // get all faces
            Body[] bodyArys = work_prt.Bodies.ToArray();
            if (bodyArys.Length == 0)
                return;
            Body bodyPrototype = bodyArys[0];

            try
            {
                Face[] facePrototypeAry = bodyPrototype.GetFaces();

                // 1. 取得所有 有屬性參數的 Face
                List<FaceGroup> FeaturetypeAry = new List<FaceGroup>();
                List<FaceGroup> Featuretype_NoneAry = new List<FaceGroup>();
                String attr_value = "";
                List<Tag> AllFaceAry = new List<Tag>();
                for (int i = 0; i < facePrototypeAry.Length; i++)
                {
                    try
                    {
                        attr_value = "";
                        attr_value = facePrototypeAry[i].GetStringAttribute(Feat_type_attr);
                        if (String.Compare(id_name, attr_value, true) == 0)
                        {
                            FaceGroup featG = new FaceGroup();
                            //1.
                            featG.feat_type = attr_value;
                            //2.
                            featG.face_cur = facePrototypeAry[i]; // face
                            //3. 取資料
                            FeaturetypeAry.Add(featG);
                            AllFaceAry.Add(facePrototypeAry[i].Tag);
                            //ftypeAry.Add(featG);
                        }

                    }
                    catch (System.Exception ex)
                    {
                        FaceGroup featG = new FaceGroup();
                        featG.feat_type = "";
                        featG.face_cur = facePrototypeAry[i];
                        Featuretype_NoneAry.Add(featG);
                        continue;
                    }
                }

//                 print log                            
//                 for (int i = 0; i < FeaturetypeAry.Count;i++ )
//                 {
//                     showlog(">org " + i + "  : Tag ID " + FeaturetypeAry[i].face_cur.Tag);
//                 }


                if (id_name == "Cooling_Hole")
                {
                    getFeature_CoolingSubGroup(work_prt, AllFaceAry,ref Out_FeatureGroupAary);
                    return;
                }
            

                // 2.找出群組面組
                List<FeatureGroup> FeatureGroupAary = new List<FeatureGroup>();
                List<Tag> usedFaceAry = new List<Tag>();
                int group_i = 0;
                for( int i=0;i<FeaturetypeAry.Count;i++ )
                {
                    bool isused = usedFaceAry.Contains(FeaturetypeAry[i].face_cur.Tag);
                    if (isused) // 已被使用
                        continue;

                    List<Tag> groupFaces = new List<Tag>();
                    groupFaces.Clear();
                    Tag cur_tag = FeaturetypeAry[i].face_cur.Tag;
                    getFeature_SubGroup_adj(cur_tag, ref AllFaceAry, ref groupFaces);

                    showlog("  ===========================");
                    String g1 = "";
                    for (int j = 0; j < groupFaces.Count; j++)
                    {
                        g1 += groupFaces[j].ToString();
                        g1 += "\t";
                    }
                    showlog("  >" + g1);

                    // start 取不重複----------------------------------------
                    usedFaceAry.AddRange(groupFaces);
                    List<Tag> list = new List<Tag>();
                    foreach (Tag each_tag in usedFaceAry)
                    {
                        if (!list.Contains(each_tag))
                            list.Add(each_tag);
                    }
                    usedFaceAry.Clear();
                    usedFaceAry.AddRange(list);

                    showlog("group1 :");
                    g1 = "";
                    for (int j = 0; j < usedFaceAry.Count; j++)
                    {
                        g1 += usedFaceAry[j].ToString();
                        g1 += "\t";
                    }
                    showlog("  >" + g1);
                    // end 取不重複----------------------------------------



                    FeatureGroup fg = new FeatureGroup();
                    fg.feat_type = FeaturetypeAry[i].feat_type;
                    fg.faceAry = new List<Tag>();
                    fg.Hole_DiameterAry = new List<double>();
                    fg.Hole_HightAry = new List<double>();
                    // get face data
                    for (int j = 0; j < groupFaces.Count; j++)
                    {
                        CFace.CFaceData fd_temp = cface_.getFacedata(groupFaces[j]);
                        // 直徑                    
                        fg.Hole_DiameterAry.Add(fd_temp.radius * 2);
                        
                        // 深度
                        //fg.Hole_HightAry = new List<double>();
                        fg.faceAry.Add(groupFaces[j]);

                        // set CIM_FEAT_SUB_TYPE string 0~n
                        //setFaceGroup_StringAttribute(groupFaces[j], group_i.ToString());
                    }
                    
                    FeatureGroupAary.Add(fg);
                    group_i++;

                }// end for i
                Out_FeatureGroupAary.AddRange(FeatureGroupAary);

            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }

            return;
        }

        // 尋找面群 main ()  修改加入 sub_type 分類 
        public void getFeature_SubGroup_new(Part work_prt, String id_name,
                                        ref List<FeatureGroup> Out_FeatureGroupAary)
        {

            //List<Face> cylindricalNoAttrAry = new List<Face>();
            CFace cface_ = new CFace();
            // get all faces
            Body[] bodyArys = work_prt.Bodies.ToArray();
            if (bodyArys.Length == 0)
                return;
            Body bodyPrototype = bodyArys[0];

            try
            {
                Face[] facePrototypeAry = bodyPrototype.GetFaces();

                // 1. 取得所有 有屬性參數的 Face
                List<FaceGroup> FeaturetypeAry = new List<FaceGroup>();
                List<FaceGroup> Featuretype_NoneAry = new List<FaceGroup>();
                String attr_value = "";
                List<Face> AllFaceAry = new List<Face>();

                int group_id_max = -1;
                
                for (int i = 0; i < facePrototypeAry.Length; i++)
                {
                    try
                    {
                        attr_value = "";
                        attr_value = facePrototypeAry[i].GetStringAttribute(Feat_type_attr);//feature_type                       
                        if (String.Compare(id_name, attr_value, true) == 0)
                        {                         
                            FaceGroup featG = new FaceGroup();
                            featG.feat_type = attr_value;
                            try
                            {
                                attr_value = "";
                                attr_value = facePrototypeAry[i].GetStringAttribute(Feat_sub_type_attr);// sub_type
                                featG.feat_sub_type = attr_value;

                                if (attr_value != "")
                                {
                                    int id = int.Parse(attr_value);
                                    if (group_id_max < id)
                                        group_id_max = id;//找最大值
                                }
                               

                            }
                            catch (System.Exception ex)
                            { 
                                featG.feat_sub_type = "";
                            }

                            //1.
                            //2.
                            featG.face_cur = facePrototypeAry[i]; // face
                            //3. 取資料
                            FeaturetypeAry.Add(featG);
                            AllFaceAry.Add(facePrototypeAry[i]);
                            //ftypeAry.Add(featG);

                            //-------------------------------------

                        }

                    }
                    catch (System.Exception ex)
                    {
                        FaceGroup featG = new FaceGroup();
                        featG.feat_type = "";
                        featG.face_cur = facePrototypeAry[i];
                        Featuretype_NoneAry.Add(featG);
                        continue;
                    }
                }

//                 print log                            
//                 for (int i = 0; i < FeaturetypeAry.Count;i++ )
//                 {
//                     showlog(">org " + i + "  : Tag ID " + FeaturetypeAry[i].face_cur.Tag);
//                 }


                showlog(" group max " + group_id_max);
                // 2.找出群組面組
                List<FeatureGroup> FeatureGroupAary = new List<FeatureGroup>();
                List<Tag> usedFaceAry = new List<Tag>();

                FeatureGroup featg = new FeatureGroup();
                featg.faceAry = new List<Tag>();
                featg.Hole_DiameterAry = new List<double>();
                featg.SubGroupAry = new List<SubGroup>();
                double Main_dismeter = 100;
                for (int j = 0; j <= group_id_max; j++)
                {

                    SubGroup subG = new SubGroup();
                    subG.faceAry = new List<Tag>();
                    // 利用 屬性分類 
                    for (int i = 0; i < FeaturetypeAry.Count; i++)
                    {
                        int id_temp = int.Parse(FeaturetypeAry[i].feat_sub_type);
                        if (j == id_temp)
                        {
                            featg.feat_type = FeaturetypeAry[i].feat_type;
                            CFace.CFaceData fd_temp = cface_.getFacedata(FeaturetypeAry[i].face_cur.Tag);
                            featg.faceAry.Add(FeaturetypeAry[i].face_cur.Tag);
                            featg.Hole_DiameterAry.Add(fd_temp.radius * 2);
                            if (fd_temp.type_face == UFConstants.UF_cylinder_subtype)
                            {
                                if (fd_temp.radius * 2 < Main_dismeter && fd_temp.radius != 0)
                                {
                                    Main_dismeter = fd_temp.radius * 2;
                                    showlog("diameter : " + Main_dismeter);
                                }
                            }

                            subG.faceAry.Add(FeaturetypeAry[i].face_cur.Tag);
                        }
                       
                    }
                    featg.SubGroupAry.Add(subG);
                }

                // 填入值徑
                //double diameter = getFaceGroupMiniDiameter(featg);
                //diameter = CTrans.DoubleToAccuracyDouble_ToEvent(diameter, 2);//四捨五入
                featg.Main_diameter = Main_dismeter;
                Out_FeatureGroupAary.Add(featg);
                


            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }

            return;
        }

        // 尋找最小直徑值
        public double getFaceGroupMiniDiameter(FeatureGroup fgobj)
        {

            // 排序
            List<double> listSort = fgobj.Hole_DiameterAry.OrderBy(s => s).ToList<double>();

            List<double> list = new List<double>();
            foreach (double each_tag in listSort)
            {
                if (!list.Contains(each_tag))
                    list.Add(each_tag);
            }

            if (list.Count == 0)
                return -1;

            if (list[0] < 0.00001 && list[0] > -0.00001)
                return list[1];


            return list[0];

        }

        // 針對 水孔分群
        public void getFeature_CoolingSubGroup(Part work_prt,
                                        List<Tag> allFace,
                                        ref List<FeatureGroup> Out_FeatureGroupAary)
        {
            // 1. cylinder face and none cylinder face split
            List<Tag> ClyinderFaceAry = new List<Tag>();
            List<Tag> NoneClyinderFaceAry = new List<Tag>();

            List<CFace.CFaceData> ClyinderFaceDataAry = new List<CFace.CFaceData>();
            List<CFace.CFaceData> NoneClyinderFacDataeAry = new List<CFace.CFaceData>();

            CFace cf = new CFace();
            for (int i = 0; i < allFace.Count;i++ )
            {
                CFace.CFaceData cf_data = cf.getFacedata(allFace[i]);

                if (cf_data.type_face == UFConstants.UF_cylinder_subtype)
                {
                    ClyinderFaceAry.Add(allFace[i]);
                    ClyinderFaceDataAry.Add(cf_data);
                }
                else
                {
                    NoneClyinderFaceAry.Add(allFace[i]);
                    NoneClyinderFacDataeAry.Add(cf_data);
                }

            }//end for

            showlog("cylinder size :" + ClyinderFaceAry.Count + " == none Cylinder size :" + NoneClyinderFaceAry.Count);

            // 2.
            List<Tag> used_tagAry  = new List<Tag>();//已被使用過的
            for (int i=0;i<ClyinderFaceAry.Count;i++)
            {
                FeatureGroup fg_temp = new FeatureGroup();
                fg_temp.faceAry = new List<Tag>();
                fg_temp.Hole_DiameterAry = new List<double>();

                bool is_uses = used_tagAry.Contains(ClyinderFaceAry[i]);
                if (is_uses)
                    continue;

                for(int j=0;j<ClyinderFaceAry.Count;j++)
                {
                    //showlog("tag f1 :" + ClyinderFaceAry[i] + "  tag f2 : " + ClyinderFaceAry[j]);
                    // 判斷同軸
                    bool isTheSame = cf.isCylinderTheSame_Axis_CenterPoint(ClyinderFaceDataAry[i],ClyinderFaceDataAry[j]);
                    if (isTheSame)
                    {
                        
                        fg_temp.feat_type = "Cooling_Hole";                       
                        fg_temp.faceAry.Add(ClyinderFaceAry[j]);//加入面
                        used_tagAry.Add(ClyinderFaceAry[j]);
                        fg_temp.Hole_DiameterAry.Add(ClyinderFaceDataAry[j].radius * 2);
                    }

                }
                //showlog("tag f1 :" + ClyinderFaceAry[i] + " === " + fg_temp.faceAry.Count);
                Out_FeatureGroupAary.Add(fg_temp);
            }

            List<int> haveNoneCly = new List<int>();
            // 將剩下的面歸類
            List<Tag> NoneCly_used = new List<Tag>();
            for (int i=0;i< NoneClyinderFaceAry.Count;i++)
            {
                if ( NoneCly_used.Contains(NoneClyinderFaceAry[i]))//被用掉
                    continue;

                for (int j = 0; j < Out_FeatureGroupAary.Count ; j++ )
                {
                    // 每個-群組面
                    List<Tag> Alladjface = new List<Tag>();
                    for (int k=0;k< Out_FeatureGroupAary[j].faceAry.Count;k++)
                    {
                        Tag ff_temp = Out_FeatureGroupAary[j].faceAry[k];
                        List<Tag> adjface = new List<Tag>();
                        cf.adjFaces(ff_temp, out adjface);
                        Alladjface.AddRange(adjface); // 所有鄰面
                        
                    }//end for k

                    if (Alladjface.Contains(NoneClyinderFaceAry[i]))//是相鄰
                    {
                        showlog("==>is used " + j + "==" + NoneClyinderFaceAry[i]);
                        CFace.CFaceData fdd = cf.getFacedata(NoneClyinderFaceAry[i]);

                        Out_FeatureGroupAary[j].faceAry.Add(NoneClyinderFaceAry[i]);
                        Out_FeatureGroupAary[j].Hole_DiameterAry.Add(0);
                        NoneCly_used.Add(NoneClyinderFaceAry[i]);
                        haveNoneCly.Add(j);
                    }


                
                }//end for j (Group)

            }//end for i


            // ***封閉水孔分類錯誤修改 (有bug -> 如果被分三段 分群會出錯)
            List<FeatureGroup> reGroup = new List<FeatureGroup>();
            List<int> newHaves = new List<int>();
            getNoneduplicate_ListINT(haveNoneCly, ref newHaves);

            List<Tag> usedNo = new List<Tag>();
            //usedNo.Except
            for (int i = 0; i < NoneClyinderFaceAry.Count; i++)
            {
                // 1. 判斷
                if (usedNo.Contains(NoneClyinderFaceAry[i]))
                    continue;

                for (int j = 0; j < newHaves.Count; j++)
                {
                   List<Tag> subGruop = new List<Tag>();
                   int haveNoneCly_i = newHaves[j];
                   List<Tag> Allf = new List<Tag>();
                   Allf.AddRange(Out_FeatureGroupAary[haveNoneCly_i].faceAry);
                   getFeature_SubGroup_adj(NoneClyinderFaceAry[i],
                                           ref Allf,
                                           ref subGruop);


                   if (subGruop.Count == 1)
                       continue;
                    
                   FeatureGroup fg_temp = new FeatureGroup();
                   fg_temp.faceAry = new List<Tag>();
                   fg_temp.Hole_DiameterAry = new List<double>();
                   
                   fg_temp.feat_type = "Cooling_Hole";
                   fg_temp.faceAry.AddRange(subGruop);
                   
                   for (int k = 0; k < subGruop.Count;k++ )
                   {
                       CFace.CFaceData fdd = cf.getFacedata(subGruop[k]);
                       if(fdd.type_face == 16)
                            fg_temp.Hole_DiameterAry.Add(fdd.radius*2);
                       else
                           fg_temp.Hole_DiameterAry.Add(0);
                   }

                   reGroup.Add(fg_temp);
                   usedNo.AddRange(subGruop);
                }

                List<Tag> inTemp = new List<Tag>();
                inTemp.AddRange(usedNo);
                getNoneduplicate_List(usedNo, ref inTemp);
                usedNo.Clear();
                usedNo.AddRange(inTemp);            
            }


            // 最後重新整理
            List<FeatureGroup> New_Group = new List<FeatureGroup>();
            for (int i=0;i<Out_FeatureGroupAary.Count;i++)
            {
                if (newHaves.Contains(i))
                    continue;
                New_Group.Add(Out_FeatureGroupAary[i]);
            }
            New_Group.AddRange(reGroup);

            Out_FeatureGroupAary.Clear();
            Out_FeatureGroupAary.AddRange(New_Group);

            // 設定參數
            for (int i = 0; i < Out_FeatureGroupAary.Count;i++ )
            {
                for (int j = 0; j < Out_FeatureGroupAary[i].faceAry.Count;j++ )
                {
                    setFaceGroup_StringAttribute(Out_FeatureGroupAary[i].faceAry[j], i.ToString());
                }
            }



        }// end public


        // 設定屬性
        public void setFaceDiameter_StringAttribute(Tag face, String Value_str)
        {
            // 1. 判斷是否存在 CIM_FEAT_SUB_TYPE
            String attr_name = Feat_Dia_attr;
            String attr_value = "";
            try
            {
                Face face_obj = (Face)CTrans.TagtoNXObject(face);
                //attr_value = face_obj.GetStringAttribute(attr_name);
                face_obj.SetAttribute(attr_name, Value_str);
            }
            catch (System.Exception ex)
            {

            }

        }

        // 設定屬性
        public void setFaceGroup_StringAttribute(Tag face , String Value_str )
        {
            // 1. 判斷是否存在 CIM_FEAT_SUB_TYPE
            String attr_name = Feat_sub_type_attr;
            String attr_value = "";
            try
            {
                Face face_obj = (Face)CTrans.TagtoNXObject(face);
                //attr_value = face_obj.GetStringAttribute(attr_name);
                face_obj.SetAttribute(attr_name, Value_str);
            }
            catch (System.Exception ex)
            {

            }
           
        }

        // 設定屬性
        public void setFaceType_StringAttribute(Tag face, String Value_str)
        {
            // 1. 判斷是否存在 CIM_FEAT_SUB_TYPE
            String attr_name = Feat_type_attr;
            String attr_value = "";
            try
            {
                Face face_obj = (Face)CTrans.TagtoNXObject(face);
                //attr_value = face_obj.GetStringAttribute(attr_name);
                face_obj.SetAttribute(attr_name, Value_str);
            }
            catch (System.Exception ex)
            {

            }

        }


        // 設定屬性
        public void DeleteFaceDiameter_StringAttribute(Tag face)
        {
            // 1. 判斷是否存在 
            String attr_name = Feat_Dia_attr;
            String attr_value = "";
            try
            {
                Face face_obj = (Face)CTrans.TagtoNXObject(face);
                //attr_value = face_obj.GetStringAttribute(attr_name);            
                face_obj.DeleteAttributeByTypeAndTitle(NXObject.AttributeType.String, attr_name);
            }
            catch (System.Exception ex)
            {

            }

        }

        // 設定屬性
        public void DeleteFaceType_StringAttribute(Tag face)
        {
            // 1. 判斷是否存在 
            String attr_name = Feat_sub_type_attr;
            String attr_value = "";
            try
            {
                Face face_obj = (Face)CTrans.TagtoNXObject(face);
                //attr_value = face_obj.GetStringAttribute(attr_name);            
                face_obj.DeleteAttributeByTypeAndTitle(NXObject.AttributeType.String, attr_name);
            }
            catch (System.Exception ex)
            {

            }

        }


        // 設定屬性
        public void DeleteFaceGroup_StringAttribute(Tag face)
        {
            // 1. 判斷是否存在 CIM_FEAT_SUB_TYPE
            String attr_name = Feat_type_attr;
            String attr_value = "";
            try
            {
                Face face_obj = (Face)CTrans.TagtoNXObject(face);
                //attr_value = face_obj.GetStringAttribute(attr_name);            
                face_obj.DeleteAttributeByTypeAndTitle(NXObject.AttributeType.String, attr_name);
            }
            catch (System.Exception ex)
            {

            }

        }





        // 抓屬性
        public String getFaceType_StringAttribute(Tag face)
        {
            // 1. 判斷是否存在 CIM_FEAT_SUB_TYPE
            String attr_name = Feat_type_attr;
            String attr_value = "";
            try
            {
                Face face_obj = (Face)CTrans.TagtoNXObject(face);
                attr_value = face_obj.GetStringAttribute(attr_name);               
            }
            catch (System.Exception ex)
            {

            }

            return attr_value;

        }


        public void getNoneduplicate_List(List<Tag> inTag,ref List<Tag> outTag)
        {
            List<Tag> list = new List<Tag>();
            foreach (Tag each_tag in inTag)
            {
                if (!list.Contains(each_tag))
                    list.Add(each_tag);
            }

            outTag.AddRange(list);
        }

        public void getNoneduplicate_ListINT(List<int> inTag, ref List<int> outTag)
        {
            List<int> list = new List<int>();
            foreach (int each_tag in inTag)
            {
                if (!list.Contains(each_tag))
                    list.Add(each_tag);
            }

            outTag.AddRange(list);
        }


        //==========================================================
        public void AutoSearch_TheSameDiameter(Part wkPrt, ref List<FeatureGroup> Out_FeatureGroupAary)
        {
            //List<Face> cylindricalNoAttrAry = new List<Face>();
            CFace cface_ = new CFace();
            // get all faces
            Body[] bodyArys = wkPrt.Bodies.ToArray();
            if (bodyArys.Length == 0)
                return;
            Body bodyPrototype = bodyArys[0];

            bool isAttribute = false;

            try
            {
                Face[] facePrototypeAry = bodyPrototype.GetFaces();

                // 1. 取得所有 有屬性參數的 Face
                List<FaceGroup> FeaturetypeAry = new List<FaceGroup>();
                List<FaceGroup> Featuretype_NoneAry = new List<FaceGroup>();
                String attr_value = "";
                List<Tag> AllFaceAry = new List<Tag>();
                double[] nor_ = new double[3];
                for (int i = 0; i < facePrototypeAry.Length; i++)
                {

                    CFace.CFaceData cfata_temp = cface_.getFacedata(facePrototypeAry[i].Tag);
                    //nor_ = cface_.GetNormal(facePrototypeAry[i].Tag);
                    //showlog("Tag : " + facePrototypeAry[i].Tag + " == nom dir : " + cfata_temp.norm_dir);
                    //showlog(nor_[0] + "===" + nor_[1] + "====" + nor_[2]);
                    if (cfata_temp.norm_dir == 1) //1: 外圓柱  -1:內圓孔
                        continue;

                    if (cfata_temp.type_face == UFConstants.UF_cylinder_subtype  // 圓柱面                      
                                                                    )
                    {
                        try
                        {
                            attr_value = "";
                            attr_value = facePrototypeAry[i].GetStringAttribute(Feat_type_attr);
                            //if ( attr_value != "" )
                            //{
                                FaceGroup featG = new FaceGroup();
                                //1.
                                featG.feat_type = attr_value;
                                //2.
                                featG.face_cur = facePrototypeAry[i]; // face
                                //3. 取資料
                                FeaturetypeAry.Add(featG);                            
                                //if (isAttribute )
                                    AllFaceAry.Add(facePrototypeAry[i].Tag);

                            //}
                        }
                        catch (System.Exception ex)
                        {
                            FaceGroup featG = new FaceGroup();
                            featG.feat_type = "";
                            featG.face_cur = facePrototypeAry[i];
                            Featuretype_NoneAry.Add(featG);
                            AllFaceAry.Add(facePrototypeAry[i].Tag);

                            continue;
                        }

                    }//end if type_face
                }

                 // 2. 搜鄰面 ===========================================
                // 所有圓柱面
                // 2.
                showlog("AllFaceAry size " + AllFaceAry.Count);

                //List<FeatureGroup> Out_FeatureGroupAary = new List<FeatureGroup>();
                List<Tag> used_tagAry = new List<Tag>();//已被使用過的
                int topo_type;
                for (int i = 0; i < AllFaceAry.Count; i++)
                {
                    FeatureGroup fg_temp = new FeatureGroup();
                    fg_temp.faceAry = new List<Tag>();
                    fg_temp.Hole_DiameterAry = new List<double>();

                    bool is_uses = used_tagAry.Contains(AllFaceAry[i]);
                    if (is_uses)
                        continue;

                    CFace.CFaceData i_face = cface_.getFacedata(AllFaceAry[i]);

                   
                    for (int j = 0; j < AllFaceAry.Count; j++)
                    {
                        CFace.CFaceData j_face = cface_.getFacedata(AllFaceAry[j]);
                        //showlog("tag f1 :" + ClyinderFaceAry[i] + "  tag f2 : " + ClyinderFaceAry[j]);
                        // 判斷同軸
                        bool isTheSame = cface_.isCylinderTheSame_Axis_CenterPoint(i_face, j_face);
                        if (isTheSame)
                        {
                            //showlog("tag f1 :" + AllFaceAry[i] + "  tag f2 : " + AllFaceAry[j]);
                            fg_temp.feat_type = "";
                            fg_temp.faceAry.Add(AllFaceAry[j]);//加入面
                            fg_temp.Hole_DiameterAry.Add(j_face.radius * 2);
                            used_tagAry.Add(AllFaceAry[j]);
                           
                        }

                    }
                    //showlog("tag f :" + AllFaceAry[i] + "  type : " + i_face.type_face + " none the same " + isNotheSame + "==> "+CLinQ.ListToString(fg_temp.faceAry));
                    // 都沒有人跟它同心且全圓形狀,則加入
                    if (fg_temp.faceAry.Count == 1)
                    {
                        
                        theUfSession_.Modl.AskFaceTopology(AllFaceAry[i], out topo_type);
                        
                        if (topo_type == UFConstants.UF_MODL_CYLINDRICAL_TOPOLOGY )// 全圓
                        {
                            fg_temp.feat_type = "";
                            fg_temp.faceAry.Add(AllFaceAry[i]);//加入面
                            fg_temp.Hole_DiameterAry.Add(i_face.radius * 2);
                            used_tagAry.Add(AllFaceAry[i]);

                            double diameter = getFaceGroupMiniDiameter(fg_temp);
                            diameter = CTrans.DoubleToAccuracyDouble_ToEvent(diameter, 2);//四捨五入
                            fg_temp.Main_diameter = diameter;

                            // 2. 加入相鄰圓錐面且同軸                           
                            getTheSameAxisConeFace(wkPrt,ref fg_temp);

                            Out_FeatureGroupAary.Add(fg_temp);

                            continue;
                        }
                    }
                    //showlog("tag f1 : === " + fg_temp.faceAry.Count);
                    if (fg_temp.faceAry.Count >1)
                    {
                        //showlog("tag f1 : === " + fg_temp.faceAry.Count + "=====is adj " + isGroupAdjFaces(fg_temp.faceAry));
                        // 判斷面之間是否有相鄰
                        if (isGroupAdjFaces(fg_temp.faceAry))
                        {
                            //showlog("tag f1 : === " + fg_temp.faceAry.Count + " == " + fg_temp.faceAry[0]);
                            // 1. 先算圓柱面的最小直徑
                            double diameter = getFaceGroupMiniDiameter(fg_temp);
                            diameter = CTrans.DoubleToAccuracyDouble_ToEvent(diameter, 2);//四捨五入
                            fg_temp.Main_diameter = diameter;

                            // 2. 加入相鄰圓錐面且同軸                           
                            getTheSameAxisConeFace(wkPrt,ref fg_temp);
                            //showlog("tag f2 : === " + fg_temp.faceAry.Count + " == " + fg_temp.faceAry[0]);
                            // 3. 加入中間平面
                            Out_FeatureGroupAary.Add(fg_temp);
                        }                     
                    }

                }//end for (int i = 0; i < AllFaceAry.Count; i++)
                
                showlog("單拆開的List<FeatureGroup> size " + Out_FeatureGroupAary.Count);

                // 3. 把相同直徑合並
                List<int> uesFG = new List<int>();
                List<FeatureGroup> outFG = new List<FeatureGroup>();
                for (int i=0;i<Out_FeatureGroupAary.Count;i++)
                {
                    bool bo = uesFG.Contains(i);//是否用過 index 
                    if (bo)
                        continue;

                    FeatureGroup fg_temp = new FeatureGroup();
                    fg_temp.faceAry = new List<Tag>();
                    fg_temp.Hole_DiameterAry = new List<double>();
                    fg_temp.SubGroupAry = new List<SubGroup>(); 

                    for (int j = 0; j < Out_FeatureGroupAary.Count; j++ )
                    {
                        if ( Out_FeatureGroupAary[i].Main_diameter == Out_FeatureGroupAary[j].Main_diameter )
                        {
                            SubGroup subg = new SubGroup();
                            subg.faceAry = new List<Tag>();

                            CLinQ.ListDeleteRepeatElemt(ref Out_FeatureGroupAary[j].faceAry);

                            fg_temp.feat_type = Out_FeatureGroupAary[j].feat_type;
                            fg_temp.faceAry.AddRange(Out_FeatureGroupAary[j].faceAry);
                            fg_temp.Hole_DiameterAry.AddRange(Out_FeatureGroupAary[j].Hole_DiameterAry);
                            fg_temp.Main_diameter = Out_FeatureGroupAary[j].Main_diameter;

                            subg.faceAry.AddRange(Out_FeatureGroupAary[j].faceAry);
                            fg_temp.SubGroupAry.Add(subg);
                            uesFG.Add(j);
                        }
                    }
                    outFG.Add(fg_temp);
                }

                showlog("合併List<FeatureGroup> size " + outFG.Count);
                Out_FeatureGroupAary.Clear();
                Out_FeatureGroupAary.AddRange(outFG);

                AutoDetermine_HoleType(wkPrt,ref  Out_FeatureGroupAary);
                AutoDetermine_HoleTypeByCooling(wkPrt, ref  Out_FeatureGroupAary);
/*
                for (int i = 0; i < Out_FeatureGroupAary.Count;i++ )
                {
                    for(int j=0;j<Out_FeatureGroupAary[i].faceAry.Count;j++)
                    {
                        Face f = (Face)CTrans.TagtoNXObject(Out_FeatureGroupAary[i].faceAry[j]);
                        f.Highlight();

                    }

                }
*/

            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }


        }// end AutoSearch_TheSameDiameter


        // 判斷群組面是否相鄰
        public bool isGroupAdjFaces(List<Tag> groupFaces)
        {
            int Topology_num = 0;
            foreach(Tag gf in groupFaces)
            {
                int topo_type = 0;
                theUfSession_.Modl.AskFaceTopology(gf, out topo_type);
                
                if (topo_type == UFConstants.UF_MODL_CYLINDRICAL_TOPOLOGY)// 全圓
                {
                    Topology_num++;
                }
            }
            if (Topology_num > (groupFaces.Count/2)  )
            {
                return true;
            }
            
            
            
            CFace cf = new CFace(); 
            int size = groupFaces.Count;
            if (size > 1) // 都是半圓 or 全圓
            {
                List<CFace.CFaceData> cfdata = new List<CFace.CFaceData>();
                List<Tag> adjs = new List<Tag>();
                List<Tag> adjs_All = new List<Tag>();
                // 1. 記錄所有鄰面
                for (int i=0;i<groupFaces.Count;i++)
                {
                   adjs.Clear();
                   Tag temp = groupFaces[i];
                   cf.adjFaces(temp, out adjs);
                   adjs_All.AddRange(adjs);
                }


                int dulipt = 0;
                for (int i = 0; i < groupFaces.Count; i++)
                {
                    bool ishave = adjs_All.Contains(groupFaces[i]);
                    if (!ishave)
                    {
                        dulipt++;
                    }
                }

                //showlog("dulipt all " + CLinQ.ListToString(adjs_All));
                //showlog("dulipt " + dulipt + "=== " + CLinQ.ListToString(groupFaces));

                if (dulipt >= groupFaces.Count)
                    return false;
                else
                    return true;

            }
           

            return false;
        }// end isGroupAdjFaces

        /// <summary>
        /// 加入相鄰圓錐面且同軸 (相鄰的相鄰且同軸 要跑遞回)
        /// </summary>
        /// <param name="featGroup"></param>
        public void getTheSameAxisConeFace(Part prt,ref FeatureGroup featGroup)
        {
            CFace cf = new CFace();
            // 1. 任意圓柱面的軸相
            CFace.CFaceData cfdata = cf.getFacedata(featGroup.faceAry[0]);

            // 2. 記錄所有鄰面
            List<Tag> adjs = new List<Tag>();
            List<Tag> adjs_All = new List<Tag>();          
            for (int i = 0; i < featGroup.faceAry.Count; i++)
            {
                adjs.Clear();
                Tag temp = featGroup.faceAry[i];
                cf.adjFaces(temp, out adjs);
                adjs_All.AddRange(adjs);
            }
       //showlog("face adj1: " + adjs_All.Count);
            // 3. 取得不重複元素
            CLinQ.ListDeleteRepeatElemt(ref adjs_All);
            CLinQ.ListDifferenceElemt(ref adjs_All, featGroup.faceAry);//排除已存在的面

       showlog("face adj2: " + adjs_All.Count + " == " + CLinQ.ListToString(adjs_All));
            List<Tag> usesTaglist = new List<Tag>();
            List<Tag> newTaglist = new List<Tag>();
            
            foreach (Tag face_tag in adjs_All)
            {
                //showlog(" Tag : " + face_tag + "  Tag_main : " + featGroup.faceAry[0]);

                CFace.CFaceData cfdata_t = cf.getFacedata(face_tag);
               
                // 是否同軸
                bool isAxis = cf.isCylinderTheSame_Axis_CenterPoint(cfdata,cfdata_t);
                if (isAxis)
                {
                    if (cfdata_t.type_face == UFConstants.UF_bounded_plane_type)
                    {
                         bool is_onface = ISOnBoundingBoxFace(prt, face_tag);
                        if (!is_onface  )
                            newTaglist.Add(face_tag);
                    }
                    else
                        newTaglist.Add(face_tag);
                   

                }
            }

            // 4. 取得不重複元素
            CLinQ.ListDeleteRepeatElemt(ref newTaglist);

            List<double> newdiameter = new List<double>();
            foreach( Tag faced in newTaglist)
            {
                if (featGroup.faceAry.Contains(faced))
                    continue;

                CFace.CFaceData cfdata_t = cf.getFacedata(faced);
                featGroup.Hole_DiameterAry.Add(cfdata_t.radius * 2);
                featGroup.faceAry.Add(faced);
            }

            if (newTaglist.Count == 0)
                return;
            else
                getTheSameAxisConeFace(prt,ref featGroup);


        }//end getTheSameAxisConeFace


        // 凸台
        public void AutoSearch_Boss(Part wkPrt, Tag face_p,Tag edge_p,
                                ref List<FeatureGroup> Out_FeatureGroupAary)
        {
            // 邊在內迴圈中 (封閉迴圈)
            if (!ISedgeInOutLoopFaceEdge(edge_p,face_p) )
            {
                showlog("in inner loop fun---------------------------");
                AutoSearch_Boss_innerLoop(wkPrt, face_p, edge_p, ref Out_FeatureGroupAary);
                return ;
            }

            showlog("in outer loop fun---------------------------");
            
            CLog log = new CLog();
            CFace cf = new CFace();
            // 1. 基準面
            CFace.CFaceData cfdata = cf.getFacedata(face_p);
            //showlog("Tag " + face_p );
            double[] minpoint = new double[3];
            minpoint[0] = cfdata.box[0];
            minpoint[1] = cfdata.box[1];
            minpoint[2] = cfdata.box[2];

            //log.showlogByPoint("min ", minpoint);
            //log.showlog("max "+cfdata.box[3] + "==" + cfdata.box[4] + "==" + cfdata.box[5]);

            double[] mindir = new double[3];
            mindir[0] = cfdata.dir[0];
            mindir[1] = cfdata.dir[1];
            mindir[2] = cfdata.dir[2];


             // get all faces
            Body[] bodyArys = wkPrt.Bodies.ToArray();
            if (bodyArys.Length == 0)
                return;
            Body bodyPrototype = bodyArys[0];

            bool isAttribute = false;

            try
            {
                double[] Face_point = new double[3];
                Face[] facePrototypeAry = bodyPrototype.GetFaces();
                FeatureGroup fg_temp = new FeatureGroup();
                fg_temp.faceAry = new List<Tag>();
                fg_temp.Hole_DiameterAry = new List<double>();
                SubGroup subg = new SubGroup();
                subg.faceAry = new List<Tag>();

                List<Tag> lastFaces = new List<Tag>();

                for (int i = 0; i < facePrototypeAry.Length;i++ )
                {
                    //showlog("start");
                    Face face_t = facePrototypeAry[i];
   
                    CFace.CFaceData cfd_t = cf.getFacedata(face_t.Tag);
                    Face_point[0] = cfd_t.box[0];
                    Face_point[1] = cfd_t.box[1];
                    Face_point[2] = cfd_t.box[2];

                    double angle_v = CMath.getTwoPoint_Normal_Angle(Face_point, minpoint, mindir);


                    if (angle_v <= 90)//比基面高
                    {
                        lastFaces.Add(face_t.Tag);
//                         showlog("--angle   " + angle_v);
//                         log.showlogByPoint("min ", Face_point);
//                         log.showlog("max " + cfd_t.box[3] + "==" + cfd_t.box[4] + "==" + cfd_t.box[5]);

                       
                        
                        
                        fg_temp.feat_type = "";
                        fg_temp.faceAry.Add(face_t.Tag);
                        fg_temp.Hole_DiameterAry.Add(0);
                        fg_temp.Main_diameter = 0;
                        subg.faceAry.Add(face_t.Tag);
                    }
                   
                    

                    //showlog("end");
                }// end for

                //showlog("lastFaces : " + lastFaces.Count);

                // 外迴圈
                List<Tag> new_lastFaces = new List<Tag>();
                getloopFaces(lastFaces, edge_p, face_p, ref new_lastFaces);

                //List<Tag> n_lastFaces = new List<Tag>();
                // 把外壁面的雜面刪除 (在主面範圍內的面,其餘排除)
                //getBoundingBoxInsideFace(new_lastFaces, face_p, ref n_lastFaces);


//                 foreach (Tag f_temp in new_lastFaces)
//                 {
//                     fg_temp.feat_type = "";
//                     fg_temp.faceAry.Add(f_temp);
//                     fg_temp.Hole_DiameterAry.Add(0);
//                     fg_temp.Main_diameter = 0;
//                     subg.faceAry.Add(f_temp);
// 
//                 }

                fg_temp.SubGroupAry = new List<SubGroup>();
                fg_temp.SubGroupAry.Add(subg);
                Out_FeatureGroupAary.Add(fg_temp);

            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }




        }


        public void AutoSearch_Boss_innerLoop(Part wkPrt, Tag face_p, Tag edge_p,
                                ref List<FeatureGroup> Out_FeatureGroupAary)
        {
            CFace cf = new CFace();
            CLog log = new CLog();

            Edge edge = (Edge)CTrans.TagtoNXObject(edge_p);
            Face[] fs = edge.GetFaces();

            List<Tag> new_fAry = new List<Tag>();

            foreach ( Face i_face in fs)
            {
                if (i_face.Tag == face_p)//等於點選面,排除
                    continue;

                new_fAry.Add(i_face.Tag);
                AutoSearch_Boss_innerLoop_sub(face_p, i_face.Tag, ref new_fAry);
            }

         
            FeatureGroup fg_temp = new FeatureGroup();
            fg_temp.faceAry = new List<Tag>();
            fg_temp.Hole_DiameterAry = new List<double>();
            SubGroup subg = new SubGroup();
            subg.faceAry = new List<Tag>();

            foreach (Tag last_face in new_fAry)
            {
                fg_temp.feat_type = "";
                fg_temp.faceAry.Add(last_face);
                fg_temp.Hole_DiameterAry.Add(0);
                fg_temp.Main_diameter = 0;
                subg.faceAry.Add(last_face);

            }

            fg_temp.SubGroupAry = new List<SubGroup>();
            fg_temp.SubGroupAry.Add(subg);
            Out_FeatureGroupAary.Add(fg_temp);
        }

        public void AutoSearch_Boss_innerLoop_sub(Tag main_f , Tag face ,ref List<Tag> new_face )
        {
            CFace cf = new CFace();
            List<Tag> adjs = new List<Tag>();
            cf.adjFaces(face, out adjs);// 鄰面的鄰面

            foreach( Tag adj_1 in adjs)
            {
                if (new_face.Contains(adj_1))
                    continue;

                if (adj_1 != main_f)
                {
                    new_face.Add(adj_1);
                    AutoSearch_Boss_innerLoop_sub(main_f, adj_1, ref new_face);
                }
            }

        }


        // 凹槽
        public void AutoSearch_Concave(Part wkPrt, Tag face_p,
                                ref List<FeatureGroup> Out_FeatureGroupAary)
        {
            
            CLog log = new CLog();
            CFace cf = new CFace();
            
            // get all faces
            Body[] bodyArys = wkPrt.Bodies.ToArray();
            if (bodyArys.Length == 0)
                return;
            Body bodyPrototype = bodyArys[0];

            bool isAttribute = false;

            try
            {
                Face[] facePrototypeAry = bodyPrototype.GetFaces();          
                // 目標構成
                CFace.CFaceData cfdata = cf.getFacedata(face_p);
                double[] main_normal = new double[3];
                double[] sub_normal = new double[3];
                main_normal = cf.GetNormal(face_p);
                foreach (Face cur_face in facePrototypeAry)
                {
                    
                    FeatureGroup fg_temp = new FeatureGroup();
                    fg_temp.faceAry = new List<Tag>();
                    fg_temp.Hole_DiameterAry = new List<double>();
                    SubGroup subg = new SubGroup();
                    subg.faceAry = new List<Tag>();
                    fg_temp.SubGroupAry = new List<SubGroup>();

                    CFace.CFaceData cfdata1 = cf.getFacedata(cur_face.Tag);
                    if (cfdata.type_face == cfdata1.type_face)// type 要相同
                    {
                        sub_normal = cf.GetNormal(cur_face.Tag);
                        double angle = CMath.getTwoVectorAngle(main_normal, sub_normal);
                        double angle_d = CMath.RadianToAngle(angle);
                      
                        if (angle_d > 60)
                            continue;

                        bool is_s = cf.IsFaceStructureEquals(face_p, cur_face.Tag);
                        if (is_s)// 結構相等
                        {
                            //List<Tag> adjs = new List<Tag>();
                            //List<CFace.CFaceData> adjs_fdata = cf.adjFaces(cur_face.Tag, out adjs);

                            // 目標面之上的面
                            List<Tag> adjs_new = new List<Tag>();
                            //cf.getFacesOnFaceNormal(adjs, cur_face.Tag, ref adjs_new,"<=");

                            // 找外回圈的邊
                            Edge[] face_edges = CaxUF_Lib.GetLoopEdges(cur_face, EdgeLoopType.Peripheral);
                            
                            foreach( Edge edge_t in face_edges)
                            {
                                Face[] faces_e = edge_t.GetFaces();

                                foreach (Face ff in faces_e)
                                {
                                    if (ff != cur_face)
                                    {
                                        int  isConcave = CaxUF_Lib.GetEdgeConnectivity(edge_t);
                                        showlog("edge " + edge_t + "==== bool : " + isConcave);
                                        if (isConcave != 1)//Concave = 凹
                                        {
                                            adjs_new.Add(ff.Tag);


                                        }
                                    }//end for
                                }//end for
  
                            }
                            

                            fg_temp.faceAry.Add(cur_face.Tag);
                            subg.faceAry.Add(cur_face.Tag);
                            fg_temp.Hole_DiameterAry.Add(0);

                            foreach (Tag face_t in adjs_new)
                            {
                                fg_temp.feat_type = "";
                                fg_temp.faceAry.Add(face_t);
                                fg_temp.Hole_DiameterAry.Add(0);
                                fg_temp.Main_diameter = 0;
                                subg.faceAry.Add(face_t);
                            }
                            
                            fg_temp.SubGroupAry.Add(subg);
                            Out_FeatureGroupAary.Add(fg_temp);
                        }

                    }//end if  the same type


                }// end for all face


                
                
                
                

            
            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }



            
        }

        // 單一
        public void AutoSearch_Concave_single(Part wkPrt, Tag face_p,
                                ref List<FeatureGroup> Out_FeatureGroupAary)
        {

            CLog log = new CLog();
            CFace cf = new CFace();

            try
            {

                FeatureGroup fg_temp = new FeatureGroup();
                fg_temp.faceAry = new List<Tag>();
                fg_temp.Hole_DiameterAry = new List<double>();
                SubGroup subg = new SubGroup();
                subg.faceAry = new List<Tag>();
                fg_temp.SubGroupAry = new List<SubGroup>();
                List<Tag> adjs_new = new List<Tag>();

                // 找外回圈的邊
                Face face_handle = (Face)CTrans.TagtoNXObject(face_p);
                Edge[] face_edges = CaxUF_Lib.GetLoopEdges(face_handle, EdgeLoopType.Peripheral);

                foreach (Edge edge_t in face_edges)
                {
                    Face[] faces_e = edge_t.GetFaces();

                    foreach (Face ff in faces_e)
                    {
                        if (ff != face_handle)
                        {
                            int isConcave = CaxUF_Lib.GetEdgeConnectivity(edge_t);
                            showlog("edge " + edge_t + "==== bool : " + isConcave);
                            if (isConcave != 1)//Concave = 凹
                            {
                                adjs_new.Add(ff.Tag);
                                if (isConcave == 0)// 相切就多找一層
                                {
                                    List<Tag> r_adjs = new List<Tag>();
                                    getloopFacesByNono_Convex(ff.Tag, ref r_adjs);
                                    adjs_new.AddRange(r_adjs);
                                }
                                
                                CFace.CFaceData cfdata = cf.getFacedata(ff.Tag);
                                //showlog(cfdata.type_face + "===" + ff.Tag);
                                if (cfdata.type_face == UFConstants.UF_cone_type)// 倒角就多找一層
                                {
                                    
                                    List<Tag> r_adjs = new List<Tag>();
                                    getloopFacesByNono_Convex(ff.Tag, ref r_adjs);
                                    adjs_new.AddRange(r_adjs);
                                }
                                

                            }
                        }//end for
                    }//end for

                }

                adjs_new.Add(face_handle.Tag);
                CLinQ.ListDeleteRepeatElemt(ref adjs_new);

//                 fg_temp.faceAry.Add(face_handle.Tag);
//                 subg.faceAry.Add(face_handle.Tag);
//                 fg_temp.Hole_DiameterAry.Add(0);

                foreach (Tag face_t in adjs_new)
                {
                    fg_temp.feat_type = "";
                    fg_temp.faceAry.Add(face_t);
                    fg_temp.Hole_DiameterAry.Add(0);
                    fg_temp.Main_diameter = 0;
                    subg.faceAry.Add(face_t);
                }

                fg_temp.SubGroupAry.Add(subg);
                Out_FeatureGroupAary.Add(fg_temp);
                       






            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }




        }



        // 種子面-邊界面
        public void AutoSearch_Seed_Bound(Part wkPrt, Tag seed_f_p,List<Tag> Bound_f_p_list,
                                ref List<FeatureGroup> Out_FeatureGroupAary)
        {
            CLog log = new CLog();
            CFace cf = new CFace();
            try
            {
                List<Tag> new_face_list = new List<Tag>();
                AutoSearch_Seed_Bound_sub(seed_f_p, Bound_f_p_list, ref new_face_list);

                FeatureGroup fg_temp = new FeatureGroup();
                fg_temp.faceAry = new List<Tag>();
                fg_temp.Hole_DiameterAry = new List<double>();
                SubGroup subg = new SubGroup();
                subg.faceAry = new List<Tag>();
                fg_temp.SubGroupAry = new List<SubGroup>();
                List<Tag> adjs_new = new List<Tag>();


                foreach (Tag face_t in new_face_list)
                {
                    fg_temp.feat_type = "";
                    fg_temp.faceAry.Add(face_t);
                    fg_temp.Hole_DiameterAry.Add(0);
                    fg_temp.Main_diameter = 0;
                    subg.faceAry.Add(face_t);
                }

                fg_temp.SubGroupAry.Add(subg);
                Out_FeatureGroupAary.Add(fg_temp);







            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }




        }


        /// <summary>
        ///  種子面-邊界面 搜尋
        /// </summary>
        /// <param name="seed_f_p"></param>
        /// <param name="Bound_f_p_list"></param>
        /// <param name="Search_f_list"></param>
        public void AutoSearch_Seed_Bound_sub(Tag seed_f_p,List<Tag> Bound_f_p_list,ref List<Tag> Search_f_list)
        {
            CLog log = new CLog();
            CFace cf = new CFace();

            List<Tag> adjs_new = new List<Tag>();
            cf.adjFaces(seed_f_p, out adjs_new);

            foreach(Tag face in adjs_new)
            {
                if (Bound_f_p_list.Contains(face))// 是邊界面就停止
                    continue;
                if (Search_f_list.Contains(face))// 已加入過就停止
                    continue;

                Search_f_list.Add(face);
                AutoSearch_Seed_Bound_sub(face, Bound_f_p_list, ref Search_f_list);

                // 

            }

        }



        // 種子面-邊界面 + normal 方向
        public void AutoSearch_Seed_BoundByNormal(Part wkPrt, Tag seed_f_p, List<Tag> Bound_f_p_list,
                                ref List<FeatureGroup> Out_FeatureGroupAary)
        {
            CLog log = new CLog();
            CFace cf = new CFace();
            try
            {

                double[] normal_f = new double[3];
                normal_f = cf.GetNormal(seed_f_p);
                showlog("dir " + normal_f[0] + "=== " + normal_f[1] + "=== " + normal_f[2]);
                List<Tag> new_face_list = new List<Tag>();
                AutoSearch_Seed_Bound_subByNormal(seed_f_p, Bound_f_p_list, normal_f, ref new_face_list);

                CLinQ.ListDeleteRepeatElemt(ref new_face_list);

                FeatureGroup fg_temp = new FeatureGroup();
                fg_temp.faceAry = new List<Tag>();
                fg_temp.Hole_DiameterAry = new List<double>();
                SubGroup subg = new SubGroup();
                subg.faceAry = new List<Tag>();
                fg_temp.SubGroupAry = new List<SubGroup>();
                List<Tag> adjs_new = new List<Tag>();


                foreach (Tag face_t in new_face_list)
                {
                    fg_temp.feat_type = "";
                    fg_temp.faceAry.Add(face_t);
                    fg_temp.Hole_DiameterAry.Add(0);
                    fg_temp.Main_diameter = 0;
                    subg.faceAry.Add(face_t);
                }

                fg_temp.SubGroupAry.Add(subg);
                Out_FeatureGroupAary.Add(fg_temp);


            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }


        }//end AutoSearch_Seed_BoundByNormal

        /// <summary>
        ///  種子面-邊界面 搜尋
        /// </summary>
        /// <param name="seed_f_p"></param>
        /// <param name="Bound_f_p_list"></param>
        /// <param name="Search_f_list"></param>
        public void AutoSearch_Seed_Bound_subByNormal(  Tag seed_f_p, 
                                                        List<Tag> Bound_f_p_list, 
                                                        double[] dmain_dir,
                                                        ref List<Tag> Search_f_list)
        {
            CLog log = new CLog();
            CFace cf = new CFace();

            List<Tag> adjs_new = new List<Tag>();
            cf.adjFaces(seed_f_p, out adjs_new);
            //showlog("dir " + main_cfdata.dir[0] + "=== " + main_cfdata.dir[1] + "=== " + main_cfdata.dir[2]);

            foreach (Tag face in adjs_new)
            {
                if (Bound_f_p_list.Contains(face))// 是邊界面就停止
                    continue;
                if (Search_f_list.Contains(face))// 已加入過就停止
                    continue;

                double[] normal_f = new double[3];
                normal_f = cf.GetNormal(face);

                double Vertical = cf.getVectdot(dmain_dir, normal_f); //0->90

                if (Math.Round(Vertical, 6, MidpointRounding.AwayFromZero) <= 1.0 &&
                Math.Round(Vertical, 6, MidpointRounding.AwayFromZero) > -1.0)
                {
                    Search_f_list.Add(face);
                    AutoSearch_Seed_Bound_subByNormal(face, Bound_f_p_list,dmain_dir, ref Search_f_list);
                }

                // 

            }

        }//end AutoSearch_Seed_Bound_subByNormal



        // Box 邊界面(底面)
        public void AutoSearch_BoxBounding(Part wkPrt, Tag seed_f_p,
                                            ref List<FeatureGroup> Out_FeatureGroupAary)
        {
            CLog log = new CLog();
            CFace cf = new CFace();
            try
            {
                List<Tag> new_face_list = new List<Tag>();//外迴圈
                AutoSearch_BoxBounding_sub(seed_f_p ,ref new_face_list);

                CLinQ.ListDeleteRepeatElemt(ref new_face_list);

                FeatureGroup fg_temp = new FeatureGroup();
                fg_temp.faceAry = new List<Tag>();
                fg_temp.Hole_DiameterAry = new List<double>();
                SubGroup subg = new SubGroup();
                subg.faceAry = new List<Tag>();
                fg_temp.SubGroupAry = new List<SubGroup>();
                List<Tag> adjs_new = new List<Tag>();


                foreach (Tag face_t in new_face_list)
                {
                    fg_temp.feat_type = "";
                    fg_temp.faceAry.Add(face_t);
                    fg_temp.Hole_DiameterAry.Add(0);
                    fg_temp.Main_diameter = 0;
                    subg.faceAry.Add(face_t);
                }
                fg_temp.SubGroupAry.Add(subg);
                Out_FeatureGroupAary.Add(fg_temp);

            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }


        }//end AutoSearch_BoxBounding

        /// <summary>
        /// 
        /// </summary>
        /// <param name="face">種子面的邊界面 + 垂直90度</param>
        public void AutoSearch_BoxBounding_sub(Tag face,ref List<Tag> Vertical_Faces)
        {

            CFace cf = new CFace();
            Face face_handle = (Face)CTrans.TagtoNXObject(face);
            Edge[] face_edges = { };

            face_edges = CaxUF_Lib.GetLoopEdges(face_handle, EdgeLoopType.Peripheral);

            double[] main_dir = new double[3];
            main_dir = cf.GetNormal(face);

            foreach (Edge edge_t in face_edges)
            {
                Face[] faces_e = edge_t.GetFaces();

                foreach (Face ff in faces_e)
                {
                    if (ff != face_handle)//不等於自己
                    {
                        double[] face_dir = new double[3];
                        face_dir = cf.GetNormal(ff.Tag);
                        bool isVertical = cf.getVectDotisVertical(main_dir, face_dir); //0->90
                        if (!isVertical)
                        {
                            List<Tag> adjs = new List<Tag>();
                            cf.adjFaces(ff.Tag, out adjs);
                            foreach(Tag ft in adjs)
                            {
                                if (ft == face_handle.Tag)
                                    continue;

                                double[] face_dir_1 = new double[3];
                                face_dir_1 = cf.GetNormal(ft);

                                bool isVertical_1 = cf.getVectDotisVertical(main_dir, face_dir_1); //0->90
                                if (isVertical_1)
                                    Vertical_Faces.Add(ft);
                            }
                        }
                        else
                        {
                            Vertical_Faces.Add(ff.Tag);
                        }


                    }//end for
                }//end for

            }
        }


        // Box 邊界面(底面)  2014 05 05 (相似比較 還不夠精準)
        public void AutoSearch_Boss2(Part wkPrt, Tag seed_f_p,
                                            ref List<FeatureGroup> Out_FeatureGroupAary)
        {
            CLog log = new CLog();
            CFace cf = new CFace();
            CPart prt = new CPart();
            try
            {

                AutoSearch_Boss2_single(wkPrt, seed_f_p, ref Out_FeatureGroupAary);
                //===========================================
                // get all faces
                Body[] bodyArys = wkPrt.Bodies.ToArray();
                if (bodyArys.Length == 0)
                    return;
                Body bodyPrototype = bodyArys[0];

                bool isAttribute = false;

                Face[] facePrototypeAry = bodyPrototype.GetFaces();          
                // 目標構成


               foreach (Face cur_face in facePrototypeAry)
               {
                   if (cur_face.Tag == seed_f_p)
                       continue;
                   
                   if (cf.IsFaceStructureEquals(seed_f_p,cur_face.Tag))
                   {
                       AutoSearch_Boss2_single(wkPrt, cur_face.Tag, ref Out_FeatureGroupAary);
                   }

               }




            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }


        }//end AutoSearch_BoxBounding

        public void AutoSearch_Boss2_single(Part wkPrt, Tag seed_f_p,
                                            ref List<FeatureGroup> Out_FeatureGroupAary
                                            )
        {
            CLog log = new CLog();
            CFace cf = new CFace();
            CPart prt = new CPart();
            try
            {
                List<Tag> new_face_list = new List<Tag>();//外迴圈
                AutoSearch_Boss2_sub(seed_f_p, ref new_face_list);
                CLinQ.ListDeleteRepeatElemt(ref new_face_list);

                // 過濾邊界面
                double[] bounding = new double[6];
                prt.getBoundingBox2Point(wkPrt, bounding);
                //showlog("box1 " + bounding[0] + "==" + bounding[1] + "==" + bounding[2]);
                //showlog("box2 " + bounding[3] + "==" + bounding[4] + "==" + bounding[5]);
                List<Tag> filter_face_list = new List<Tag>();//外迴圈
                foreach (Tag face in new_face_list)
                {
                    bool isonFace = cf.IsThroughFace(face, bounding);
                    if (!isonFace)
                        filter_face_list.Add(face);
                }

                filter_face_list.Add(seed_f_p);
                FeatureGroup fg_temp = new FeatureGroup();
                fg_temp.faceAry = new List<Tag>();
                fg_temp.Hole_DiameterAry = new List<double>();
                SubGroup subg = new SubGroup();
                subg.faceAry = new List<Tag>();
                fg_temp.SubGroupAry = new List<SubGroup>();
                List<Tag> adjs_new = new List<Tag>();

                foreach (Tag face_t in filter_face_list)
                {
                    fg_temp.feat_type = "";
                    fg_temp.faceAry.Add(face_t);
                    fg_temp.Hole_DiameterAry.Add(0);
                    fg_temp.Main_diameter = 0;
                    subg.faceAry.Add(face_t);
                }
                fg_temp.SubGroupAry.Add(subg);
                Out_FeatureGroupAary.Add(fg_temp);
            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }
        }//end AutoSearch_Boss2_single


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face">種子面的邊界面 + 垂直90度</param>
        public void AutoSearch_Boss2_sub(Tag face, ref List<Tag> Vertical_Faces)
        {

            CFace cf = new CFace();
            Face face_handle = (Face)CTrans.TagtoNXObject(face);
            Edge[] face_edges = { };
            //face_edges = CaxUF_Lib.GetLoopEdges(face_handle, EdgeLoopType.Peripheral);
                        
            List<Tag> adjs = new List<Tag>();
            cf.adjFaces(face, out adjs);

            Vertical_Faces.Add(face);
            Vertical_Faces.AddRange(adjs);
            foreach (Tag face_t in adjs)
            {
                Face face_current = (Face)CTrans.TagtoNXObject(face_t);

                List<Tag> adjs_1 = new List<Tag>();
                cf.adjFaces(face_t, out adjs_1);
                Vertical_Faces.AddRange(adjs_1);

                foreach( Tag adj_face in adjs_1)
                {
                    Face face_second = (Face)CTrans.TagtoNXObject(adj_face);

                    Edge[] edges = face_second.GetEdges();
                    foreach(Edge e in edges)
                    {
                        int connect = CaxUF_Lib.GetEdgeConnectivity(e);
                        if (connect == 1)// 凸
                        {
                            Face[] e_faces = e.GetFaces();
                            foreach(Face ff in e_faces)
                            {
                                if (Vertical_Faces.Contains(ff.Tag))
                                    continue;

                                Vertical_Faces.Add(ff.Tag);
                            }
                        }
                    }
                }
            }

            //CLinQ.ListDeleteRepeatElemt(ref Vertical_Faces);
        }


        public void AutoSearch_NoneHole(Part wkPrt, Tag seed_f_p,
                                            ref List<FeatureGroup> Out_FeatureGroupAary
                                            )
        {
            CLog log = new CLog();
            CFace cf = new CFace();
            CPart prt = new CPart();
            try
            {
                List<Tag> new_face_list = new List<Tag>();//外迴圈
                AutoSearch_NoneHole_sub(seed_f_p, ref new_face_list);
               

//                 // 過濾邊界面
//                 double[] bounding = new double[6];
//                 prt.getBoundingBox2Point(wkPrt, bounding);
//                 //showlog("box1 " + bounding[0] + "==" + bounding[1] + "==" + bounding[2]);
//                 //showlog("box2 " + bounding[3] + "==" + bounding[4] + "==" + bounding[5]);
//                 List<Tag> filter_face_list = new List<Tag>();//外迴圈
//                 foreach (Tag face in new_face_list)
//                 {
//                     bool isonFace = cf.IsThroughFace(face, bounding);
//                     if (!isonFace)
//                         filter_face_list.Add(face);
//                 }

                //new_face_list.Add(seed_f_p);
                CLinQ.ListDeleteRepeatElemt(ref new_face_list);

                FeatureGroup fg_temp = new FeatureGroup();
                fg_temp.faceAry = new List<Tag>();
                fg_temp.Hole_DiameterAry = new List<double>();
                SubGroup subg = new SubGroup();
                subg.faceAry = new List<Tag>();
                fg_temp.SubGroupAry = new List<SubGroup>();
                List<Tag> adjs_new = new List<Tag>();

                foreach (Tag face_t in new_face_list)
                {
                    fg_temp.feat_type = "";
                    fg_temp.faceAry.Add(face_t);
                    fg_temp.Hole_DiameterAry.Add(0);
                    fg_temp.Main_diameter = 0;
                    subg.faceAry.Add(face_t);
                }
                fg_temp.SubGroupAry.Add(subg);
                Out_FeatureGroupAary.Add(fg_temp);
            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }
        }//end AutoSearch_Boss2_single


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face">種子面的邊界面 + 垂直90度</param>
        public void AutoSearch_NoneHole_sub(Tag face, ref List<Tag> Vertical_Faces)
        {

            CFace cf = new CFace();
            Face face_handle = (Face)CTrans.TagtoNXObject(face);
            Edge[] face_edges = { };
            face_edges = CaxUF_Lib.GetLoopEdges(face_handle, EdgeLoopType.Hole);

            showlog("edge size " + face_edges.Length);
            foreach (Edge edge_t in face_edges)
            {
                Face[] faces_e = edge_t.GetFaces();

                foreach (Face ff in faces_e)
                {
                    if (ff.Tag != face)//不等於自己
                    {
                        Vertical_Faces.Add(ff.Tag);
                    }
                }
            }



            //CLinQ.ListDeleteRepeatElemt(ref Vertical_Faces);
        }


        // 2014 05 25 選取相同色
        public void AutoSearch_Color(Part wkPrt, Tag face_p,
                                            ref List<FeatureGroup> Out_FeatureGroupAary
                                            )
        {
            CLog log = new CLog();
            CFace cf = new CFace();

            // get all faces
            Body[] bodyArys = wkPrt.Bodies.ToArray();
            if (bodyArys.Length == 0)
                return;
            Body bodyPrototype = bodyArys[0];

            bool isAttribute = false;

            try
            {
                Face[] facePrototypeAry = bodyPrototype.GetFaces();
                Face current_f = (Face)CTrans.TagtoNXObject(face_p);
                List<Face> ColorFaces = new List<Face>();
                List<Tag> ColorTags = new List<Tag>();
                ColorFaces.Add(current_f);

                FeatureGroup fg_temp = new FeatureGroup();
                fg_temp.faceAry = new List<Tag>();
                fg_temp.Hole_DiameterAry = new List<double>();
                SubGroup subg = new SubGroup();
                subg.faceAry = new List<Tag>();
                fg_temp.SubGroupAry = new List<SubGroup>();
                foreach (Face cur_face in facePrototypeAry)
                {

                    if (current_f.Color == cur_face.Color)// type 要相同
                    {
                        ColorFaces.Add(cur_face);
                        ColorTags.Add(cur_face.Tag);
                    }//end if  the same color


                }// end for all face




                fg_temp.faceAry.AddRange(ColorTags);
                subg.faceAry.AddRange(ColorTags);
                //fg_temp.Hole_DiameterAry.Add(0);


                fg_temp.SubGroupAry.Add(subg);
                Out_FeatureGroupAary.Add(fg_temp);




            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }


        }



        //  找外回圈的邊的面且 不是凸的面
        public void getloopFacesByNono_Convex(Tag face,ref List<Tag> new_faces)
        {
            // 找外回圈的邊
            Face face_handle = (Face)CTrans.TagtoNXObject(face);
            Edge[] face_edges = CaxUF_Lib.GetLoopEdges(face_handle, EdgeLoopType.Peripheral);

            foreach (Edge edge_t in face_edges)
            {
                Face[] faces_e = edge_t.GetFaces();

                foreach (Face ff in faces_e)
                {
                    if (ff != face_handle)
                    {
                        int isConcave = CaxUF_Lib.GetEdgeConnectivity(edge_t);
                        //showlog("edge " + edge_t + "==== bool : " + isConcave);
                        if (isConcave != 1)//Concave = 凹
                        {
                            new_faces.Add(ff.Tag);
                        }
                    }//end for
                }//end for

            }
        }

        /// <summary>
        /// 有線-面 從面群中找出
        /// </summary>
        /// <param name="Faces"></param>
        /// <param name="edge_cur"></param>
        /// <param name="face_cur"></param>
        /// <param name="new_Faces"></param>
        public void getloopFaces(List<Tag> Faces , Tag edge_cur, Tag face_cur,ref List<Tag> new_Faces)
        {
            CFace cf = new CFace();
            CLog log = new CLog();

            Edge edge = (Edge)CTrans.TagtoNXObject(edge_cur);
            Face[] fs = edge.GetFaces();

            showlog("edgeface  size " + fs.Length);

            List<Tag> new_fAry = new List<Tag>();

            showlog("Faces  " + CLinQ.ListToString(Faces));
            showlog("main  " + face_cur);
            foreach (Face ff in fs)
            {
                showlog("adj  " + ff.Tag);
                if (ff.Tag == face_cur)//等於點選面,排除
                    continue;

                if (!Faces.Contains(ff.Tag))//不在範圍內,排除
                    continue;

                showlog("ff  " + ff.Tag);
                new_fAry.Add(ff.Tag);
                
                getloopFaces_sub(Faces, ff.Tag, face_cur,ref new_fAry);

            }

            // 80 成已沒問題

            new_Faces.AddRange(new_fAry);

        }


        public void getloopFaces_sub(List<Tag> Faces, Tag f_cur, Tag main_f , ref List<Tag> new_Faces)
        {
            CFace cf = new CFace();
            CLog log = new CLog();

            List<Tag> f_adj = new List<Tag>();
            cf.adjFaces(f_cur, out  f_adj);
            //showlog("adjs " + CLinQ.ListToString(f_adj));

            foreach( Tag f in f_adj)
            {
                if (Faces.Contains(f))
                {
                    //showlog("in " +f);
                    if (new_Faces.Contains(f)) // 用過了
                        continue;
                    
                    new_Faces.Add(f);
                    getloopFaces_sub(Faces, f, main_f,ref new_Faces);

                    
                }
            }

        }

        /// <summary>
        /// 這個面的邊是否有主面的外迴圈的邊
        /// </summary>
        /// <param name="cur_f"></param>
        /// <param name="Main_f"></param>
        public bool IsFaceHaveoutLoopedge(Tag cur_f,Tag Main_f)
        {
            Face main_face = (Face)CTrans.TagtoNXObject(Main_f);
            Edge[] main_edges = CaxUF_Lib.GetLoopEdges(main_face, EdgeLoopType.Peripheral);

            Face cur_face = (Face)CTrans.TagtoNXObject(cur_f);
            Edge[] cur_edges = CaxUF_Lib.GetLoopEdges(cur_face, EdgeLoopType.Peripheral);

            foreach ( Edge edge in cur_edges)
            {
                foreach(Edge m_edge in main_edges)
                {
                    if (edge == m_edge)
                        return true;
                }
            }


            return false;


        }


        // 判斷邊是否在外迴圈中
        public bool ISedgeInOutLoopFaceEdge(Tag edge , Tag face)
        {
            Face face_handle = (Face)CTrans.TagtoNXObject(face);
            Edge[] edges = CaxUF_Lib.GetLoopEdges(face_handle, EdgeLoopType.Peripheral);


            foreach (Edge i_edge in edges)
            {
                if (i_edge.Tag == edge)
                {
                    return true;
                }
            }

            Edge[] edges_1 = face_handle.GetEdges();
            int ii = 0;
            foreach (Edge i_edge in edges_1)
            {
                if (i_edge.Tag != edge )
                    ii++;
            }

            if (ii == edges_1.Length)
                return true;

            return false;

        }

        // 把外壁面的雜面刪除 (在主面範圍內的面,其餘排除)
        public void getBoundingBoxInsideFace(List<Tag> old_Faces,Tag main_f, ref List<Tag> new_Faces)
        {
            CFace cf = new CFace();
            CFace.CFaceData cfdata = cf.getFacedata(main_f);
            double xx  = cfdata.dir[0];
            double yy = cfdata.dir[1];
            double zz = cfdata.dir[2];
            double[] dir_normal = new double[3];

            if (xx < 0)
                xx = xx * (-1);
            if (yy < 0)
                yy = yy * (-1);
            if (zz < 0)
                zz = zz * (-1);

            showlog("old " + xx + "====" + yy + "====" + zz);
            if ( xx > yy )
            {
                if (xx > zz)
                {
                    dir_normal[0] = 0;
                    dir_normal[1] = 1;
                    dir_normal[2] = 1;
                }
                else//xx <zz
                {
                    if (zz > yy)
                    {
                        dir_normal[0] = 1;
                        dir_normal[1] = 1;
                        dir_normal[2] = 0;
                    }
                    else//yy>zz
                    {
                        dir_normal[0] = 0;
                        dir_normal[1] = 1;
                        dir_normal[2] = 1;
                        
                    }
                }
            }
            else//xx<yy
            {
                if (yy > zz)
                {
                    dir_normal[0] = 1;
                    dir_normal[1] = 0;
                    dir_normal[2] = 1;
                }
                else// yy<zz
                {
                    if (zz> xx)
                    {
                        dir_normal[0] = 1;
                        dir_normal[1] = 1;
                        dir_normal[2] = 0;
                    }
                    else//zz<xx
                    {
                        dir_normal[0] = 1;
                        dir_normal[1] = 0;
                        dir_normal[2] = 1;
                    }
                }
            }
            showlog("new " + dir_normal[0] + "====" + dir_normal[1] + "====" + dir_normal[2]);
            // cfdata
            double[] v_min = new double[3];
            double[] v_max = new double[3];
            double[] t_min = new double[3];
            double[] t_max = new double[3];
            v_min[0] = cfdata.box[0] * dir_normal[0];
            v_min[1] = cfdata.box[1] * dir_normal[1];
            v_min[2] = cfdata.box[2] * dir_normal[2];
            v_min = getNoneZeroDir(v_min);

            v_max[0] = cfdata.box[3] * dir_normal[0];
            v_max[1] = cfdata.box[4] * dir_normal[1];
            v_max[2] = cfdata.box[5] * dir_normal[2];
            v_max = getNoneZeroDir(v_max);

            foreach (Tag old_f in old_Faces)
            {
                CFace.CFaceData temp_data = cf.getFacedata(old_f);

               
                t_min[0] = temp_data.box[0] * dir_normal[0];
                t_min[1] = temp_data.box[1] * dir_normal[1];
                t_min[2] = temp_data.box[2] * dir_normal[2];
                t_min = getNoneZeroDir(t_min);

                t_max[0] = temp_data.box[3] * dir_normal[0];
                t_max[1] = temp_data.box[4] * dir_normal[1];
                t_max[2] = temp_data.box[5] * dir_normal[2];
                t_max = getNoneZeroDir(t_max);

                // 範圍內的面才要
                if (v_min[0] < t_min[0] && v_max[0] > t_max[0] &&
                    v_min[1] < t_min[1] && v_max[1] > t_max[1] 
                    )
                {
                    new_Faces.Add(old_f);
                }

            }//end for


        }//end fun


        // 將非零值排在 [0]  [1]
        public double[] getNoneZeroDir(double[] dir)
        {
            double[] result = new double[3]{0,0,0};

            for(int i=0;i<dir.Length;i++)
            {
                if (dir[i] != 0)
                    result[i] = dir[i];
            }

            return result;
        }


        // 自動判斷孔類型(螺絲孔),並填入屬性
        public void AutoDetermine_HoleType(Part wk,ref List<FeatureGroup> Out_FeatureGroupAary)
        {

            CFace cf = new CFace();

            String id_name = "SHCS_Hole";
            String id_name_cooling = "Cooling_Hole";

            for(int i=0;i<Out_FeatureGroupAary.Count;i++)
            {
                for(int j=0;j<Out_FeatureGroupAary[i].SubGroupAry.Count;j++)
                {
                    foreach(Tag f_t in Out_FeatureGroupAary[i].SubGroupAry[j].faceAry )
                    {
                        CFace.CFaceData cfdata = cf.getFacedata(f_t);
                        //showlog(f_t + " == " + cfdata.type_face);
                        if (cfdata.type_face == UFConstants.UF_bounded_plane_type)
                        {
                            Out_FeatureGroupAary[i].feat_type = id_name;
                            break;
                        }
                    }

                }//end for j
            }//end for i

            //------find max group by Hole Type in current prt--------------------------------------------------------
            int group_max = -1;
            List<FeatureAutoReg.FeatureGroup> temp_fg = new List<FeatureAutoReg.FeatureGroup>();
            getFeature_SubGroup_new(wk, id_name, ref temp_fg);
            if (temp_fg.Count == 0)
                group_max = 0;
            else
                group_max = temp_fg[0].SubGroupAry.Count;

            //------set attribute value by sub group------------------------------------------------------------------
            for (int i = 0; i < Out_FeatureGroupAary.Count; i++)
            {
                String feat_type_name = Out_FeatureGroupAary[i].feat_type;
                if (feat_type_name == "")
                    continue;

                for (int j = 0; j < Out_FeatureGroupAary[i].SubGroupAry.Count; j++)
                {
                    foreach (Tag face_t in Out_FeatureGroupAary[i].SubGroupAry[j].faceAry)
                    {
                        // set feature type
                        setFaceType_StringAttribute(face_t, feat_type_name);
                        // set feature sub type  0~n 
                        setFaceGroup_StringAttribute(face_t, group_max.ToString());       
                    }
                    group_max++;

                }//end for j

            }//end for i



        }//end AutoDetermine_HoleType




        // 自動判斷孔類型(水孔),並填入屬性
        public void AutoDetermine_HoleTypeByCooling(Part wk, ref List<FeatureGroup> Out_FeatureGroupAary)
        {
            CFace cf = new CFace();
            String id_name = "Cooling_Hole";
            
            for (int i = 0; i < Out_FeatureGroupAary.Count; i++)
            {
                //if (Out_FeatureGroupAary[i].feat_type != "")
                //    continue;
                int iscooling = 0;
                for (int j = 0; j < Out_FeatureGroupAary[i].SubGroupAry.Count; j++)
                {

                    List<Tag> face_list = new List<Tag>();
                    face_list.AddRange(Out_FeatureGroupAary[i].SubGroupAry[j].faceAry);

                    foreach (Tag f_t in Out_FeatureGroupAary[i].SubGroupAry[j].faceAry)
                    {
                        List<Tag> temp_list = new List<Tag>();
                        cf.adjFaces(f_t, out temp_list);
                        face_list.AddRange(temp_list);
                    }
                    // 刪除重複元素
                    CLinQ.ListDeleteRepeatElemt(ref face_list);
                    //showlog("org + adj " + CLinQ.ListToString(face_list));
                    for (int k = 0; k < Out_FeatureGroupAary[i].SubGroupAry.Count; k++)
                    {
                        if (j == k)
                            continue;

                        List<Tag> Intersec_list = new List<Tag>();
                        Intersec_list = CLinQ.ListIntersected(Out_FeatureGroupAary[i].SubGroupAry[k].faceAry, face_list);
                        //showlog("intersec " + CLinQ.ListToString(Intersec_list));
                        if (Intersec_list.Count != 0)
                        {
                            iscooling++;
                            Out_FeatureGroupAary[i].feat_type = id_name;
                            break;
                        }
                    }

                    //if (iscooling == 0)// 都跟別人沒有相鄰, 則判斷
                    //{

                    //}

                }//end for j
            }//end for i

            //------find max group by Hole Type in current prt--------------------------------------------------------
            int group_max = -1;
            List<FeatureAutoReg.FeatureGroup> temp_fg = new List<FeatureAutoReg.FeatureGroup>();
            getFeature_SubGroup_new(wk, id_name, ref temp_fg);
            if (temp_fg.Count == 0)
                group_max = 0;
            else
                group_max = temp_fg[0].SubGroupAry.Count;

            //------set attribute value by sub group------------------------------------------------------------------
            for (int i = 0; i < Out_FeatureGroupAary.Count; i++)
            {
                String feat_type_name = Out_FeatureGroupAary[i].feat_type;
                if (feat_type_name == "")
                    continue;

                for (int j = 0; j < Out_FeatureGroupAary[i].SubGroupAry.Count; j++)
                {
                    foreach (Tag face_t in Out_FeatureGroupAary[i].SubGroupAry[j].faceAry)
                    {
                        // set feature type
                        setFaceType_StringAttribute(face_t, feat_type_name);
                        // set feature sub type  0~n 
                        setFaceGroup_StringAttribute(face_t, group_max.ToString());
                    }
                    group_max++;

                }//end for j

            }//end for i



        }//end AutoDetermine_HoleTypeByCooling

        // 面是否是外形邊界面
        public bool ISOnBoundingBoxFace(Part wkPrt,Tag face_t)
        {
            CPart prt = new CPart();
            CFace cf = new CFace();
            // 過濾邊界面
            double[] bounding = new double[6];
            prt.getBoundingBox2Point(wkPrt, bounding);
            //showlog("box1 " + bounding[0] + "==" + bounding[1] + "==" + bounding[2]);
            //showlog("box2 " + bounding[3] + "==" + bounding[4] + "==" + bounding[5]);
            List<Tag> filter_face_list = new List<Tag>();//外迴圈

            bool isonFace = cf.IsThroughFace(face_t, bounding);
            return isonFace;
          
        }


    } // end FeatureAutoReg


    public class CPart
    {
        // class members
        private Session theSession_;
        private UI theUI_;
        private UFSession theUfSession_;
        private CLog clog;
    
        public CPart()
        {
            try
            {
                theSession_ = Session.GetSession();
                theUI_ = UI.GetUI();
                theUfSession_ = UFSession.GetUFSession();
                clog = new CLog();

            }
            catch (NXOpen.NXException ex)
            {
                clog.showlog(ex.Message);

            }
        }

        public void showlog(String log)
        {
            clog.showlog(log);
        }

        public void showmsg(String msg)
        {
            clog.showmsg(msg);
        }


        public Body getPartBodyBySession()
        {
            Part wk = theSession_.Parts.Work;

            Body[] bodyAry = wk.Bodies.ToArray();

            return bodyAry[0];
        }

        public Body getPartBodyByTag(Tag prt_tag)
        {
            Part obj = (Part)NXObjectManager.Get(prt_tag);
            Body[] bodyAry = obj.Bodies.ToArray();
            return bodyAry[0];
        }


        public bool ISAsm(out Tag rootPrt)
        {
            Part displayPart = theSession_.Parts.Display;
            Tag tagRootPart = theUfSession_.Assem.AskRootPartOcc(displayPart.Tag);
            if (tagRootPart == NXOpen.Tag.Null)
            {
                rootPrt = NXOpen.Tag.Null;
                return false;
            }
            else
            {
                rootPrt = tagRootPart;
                return true;
            }
        }

        public bool ISAsm(Part obj_prt)
        {
            Part displayPart = obj_prt;
            Tag tagRootPart = theUfSession_.Assem.AskRootPartOcc(displayPart.Tag);
            if (tagRootPart == NXOpen.Tag.Null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public static void Refresh()
        {
            UFSession theUfSession1 = UFSession.GetUFSession();
            theUfSession1.Disp.Refresh();
        }

        public List<String> getHoleFeatureTypeName()
        {
            List<String> names = new List<String>();
            names.Add("HOLE PACKAGE");
            names.Add("CBORE_HOLE");       
            return names;
        }

        public bool isHoleFeature(String feat_type)
        {
            List<String> holeAry = new List<String>();
            holeAry = getHoleFeatureTypeName();

            bool ishole = holeAry.Contains(feat_type);

            return ishole;
        }


        public List<Feature> getHoleFeature(Part wkPart)
        {
            List<Feature> featAllAry = new List<Feature>();
            Feature[] featAry = wkPart.Features.GetFeatures();
            for (int i=1;i<featAry.Length ;i++)
            {
                Feature feat_temp = featAry[i];
                String str = feat_temp.FeatureType;
                String name = feat_temp.GetFeatureName();
                showlog(" >> " + i +" == "+ feat_temp.GetType() + "==" + str + " == " + name);               
                if (isHoleFeature(str))
                {
                    List<Face> feat_faces = new List<Face>();
                    feat_faces = getFeatureFace(feat_temp);
                    showlog(" feature feacs : " + feat_faces.Count);
                    if (feat_faces.Count > 0)
                    {
                        featAllAry.Add(feat_temp);
                    }
                }//end if

            }//end for

            return featAllAry;
        }

        public List<Face> getFeatureFace(Feature current_face)
        {
            Tag[] featFaceAry = { };
            theUfSession_.Modl.AskFeatFaces(current_face.Tag, out featFaceAry);

            List<Face> allFace = new List<Face>();
            for (int i = 0; i < featFaceAry.Length; i++)
            {
                Face temp_face = (Face)CTrans.TagtoNXObject(featFaceAry[i]);
                allFace.Add(temp_face);
            }

            return allFace;
        }

        public List<Tag> getFeatureFaceByTag(Feature current_face)
        {
            Tag[] featFaceAry = { };
            theUfSession_.Modl.AskFeatFaces(current_face.Tag, out featFaceAry);

            //showlog("size" + featFaceAry.Length);
            List<Tag> allFace = new List<Tag>();
            for (int i = 0; i < featFaceAry.Length; i++)
            {
                allFace.Add(featFaceAry[i]);
            }
            return allFace;
        }

        public List<double> getFeatureFaceByDiameter(Feature current_face)
        {
            Tag[] featFaceAry = { };
            theUfSession_.Modl.AskFeatFaces(current_face.Tag, out featFaceAry);

            CFace fd = new CFace();
            List<double> allFaceData = new List<double>();
            for (int i = 0; i < featFaceAry.Length; i++)
            {
                CFace.CFaceData fdd = fd.getFacedata(featFaceAry[i]);
                allFaceData.Add(fdd.radius * 2);
            }
            return allFaceData;
        }

        /*
          
                // 取得最top comp or curremt comp
                NXOpen.Assemblies.Component topComp = theSession.Parts.WorkComponent;
                // 1. 先 set work part
                PartLoadStatus loadstatus;
                theSession.Parts.SetWorkComponent(wpComp, out loadstatus);

                // 2. 再設回 top work part
                theSession.Parts.SetWorkComponent(topComp, out loadstatus);
         */


        /// <summary>
        /// 求BOX 的8個點
        /// </summary>
        /// <param name="prt"></param>
        /// <param name="BoxPoint"></param>
        public void getBoundingBox8Point(Part prt, double[,] BoxPoint)
        {
            CLog log = new CLog();
            try
            {
                Body[] bodys = prt.Bodies.ToArray();
                if (bodys.Length == 0)
                {
                    log.showmsg("找不到實體，請確認零件");
                    return;
                }

                double[] min_corner = new double[3];
                double[,] directions = new double[3, 3];
                double[] distances = new double[3];

                theUfSession_.Modl.AskBoundingBoxExact(bodys[0].Tag, NXOpen.Tag.Null, min_corner, directions, distances);

                // 下4點
                BoxPoint[0, 0] = min_corner[0];
                BoxPoint[0, 1] = min_corner[1];
                BoxPoint[0, 2] = min_corner[2];

                BoxPoint[1, 0] = min_corner[0] + distances[0];
                BoxPoint[1, 1] = min_corner[1];
                BoxPoint[1, 2] = min_corner[2];

                BoxPoint[2, 0] = min_corner[0];
                BoxPoint[2, 1] = min_corner[1] + distances[1];
                BoxPoint[2, 2] = min_corner[2];

                BoxPoint[3, 0] = min_corner[0] + distances[0];
                BoxPoint[3, 1] = min_corner[1] + distances[1];
                BoxPoint[3, 2] = min_corner[2];

                // 上4點
                BoxPoint[4, 0] = min_corner[0];
                BoxPoint[4, 1] = min_corner[1];
                BoxPoint[4, 2] = min_corner[2] + distances[2];

                BoxPoint[5, 0] = min_corner[0] + distances[0];
                BoxPoint[5, 1] = min_corner[1];
                BoxPoint[5, 2] = min_corner[2] + distances[2];

                BoxPoint[6, 0] = min_corner[0];
                BoxPoint[6, 1] = min_corner[1] + distances[1];
                BoxPoint[6, 2] = min_corner[2] + distances[2];

                BoxPoint[7, 0] = min_corner[0] + distances[0];
                BoxPoint[7, 1] = min_corner[1] + distances[1];
                BoxPoint[7, 2] = min_corner[2] + distances[2];



            }
            catch (System.Exception ex)
            {

            }



        }

        /// <summary>
        /// 求BOX 的8個點
        /// </summary>
        /// <param name="prt"></param>
        /// <param name="BoxPoint"></param>
        public void getBoundingBox2Point(Part prt, double[] BoxPoint)
        {
            CLog log = new CLog();
            try
            {
                Body[] bodys = prt.Bodies.ToArray();
                if (bodys.Length == 0)
                {
                    log.showmsg("找不到實體，請確認零件");
                    return;
                }

                double[] min_corner = new double[3];
                double[,] directions = new double[3, 3];
                double[] distances = new double[3];

                theUfSession_.Modl.AskBoundingBoxExact(bodys[0].Tag, NXOpen.Tag.Null, min_corner, directions, distances);

                // 下4點
                BoxPoint[0] = min_corner[0];
                BoxPoint[1] = min_corner[1];
                BoxPoint[2] = min_corner[2];

                BoxPoint[3] = min_corner[0] + distances[0];
                BoxPoint[4] = min_corner[1] + distances[1];
                BoxPoint[5] = min_corner[2] + distances[2];



            }
            catch (System.Exception ex)
            {

            }
        }


        public bool getSolidBody(Part wk, ref List<Body> objBodys)
        {
            Body[] allBody = wk.Bodies.ToArray();
            foreach (Body obj in allBody)
            {
                if (obj.IsSolidBody && obj.Layer == 1)
                {
                    objBodys.Add(obj);
                }
            }

            if (objBodys.Count == 0)
                return false;
            else
                return true;
        }


        public bool getSolidBodyByPoint(Part wk,double[] pt  ,int dir, out Body body_obj  )
        {
            List<Body> bodys = new List<Body>();
            bool isok = getSolidBody(wk, ref bodys);
            if (!isok)
            {
                body_obj = null;
                return  false;
            }

            double[] direction = new double[3];
            if (dir == 0)// x
            {
                direction[0] = 0;
                direction[1] = 0;
                direction[2] = 1;
            }
            else if (dir == 1)//y
            {
                direction[0] = 0;
                direction[1] = 0;
                direction[2] = 1;
            }
            else if (dir == 2)//y
            {
                direction[0] = 1;
                direction[1] = 0;
                direction[2] = 0;
            }


            // 射線
            double[] sstransform = 
            { 
                1.0,0.0,0.0,0.0,
                0.0,1.0,0.0,0.0,
                0.0,0.0,1.0,0.0,
                0.0,0.0,0.0,1.0 
            };

            UFModl.RayHitPointInfo[] hit_list = new UFModl.RayHitPointInfo[bodys.Count];
        
            int num_results = 0;

            List<Tag> bodys_t = new List<Tag>();
            foreach(Body bb in bodys)
            {
                bodys_t.Add(bb.Tag);
            }

            try
            {
                theUfSession_.Modl.TraceARay(
                    bodys.Count,
                    bodys_t.ToArray(),
                    pt,
                    direction,
                    sstransform,
                    0,
                    out num_results,
                    out hit_list);
            }
            catch (System.Exception ex)
            {
                showlog("X ray Detect error !");
            }

            if (num_results == 0)
            {
                if (bodys.Count != 0)
                {
                    body_obj = bodys[0];
                    return true;
                }
                else
                {
                    body_obj = null;
                    return false;
                }
            }
            else
            {
                body_obj = (Body) CTrans.TagtoNXObject(hit_list[0].hit_body);
                return true;
            }
           

        }



        public bool getSolidBodyByMultiPoint(Part wk, List<Point3d> pts , int dir, ref List<Body> allBodys)
        {
            List<Body> bodys = new List<Body>();
            bool isok = getSolidBody(wk, ref bodys);
            if (!isok)
            {
                return false;
            }

            double[] direction = new double[3];
            if (dir == 0)// x
            {
                direction[0] = 0;
                direction[1] = 0;
                direction[2] = 1;
            }
            else if (dir == 1)//y
            {
                direction[0] = 0;
                direction[1] = 0;
                direction[2] = 1;
            }
            else if (dir == 2)//y
            {
                direction[0] = 1;
                direction[1] = 0;
                direction[2] = 0;
            }


            // 射線
            double[] sstransform = 
            { 
                1.0,0.0,0.0,0.0,
                0.0,1.0,0.0,0.0,
                0.0,0.0,1.0,0.0,
                0.0,0.0,0.0,1.0 
            };

            UFModl.RayHitPointInfo[] hit_list = new UFModl.RayHitPointInfo[bodys.Count];

            int num_results = 0;

            List<Tag> bodys_t = new List<Tag>();
            foreach (Body bb in bodys)
            {
                bodys_t.Add(bb.Tag);
            }

            for (int i = 0; i < pts.Count;i++ )
            {
                double[] pt = new double[3];
                pt[0] = pts[i].X;
                pt[1] = pts[i].Y;
                pt[2] = pts[i].Z;
                try
                {
                    theUfSession_.Modl.TraceARay(
                        bodys.Count,
                        bodys_t.ToArray(),
                        pt,
                        direction,
                        sstransform,
                        0,
                        out num_results,
                        out hit_list);
                }
                catch (System.Exception ex)
                {
                    showlog("X ray Detect error !");
                    continue;
                }

                if (num_results != 0)
                {
                    foreach(UFModl.RayHitPointInfo hit in hit_list)
                    {
                        Body body_obj = (Body)CTrans.TagtoNXObject(hit_list[0].hit_body);
                        allBodys.Add(body_obj);
                    }
                }
  
            }

            allBodys.AddRange(bodys);
            if (allBodys.Count == 0)
                return false;

            return true;

        }



    }//end class



    public class CTrans
    {
        // class members
        private Session theSession_;
        private UI theUI_;
        private UFSession theUfSession_;
        private CLog clog;

        public CTrans()
        {
            try
            {
                theSession_ = Session.GetSession();
                theUI_ = UI.GetUI();
                theUfSession_ = UFSession.GetUFSession();

                clog = new CLog();
            }
            catch (NXOpen.NXException ex)
            {
                clog.showlog(ex.Message);

            }
        }

        public void showlog(String log)
        {
            //lwu_.WriteLine(log + "\n");
            clog.showlog(log + "\n");
        }

        public void showmsg(String msg)
        {
            clog.showmsg(msg);
        }

        public static TaggedObject TagtoNXObject(Tag tag_)
        {
            TaggedObject obj = NXObjectManager.Get(tag_);
            return obj;
        }

        public static String TagToString(Tag obj)
        {
            return obj.ToString();
        }

        public static Tag StringToTag(String obj_str)
        {
            Tag xFaceTag = (Tag)Convert.ToInt32(obj_str);
            return xFaceTag;
        }

        public static Face StringToFace(String obj_str)
        {
            Tag tag = StringToTag(obj_str);
            Face obj = (Face)NXObjectManager.Get(tag);
            return obj;
        }


        public static String DoubleToString(double val)
        {
            double temp_d = Math.Round(val, 4, MidpointRounding.ToEven);//四捨五入
            return temp_d.ToString();
        }

        //四捨五入
        public static double DoubleToAccuracyDouble_ToEvent(double val,int num)
        {
            double temp_d = Math.Round(val, num, MidpointRounding.ToEven);//四捨五入
            return temp_d;
        }


        //無條件捨去
        public static double DoubleToAccuracyDouble_FromZero(double val, int num)
        {
            double temp_d = Math.Round(val, num, MidpointRounding.AwayFromZero);//無條件捨去
            return temp_d;
        }


    }



    public class CLog
    {
        // class members
        private Session theSession_;
        private UI theUI_;
        private UFSession theUfSession_;
        private ListingWindow lwu_;
        private LogFile logF;
        private bool isopen_wlog = true;//LOG的開關

        public CLog(bool islog)
        {
            try
            {
                isopen_wlog = islog;
                theSession_ = Session.GetSession();
                theUI_ = UI.GetUI();
                theUfSession_ = UFSession.GetUFSession();
                lwu_ = theSession_.ListingWindow;
                if (isopen_wlog)
                    lwu_.Open();

                logF = theSession_.LogFile;
            }
            catch (NXOpen.NXException ex)
            {
                lwu_.WriteLine(ex.Message);

            }
        }

        public CLog()
        {
            try
            {
                theSession_ = Session.GetSession();
                theUI_ = UI.GetUI();
                theUfSession_ = UFSession.GetUFSession();
                lwu_ = theSession_.ListingWindow;
                if (isopen_wlog)
                    lwu_.Open();

                logF = theSession_.LogFile;
            }
            catch (NXOpen.NXException ex)
            {
                lwu_.WriteLine(ex.Message);

            }
        }

        public void showlog(String log)
        {
            if (isopen_wlog)
                lwu_.WriteLine(log + "\n");
            else
            logF.WriteLine(log + "\n");
        }

        public void showmsg(String msg)
        {
            UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, msg);
        }


        public void showlogByPoint(String str,double[] p)
        {
            showlog(str + "X : " + p[0] + " Y : " + p[1] + " Z : " + p[2]);
        }

        public void showlogByMtx16(double[] final_mm)
        {
            String msg = String.Format("final_mm 1\n{0},{1},{2},{3}\n{4},{5},{6},{7}\n{8},{9},{10},{11}\n{12},{13},{14},{15}",
                               final_mm[0], final_mm[1], final_mm[2], final_mm[3],
                               final_mm[4], final_mm[5], final_mm[6], final_mm[7],
                               final_mm[8], final_mm[9], final_mm[10], final_mm[11],
                               final_mm[12], final_mm[13], final_mm[14], final_mm[15]);
            showlog(msg);
        }

        public void showlogByMtx9(double[] final_mm)
        {
            String msg = String.Format("final_mm 1\n{0},{1},{2}\n{3},{4},{5}\n{6},{7},{8}",
                               final_mm[0], final_mm[1], final_mm[2], final_mm[3],
                               final_mm[4], final_mm[5], final_mm[6], final_mm[7],
                               final_mm[8]);
            showlog(msg);
        }


    }


    // udf 包裝
    class CUdf
    {
        // class members
        private Session theSessionudf;
        private UI theUIudf;
        private UFSession theUfSessionudf;
        private CLog clog;

        private String udf_lib_prt = "";

        private String[] ref_msg_txtAry;
        private Tag[] ref_mag_tagAry;
        private String[] dim_msg_txtAry;
        private String[] dim_value_txtAry;


        public CUdf()
        {
            try
            {
                theSessionudf = Session.GetSession();
                theUIudf = UI.GetUI();
                theUfSessionudf = UFSession.GetUFSession();
                clog = new CLog();

            }
            catch (NXOpen.NXException ex)
            {
                clog.showlog(ex.Message);

            }
        }

        public void showlog(String log)
        {
            //lwudf.WriteLine(log + "\n");
            clog.showlog(log + "\n");
        }

        public void showmsg(String msg)
        {
            clog.showmsg(msg);
        }

        public Feature udfCreate(String udf_path,        //  路徑
                                String[] ref_msg_txt,
                                Tag[] ref_mag_tag,
                                String[] dim_msg_txt,
                                String[] dim_value_txt
                                )
        {
            // 1.udf part 位置
            udf_lib_prt = udf_path;

            // 2. 
            ref_msg_txtAry = new String[ref_msg_txt.Length];
            ref_mag_tagAry = new Tag[ref_mag_tag.Length];

            for (int i = 0; i < ref_msg_txt.Length; i++)
            {
                ref_msg_txtAry[i] = ref_msg_txt[i];
                ref_mag_tagAry[i] = ref_mag_tag[i];
            }
                

            // 3. 
            dim_msg_txtAry = new String[dim_msg_txt.Length];
            dim_value_txtAry = new String[dim_value_txt.Length];
            for (int i = 0; i < dim_msg_txt.Length; i++)
            {
                dim_msg_txtAry[i] = dim_msg_txt[i];
                dim_value_txtAry[i] = dim_value_txt[i];
            }
           

            Feature udg_feat = udfMain();
            return udg_feat;
        }

        public Feature udfMain()
        {
            string udfpartname = null;
            // check for existing target work part
//             Tag work_part = theUfSessionudf.Assem.AskWorkPart();
//             if (work_part == Tag.Null)
//             {
//                 showlog("No active work part...exit.");
//                 return;
//             }

            // 1.判斷是否有 udf 零件
            if (udf_lib_prt == "")
            {
                if (Part_selection("Select an UDF part", ref udfpartname) != false)
                    showlog("Part selected: " + udfpartname);
                else
                {
                    showlog("No Part selected");
                    return null;
                }
            }
            else
                udfpartname = udf_lib_prt;

            // 2. 設定udf part 為工作零件(記憶體中)
            Tag UdfPartTag = Tag.Null;
            UFPart.LoadStatus error_status;
            theUfSessionudf.Part.OpenQuiet(udfpartname, out UdfPartTag, out error_status);
            if (error_status.failed)
            {
                report_load_status(error_status);
                return null;
            }

            // 3. 尋找 udf 特徵 (當時包出來的特徵)
            Tag UdfFeatTag = Tag.Null;
            ask_next_feature_of_type(UdfPartTag, "UDF_DEF", ref UdfFeatTag);
            if (UdfFeatTag == Tag.Null)
            {
                showlog("No UDF in Part " + udfpartname + " found.");
                return null;
            }

            String udf_name;
            theUfSessionudf.Obj.AskName(UdfFeatTag, out udf_name);
            showlog("UDF Feature  Tag=" + UdfPartTag.ToString() + " Name=" + udf_name);
            showlog("\nAskUdfDefinition...");

            // 4. 取得 udf 的參照與尺寸參數
            Tag[] lParents;
            String[] lParentsPrompt;
            String[] lExpression‍Prompt;
            int lNumberParents, lNumberExpressions;
            Tag[] lExpressions;

            theUfSessionudf.Modl.AskUdfDefinition(
                UdfFeatTag,
                out lParents,
                out lParentsPrompt,
                out lNumberParents,
                out lExpressions,
                out lExpression‍Prompt,
                out lNumberExpressions);



            // 5. mapping 順序
            showlog("lNumberParents=" + lNumberParents.ToString());
            Tag[] newParents = new Tag[lNumberParents];
            for (int i = 0; i < lNumberParents; i++)
            {
                //showlog(" " + (i + 1).ToString() + ". " + lParents[i].ToString() + " " + lParentsPrompt[i]);

                for (int j = 0; j < ref_msg_txtAry.Length; j++)
                {
                    if (lParentsPrompt[i] == ref_msg_txtAry[j])
                    {
                        newParents[i] = ref_mag_tagAry[j];
                        showlog(" " + (i + 1).ToString() + ". " + lParents[i].ToString() + " " + lParentsPrompt[i] + "  " + ref_mag_tagAry[i].ToString());
                        break;
                    }
                }

                //newParents[i] = select_anything_by_mask(lParentsPrompt[i]).Tag;
            }



            // 6. 尺寸設定
            String[] new_exp_rhs = new String[lNumberExpressions];
            String exp_str, lhs_str, rhs_str;
            Tag exp_tag;
            showlog("lNumberExpressions=" + lNumberExpressions.ToString());
            for (int i = 0; i < lNumberExpressions; i++)
            {
                theUfSessionudf.Modl.AskExpTagString(lExpressions[i], out exp_str);
                theUfSessionudf.Modl.DissectExpString(exp_str, out lhs_str, out rhs_str, out exp_tag);
                
                //showlog(" " + (i + 1).ToString() + ". " + lExpressions[i].ToString() + " " + lExpression‍Prompt[i] + " = " + rhs_str);

                for (int j = 0; j < dim_msg_txtAry.Length; j++)
                {
                    if (lExpression‍Prompt[i] == dim_msg_txtAry[j])
                    {
                        new_exp_rhs[i] = dim_value_txtAry[j];
                        showlog(" " + (i + 1).ToString() + ". " + lExpressions[i].ToString() + " " + lExpression‍Prompt[i] + " = " + rhs_str + "  " + dim_value_txtAry[j]);
                        break;
                    }
                }

                //new_exp_rhs[i] = NXInputBox.GetInputString(lExpression‍Prompt[i], "Enter new Expression‍:", rhs_str);

            }

            // works as of 5.0.3, see PR-1635826
            //theUfSession.Modl.RegisterRpoRoutine( theUfSession.Modl.UdfRpoMenu);

            String cgmName = Path.GetFileNameWithoutExtension(udfpartname);
            cgmName = cgmName + ".cgm";

            showlog("\nCreateInstantiatedUdf...");
           
            
            try
            {
                      
                 Tag new_udf = NXOpen.Tag.Null;
                 theUfSessionudf.Modl.CreateInstantiatedUdf(UdfFeatTag, cgmName, lParents, newParents, lNumberParents, lExpressions, new_exp_rhs, lNumberExpressions, out new_udf);

                 if (new_udf != NXOpen.Tag.Null)
                     return (Feature)CTrans.TagtoNXObject(new_udf);
                 else
                     return null;
            }
            catch (NXOpen.NXException ex)
            {
                clog.showlog(ex.Message);
            }


            return null;

        }


        //------------------------------------------------------------------------------
        //  Part selection
        //------------------------------------------------------------------------------
        public bool Part_selection(string prompt, ref string filename)
        {
            int resp;
            string filter, dir_name, fltr_name;

            theUfSessionudf.Ui.AskDialogDirectory(UFUi.DialogDirId.PartDir, out dir_name);
            theUfSessionudf.Ui.AskDialogFilter(UFUi.DialogFilterId.PartOpenFltr, out fltr_name);

            filter = dir_name + fltr_name;

            theUfSessionudf.Ui.CreateFilebox(prompt, prompt, ref filter, "*.prt", out filename, out resp);
            if (resp != UFConstants.UF_UI_CANCEL) return true;
            else return false;
        }

        //------------------------------------------------------------------------------
        //  Report load status on failure
        //------------------------------------------------------------------------------
        public void report_load_status(UFPart.LoadStatus ls)
        {
            string msg;
            int ii;

            for (ii = 0; ii < ls.n_parts; ii++)
            {
                theUfSessionudf.UF.GetFailMessage(ls.statuses[ii], out msg);
                showlog("Error loading Part " + ls.file_names[ii]);
                showlog(msg);
            }
        }

        //------------------------------------------------------------------------------
        //  Select any object by mask
        //------------------------------------------------------------------------------
        public NXObject select_anything_by_mask(string prompt)
        {
            NXObject selobj;
            Point3d cursor;
            Selection.MaskTriple[] mask = new Selection.MaskTriple[3];

            mask[0].Type = UFConstants.UF_datum_plane_type;
            mask[0].Subtype = 0;
            mask[0].SolidBodySubtype = 0;
            mask[1].Type = UFConstants.UF_solid_type;
            mask[1].Subtype = 0;
            mask[1].SolidBodySubtype = UFConstants.UF_UI_SEL_FEATURE_ANY_FACE;

            mask[2].Type = UFConstants.UF_solid_type;
            mask[2].Subtype = 0;
            mask[2].SolidBodySubtype = UFConstants.UF_solid_body_subtype;

            Selection.Response resp = theUIudf.SelectionManager.SelectObject("Select", prompt,
                Selection.SelectionScope.WorkPart, Selection.SelectionAction.ClearAndEnableSpecific, false, false, mask, out selobj, out cursor);

            if (resp == Selection.Response.ObjectSelected ||
                resp == Selection.Response.ObjectSelectedByName)
            {
                return selobj;
            }
            else
                return null;
        }

        //------------------------------------------------------------------------------
        //  Cycle part for specified feature type
        //------------------------------------------------------------------------------
        public Tag ask_next_feature_of_type(Tag parttag, string type, ref Tag feat)
        {
            string this_type = null;

            do
            {
                theUfSessionudf.Obj.CycleObjsInPart(parttag, UFConstants.UF_feature_type, ref feat);
                if (feat != Tag.Null && theUfSessionudf.Obj.AskStatus(feat) == UFConstants.UF_OBJ_ALIVE)
                {
                    theUfSessionudf.Modl.AskFeatType(feat, out this_type);
                    if (this_type.CompareTo(type) == 0) return feat;
                }
            } while (feat != Tag.Null);

            return Tag.Null;
        }


        public void getUdfFeatureAllFeats(Feature udf_ins, ref List<Feature> Feats)
        {


            Tag[] udf_feats = {};
            int udf_feats_size = 0;
            theUfSessionudf.Modl.AskFeaturesOfUdf(udf_ins.Tag, out udf_feats, out udf_feats_size);

            for (int i = 0; i < udf_feats_size;i++ )
            {
                Feature t_feat = (Feature)CTrans.TagtoNXObject(udf_feats[i]);
                Feats.Add(t_feat);
            }

        }

        public void getUdfFeatureAllFace(Feature udf_ins, ref List<Face> Faces)
        {
            //List<Face> Faces = new List<Face>();
            Tag[] udf_feats = { };
            int udf_feats_size = 0;
            theUfSessionudf.Modl.AskFeaturesOfUdf(udf_ins.Tag, out udf_feats, out udf_feats_size);

            for (int i = 0; i < udf_feats_size; i++)
            {
                Tag[] faces_temp = { };
                theUfSessionudf.Modl.AskFeatFaces(udf_feats[i], out faces_temp);

                for (int j = 0; j < faces_temp.Length; j++)
                {
                    Face face_obj = (Face)CTrans.TagtoNXObject(faces_temp[j]);
                    Faces.Add(face_obj);
                }
            }

        }


    }//end class





    //================================================================================
    class CFace
    {
        // class members
        private Session theSessionudf;
        private UI theUIudf;
        private UFSession theUfSessionudf;
        private CLog clog;

        public struct CFaceData
        {
            public int type_face;
            public double[] point;
            public double[] dir;
            public double[] box;
            public double radius;
            public double rad_data;
            public int norm_dir;
          

        }


        private int filter_i = 16;


        public CFace()
        {
            try
            {
                theSessionudf = Session.GetSession();
                theUIudf = UI.GetUI();
                theUfSessionudf = UFSession.GetUFSession();
             
                //getFacedata(obj_face);
                clog = new CLog();

            }
            catch (NXOpen.NXException ex)
            {
                clog.showlog(ex.Message);

            }
        }


        public void showlog(String log)
        {
            //lwudf.WriteLine(log + "\n");
            clog.showlog(log + "\n");
        }

        public void showmsg(String msg)
        {
             clog.showmsg(msg);
        }


        // 取得面資料
        public CFaceData getFacedata(Tag f_tag)
        {
            CFaceData fd;
            fd.point = new double[3];
            fd.dir = new double[3];
            fd.box = new double[6];

            theUfSessionudf.Modl.AskFaceData(f_tag, out fd.type_face,
                  fd.point, fd.dir, fd.box, out fd.radius, out fd.rad_data, out fd.norm_dir);

            //Face ff = (Face)CTrans.TagtoNXObject(f_tag);
            //fd.face_normal = new double[3];
            //fd.face_normal = GetNormal(ff);

            //fd.face_normal = new double[3];

            return fd;
        }


        // 取得所有臨面資料
        public List<CFaceData> adjFace(Tag f_tag)
        {
            Tag[] adjFaces = { };
            theUfSessionudf.Modl.AskAdjacFaces(f_tag, out adjFaces);

            List<CFaceData> fd_list = new List<CFaceData>();
            for (int i = 0; i < adjFaces.Length; i++)
            {
                fd_list.Add(getFacedata(adjFaces[i]));
            }


            return fd_list;
        }

        // 取得所有臨面資料
        public List<CFaceData> adjFaces(Tag f_tag, out List<Tag> facelist)
        {
            Tag[] adjFaces = { };
            theUfSessionudf.Modl.AskAdjacFaces(f_tag, out adjFaces);

            facelist = new List<Tag>();
            List<CFaceData> fd_list = new List<CFaceData>();
            for (int i = 0; i < adjFaces.Length; i++)
            {
                fd_list.Add(getFacedata(adjFaces[i]));
                facelist.Add(adjFaces[i]);
            }


            return fd_list;
        }

        // 找出 零件圓住面
        public void getCylinderFace(Part wkPart)
        {
            Part wk = wkPart;
            Body[] bodyAry = wk.Bodies.ToArray();

            if (bodyAry.Length <= 0)
                return;

            Face[] fAry = bodyAry[0].GetFaces();

            List<Face> fAry1 = new List<Face>();
            for (int i = 0; i < fAry.Length; i++)
            {
                Tag temp_tag = fAry[i].Tag;
                CFaceData fd_temp;
                fd_temp = getFacedata(temp_tag);


                if (fd_temp.type_face == filter_i)// 只有圓柱面
                {
                    //String target_str = String.Format("{0},{1},{2}",fd_temp.point[0],fd_temp.point[1],fd_temp.point[2]);

                    List<CFaceData> adjAry = new List<CFaceData>();
                    adjAry = adjFace(temp_tag);
                    int adj_cyl_num = 0;
                    List<CFaceData> pointAry = new List<CFaceData>();
                    for (int j = 0; j < adjAry.Count; j++)
                    {
                        if (adjAry[j].type_face == filter_i)
                        {
                            //String p_str = String.Format("{0},{1},{2}",adjAry[j].point[0],adjAry[j].point[1],adjAry[j].point[2]);
                            pointAry.Add(adjAry[j]);
                            adj_cyl_num++;
                        }
                    }

                    if (adj_cyl_num == 0) // 半圓弧 + 全圓住曲面
                    {
                        //fAry[i].Highlight();
                    }

                    if (adj_cyl_num == 1) // 圓柱類型 ( 通孔/階梯孔) + 雜的半圓弧
                    {
                        pointAry.Sort();
                        CFaceData cur_fdata = new CFaceData();
                        for (int j = 0; j < pointAry.Count; j++)
                        {
                            if (pointAry[j].type_face == filter_i)
                            {
                                cur_fdata = pointAry[j];
                                break;
                            }
                        }

                        bool ispara = getVectDotisParallel(fd_temp.dir, cur_fdata.dir);


                        //showlog(fd_temp.dir[0] + "==" + fd_temp.dir[1] + "===" + fd_temp.dir[2] + "===" + pointAry[0].dir[0] + "===" + pointAry[0].dir[1] + "===" + pointAry[0].dir[2] + "====" + ispara);

                        if (ispara)// 同軸向
                        {
                            //showlog(">" + fd_temp.dir[0] + "==" + fd_temp.dir[1] + "===" + fd_temp.dir[2] + "===" + pointAry[0].dir[0] + "===" + pointAry[0].dir[1] + "===" + pointAry[0].dir[2] + "====" + ispara);


                            // 是否同心圓
                            double[] pp = new double[3];
                            pp = getVectorByTwoPoint(fd_temp.point, cur_fdata.point);
                            if (pp[0] == 0 && pp[1] == 0 && pp[2] == 0)
                            {
                                fAry1.Add(fAry[i]);
                                //fAry[i].Highlight();
                            }
                            //showlog(">" + pp[0] + "==" + pp[1] + "===" + pp[2] );
                            //showlog("@" + fd_temp.dir[0] + "==" + fd_temp.dir[1] + "===" + fd_temp.dir[2]);

                            bool ispara1 = getVectDotisParallel(fd_temp.dir, pp);
                            if (ispara1)
                            {
                                fAry1.Add(fAry[i]);
                                //fAry[i].Highlight();
                            }

                        }

                    }

                    if (adj_cyl_num >= 2) // 水路孔類型
                    {
                        pointAry.Sort();
                        CFaceData cur_fdata = new CFaceData();
                        for (int j = 0; j < pointAry.Count; j++)
                        {
                            if (pointAry[j].type_face == filter_i)
                            {
                                cur_fdata = pointAry[j];

                            }
                        }





                        fAry1.Add(fAry[i]);
                        //fAry[i].Highlight();
                    }

                }



                //UF_MODL_ask_face_data
                //UF_MODL_ask_face_face_intersect

                //UF_FACET_ask_face_id_of_solid_face
                //UF_FACET_cycle_facets_in_face
            }//end for
        }//end getCylinderFace 




        //兩向量求內積 1 => 平行  0 => 垂直
        public double getVectdot(double[] vec1, double[] vec2)
        {
            double dit_res = vec1[0] * vec2[0] + vec1[1] * vec2[1] + vec1[2] * vec2[2];
            return dit_res;
        }

        // 判斷是否平行(同軸)
        public bool getVectDotisParallel(double[] vec1, double[] vec2)
        {
            double dit_res1 = getVectdot(vec1, vec2);

            if (Math.Round(dit_res1, 6, MidpointRounding.AwayFromZero) >= 1.0 ||
                Math.Round(dit_res1, 6, MidpointRounding.AwayFromZero) <= -1.0)// 相臨圓弧面且同方向
            {
                return true;
            }
            else
                return false;
        }


        // 判斷是否平行(同軸)
        public bool getVectDotisVertical(double[] vec1, double[] vec2)
        {
            double dit_res1 = getVectdot(vec1, vec2);

            if (Math.Round(dit_res1, 6, MidpointRounding.AwayFromZero) <= 0.000001 &&
                Math.Round(dit_res1, 6, MidpointRounding.AwayFromZero) >= -0.000001)// 相臨圓弧面且同方向
            {
                return true;
            }
            else
                return false;
        }

        // 求單位矩陣
        public double[] getVectorByTwoPoint(double[] p1, double[] p2)
        {
            double[] new_vector = new double[3];
            double x = p2[0] - p1[0];
            double y = p2[1] - p1[1];
            double z = p2[2] - p1[2];

            if (x == 0 && y == 0 && z == 0)
            {
                new_vector[0] = 0;
                new_vector[1] = 0;
                new_vector[2] = 0;

                return new_vector;
            }

            double k = Math.Sqrt(x * x + y * y + z * z);

            new_vector[0] = x / k;
            new_vector[1] = y / k;
            new_vector[2] = z / k;

            return new_vector;
        }


        // 判斷 圓柱面是否同軸向
        public bool isCylinderTheSame_Axis_CenterPoint(CFaceData f1,CFaceData f2)
        {           
            bool ispara = getVectDotisParallel(f1.dir, f2.dir);


            //showlog("* " + f1.dir[0] + "==" + f1.dir[1] + "===" + f1.dir[2] + "===" + f2.dir[0] + "===" + f2.dir[1] + "===" + f2.dir[2] + "====" + ispara);

            if (ispara)// 同向
            {
                // 是否同心圓 (同軸)
                double[] pp = new double[3];
                //showlog("# " + f1.point[0] + "==" + f1.point[1] + "===" + f1.point[2] + "===" + f2.point[0] + "===" + f2.point[1] + "===" + f2.point[2] + "====" + ispara);

                pp = getVectorByTwoPoint(f1.point, f2.point);

                //showlog("#1 " + pp[0] + "==" + pp[1] + "===" + pp[2]);

                if (pp[0] == 0 && pp[1] == 0 && pp[2] == 0)
                {
                    //showlog("000 " +f1.dir[0] + "==" + f1.dir[1] + "===" + f1.dir[2] + "===" + f2.dir[0] + "===" + f2.dir[1] + "===" + f2.dir[2] + "====" + ispara);

                    return true; // 點重疊
                }
                bool ispara1 = getVectDotisParallel(f1.dir, pp);
                if (ispara1)
                {
                    //showlog("--- " + f1.dir[0] + "==" + f1.dir[1] + "===" + f1.dir[2] + "===" + f2.dir[0] + "===" + f2.dir[1] + "===" + f2.dir[2] + "====" + ispara);

                    return true;
                }
            }

            return false;

        }//end fun


        public void getFace4Point(Face face,double[,] fBox)
        {           
            double[] uv_min_max = new double[4];
            /*
             [0] - umin 
             [1] - umax 
             [2] - vmin 
             [3] - vmax
             */


            theUfSessionudf.Modl.AskFaceUvMinmax(face.Tag,uv_min_max);
//clog.showlog("" + uv_min_max[0] + "==" + uv_min_max[1] + "==" + uv_min_max[2] + "==" + uv_min_max[3]);           
           
            double[] param = new double[2]; 
            double[] point = new double[3];
            double[] u1 = new double[3];
            double[] v1 = new double[3];
            double[] u2 = new double[3];
            double[] v2 = new double[3];
            double[] unit_norm = new double[3];
            double[] radii = new double[2];

            // 
            param[0] = uv_min_max[0];
            param[1] = uv_min_max[2];
            theUfSessionudf.Modl.AskFaceProps(face.Tag, param, point, u1, v1, u2, v2, unit_norm, radii);
            fBox[0, 0] = point[0];
            fBox[0, 1] = point[1];
            fBox[0, 2] = point[2];
//clog.showlogByPoint("1 " , point);
            param[0] = uv_min_max[1];
            param[1] = uv_min_max[2];
            theUfSessionudf.Modl.AskFaceProps(face.Tag, param, point, u1, v1, u2, v2, unit_norm, radii);
            fBox[1, 0] = point[0];
            fBox[1, 1] = point[1];
            fBox[1, 2] = point[2];
//clog.showlogByPoint("2 ", point);


            param[0] = uv_min_max[0];
            param[1] = uv_min_max[3];
            theUfSessionudf.Modl.AskFaceProps(face.Tag, param, point, u1, v1, u2, v2, unit_norm, radii);
            fBox[2, 0] = point[0];
            fBox[2, 1] = point[1];
            fBox[2, 2] = point[2];
//clog.showlogByPoint("3 ",point);
            param[0] = uv_min_max[1];
            param[1] = uv_min_max[3];
            theUfSessionudf.Modl.AskFaceProps(face.Tag, param, point, u1, v1, u2, v2, unit_norm, radii);
            fBox[3, 0] = point[0];
            fBox[3, 1] = point[1];
            fBox[3, 2] = point[2];
//clog.showlogByPoint("4 " ,point);
        }


        // 判斷全圓與半圓
        public bool isFullCircle(Face face)
        {
            int topo_type;
            CFaceData i_face = getFacedata(face.Tag);
            theUfSessionudf.Modl.AskFaceTopology(face.Tag, out topo_type);
            //showlog("tag f :" + face.Tag + "  type : " + i_face.type_face + "  topo Type : " + topo_type);
            if (topo_type == UFConstants.UF_MODL_CYLINDRICAL_TOPOLOGY)// 全圓
                return true;
            else
                return false;
        }

        // 判斷全圓與半圓
        public bool isFullCircle(Tag face_t)
        {
            int topo_type;
            CFaceData i_face = getFacedata(face_t);
            theUfSessionudf.Modl.AskFaceTopology(face_t, out topo_type);
            //showlog("tag f :" + face_t + "  type : " + i_face.type_face + "  topo Type : " + topo_type);
            if (topo_type == UFConstants.UF_MODL_CYLINDRICAL_TOPOLOGY)// 全圓
                return true;
            else
                return false;
        }


        public double getDirHight(double[] point ,double[] ver)
        {
            // 只要方向 , 不要正負 (因為point 已分正負)
            if (ver[0] < 0)
                ver[0] = ver[0] * (-1);

            if (ver[1] < 0)
                ver[1] = ver[1] * (-1);

            if (ver[2] < 0)
                ver[2] = ver[2] * (-1);
            
            
            double[] base_h = new double[3];
            base_h[0] = point[0] * ver[0];
            base_h[1] = point[1] * ver[1];
            base_h[2] = point[2] * ver[2];

            // 取該方向的大小值
            double h = base_h[0] + base_h[1] + base_h[2];
            return h;
        }

        // 面的組成
        public void getFaceStructure(Tag face , ref List<int> flist,ref List<int> elist,
                                                ref List<String>adjlist)
        {
            /*
            20 = extruded 
            22 = bounded plane 
            23 = fillet (blend) 
            43 = b-surface 
            65 = offset surface 
            66 = foreign surface
             */
            List<String> Main_adjlist = new List<String>();

            List<int> FaceTypelist = new List<int>();
            List<int> EdgeTypelist = new List<int>();

            FaceTypelist.Add(UFConstants.UF_cylinder_type);
            FaceTypelist.Add(UFConstants.UF_cone_type);
            FaceTypelist.Add(UFConstants.UF_sphere_type);
            FaceTypelist.Add(UFConstants.UF_MODL_TOROIDAL_FACE);
            FaceTypelist.Add(20);
            FaceTypelist.Add(22);
            FaceTypelist.Add(23);
            FaceTypelist.Add(43);
            FaceTypelist.Add(65);
            FaceTypelist.Add(66);

            EdgeTypelist.Add(UFConstants.UF_MODL_LINEAR_EDGE);
            EdgeTypelist.Add(UFConstants.UF_MODL_CIRCULAR_EDGE);
            EdgeTypelist.Add(UFConstants.UF_MODL_ELLIPTICAL_EDGE);
            EdgeTypelist.Add(UFConstants.UF_MODL_INTERSECTION_EDGE);
            EdgeTypelist.Add(UFConstants.UF_MODL_SPLINE_EDGE);
            EdgeTypelist.Add(UFConstants.UF_MODL_SP_CURVE_EDGE);
            EdgeTypelist.Add(UFConstants.UF_MODL_FOREIGN_EDGE);
            EdgeTypelist.Add(UFConstants.UF_MODL_CONST_PARAMETER_EDGE);
            EdgeTypelist.Add(UFConstants.UF_MODL_TRIMMED_CURVE_EDGE);

            List<int> Facelist = new List<int>();
            List<int> Edgelist = new List<int>();

            foreach (int i in FaceTypelist)
                Facelist.Add(0);

            foreach (int i in EdgeTypelist)
                Edgelist.Add(0);
            
            CFaceData cfdata = getFacedata(face);

           
            // 面組成
            List<Tag> adjs = new List<Tag>();
            List<CFaceData> adjs_fdata = new List<CFaceData>();
            adjs_fdata = adjFaces(face, out adjs);

           
            for (int i=0;i<adjs.Count;i++)
            {
                
                int ii = FaceTypelist.IndexOf(adjs_fdata[i].type_face);
                if (ii > 0)// 找到
                {
                    Facelist[ii] = Facelist[ii] + 1;
                }
            }

            // 邊組成
            Face ff = (Face)CTrans.TagtoNXObject(face);
            Edge[] edge = ff.GetEdges();

            for (int i = 0; i < adjs.Count; i++)
            {
                int edge_type=0;
                theUfSessionudf.Modl.AskEdgeType(edge[i].Tag, out edge_type);

                int ii = EdgeTypelist.IndexOf(edge_type);
                if (ii > 0)// 找到
                {
                    Edgelist[ii] = Edgelist[ii] + 1;
                }
            }

            /*String adjs_m_str = cfdata.type_face +",";*/
            String adjs_adj_str = "";
            String line = "";
            for (int i = 0; i < edge.Length;i++ )
            {
                int adjs_connect = CaxUF_Lib.GetEdgeConnectivity(edge[i]);
                int edge_type = 0;
                theUfSessionudf.Modl.AskEdgeType(edge[i].Tag, out edge_type);
                adjs_adj_str = edge_type.ToString() + "#" + adjs_connect.ToString();
                Main_adjlist.Add(adjs_adj_str);
            }


                // 主/鄰關係 + type



                flist.AddRange(Facelist);
            elist.AddRange(Edgelist);
            adjlist.AddRange(Main_adjlist);

        }


        public bool IsFaceStructureEquals(Tag face1,Tag face2)
        {
            List<int> f1_list = new List<int>();
            List<int> f1_e_list = new List<int>();
            List<int> f2_list = new List<int>();
            List<int> f2_e_list = new List<int>();

            List<String> f1_adj_list = new List<String>();
            List<String> f2_adj_list = new List<String>();

            getFaceStructure(face1, ref f1_list, ref f1_e_list, ref f1_adj_list);
            getFaceStructure(face2, ref f2_list, ref f2_e_list,ref f2_adj_list);

            bool is_f = CLinQ.ListIsEquals(f1_list, f2_list);
            bool is_d = CLinQ.ListIsEquals(f1_e_list, f2_e_list);
            bool is_adj = CLinQ.ListIsEquals(f1_adj_list, f2_adj_list);



            if (is_f == true && is_d == true && is_adj == true)
            {
                showlog("f1 " + CLinQ.ListToString(f1_list));
                showlog("f2 " + CLinQ.ListToString(f2_list));

                showlog("e1 " + CLinQ.ListToString(f1_e_list));
                showlog("e2 " + CLinQ.ListToString(f2_e_list));

                showlog("adj1 " + CLinQ.ListToString(f1_adj_list));
                showlog("adj2 " + CLinQ.ListToString(f2_adj_list));
                return true;
            }
            else
                return false;

        }

        /// <summary>
        /// 取得曲面法線方向的面
        /// </summary>
        /// <param name="Faces"></param>
        /// <param name="obj_face"></param>
        /// <param name="newFaces"></param>
        public void getFacesOnFaceNormal(List<Tag> Faces,Tag obj_face,ref List<Tag> newFaces,String equal_type)
        {
            CLog log = new CLog();
            CFace cf = new CFace();
            // 1. 基準面
            CFace.CFaceData cfdata = cf.getFacedata(obj_face);
            //showlog("Tag " + obj_face);
            double[] minpoint = new double[3];
            minpoint[0] = cfdata.box[0];
            minpoint[1] = cfdata.box[1];
            minpoint[2] = cfdata.box[2];

            //log.showlogByPoint("min ", minpoint);
            //log.showlog("max " + cfdata.box[3] + "==" + cfdata.box[4] + "==" + cfdata.box[5]);

            double[] mindir = new double[3];
            mindir[0] = cfdata.dir[0];
            mindir[1] = cfdata.dir[1];
            mindir[2] = cfdata.dir[2];

            double[] Face_point_min = new double[3];
            double[] Face_point_max = new double[3];
            // get all faces
            try
            {

                for (int i = 0; i < Faces.Count; i++)
                {
                    //showlog("start");
                    Tag cur_face = Faces[i];

                    CFace.CFaceData cfd_t = cf.getFacedata(cur_face);
                    Face_point_min[0] = cfd_t.box[0];
                    Face_point_min[1] = cfd_t.box[1];
                    Face_point_min[2] = cfd_t.box[2];

                    Face_point_max[0] = cfd_t.box[3];
                    Face_point_max[1] = cfd_t.box[4];
                    Face_point_max[2] = cfd_t.box[5];

                    double angle_v = CMath.getTwoPoint_Normal_Angle(Face_point_min, minpoint, mindir);
                    double angle_v2 = CMath.getTwoPoint_Normal_Angle(Face_point_max, minpoint, mindir);

                    log.showlogByPoint("base point :", minpoint);
                    log.showlogByPoint("base dir :", mindir);
                    log.showlogByPoint("main :", Face_point_min);
                    log.showlogByPoint("max :", Face_point_max);
                    showlog("Tag " + cur_face + " main angle :" + angle_v + " max angle :" + angle_v2);
                    // 跨界面不要
                    if (angle_v < 90 && angle_v2 > 90  ||
                        angle_v > 90 && angle_v2 < 90
                        )//比基面高
                    {
                        continue;
                    }

                    if (equal_type == "<=")
                    {
                        if (angle_v <= 90)//比基面高
                        {
                            //showlog("--angle   " + angle_v);
                            //log.showlogByPoint("min ", Face_point);
                            //log.showlog("max " + cfd_t.box[3] + "==" + cfd_t.box[4] + "==" + cfd_t.box[5]);
                            newFaces.Add(cur_face);
                        }//end if
                    }
                    else if (equal_type =="<")
                    {
                        if (angle_v < 90)//比基面高
                            newFaces.Add(cur_face);
                    }
                    else if (equal_type == ">")
                    {
                        if (angle_v > 90)//比基面高
                            newFaces.Add(cur_face);
                    }
                    
                }//end for

           
            }
            catch (System.Exception ex)
            {
                showlog(ex.Message);
                return;
            }

        }


        /// <summary>
        /// 找到另一個半圓面(未完成)
        /// 1. 邊方向同軸向 
        /// 2. 一個面的兩邊以上 找到相同鄰面
        /// </summary>
        /// <param name="face_t"></param>
        /// <returns></returns>
        public Tag FindHalfCircle(Tag face_t)
        {
            CFaceData fd = getFacedata(face_t);

            Face f = (Face)CTrans.TagtoNXObject(face_t);
            Edge[] edges = f.GetEdges();

            // UF_MODL_ask_edge_type
            int edge_type=0;
           
            for (int i=0;i<edges.Length;i++)
            {
                theUfSessionudf.Modl.AskEdgeType(edges[i].Tag, out edge_type);
                if (edge_type == UFConstants.UF_MODL_LINEAR_EDGE )
                {

                }
            }
          

            return NXOpen.Tag.Null;
        }


        ///<summary>取得面的參數中點(ABS)</summary>
        /// <param name="point">得到的點</param>
        /// <param name="force">true: 中點一定要在面上</param>
        public void FindFaceMidPoint(Face face, out double[] point, bool force)
        {
            IntPtr evaluator;
            double[] uvLimits = new double[4];
            double[] uv = new double[2];
            ModlSrfValue surfEval;
            point = new double[3];
            int ptStatus;

            theUfSessionudf.Evalsf.Initialize(face.Tag, out evaluator);
            theUfSessionudf.Evalsf.AskFaceUvMinmax(evaluator, uvLimits);
            uv[0] = (uvLimits[0] + uvLimits[1]) * 0.5;
            uv[1] = (uvLimits[2] + uvLimits[3]) * 0.5;
            theUfSessionudf.Evalsf.Evaluate(evaluator, UFConstants.UF_MODL_EVAL, uv, out surfEval);
            surfEval.srf_pos.CopyTo(point, 0);



            if (force)
            {
                try
                {
                    theUfSessionudf.Modl.AskPointContainment(surfEval.srf_pos, face.Tag, out ptStatus);
                    if (ptStatus != 1) //outside/on the face (例如一個L形的平面, 其Middle point就不在面上)
                    {


                        double[] newPt;
                        if (FindPointOnFace(face, out newPt))
                            newPt.CopyTo(point, 0);
                    }
                }
                catch (Exception ex) //2009.09.03 test_insert.prt發現有面無法執行AskPointContainment
                {
                    //face.SetName(face.Tag.ToString());
                    showlog(face.ToString() + ": " + ex.Message);
                }
            }

            //theUfSessionudf.Evalsf(out evaluator);
        }


        /// <summary>
        /// 求出edge上與pt最近的點
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="pt">輸入點</param>
        /// <param name="nearPt">找到的最近點</param>
        /// <param name="tangent">edge在最近點的切線向量</param>
        public  void AskEdgeNearestPoint(Edge edge, double[] pt, out double[] nearPt, out double[] tangent)
        {
            IntPtr evaluator;
            double param;
            nearPt = new double[3];
            tangent = new double[3];

            theUfSessionudf.Eval.Initialize(edge.Tag, out evaluator);
            theUfSessionudf.Eval.EvaluateClosestPoint(evaluator, pt, out param, nearPt);
            theUfSessionudf.Eval.Evaluate(evaluator, 1, param, nearPt, tangent);
            //theUfSessionudf.Eval.Free(evaluator);
        }


        public void AskEdgeMidPoint(Edge edge, out double[] midPt, out double[] tangent)
        {
            IntPtr evaluator;
            double[] limits = new double[2];
            midPt = new double[3];
            tangent = new double[3];

            theUfSessionudf.Eval.Initialize(edge.Tag, out evaluator);
            theUfSessionudf.Eval.AskLimits(evaluator, limits);
            theUfSessionudf.Eval.Evaluate(evaluator, 1, (limits[0] + limits[1]) * 0.5, midPt, tangent);
            //theUfSessionudf.Eval.Free(evaluator);

        }


        /// <summary>
        /// 找出任意兩個edges的中點, 且須落在面上
        /// </summary>
        /// <param name="face"></param>
        /// <param name="facePt"></param>
        /// <returns></returns>
        public  bool FindPointOnFace(Face face, out double[] facePt)
        {
            Edge[] edges = face.GetEdges();
            double[] pt1, pt2;
            double[] tang;
            facePt = new double[3];
            int ptStatus;
            IntPtr evaluator;
            UFEvalsf.Pos3 srfPos3;

            double[] tangent= new double[3];
            theUfSessionudf.Evalsf.Initialize(face.Tag, out evaluator);
            try
            {
                for (int i = 0; i < edges.Length - 1; i++)
                {
                    AskEdgeMidPoint(edges[i], out pt1, out tangent);

                    for (int j = i + 1; j < edges.Length; j++)
                    {
                        AskEdgeNearestPoint(edges[j], pt1, out pt2, out tang);

                        theUfSessionudf.Vec3.Midpt(pt1, pt2, facePt);
                        //確保點在面上
                        theUfSessionudf.Evalsf.AskMinimumFaceDist(evaluator, facePt, out srfPos3);

                        theUfSessionudf.Modl.AskPointContainment(srfPos3.pnt3, face.Tag, out ptStatus);
                        if (ptStatus == 1) // inside the face
                        {
                            srfPos3.pnt3.CopyTo(facePt, 0);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                showlog(face.ToString() + ": " + ex.Message);
            }
            //theUfSessionudf.Evalsf.Free(out evaluator);

            return false;
        }


        /// <summary>
        /// 傳回表面某點上的單位法向量(ABS)
        /// </summary>
        /// <param name="face"></param>
        /// <param name="pt"></param>
        /// <param name="normal"></param>
        public void GetFaceNormalAtPoint(Face face, double[] pt, out double[] normal)
        {
            double[] parm = new double[2];
            double[] junk = new double[3];
            double[] radii = new double[2];
            normal = new double[3];

            theUfSessionudf.Modl.AskFaceParm(face.Tag, pt, parm, normal);
            theUfSessionudf.Modl.AskFaceProps(face.Tag, parm, junk, junk, junk, junk, junk, normal, radii);
        }

        public double[] GetNormal(Tag face_tag)
        {
            Face face = (Face)CTrans.TagtoNXObject(face_tag);


            double[] pt1, nrm1;
            double radii;
            nrm1 = new double[3];

            if (face.SolidFaceType == Face.FaceType.Planar)
            {
                double[]  fd_point = new double[3];
                double[]  fd_dir = new double[3];
                double[]  fd_box = new double[6];
                int fd_type_face=0;
                double fd_radius = 0;
                double fd_rad_data = 0;
                int   fd_norm_dir = 0;

                theUfSessionudf.Modl.AskFaceData(face.Tag, out fd_type_face,
                      fd_point, fd_dir, fd_box, out fd_radius, out fd_rad_data, out fd_norm_dir);

                nrm1[0] = fd_dir[0];
                nrm1[1] = fd_dir[1];
                nrm1[2] = fd_dir[2];
            }
            else
            {
                FindFaceMidPoint(face, out pt1, true);
                GetFaceNormalAtPoint(face, pt1, out nrm1);
            }

            return nrm1;
        }

        /// <summary>
        /// 判斷點是否通過平面
        /// </summary>
        /// <returns></returns>
        public bool IsThroughFace(Tag face_t,double[] pp)
        {
            CFace cf = new CFace();
            // get A 4 point 
            double[,] FaePA = new double[4, 3];
            Face face_obj = (Face)CTrans.TagtoNXObject(face_t);
            cf.getFace4Point(face_obj, FaePA);

          
            int[] isOnPlaneNum = { 0, 0, 0, 0 };

            // 判斷是否在 A 面上
            double[] p = new double[3];
            double[] p1 = new double[3];
            double[] p2 = new double[3];
            double[] p3 = new double[3];
            p1[0] = FaePA[0, 0];
            p1[1] = FaePA[0, 1];
            p1[2] = FaePA[0, 2];

            p2[0] = FaePA[1, 0];
            p2[1] = FaePA[1, 1];
            p2[2] = FaePA[1, 2];

            p3[0] = FaePA[2, 0];
            p3[1] = FaePA[2, 1];
            p3[2] = FaePA[2, 2];

            double[,] BoxP = new double[2, 3];
            BoxP[0,0] = pp[0];
            BoxP[0,1] = pp[1];
            BoxP[0,2] = pp[2];
            BoxP[1,0] = pp[3];
            BoxP[1,1] = pp[4];
            BoxP[1, 2] = pp[5];

            //log.showlog("========C4");    
            bool isPlane = false;
            for (int i = 0; i < 2; i++)
            {
                p[0] = BoxP[i, 0];
                p[1] = BoxP[i, 1];
                p[2] = BoxP[i, 2];

                isPlane = CMath.isOnPlane(p1, p2, p3, p);
                if (isPlane)
                {
                    return true;
                }

            }

            return false;
        }


    }//end cface class






    public class CAsm
    {
        // class members
        private Session theSession_;
        private UI theUI_;
        private UFSession theUfSession_;
        private CLog clog;

        public CAsm()
        {
            try
            {
                theSession_ = Session.GetSession();
                theUI_ = UI.GetUI();
                theUfSession_ = UFSession.GetUFSession();
                clog = new CLog();

            }
            catch (NXOpen.NXException ex)
            {
                clog.showlog(ex.Message);

            }
        }

        public void showlog(String log)
        {
            clog.showlog(log);
        }

        public void showmsg(String msg)
        {
            clog.showmsg(msg);
        }


        public Part ComponentToPart(NXOpen.Assemblies.Component comp)
        {
            if (!comp.IsOccurrence)
                return null;

            Tag prt_tag = theUfSession_.Assem.AskPrototypeOfOcc(comp.Tag);
            Part prt = (Part)CTrans.TagtoNXObject(prt_tag);
            return prt;
        }

        public String getStringAttr(Part obj_comp,String Attr_Name)
        {
            try
            {
                String attr_value = "";
                attr_value = obj_comp.GetStringAttribute(Attr_Name);
                return attr_value;
            }
            catch (System.Exception ex)
            {
                return "-1";
            }

            return "";
        }
        
        public void getAllChildren(Part topPrt,
            ref  List<Component> allcomp, ref List<int> allLevel)
        {
            topPrt.LoadFully();
            String msg = ""; 
            ComponentAssembly casm = topPrt.ComponentAssembly;
            if (casm.RootComponent == null)//零件
            {
                msg = String.Format("{0} : 不是組件.", topPrt.Name );
                showlog(msg);
                return;
            }

            //List<Component> allcomp = new List<Component>();
            //List<int > allLevel = new List<int>();
            getComponentChildren(casm.RootComponent, 1, ref allcomp, ref allLevel);

        }

        public void getComponentChildren(Component root_comp, int level,
                                        ref List<Component> allcomp_t, 
                                        ref  List<int> allLevel_t
                                           )
        {
            Component[] comp = root_comp.GetChildren();
            
            for(int i=0;i<comp.Length;i++)
            {
                Component current_comp = comp[i];
                allcomp_t.Add(current_comp);
                allLevel_t.Add(level);
                int size = current_comp.GetChildren().Length;

                getComponentChildren(current_comp, level + 1, ref allcomp_t, ref allLevel_t);
                
            }

        }


        public void MoveCompPoitn2Point(double[] point1,double[] point2)
        {

        }


/*
        public  void ResetCompOPsToWCS(
                                        double[] selFace_x,
                                        double[] selFace_y,
                                        NXOpen.Assemblies.Component Move_component)
        {
            // init
            double[] selFX = { selFace_x[0], selFace_x[1], selFace_x[2] };
            double[] selFY = { selFace_y[0], selFace_y[1], selFace_y[2] };

            //設定WCS
            double[] matrix_values = new double[9];
            double[] new_matrix = new double[9];
            double[] csys_origin = new double[3];
            GeomYif.GetMatrixByWCS(matrix_values, csys_origin);




            theUfSession.Mtx3.Initialize(selFX, selFY, new_matrix);
            //GeomYif.SetMatrixForWCS(new_matrix, csys_origin);

            Tag inst_obj = theUfSession.Assem.AskInstOfPartOcc(Move_component.Tag);

            // 1.0 取得目前零件資料
            String pn = "", rn = "", i_n = "";
            double[] comp_origin = { 0, 0, 0 };
            double[] csys_matrix = new double[9];
            double[,] transform = new double[4, 4];
            theUfSession.Assem.AskComponentData(Move_component.Tag, out pn, out rn, out i_n, comp_origin, csys_matrix, transform);


            // 1.1 零件目前座標方位
            double[] comp_xvect = { csys_matrix[0], csys_matrix[1], csys_matrix[2] };
            double[] comp_yvect = { csys_matrix[3], csys_matrix[4], csys_matrix[5] };

            double[] abs_xvect = { 1, 0, 0 };
            double[] abs_yvect = { 0, 1, 0 };
            double[] abs_zvect = { 0, 0, 1 };
            double[] abs_org = { 0, 0, 0 };

            // 1.2 工作座標系相對於世界座標系
            double[] wcs_mm = new double[16];
            double[] wcs_xvect = { matrix_values[0], matrix_values[1], matrix_values[2] };
            double[] wcs_yvect = { matrix_values[3], matrix_values[4], matrix_values[5] };

            theUfSession.Mtx4.CsysToCsys(


                                        csys_origin,
                                        wcs_xvect,
                                        wcs_yvect,
                                        abs_org,
                                        abs_xvect,
                                        abs_yvect,

                                        wcs_mm
                                        );

            // 1.2 零件轉換矩陣 (目前零件方位相對於世界座標系)
            double[] comp_mm = new double[16];
            theUfSession.Mtx4.CsysToCsys(
                                        comp_origin,
                                        comp_xvect,
                                        comp_yvect,
                                        abs_org,
                                        abs_xvect,
                                        abs_yvect,
                                        comp_mm
                                        );



            // 2. 選擇面後的座標係相對於世界座標系
            double[] selFace_mm = new double[16];
            theUfSession.Mtx4.CsysToCsys(


                                        abs_org,
                                        abs_xvect,
                                        abs_yvect,
                                        abs_org,
                                        selFX,
                                        selFY,
                                        selFace_mm
                                        );

            double[] new_point = new double[3];
            new_point = getPoint(

                    comp_origin,
                    selFX,
                    selFY,
                    csys_origin,
                    wcs_xvect,
                    wcs_yvect,
                    comp_origin
                    );

            // 3. 矩陣相乘
            double[] new_csys_matrix = new double[6];
            double[] final_mm = new double[16];
            theUfSession.Mtx4.Multiply(selFace_mm, comp_mm, final_mm);
            msg = String.Format("final_mm 1\n{0},{1},{2},{3}\n{4},{5},{6},{7}\n{8},{9},{10},{11}\n{12},{13},{14},{15}",
                                final_mm[0], final_mm[1], final_mm[2], final_mm[3],
                                final_mm[4], final_mm[5], final_mm[6], final_mm[7],
                                final_mm[8], final_mm[9], final_mm[10], final_mm[11],
                                final_mm[12], final_mm[13], final_mm[14], final_mm[15]
                                );
            //MessageBox.Show(msg);
            theUfSession.Mtx4.Multiply(wcs_mm, final_mm, final_mm);


            // 4.取得最後矩陣的X向量
            new_csys_matrix[0] = final_mm[0];
            new_csys_matrix[1] = final_mm[1];
            new_csys_matrix[2] = final_mm[2];
            new_csys_matrix[3] = final_mm[4];
            new_csys_matrix[4] = final_mm[5];
            new_csys_matrix[5] = final_mm[6];

            msg = String.Format("final_mm 2\n{0},{1},{2},{3}\n{4},{5},{6},{7}\n{8},{9},{10},{11}\n{12},{13},{14},{15}",
                                final_mm[0], final_mm[1], final_mm[2], final_mm[3],
                                final_mm[4], final_mm[5], final_mm[6], final_mm[7],
                                final_mm[8], final_mm[9], final_mm[10], final_mm[11],
                                final_mm[12], final_mm[13], final_mm[14], final_mm[15]
                                );
            //MessageBox.Show(msg);


            abs_org[0] = new_point[0];
            abs_org[1] = new_point[1];
            abs_org[2] = new_point[2];

            // 5. 重新定位 (永遠從世界座標開始算)
            theUfSession.Assem.RepositionInstance(inst_obj, abs_org, new_csys_matrix);

        }


        public  void getPointforMtx(double[] mm, double[] point, double[] out_point)
        {
            out_point[0] = point[0] * mm[0] + point[1] * mm[4] + point[2] * mm[8];

            out_point[1] = point[0] * mm[1] + point[1] * mm[5] + point[2] * mm[9];

            out_point[2] = point[0] * mm[2] + point[1] * mm[6] + point[2] * mm[10];

        }


        public  double[] getPoint(

                                double[] org, double[] org_x, double[] org_y,
                                double[] to, double[] to_x, double[] to_y,
                                double[] comp_origin
                                )
        {

            double[] selFace_mm = new double[16];
            theUfSession.Mtx4.CsysToCsys(
                                        org,
                                        org_x,
                                        org_y,
                                        to,
                                        to_x,
                                        to_y,
                                        selFace_mm
                                        );

            String msg;

            double[] new_point = new double[3];

            double[] new_point_1 = new double[3];
            new_point_1[0] = 0;
            new_point_1[1] = 0;
            new_point_1[2] = 0;

            theUfSession.Mtx4.Transpose(selFace_mm, selFace_mm);
            msg = String.Format("CsysToCsys \n{0},{1},{2},{3}\n{4},{5},{6},{7}\n{8},{9},{10},{11}\n{12},{13},{14},{15}",
                              selFace_mm[0], selFace_mm[1], selFace_mm[2], selFace_mm[3],
                              selFace_mm[4], selFace_mm[5], selFace_mm[6], selFace_mm[7],
                              selFace_mm[8], selFace_mm[9], selFace_mm[10], selFace_mm[11],
                              selFace_mm[12], selFace_mm[13], selFace_mm[14], selFace_mm[15]
                              );
            //MessageBox.Show(msg);
            getPointforMtx(selFace_mm, comp_origin, new_point);
            return new_point;

        }

*/
    }//end class CAsm




    public class CJnal
    {
        // class members
        private Session theSession_;
        private UI theUI_;
        private UFSession theUfSession_;
        private CLog clog;

        public CJnal()
        {
            try
            {
                theSession_ = Session.GetSession();
                theUI_ = UI.GetUI();
                theUfSession_ = UFSession.GetUFSession();
                clog = new CLog();
            }
            catch (NXOpen.NXException ex)
            {
                clog.showlog(ex.Message);

            }
        }

        public void showlog(String log)
        {
            clog.showlog(log);
        }

        public void showmsg(String msg)
        {
            clog.showmsg(msg);
        }

        // Wave link 並非全部支援 journal
        public Feature CreateWaveLink_DtaumPlane(Part workPart, NXOpen.Features.Feature otherFeat)
        {

            // 0. get dtaum plane feature
            NXOpen.Features.DatumPlaneFeature DF = (NXOpen.Features.DatumPlaneFeature)otherFeat;

            // ----------------------------------------------
            //   Menu: Insert->Associative Copy->WAVE Geometry Linker...
            // ----------------------------------------------

            NXOpen.Features.Feature nullFeatures_Feature = null;


            NXOpen.Features.WaveLinkBuilder waveLinkBuilder1;
            waveLinkBuilder1 = workPart.BaseFeatures.CreateWaveLinkBuilder(nullFeatures_Feature);

            NXOpen.Features.WaveDatumBuilder waveDatumBuilder1;
            waveDatumBuilder1 = waveLinkBuilder1.WaveDatumBuilder;

            NXOpen.Features.CompositeCurveBuilder compositeCurveBuilder1;
            compositeCurveBuilder1 = waveLinkBuilder1.CompositeCurveBuilder;

            NXOpen.Features.WaveSketchBuilder waveSketchBuilder1;
            waveSketchBuilder1 = waveLinkBuilder1.WaveSketchBuilder;

            NXOpen.Features.WaveRoutingBuilder waveRoutingBuilder1;
            waveRoutingBuilder1 = waveLinkBuilder1.WaveRoutingBuilder;

            NXOpen.Features.WavePointBuilder wavePointBuilder1;
            wavePointBuilder1 = waveLinkBuilder1.WavePointBuilder;

            NXOpen.Features.ExtractFaceBuilder extractFaceBuilder1;
            extractFaceBuilder1 = waveLinkBuilder1.ExtractFaceBuilder;

            NXOpen.Features.MirrorBodyBuilder mirrorBodyBuilder1;
            mirrorBodyBuilder1 = waveLinkBuilder1.MirrorBodyBuilder;


            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

            waveLinkBuilder1.Type = NXOpen.Features.WaveLinkBuilder.Types.DatumLink;

            extractFaceBuilder1.FaceOption = NXOpen.Features.ExtractFaceBuilder.FaceOptionType.FaceChain;

            extractFaceBuilder1.AngleTolerance = 45.0;

            waveDatumBuilder1.DisplayScale = 2.0;

            extractFaceBuilder1.ParentPart = NXOpen.Features.ExtractFaceBuilder.ParentPartType.OtherPart;

            mirrorBodyBuilder1.ParentPartType = NXOpen.Features.MirrorBodyBuilder.ParentPart.OtherPart;


            waveDatumBuilder1.Associative = true;

            waveDatumBuilder1.MakePositionIndependent = false;

            waveDatumBuilder1.HideOriginal = false;

            waveDatumBuilder1.InheritDisplayProperties = false;

           // 1. set datum plane
            SelectObjectList selectObjectList1;
            selectObjectList1 = waveDatumBuilder1.Datums;
            DatumPlane datumPlane1 = (DatumPlane)DF.DatumPlane;
            bool added1;
            added1 = selectObjectList1.Add(datumPlane1);

            NXObject nXObject1;
            nXObject1 = waveLinkBuilder1.Commit();
            waveLinkBuilder1.Destroy();


            return (Feature)nXObject1;
        }



        public void ReplaceComponent(Component OrgComp,String replaceCompPath)
        {
            String OrgComp_name = OrgComp.DisplayName;
            String ReplaceComp_name = Path.GetFileNameWithoutExtension(replaceCompPath);
            String ReplaceComp_fullPath = replaceCompPath;
            
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Assemblies.ReplaceComponentBuilder replaceComponentBuilder1;
            replaceComponentBuilder1 = workPart.AssemblyManager.CreateReplaceComponentBuilder();

            replaceComponentBuilder1.ComponentNameType = NXOpen.Assemblies.ReplaceComponentBuilder.ComponentNameOption.AsSpecified;

            replaceComponentBuilder1.ComponentName = ReplaceComp_name;

            theSession.SetUndoMarkName(markId1, "Replace Component Dialog");

            replaceComponentBuilder1.ComponentName = "";

            NXOpen.Assemblies.Component component1 = OrgComp;//(NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT CIMTEST006_U002_ET101_1 1");
            bool added1;
            added1 = replaceComponentBuilder1.ComponentsToReplace.Add(component1);

            replaceComponentBuilder1.ComponentName = OrgComp_name;

            replaceComponentBuilder1.ComponentName = ReplaceComp_name;

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Replace Component");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Replace Component");

            replaceComponentBuilder1.ReplacementPart = ReplaceComp_fullPath;

            replaceComponentBuilder1.SetComponentReferenceSetType(NXOpen.Assemblies.ReplaceComponentBuilder.ComponentReferenceSet.EntirePart, null);

            PartLoadStatus partLoadStatus1;
            partLoadStatus1 = replaceComponentBuilder1.RegisterReplacePartLoadStatus();

            NXObject nXObject1;
            nXObject1 = replaceComponentBuilder1.Commit();

            if (nXObject1 == null)
            {
                //showlog("ReplaceComponent null");
            }

            partLoadStatus1.Dispose();
            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Replace Component");

            replaceComponentBuilder1.Destroy();

        }//end ReplaceComponent


        public void RestructureComponents(Component TopComp,Component child_comp)
        {
            CAsm c_asm = new CAsm();
            Part TopPart = c_asm.ComponentToPart(TopComp);
            
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Drag and Drop Components");

            Part part1 = TopPart;// (Part)theSession.Parts.FindObject("CIMTEST006_U002_ET101");
            NXOpen.Assemblies.Component[] origComponents1 = new NXOpen.Assemblies.Component[1];
            NXOpen.Assemblies.Component component1 = child_comp;// (NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT CIMTEST006_U002_ET101_1 1");
            origComponents1[0] = component1;
            NXOpen.Assemblies.Component component2 = TopComp;// (NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT CIMTEST006_U002_ET101 1");
            NXOpen.Assemblies.Component[] newComponents1;
            ErrorList errorList1;
            part1.ComponentAssembly.RestructureComponents(origComponents1, component2, true, out newComponents1, out errorList1);

            errorList1.Dispose();

        }//end RestructureComponents


        public Component MoveComponentByCopy(Component MoveComp, double[] point1, double[] point2,bool isCopy=true)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;

            CAsm c_asm = new CAsm();
            Part MovePart = c_asm.ComponentToPart(MoveComp);

            Point3d p1 = new Point3d(point1[0], point1[1], point1[2]);
            Point pp1 = workPart.Points.CreatePoint(p1);

            Point3d p2 = new Point3d(point2[0], point2[1], point2[2]);
            Point pp2 = workPart.Points.CreatePoint(p2);

            Point3d p2p1 = new Point3d(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);


            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Positioning.ComponentPositioner componentPositioner1;
            componentPositioner1 = workPart.ComponentAssembly.Positioner;

            componentPositioner1.ClearNetwork();

            NXOpen.Assemblies.Arrangement arrangement1 = (NXOpen.Assemblies.Arrangement)workPart.ComponentAssembly.Arrangements.FindObject("Arrangement 1");
            componentPositioner1.PrimaryArrangement = arrangement1;

            componentPositioner1.BeginMoveComponent();

            bool allowInterpartPositioning1;
            allowInterpartPositioning1 = theSession.Preferences.Assemblies.InterpartPositioning;

            NXOpen.Positioning.Network network1;
            network1 = componentPositioner1.EstablishNetwork();

            NXOpen.Positioning.ComponentNetwork componentNetwork1 = (NXOpen.Positioning.ComponentNetwork)network1;
            componentNetwork1.MoveObjectsState = true;

            NXOpen.Assemblies.Component nullAssemblies_Component = null;
            componentNetwork1.DisplayComponent = nullAssemblies_Component;

            componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;

            componentNetwork1.NonMovingGroupGrounded = true;

            componentNetwork1.Solve();

            componentNetwork1.RemoveAllConstraints();

            NXObject[] movableObjects1 = new NXObject[1];
            NXOpen.Assemblies.Component component1 = MoveComp;// (NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT CIMTEST006_U002_ET101 2");
            movableObjects1[0] = component1;
            componentNetwork1.SetMovingGroup(movableObjects1);

            componentNetwork1.Solve();

            theSession.SetUndoMarkName(markId1, "Move Component Dialog");

            componentNetwork1.MoveObjectsState = true;

            componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.InUsed;

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component Update");

            Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Scalar scalar1;
            scalar1 = workPart.Scalars.CreateScalar(0.0, NXOpen.Scalar.DimensionalityType.None, NXOpen.SmartObject.UpdateOption.AfterModeling);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Point to Point");

            componentNetwork1.Solve();

            componentPositioner1.ClearNetwork();

            int nErrs3;
            nErrs3 = theSession.UpdateManager.AddToDeleteList(componentNetwork1);

            int nErrs4;
            nErrs4 = theSession.UpdateManager.DoUpdate(markId2);

            NXOpen.Assemblies.Component[] components1 = new NXOpen.Assemblies.Component[1];
            components1[0] = component1;
            NXOpen.Assemblies.Component[] newComponents1;
            if (isCopy)
                newComponents1 = workPart.ComponentAssembly.CopyComponents(components1);
            else
                newComponents1 = new Component[1] { MoveComp };

            NXOpen.Positioning.Network network2;
            network2 = componentPositioner1.EstablishNetwork();

            NXOpen.Positioning.ComponentNetwork componentNetwork2 = (NXOpen.Positioning.ComponentNetwork)network2;
            componentNetwork2.MoveObjectsState = true;

            componentNetwork2.DisplayComponent = nullAssemblies_Component;

            componentNetwork2.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.InUsed;

            componentNetwork2.NonMovingGroupGrounded = true;

            componentNetwork2.Solve();

            componentNetwork2.RemoveAllConstraints();

            NXObject[] movableObjects2 = new NXObject[1];
            movableObjects2[0] = component1;
            componentNetwork2.SetMovingGroup(movableObjects2);

            componentNetwork2.Solve();

            componentNetwork2.RemoveAllConstraints();

            NXObject[] movableObjects3 = new NXObject[1];
            movableObjects3[0] = newComponents1[0];
            componentNetwork2.SetMovingGroup(movableObjects3);

            componentNetwork2.Solve();

            bool loaded1;
            loaded1 = componentNetwork2.IsReferencedGeometryLoaded();

            componentNetwork2.BeginDrag();

            Vector3d translation1 = new Vector3d(p2p1.X, p2p1.Y, p2p1.Z);
            componentNetwork2.DragByTranslation(translation1);

            componentNetwork2.EndDrag();

            componentNetwork2.ResetDisplay();

            componentNetwork2.ApplyToModel();

            componentNetwork2.RemoveAllConstraints();

            NXObject[] movableObjects4 = new NXObject[1];
            movableObjects4[0] = newComponents1[0];
            componentNetwork2.SetMovingGroup(movableObjects4);

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component");

            theSession.DeleteUndoMark(markId4, null);

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component");

            componentNetwork2.Solve();

            Component mComp = newComponents1[0];

            //showlog(" componentNetwork2 " + mComp.DisplayName);

            componentPositioner1.ClearNetwork();

            int nErrs5;
            nErrs5 = theSession.UpdateManager.AddToDeleteList(componentNetwork2);

            int nErrs6;
            nErrs6 = theSession.UpdateManager.DoUpdate(markId2);

            componentPositioner1.DeleteNonPersistentConstraints();

            int nErrs7;
            nErrs7 = theSession.UpdateManager.DoUpdate(markId2);

            theSession.DeleteUndoMark(markId5, null);

            theSession.SetUndoMarkName(markId1, "Move Component");

            componentPositioner1.EndMoveComponent();


            NXOpen.Assemblies.Arrangement nullAssemblies_Arrangement = null;
            componentPositioner1.PrimaryArrangement = nullAssemblies_Arrangement;

            theSession.DeleteUndoMark(markId2, null);

            workPart.Expressions.Delete(expression1);

            theSession.DeleteUndoMark(markId3, null);

            return mComp;
        }


        public void ReNameNewComponent(Component Comp ,String NewCompPath)
        {
            CAsm c_asm = new CAsm();
            Part compPart = c_asm.ComponentToPart(Comp);
            
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Assemblies.MakeUniquePartBuilder makeUniquePartBuilder1;
            makeUniquePartBuilder1 = workPart.AssemblyManager.CreateMakeUniquePartBuilder();

            NXOpen.Assemblies.Component component1 = Comp;//(NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT CIMTEST006_U002_ET101 1");
            bool added1;
            added1 = makeUniquePartBuilder1.SelectedComponents.Add(component1);

            theSession.SetUndoMarkName(markId1, "Make Unique Dialog");

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Start");

            theSession.SetUndoMarkName(markId2, "Name Unique Parts Dialog");

            // ----------------------------------------------
            //   Dialog Begin Name Unique Parts
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Name Unique Parts");

            theSession.DeleteUndoMark(markId3, null);

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Name Unique Parts");

            Part part1 = compPart;// (Part)theSession.Parts.FindObject("CIMTEST006_U002_ET101");
            part1.SetMakeUniqueName(NewCompPath);

            theSession.DeleteUndoMark(markId4, null);

            theSession.SetUndoMarkName(markId2, "Name Unique Parts");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Make Unique");

            theSession.DeleteUndoMark(markId5, null);

            NXOpen.Session.UndoMarkId markId6;
            markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Make Unique");

            NXOpen.Session.UndoMarkId markId7;
            markId7 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Make Unique Apply");

            NXObject nXObject1;
            nXObject1 = makeUniquePartBuilder1.Commit();

            theSession.DeleteUndoMark(markId6, null);

            theSession.SetUndoMarkName(markId1, "Make Unique");

            makeUniquePartBuilder1.Destroy();

            theSession.DeleteUndoMark(markId7, null);
        }


        // 輸入方向與距離
        public Component MoveComponentDisByCopy(Component MoveComp , double[] dis)
        {
            
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Positioning.ComponentPositioner componentPositioner1;
            componentPositioner1 = workPart.ComponentAssembly.Positioner;

            componentPositioner1.ClearNetwork();

            NXOpen.Assemblies.Arrangement arrangement1 = (NXOpen.Assemblies.Arrangement)workPart.ComponentAssembly.Arrangements.FindObject("Arrangement 1");
            componentPositioner1.PrimaryArrangement = arrangement1;

            componentPositioner1.BeginMoveComponent();

            bool allowInterpartPositioning1;
            allowInterpartPositioning1 = theSession.Preferences.Assemblies.InterpartPositioning;

            NXOpen.Positioning.Network network1;
            network1 = componentPositioner1.EstablishNetwork();

            NXOpen.Positioning.ComponentNetwork componentNetwork1 = (NXOpen.Positioning.ComponentNetwork)network1;
            componentNetwork1.MoveObjectsState = true;

            NXOpen.Assemblies.Component nullAssemblies_Component = null;
            componentNetwork1.DisplayComponent = nullAssemblies_Component;

            componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;

            componentNetwork1.NonMovingGroupGrounded = true;

            componentNetwork1.Solve();

            componentNetwork1.RemoveAllConstraints();

            NXObject[] movableObjects1 = new NXObject[1];
            UI theUI = UI.GetUI();

            movableObjects1[0] = MoveComp;//(NXOpen.Assemblies.Component)theUI.SelectionManager.GetSelectedObject(0);
            componentNetwork1.SetMovingGroup(movableObjects1);

            componentNetwork1.Solve();

            theSession.SetUndoMarkName(markId1, "Move Component Dialog");

            componentNetwork1.MoveObjectsState = true;

            componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.InUsed;

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component Update");

            Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            componentNetwork1.Solve();

            componentPositioner1.ClearNetwork();

            int nErrs1;
            nErrs1 = theSession.UpdateManager.AddToDeleteList(componentNetwork1);

            int nErrs2;
            nErrs2 = theSession.UpdateManager.DoUpdate(markId2);

            NXOpen.Positioning.Network network2;
            network2 = componentPositioner1.EstablishNetwork();

            NXOpen.Positioning.ComponentNetwork componentNetwork2 = (NXOpen.Positioning.ComponentNetwork)network2;
            componentNetwork2.MoveObjectsState = true;

            componentNetwork2.DisplayComponent = nullAssemblies_Component;

            componentNetwork2.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.InUsed;

            componentNetwork2.NonMovingGroupGrounded = true;

            componentNetwork2.Solve();

            componentNetwork2.RemoveAllConstraints();

            NXObject[] movableObjects2 = new NXObject[1];
            movableObjects2[0] = MoveComp;// (NXOpen.Assemblies.Component)theUI.SelectionManager.GetSelectedObject(0);
            componentNetwork2.SetMovingGroup(movableObjects2);

            componentNetwork2.Solve();

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Distance");

            bool loaded1;
            loaded1 = componentNetwork2.IsReferencedGeometryLoaded();

            componentNetwork2.BeginDrag();

            Vector3d translation1 = new Vector3d(dis[0], dis[1], dis[2]);
            componentNetwork2.DragByTranslation(translation1);

            componentNetwork2.EndDrag();

            componentNetwork2.ResetDisplay();

            componentNetwork2.ApplyToModel();

            componentNetwork2.Solve();

            componentPositioner1.ClearNetwork();

            int nErrs3;
            nErrs3 = theSession.UpdateManager.AddToDeleteList(componentNetwork2);

            int nErrs4;
            nErrs4 = theSession.UpdateManager.DoUpdate(markId2);

            NXOpen.Positioning.Network network3;
            network3 = componentPositioner1.EstablishNetwork();

            NXOpen.Positioning.ComponentNetwork componentNetwork3 = (NXOpen.Positioning.ComponentNetwork)network3;
            componentNetwork3.MoveObjectsState = true;

            componentNetwork3.DisplayComponent = nullAssemblies_Component;

            componentNetwork3.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.InUsed;

            componentNetwork3.NonMovingGroupGrounded = true;

            componentNetwork3.Solve();

            componentNetwork3.RemoveAllConstraints();

            NXObject[] movableObjects3 = new NXObject[1];
            movableObjects3[0] = MoveComp;// (NXOpen.Assemblies.Component)theUI.SelectionManager.GetSelectedObject(0);
            componentNetwork3.SetMovingGroup(movableObjects3);

            componentNetwork3.Solve();

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component");

            theSession.DeleteUndoMark(markId4, null);

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component");

            componentNetwork3.Solve();

            componentPositioner1.ClearNetwork();

            int nErrs5;
            nErrs5 = theSession.UpdateManager.AddToDeleteList(componentNetwork3);

            int nErrs6;
            nErrs6 = theSession.UpdateManager.DoUpdate(markId2);

            componentPositioner1.DeleteNonPersistentConstraints();

            int nErrs7;
            nErrs7 = theSession.UpdateManager.DoUpdate(markId2);

            theSession.DeleteUndoMark(markId5, null);

            theSession.SetUndoMarkName(markId1, "Move Component");

            componentPositioner1.EndMoveComponent();

            NXOpen.Assemblies.Arrangement nullAssemblies_Arrangement = null;
            componentPositioner1.PrimaryArrangement = nullAssemblies_Arrangement;

            theSession.DeleteUndoMark(markId2, null);

            workPart.Expressions.Delete(expression1);

            workPart.Expressions.Delete(expression2);

            theSession.DeleteUndoMark(markId3, null);


            return MoveComp;
 
        }

        public Component RoationComponentByAxis(Component MoveComp ,double[] transfrom )
        {
            double[] trans  = new double[3];
            trans[0] = transfrom[3];
            trans[1] = transfrom[7];
            trans[2] = transfrom[11];

            double[] roataion = new double[9];
            roataion[0] = transfrom[0];
            roataion[1] = transfrom[1];
            roataion[2] = transfrom[2];

            roataion[3] = transfrom[4];
            roataion[4] = transfrom[5];
            roataion[5] = transfrom[6];

            roataion[6] = transfrom[8];
            roataion[7] = transfrom[8];
            roataion[8] = transfrom[10];
            
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Positioning.ComponentPositioner componentPositioner1;
            componentPositioner1 = workPart.ComponentAssembly.Positioner;

            componentPositioner1.ClearNetwork();

            NXOpen.Assemblies.Arrangement arrangement1 = (NXOpen.Assemblies.Arrangement)workPart.ComponentAssembly.Arrangements.FindObject("Arrangement 1");
            componentPositioner1.PrimaryArrangement = arrangement1;

            componentPositioner1.BeginMoveComponent();

            bool allowInterpartPositioning1;
            allowInterpartPositioning1 = theSession.Preferences.Assemblies.InterpartPositioning;

            NXOpen.Positioning.Network network1;
            network1 = componentPositioner1.EstablishNetwork();

            NXOpen.Positioning.ComponentNetwork componentNetwork1 = (NXOpen.Positioning.ComponentNetwork)network1;
            componentNetwork1.MoveObjectsState = true;

            NXOpen.Assemblies.Component nullAssemblies_Component = null;
            componentNetwork1.DisplayComponent = nullAssemblies_Component;

            componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.Existing;

            componentNetwork1.NonMovingGroupGrounded = true;

            componentNetwork1.Solve();

            componentNetwork1.RemoveAllConstraints();

            NXObject[] movableObjects1 = new NXObject[1];
            NXOpen.Assemblies.Component component1 = MoveComp;//(NXOpen.Assemblies.Component)workPart.ComponentAssembly.RootComponent.FindObject("COMPONENT CIMTEST006_U002_ET101 3");
            movableObjects1[0] = component1;
            componentNetwork1.SetMovingGroup(movableObjects1);

            componentNetwork1.Solve();

            theSession.SetUndoMarkName(markId1, "Move Component Dialog");

            componentNetwork1.MoveObjectsState = true;

            componentNetwork1.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.InUsed;

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component Update");

            Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            componentNetwork1.Solve();

            componentPositioner1.ClearNetwork();

            int nErrs1;
            nErrs1 = theSession.UpdateManager.AddToDeleteList(componentNetwork1);

            int nErrs2;
            nErrs2 = theSession.UpdateManager.DoUpdate(markId2);

            NXOpen.Positioning.Network network2;
            network2 = componentPositioner1.EstablishNetwork();

            NXOpen.Positioning.ComponentNetwork componentNetwork2 = (NXOpen.Positioning.ComponentNetwork)network2;
            componentNetwork2.MoveObjectsState = true;

            componentNetwork2.DisplayComponent = nullAssemblies_Component;

            componentNetwork2.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.InUsed;

            componentNetwork2.NonMovingGroupGrounded = true;

            componentNetwork2.Solve();

            componentNetwork2.RemoveAllConstraints();

            NXObject[] movableObjects2 = new NXObject[1];
            movableObjects2[0] = component1;
            componentNetwork2.SetMovingGroup(movableObjects2);

            componentNetwork2.Solve();

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Angle");

            componentNetwork2.Solve();

            componentPositioner1.ClearNetwork();

            int nErrs3;
            nErrs3 = theSession.UpdateManager.AddToDeleteList(componentNetwork2);

            int nErrs4;
            nErrs4 = theSession.UpdateManager.DoUpdate(markId2);

            NXOpen.Assemblies.Component[] components1 = new NXOpen.Assemblies.Component[1];
            components1[0] = component1;
            NXOpen.Assemblies.Component[] newComponents1;
            newComponents1 = new Component[1] { component1 };//workPart.ComponentAssembly.CopyComponents(components1);

            NXOpen.Positioning.Network network3;
            network3 = componentPositioner1.EstablishNetwork();

            NXOpen.Positioning.ComponentNetwork componentNetwork3 = (NXOpen.Positioning.ComponentNetwork)network3;
            componentNetwork3.MoveObjectsState = true;

            componentNetwork3.DisplayComponent = nullAssemblies_Component;

            componentNetwork3.NetworkArrangementsMode = NXOpen.Positioning.ComponentNetwork.ArrangementsMode.InUsed;

            componentNetwork3.NonMovingGroupGrounded = true;

            componentNetwork3.Solve();

            componentNetwork3.RemoveAllConstraints();

            NXObject[] movableObjects3 = new NXObject[1];
            movableObjects3[0] = component1;
            componentNetwork3.SetMovingGroup(movableObjects3);

            componentNetwork3.Solve();

            componentNetwork3.RemoveAllConstraints();

            NXObject[] movableObjects4 = new NXObject[1];
            movableObjects4[0] = newComponents1[0];
            componentNetwork3.SetMovingGroup(movableObjects4);

            componentNetwork3.Solve();

            bool loaded1;
            loaded1 = componentNetwork3.IsReferencedGeometryLoaded();

            componentNetwork3.BeginDrag();

            Vector3d translation1 = new Vector3d(trans[0], trans[1], trans[2]);
            Matrix3x3 rotation1;
            rotation1.Xx = roataion[0];
            rotation1.Xy = roataion[1];
            rotation1.Xz = roataion[2];
            rotation1.Yx = roataion[3];
            rotation1.Yy = roataion[4];
            rotation1.Yz = roataion[5];
            rotation1.Zx = roataion[6];
            rotation1.Zy = roataion[7];
            rotation1.Zz = roataion[8];
            componentNetwork3.DragByTransform(translation1, rotation1);

            componentNetwork3.EndDrag();

            componentNetwork3.ResetDisplay();

            componentNetwork3.ApplyToModel();

            componentNetwork3.RemoveAllConstraints();

            NXObject[] movableObjects5 = new NXObject[1];
            movableObjects5[0] = newComponents1[0];
            componentNetwork3.SetMovingGroup(movableObjects5);

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component");

            theSession.DeleteUndoMark(markId4, null);

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Move Component");

            componentNetwork3.Solve();

            componentPositioner1.ClearNetwork();

            int nErrs5;
            nErrs5 = theSession.UpdateManager.AddToDeleteList(componentNetwork3);

            int nErrs6;
            nErrs6 = theSession.UpdateManager.DoUpdate(markId2);

            componentPositioner1.DeleteNonPersistentConstraints();

            int nErrs7;
            nErrs7 = theSession.UpdateManager.DoUpdate(markId2);

            theSession.DeleteUndoMark(markId5, null);

            theSession.SetUndoMarkName(markId1, "Move Component");

            componentPositioner1.EndMoveComponent();

            NXOpen.Assemblies.Arrangement nullAssemblies_Arrangement = null;
            componentPositioner1.PrimaryArrangement = nullAssemblies_Arrangement;

            theSession.DeleteUndoMark(markId2, null);

            workPart.Expressions.Delete(expression1);

            theSession.DeleteUndoMark(markId3, null);


            return MoveComp;
        }


        // 拔模
        public Feature CreateDraft(Part workPart, DatumPlane dtm_plane, Face draft_f, double draft_value, bool isRevDir)
        {
            // ----------------------------------------------
            //   Menu: Insert->Detail Feature->Draft...
            // ----------------------------------------------
            NXOpen.Features.Feature nullFeatures_Feature = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.DraftBuilder draftBuilder1;
            draftBuilder1 = workPart.Features.CreateDraftBuilder(nullFeatures_Feature);

            Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            draftBuilder1.DistanceTolerance = 0.01;

            draftBuilder1.AngleTolerance = 0.5;

            draftBuilder1.AngleTolerance = 0.5;

            draftBuilder1.DistanceTolerance = 0.01;

            draftBuilder1.DraftIsoclineOrTruedraft = NXOpen.Features.DraftBuilder.Method.Isocline;

            NXOpen.GeometricUtilities.DraftVariableAngleData draftVariableAngleData1;
            draftVariableAngleData1 = draftBuilder1.VariableAngleData;

            draftBuilder1.TypeOfDraft = NXOpen.Features.DraftBuilder.Type.Face;

            draftBuilder1.TwoDimensionFaceSetsData.Clear(NXOpen.ObjectList.DeleteOption.Delete);

            draftBuilder1.FaceSetAngleExpressionList.Clear(NXOpen.ObjectList.DeleteOption.Delete);

            draftBuilder1.EdgeSetAngleExpressionList.Clear(NXOpen.ObjectList.DeleteOption.Delete);

            // 1.設定拔模方向面
            DatumPlane[] faces1 = new DatumPlane[1];
            DatumPlane datumPlane1 = dtm_plane;
            faces1[0] = datumPlane1;
            FaceDumbRule faceDumbRule1;
            faceDumbRule1 = workPart.ScRuleFactory.CreateRuleFaceDatum(faces1);

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = faceDumbRule1;
            draftBuilder1.StationaryReference.ReplaceRules(rules1, false);

            String draft_v = draft_value.ToString();
            ScCollector nullScCollector = null;
            NXOpen.GeometricUtilities.TwoExpressionsCollectorSet twoExpressionsCollectorSet1;
            twoExpressionsCollectorSet1 = workPart.CreateTwoExpressionsCollectorSet(nullScCollector, draft_v, draft_v, "Angle", 0);

            draftBuilder1.TwoDimensionFaceSetsData.Append(twoExpressionsCollectorSet1);

            ScCollector scCollector1;
            scCollector1 = workPart.ScCollectors.CreateCollector();

            // 2. 傳入拔模面
            Face face1 = draft_f;
            Face[] boundaryFaces1 = new Face[0];
            FaceTangentRule faceTangentRule1;
            faceTangentRule1 = workPart.ScRuleFactory.CreateRuleFaceTangent(face1, boundaryFaces1, 0.5);

            SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
            rules2[0] = faceTangentRule1;
            scCollector1.ReplaceRules(rules2, false);

            twoExpressionsCollectorSet1.Collector = scCollector1;

            Direction direction1;
            direction1 = draftBuilder1.Direction;

            bool success1;
            if (isRevDir)//設定反向
                success1 = direction1.ReverseDirection();

            twoExpressionsCollectorSet1.ItemValue.RightHandSide = "1";

            NXOpen.Features.Feature feature1;
            feature1 = draftBuilder1.CommitFeature();

            draftBuilder1.Destroy();
            workPart.Expressions.Delete(expression1);

            return feature1;
        }



        public void sketchMoveToLayer(Part workPart, NXOpen.Features.SketchFeature sketch_feat, int layer)
        {

            // ----------------------------------------------
            //   Menu: Format->Move to Layer...
            // ----------------------------------------------
            DisplayableObject[] objectArray1 = new DisplayableObject[1];
            Sketch sketch1 = sketch_feat.Sketch;
            objectArray1[0] = sketch1;
            workPart.Layers.MoveDisplayableObjects(layer, objectArray1);

        }

        public void FeatureMoveToLayer(Part workPart, NXOpen.Features.Feature feat, int layer)
        {
            // ----------------------------------------------
            //   Menu: Format->Move to Layer...
            // ----------------------------------------------
            DisplayableObject[] objectArray1 = new DisplayableObject[1];
            Tag body_feat;
            theUfSession_.Modl.AskFeatBody(feat.Tag,out body_feat);
            Body temp = (Body)CTrans.TagtoNXObject(body_feat);
            objectArray1[0] = temp;
            workPart.Layers.MoveDisplayableObjects(layer, objectArray1);

        }

        public void LineMoveToLayer(Part workPart, List<Line> lines, int layer)
        {
            // ----------------------------------------------
            //   Menu: Format->Move to Layer...
            // ----------------------------------------------
            DisplayableObject[] objectArray1 = new DisplayableObject[lines.Count];

            for (int i = 0; i < lines.Count; i++)
            {
                objectArray1[i] = lines[i];
            }          
            workPart.Layers.MoveDisplayableObjects(layer, objectArray1);

        }


        public void editExtrudeDir(Part wk, Feature Extrude, bool isRev)
        {
            NXOpen.Features.Extrude extrude1 = (NXOpen.Features.Extrude)Extrude;
            wk.Features.SetEditWithRollbackFeature(extrude1);

            NXOpen.Features.ExtrudeBuilder extrudeBuilder1;
            extrudeBuilder1 = wk.Features.CreateExtrudeBuilder(extrude1);

            //theSession.UpdateManager.InterpartDelay = true;

            if (!wk.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            Direction direction1;
            direction1 = extrudeBuilder1.Direction;

            bool success1;
            if (isRev)
                success1 = direction1.ReverseDirection();

            extrudeBuilder1.Direction = direction1;

            NXOpen.Features.Feature feature1;
            feature1 = extrudeBuilder1.CommitFeature();
            extrudeBuilder1.Destroy();
        }

        public void DeleteFeature(List<Feature> delfeatures)
        {

            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Edit->Delete...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Delete");

            bool notifyOnDelete1;
            notifyOnDelete1 = theSession.Preferences.Modeling.NotifyOnDelete;

            theSession.UpdateManager.ClearErrorList();

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Delete");

            NXObject[] objects1 = new NXObject[1];
            //             NXOpen.Features.Coplanar coplanar1 = (NXOpen.Features.Coplanar)workPart.Features.FindObject("Coplanar(19)");
            objects1 = delfeatures.ToArray();
            int nErrs1;
            nErrs1 = theSession.UpdateManager.AddToDeleteList(objects1);

            bool notifyOnDelete2;
            notifyOnDelete2 = theSession.Preferences.Modeling.NotifyOnDelete;

            int nErrs2;
            nErrs2 = theSession.UpdateManager.DoUpdate(markId2);

            theSession.DeleteUndoMark(markId1, null);

            // ----------------------------------------------
            //   Menu: Tools->Journal->Stop Recording
            // ----------------------------------------------

        }

        // ok
        public void CreateSketchbyPolygon( List<Point3d> Polygon_point , ref List<Line> lines)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Curve->Rectangle...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Rectangle");


            for (int i = 0; i < Polygon_point.Count;i++ )
            {
                Point3d startPoint1 = new Point3d(Polygon_point[i].X, Polygon_point[i].Y, Polygon_point[i].Z);
                Point3d endPoint1 = new Point3d();
                Line line1;
                if (Polygon_point.Count == 2)
                {
                    endPoint1 = new Point3d(Polygon_point[i + 1].X, Polygon_point[i + 1].Y, Polygon_point[i + 1].Z);
                    line1 = workPart.Curves.CreateLine(startPoint1, endPoint1);
                    lines.Add(line1);
                    break;
                }
                else if (Polygon_point.Count > 2)
                {
                    if (i != Polygon_point.Count - 1)
                        endPoint1 = new Point3d(Polygon_point[i + 1].X, Polygon_point[i + 1].Y, Polygon_point[i + 1].Z);
                    else
                        endPoint1 = new Point3d(Polygon_point[0].X, Polygon_point[0].Y, Polygon_point[0].Z);
                }
              
                line1 = workPart.Curves.CreateLine(startPoint1, endPoint1);
                lines.Add(line1);
            }

            NXOpen.Session.UndoMarkId id1;
            id1 = theSession.NewestVisibleUndoMark;

            int nErrs1;
            nErrs1 = theSession.UpdateManager.DoUpdate(id1);

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Rectangle");

            theSession.UndoToMarkWithStatus(markId2, null);

            theSession.DeleteUndoMarksUpToMark(markId2, null, false);
    
        }

        // ok
        public Feature CreateExtrudeByLine(List<Line> line_objs , String s_v , String e_v)
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Design Feature->Extrude...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.Feature nullFeatures_Feature = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.ExtrudeBuilder extrudeBuilder1;
            extrudeBuilder1 = workPart.Features.CreateExtrudeBuilder(nullFeatures_Feature);

            Section section1;
            section1 = workPart.Sections.CreateSection(0.02413, 0.0254, 0.5);

            extrudeBuilder1.Section = section1;

            extrudeBuilder1.AllowSelfIntersectingSection(true);

            Unit unit1;
            unit1 = extrudeBuilder1.Draft.FrontDraftAngle.Units;

            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("2.00", unit1);

            extrudeBuilder1.DistanceTolerance = 0.0254;

            extrudeBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

            Body[] targetBodies1 = new Body[1];
            Body nullBody = null;
            targetBodies1[0] = nullBody;
            extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies1);

            extrudeBuilder1.Limits.StartExtend.Value.RightHandSide = s_v;

            extrudeBuilder1.Limits.EndExtend.Value.RightHandSide = e_v;

            extrudeBuilder1.BooleanOperation.Type = NXOpen.GeometricUtilities.BooleanOperation.BooleanType.Create;

            Body[] targetBodies2 = new Body[1];
            targetBodies2[0] = nullBody;
            extrudeBuilder1.BooleanOperation.SetTargetBodies(targetBodies2);

            extrudeBuilder1.Draft.FrontDraftAngle.RightHandSide = "2";

            extrudeBuilder1.Draft.BackDraftAngle.RightHandSide = "2";

            extrudeBuilder1.Offset.StartOffset.RightHandSide = "-0.5";

            extrudeBuilder1.Offset.EndOffset.RightHandSide = "1";

            NXOpen.GeometricUtilities.SmartVolumeProfileBuilder smartVolumeProfileBuilder1;
            smartVolumeProfileBuilder1 = extrudeBuilder1.SmartVolumeProfile;

            smartVolumeProfileBuilder1.OpenProfileSmartVolumeOption = false;

            smartVolumeProfileBuilder1.CloseProfileRule = NXOpen.GeometricUtilities.SmartVolumeProfileBuilder.CloseProfileRuleType.Fci;

            theSession.SetUndoMarkName(markId1, "Extrude Dialog");

            section1.DistanceTolerance = 0.0254;

            section1.ChainingTolerance = 0.02413;

            section1.SetAllowedEntityTypes(NXOpen.Section.AllowTypes.OnlyCurves);

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "section mark");

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, null);

            foreach (Line line_t in line_objs)
            {
                IBaseCurve[] curves1 = new IBaseCurve[1];

                Line line1 = line_t;// (Line)workPart.Lines.FindObject("ENTITY 3 2 1");
                curves1[0] = line_t;
                CurveDumbRule curveDumbRule1;
                curveDumbRule1 = workPart.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);

                section1.AllowSelfIntersection(true);

                SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
                rules1[0] = curveDumbRule1;
                NXObject nullNXObject = null;
                Point3d helpPoint1 = line_t.StartPoint;
                section1.AddToSection(rules1, line1, nullNXObject, nullNXObject, helpPoint1, NXOpen.Section.Mode.Create, false);

            }
            // 1. 
           
            theSession.DeleteUndoMark(markId3, null);

            Point3d origin1 = new Point3d(0,0,0);
            Vector3d vector1 = new Vector3d(0.0, 0.0, 1.0);
            Direction direction1;
            direction1 = workPart.Directions.CreateDirection(origin1, vector1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            extrudeBuilder1.Direction = direction1;

            Unit unit2;
            unit2 = extrudeBuilder1.Offset.StartOffset.Units;

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit2);

            theSession.DeleteUndoMark(markId2, null);

            extrudeBuilder1.FeatureOptions.BodyType = NXOpen.GeometricUtilities.FeatureOptions.BodyStyle.Sheet;

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Extrude");

            theSession.DeleteUndoMark(markId4, null);

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Extrude");

            extrudeBuilder1.ParentFeatureInternal = false;

            NXOpen.Features.Feature feature1;
            feature1 = extrudeBuilder1.CommitFeature();

            theSession.DeleteUndoMark(markId5, null);

            theSession.SetUndoMarkName(markId1, "Extrude");

            Expression expression3 = extrudeBuilder1.Limits.StartExtend.Value;
            Expression expression4 = extrudeBuilder1.Limits.EndExtend.Value;
            extrudeBuilder1.Destroy();

            workPart.Expressions.Delete(expression1);

            workPart.Expressions.Delete(expression2);


            return feature1;
        }

        // ok 
        public void CreateSplitBody_NewPlane(   
                                                Body objBody,
                                                double[] pt ,
                                                int dir_index // x y z 方向
                                                )
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Trim->Split Body...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.SplitBody nullFeatures_SplitBody = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.SplitBodyBuilder splitBodyBuilder1;
            splitBodyBuilder1 = workPart.Features.CreateSplitBodyBuilderUsingCollector(nullFeatures_SplitBody);

            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Vector3d normal1 = new Vector3d(0.0, 0.0, 1.0);
            Plane plane1;
            plane1 = workPart.Planes.CreatePlane(origin1, normal1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            splitBodyBuilder1.BooleanTool.FacePlaneTool.ToolPlane = plane1;

            Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            splitBodyBuilder1.BooleanTool.ToolOption = NXOpen.GeometricUtilities.BooleanToolBuilder.BooleanToolType.NewPlane;

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.PrepareMappingData();

            theSession.SetUndoMarkName(markId1, "Split Body Dialog");

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.DistanceTolerance = 0.0254;

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.ChainingTolerance = 0.02413;

            ScCollector scCollector1;
            scCollector1 = workPart.ScCollectors.CreateCollector();

            // 1. 設定欲切的實體
            Body[] bodies1 = new Body[1];
            Body body1 = objBody;// (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(0)");
            bodies1[0] = body1;
            BodyDumbRule bodyDumbRule1;
            bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(bodies1);

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = bodyDumbRule1;
            scCollector1.ReplaceRules(rules1, false);

            splitBodyBuilder1.TargetBodyCollector = scCollector1;

            Point3d origin2 = new Point3d(0.0, 0.0, 0.0);
            Vector3d vector1 =new Vector3d();

            // 2. 設定方向
            if ( dir_index == 0 )
                vector1 = new Vector3d(1.0, 0.0, 0.0);
            else if (dir_index == 1 )
                vector1 = new Vector3d(0.0, 1.0, 0.0);
            else if (dir_index == 2)
                vector1 = new Vector3d(0.0, 0.0, 1.0);

            Direction direction2;
            direction2 = workPart.Directions.CreateDirection(origin2, vector1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.PointDir);

            // 3. 設定點
            Point3d pt3d = new Point3d(pt[0],pt[1],pt[2]);
            Point point2 = workPart.Points.CreatePoint(pt3d);


            NXObject[] geom3 = new NXObject[2];
            geom3[0] = point2;
            geom3[1] = direction2;
            plane1.SetGeometry(geom3);

            plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

            plane1.Evaluate();

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Plane");

            theSession.DeleteUndoMark(markId3, null);

            NXOpen.Session.UndoMarkId markId4;
            markId4 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Plane");

            plane1.RemoveOffsetData();

            plane1.Evaluate();

            theSession.DeleteUndoMark(markId4, null);

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.PointDir);

            NXObject[] geom4 = new NXObject[2];
            geom4[0] = point2;
            geom4[1] = direction2;
            plane1.SetGeometry(geom4);

            plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

            plane1.Evaluate();

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.PointDir);

            NXObject[] geom5 = new NXObject[2];
            geom5[0] = point2;
            geom5[1] = direction2;
            plane1.SetGeometry(geom5);

            plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

            NXOpen.Session.UndoMarkId markId5;
            markId5 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Split Body");

            theSession.DeleteUndoMark(markId5, null);

            NXOpen.Session.UndoMarkId markId6;
            markId6 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Split Body");

            NXObject nXObject2;
            nXObject2 = splitBodyBuilder1.Commit();

            theSession.DeleteUndoMark(markId6, null);

            theSession.SetUndoMarkName(markId1, "Split Body");

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.CleanMappingData();

            splitBodyBuilder1.Destroy();

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression2);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression1);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

         
        }


        // ok
        public void CreateSplitBody_NewPlaneByFace(
                                                    List<Body> objBodys,
                                                    List<Face> face_ts
                                                    )
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Trim->Split Body...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.SplitBody nullFeatures_SplitBody = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.SplitBodyBuilder splitBodyBuilder1;
            splitBodyBuilder1 = workPart.Features.CreateSplitBodyBuilderUsingCollector(nullFeatures_SplitBody);

            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Vector3d normal1 = new Vector3d(0.0, 0.0, 1.0);
            Plane plane1;
            plane1 = workPart.Planes.CreatePlane(origin1, normal1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            splitBodyBuilder1.BooleanTool.FacePlaneTool.ToolPlane = plane1;

            Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            splitBodyBuilder1.BooleanTool.ToolOption = NXOpen.GeometricUtilities.BooleanToolBuilder.BooleanToolType.NewPlane;

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.PrepareMappingData();

            theSession.SetUndoMarkName(markId1, "Split Body Dialog");

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.Distance);

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.DistanceTolerance = 0.0254;

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.ChainingTolerance = 0.02413;

            ScCollector scCollector1;
            scCollector1 = workPart.ScCollectors.CreateCollector();

            // 1. set body
//             Body[] bodies1 = new Body[1];
//             Body body1 = objBody;// (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(0)");
//             bodies1[0] = body1;
            BodyDumbRule bodyDumbRule1;
            bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(objBodys.ToArray());

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = bodyDumbRule1;
            scCollector1.ReplaceRules(rules1, false);

            splitBodyBuilder1.TargetBodyCollector = scCollector1;

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.Distance);

            // 2. set face
            NXObject[] geom1 = new NXObject[face_ts.Count];         
            for(int i=0;i<face_ts.Count;i++)
            {
               geom1[i] = face_ts[i];
            }

            plane1.SetGeometry(geom1);

            plane1.SetFlip(false);

            plane1.SetReverseSide(false);

            Expression expression3;
            expression3 = plane1.Expression;

            expression3.RightHandSide = "0";

            plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

            plane1.Evaluate();

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Split Body");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Split Body");

            NXObject nXObject1;
            nXObject1 = splitBodyBuilder1.Commit();

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Split Body");

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.CleanMappingData();

            splitBodyBuilder1.Destroy();

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression2);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression1);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

    
        }


        //ok
        public void CreateSplitBody_Faces(
                                            List<Body> objBodys,
                                            Body face_ext_body
                                            )
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Trim->Split Body...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.SplitBody nullFeatures_SplitBody = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.SplitBodyBuilder splitBodyBuilder1;
            splitBodyBuilder1 = workPart.Features.CreateSplitBodyBuilderUsingCollector(nullFeatures_SplitBody);

            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Vector3d normal1 = new Vector3d(0.0, 0.0, 1.0);
            Plane plane1;
            plane1 = workPart.Planes.CreatePlane(origin1, normal1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            splitBodyBuilder1.BooleanTool.FacePlaneTool.ToolPlane = plane1;

            Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            splitBodyBuilder1.BooleanTool.ToolOption = NXOpen.GeometricUtilities.BooleanToolBuilder.BooleanToolType.NewPlane;

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.PrepareMappingData();

            theSession.SetUndoMarkName(markId1, "Split Body Dialog");

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.Distance);

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.DistanceTolerance = 0.0254;

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.ChainingTolerance = 0.02413;

            ScCollector scCollector1;
            scCollector1 = workPart.ScCollectors.CreateCollector();

//             Body[] bodies1 = new Body[1];
//             Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(0)");
//             bodies1[0] = body1;
            BodyDumbRule bodyDumbRule1;
            bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(objBodys.ToArray());

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = bodyDumbRule1;
            scCollector1.ReplaceRules(rules1, false);

            splitBodyBuilder1.TargetBodyCollector = scCollector1;

            splitBodyBuilder1.BooleanTool.ToolOption = NXOpen.GeometricUtilities.BooleanToolBuilder.BooleanToolType.FaceOrPlane;

            Body body2 = face_ext_body;
            FaceBodyRule faceBodyRule1;
            faceBodyRule1 = workPart.ScRuleFactory.CreateRuleFaceBody(body2);

            SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
            rules2[0] = faceBodyRule1;
            splitBodyBuilder1.BooleanTool.FacePlaneTool.ToolFaces.FaceCollector.ReplaceRules(rules2, false);

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Split Body");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Split Body");

            NXObject nXObject1;
            nXObject1 = splitBodyBuilder1.Commit();

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Split Body");

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.CleanMappingData();

            splitBodyBuilder1.Destroy();

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression2);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression1);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            plane1.DestroyPlane();
    
        }

        //ok
        public void CreateSplitBody_Plane(
                                            List<Body> objBodys,
                                            Face face_obj
                                           )
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Trim->Split Body...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.SplitBody nullFeatures_SplitBody = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.SplitBodyBuilder splitBodyBuilder1;
            splitBodyBuilder1 = workPart.Features.CreateSplitBodyBuilderUsingCollector(nullFeatures_SplitBody);

            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            Vector3d normal1 = new Vector3d(0.0, 0.0, 1.0);
            Plane plane1;
            plane1 = workPart.Planes.CreatePlane(origin1, normal1, NXOpen.SmartObject.UpdateOption.WithinModeling);

            splitBodyBuilder1.BooleanTool.FacePlaneTool.ToolPlane = plane1;

            Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            splitBodyBuilder1.BooleanTool.ToolOption = NXOpen.GeometricUtilities.BooleanToolBuilder.BooleanToolType.NewPlane;

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.PrepareMappingData();

            theSession.SetUndoMarkName(markId1, "Split Body Dialog");

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.Distance);

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.DistanceTolerance = 0.0254;

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.ChainingTolerance = 0.02413;

            ScCollector scCollector1;
            scCollector1 = workPart.ScCollectors.CreateCollector();

//             Body[] bodies1 = new Body[1];
//             Body body1 = (Body)workPart.Bodies.FindObject("UNPARAMETERIZED_FEATURE(0)");
//             bodies1[0] = body1;
            BodyDumbRule bodyDumbRule1;
            bodyDumbRule1 = workPart.ScRuleFactory.CreateRuleBodyDumb(objBodys.ToArray());

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = bodyDumbRule1;
            scCollector1.ReplaceRules(rules1, false);

            splitBodyBuilder1.TargetBodyCollector = scCollector1;

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.Distance);

            NXObject[] geom1 = new NXObject[1];
            
            Face face1 = face_obj;
            geom1[0] = face1;
            plane1.SetGeometry(geom1);

            plane1.SetFlip(false);

            plane1.SetReverseSide(false);

            Expression expression3;
            expression3 = plane1.Expression;

            expression3.RightHandSide = "0";

            plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

            plane1.Evaluate();

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Split Body");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Split Body");

            NXObject nXObject1;
            nXObject1 = splitBodyBuilder1.Commit();

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Split Body");

            splitBodyBuilder1.BooleanTool.ExtrudeRevolveTool.ToolSection.CleanMappingData();

            splitBodyBuilder1.Destroy();

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression2);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression1);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }
        }





        public DatumPlane CreateDatumPlane()
        {
            Session theSession = Session.GetSession();
            Part workPart = theSession.Parts.Work;
            Part displayPart = theSession.Parts.Display;
            // ----------------------------------------------
            //   Menu: Insert->Datum/Point->Datum Plane...
            // ----------------------------------------------
            NXOpen.Session.UndoMarkId markId1;
            markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Features.Feature nullFeatures_Feature = null;

            if (!workPart.Preferences.Modeling.GetHistoryMode())
            {
                throw new Exception("Create or edit of a Feature was recorded in History Mode but playback is in History-Free Mode.");
            }

            NXOpen.Features.DatumPlaneBuilder datumPlaneBuilder1;
            datumPlaneBuilder1 = workPart.Features.CreateDatumPlaneBuilder(nullFeatures_Feature);

            Plane plane1;
            plane1 = datumPlaneBuilder1.GetPlane();

            Unit unit1 = (Unit)workPart.UnitCollection.FindObject("MilliMeter");
            Expression expression1;
            expression1 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Expression expression2;
            expression2 = workPart.Expressions.CreateSystemExpressionWithUnits("0", unit1);

            Point3d coordinates1 = new Point3d(0.0, 0.0, 0.0);
            Point point1;
            point1 = workPart.Points.CreatePoint(coordinates1);

            theSession.SetUndoMarkName(markId1, "Datum Plane Dialog");

            plane1.SetUpdateOption(NXOpen.SmartObject.UpdateOption.WithinModeling);

            NXObject[] geom1 = new NXObject[0];
            plane1.SetGeometry(geom1);

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.FixedZ);

            NXObject[] geom2 = new NXObject[0];
            plane1.SetGeometry(geom2);

            Point3d origin1 = new Point3d(0.0, 0.0, 0.0);
            plane1.Origin = origin1;

            Matrix3x3 matrix1;
            matrix1.Xx = 1.0;
            matrix1.Xy = 0.0;
            matrix1.Xz = 0.0;
            matrix1.Yx = 0.0;
            matrix1.Yy = 1.0;
            matrix1.Yz = 0.0;
            matrix1.Zx = 0.0;
            matrix1.Zy = 0.0;
            matrix1.Zz = 1.0;
            plane1.Matrix = matrix1;

            plane1.SetAlternate(NXOpen.PlaneTypes.AlternateType.One);

            plane1.Evaluate();

            plane1.SetMethod(NXOpen.PlaneTypes.MethodType.FixedZ);

            Point3d coordinates2 = new Point3d(0.0, 0.0, 0.0);
            Point point2;
            point2 = workPart.Points.CreatePoint(coordinates2);

            workPart.Points.DeletePoint(point1);

            NXOpen.Session.UndoMarkId markId2;
            markId2 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Datum Plane");

            theSession.DeleteUndoMark(markId2, null);

            NXOpen.Session.UndoMarkId markId3;
            markId3 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "Datum Plane");

            plane1.RemoveOffsetData();

            plane1.Evaluate();

            Point3d corner1_1 = new Point3d(-18.6811400427754, -18.6811400427754, 0.0);
            Point3d corner2_1 = new Point3d(18.6811400427754, -18.6811400427754, 0.0);
            Point3d corner3_1 = new Point3d(18.6811400427754, 18.6811400427754, 0.0);
            Point3d corner4_1 = new Point3d(-18.6811400427754, 18.6811400427754, 0.0);
            datumPlaneBuilder1.SetCornerPoints(corner1_1, corner2_1, corner3_1, corner4_1);

            datumPlaneBuilder1.ResizeDuringUpdate = true;

            NXOpen.Features.Feature feature1;
            feature1 = datumPlaneBuilder1.CommitFeature();

            NXOpen.Features.DatumPlaneFeature datumPlaneFeature1 = (NXOpen.Features.DatumPlaneFeature)feature1;
            DatumPlane datumPlane1;
            datumPlane1 = datumPlaneFeature1.DatumPlane;

            datumPlane1.SetReverseSection(false);

            theSession.DeleteUndoMark(markId3, null);

            theSession.SetUndoMarkName(markId1, "Datum Plane");

            datumPlaneBuilder1.Destroy();

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression2);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }

            try
            {
                // Expression is still in use.
                workPart.Expressions.Delete(expression1);
            }
            catch (NXException ex)
            {
                ex.AssertErrorCode(1050029);
            }


            return datumPlane1;
            // ----------------------------------------------
            //   Menu: Tools->Journal->Stop Recording
            // ----------------------------------------------
        }

    }// end class CJnal (journal)



    public static class CLinQ
    {

        // 計算重複個數
        public static void ListComputeNum(ref List<String> point ,ref List<int> point_num)
        {
            
            //List<int> intlist = new List<int> { 1, 2, 2, 3, 3, 4, 5 };
            var q =
            from p in point
            group p by p.ToString() into g
            select new
            {
                g.Key,
                NumProducts = g.Count()
            };

            foreach (var x in q)
            {
                point.Add(x.Key);
                point_num.Add(x.NumProducts);//陣列中 每個數字出現的數量  
            }  
              
        }

        // 刪除重複的元素
        public static void ListDeleteRepeatElemt(ref List<Face> Taglist)
        {
            List<Face> list = new List<Face>();
            foreach (Face each_tag in Taglist)
            {
                if (!list.Contains(each_tag))
                    list.Add(each_tag);
            }


            Taglist.Clear();
            Taglist.AddRange(list);
        }

        // 刪除重複的元素
        public static void ListDeleteRepeatElemt(ref List<Tag> Taglist)
        {
            List<Tag> list = new List<Tag>();
            foreach (Tag each_tag in Taglist)
            {
                if (!list.Contains(each_tag))
                    list.Add(each_tag);
            }


            Taglist.Clear();
            Taglist.AddRange(list);
        }

        // 刪除重複的元素
        public static void ListDeleteRepeatElemt(ref List<int> Taglist)
        {
            List<int> list = new List<int>();
            foreach (int each_tag in Taglist)
            {
                if (!list.Contains(each_tag))
                    list.Add(each_tag);
            }


            Taglist.Clear();
            Taglist.AddRange(list);
        }

        // 刪除重複的元素
        public static void ListDeleteRepeatElemt(ref List<double> Taglist)
        {
            List<double> list = new List<double>();
            foreach (int each_tag in Taglist)
            {
                if (!list.Contains(each_tag))
                    list.Add(each_tag);
            }


            Taglist.Clear();
            Taglist.AddRange(list);
        }

        // 陣列差集
        public static void ListDifferenceElemt(ref List<Tag> TagAlllist , List<Tag> Targetlist)
        {
            List<Tag> temp = new List<Tag>();

            foreach (Tag t in TagAlllist)
            {
                if (Targetlist.Contains(t))
                    continue;
                temp.Add(t);
            }

            TagAlllist.Clear();
            TagAlllist.AddRange(temp);
           
        }


        public static bool ListIsEquals(List<int> AList, List<int> BList)
        {
            if (AList.Except(BList).Count() == 0 && BList.Except(AList).Count() == 0)
            {
                return true;
            }
            else
                return false;
        }

        public static bool ListIsEquals(List<String> AList, List<String> BList)
        {
            if (AList.Except(BList).Count() == 0 && BList.Except(AList).Count() == 0)
            {
                return true;
            }
            else
                return false;
        }


        public static String ListToString(List<int> list)
        {
            string str = string.Join(",", list.ToArray());

            return str;
        }

        public static String ListToString(List<String> list)
        {
            string str = string.Join(",", list.ToArray());

            return str;
        }


        public static String ListToString(List<Tag> list)
        {
            string str = string.Join(",", list.ToArray());

            return str;
        }


        // 取交集
        /// <summary>
        /// List A : { 1 , 2 , 3 , 5 , 9 }
        /// List B : { 4 , 3 , 9 }
        /// 結果 : { 3 , 9 }
        /// </summary>
        /// <param name="list"></param>
        /// <param name="list1"></param>
        public static List<T> ListIntersected<T>(this List<T> list1, List<T> list2)
        {
            List<T> intersectedList = new List<T>();
            intersectedList.AddRange(list1.Intersect(list2) );
            bool isIntersected = list1.Intersect(list2).Count() > 0;

            return intersectedList;
        }

        /// <summary>
        /// 取差集 (A有，B沒有)
        /// List A : { 1 , 2 , 3 , 5 , 9 }
        /// List B : { 4 , 3 , 9 }
        /// 結果 : { 1 , 2 , 5 }
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static List<T> ListExpected<T>(this List<T> list1, List<T> list2)
        {
            List<T> mergedList = new List<T>();
            mergedList.AddRange(list1.Except(list2));
            bool isExpected = list1.Except(list2).Count() > 0;

            return mergedList;
        }

        /// <summary>
        /// 取聯集 (包含A和B)
        /// List A : { 1 , 2 , 3 , 5 , 9 }
        /// List B : { 4 , 3 , 9 }
        /// 結果 : { 1 , 2 , 3 , 5 ,9 , 4 }
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static List<T> ListMerge<T>(this List<T> source, List<T> target)
        {
            List<T> mergedList = new List<T>(source);
            mergedList.AddRange(target.Except(source));
            return mergedList;
        }   



    }//end class clinq



    public class CMath
    {
        public CMath()
        {

        }

        // 建立平面方程試
        public static double[] CreatePlaneEquation(double[] p1, double[] p2, double[] p3)
        {
            CLog log = new CLog();
            double[] p1p2 = new double[3];
            double[] p1p3 = new double[3];
            //log.showlogByPoint("p1 ", p1);
            //log.showlogByPoint("p2 ", p2);
            //log.showlogByPoint("p3 ", p3);
            p1p2[0] = p2[0] - p1[0];
            p1p2[1] = p2[1] - p1[1];
            p1p2[2] = p2[2] - p1[2];

            p1p3[0] = p3[0] - p1[0];
            p1p3[1] = p3[1] - p1[1];
            p1p3[2] = p3[2] - p1[2];


            double[] crossP = new double[3];
            crossP[0] = p1p2[1] * p1p3[2] - p1p2[2] * p1p3[1];
            crossP[1] = p1p2[2] * p1p3[0] - p1p2[0] * p1p3[2];
            crossP[2] = p1p2[0] * p1p3[1] - p1p2[1] * p1p3[0];
            //log.showlogByPoint("crossP ", crossP);
            // Ax+By+Cz+D=0
            // A=crossP[0] , B=crossP[1] , C=crossP[2]
            double D = (crossP[0] * p1[0] + crossP[1] * p1[1] + crossP[2] * p1[2]) * (-1);

            double[] planeEq = new double[4];
            planeEq[0] = crossP[0];
            planeEq[1] = crossP[1];
            planeEq[2] = crossP[2];
            planeEq[3] = D;

            return planeEq;

        }

        // 判斷點是否同一平面上
        public static bool isOnPlane(double[] plane_p1, //in: 平面中的點1
                                double[] plane_p2, //in: 平面中的點2
                                double[] plane_p3, //in: 平面中的點3
                                double[] target_p  //in: 目標點
                                )
        {
            CLog log = new CLog();
            //1. get plane equation
            double[] Eq = CreatePlaneEquation(plane_p1, plane_p2, plane_p3);
            
            //2. 點代入程式 , 看是否符合
            // Ax+By+Cz+D = 0 
            double v = Eq[0] * target_p[0] + Eq[1] * target_p[1] + Eq[2] * target_p[2] + Eq[3];

            //log.showlog(Eq[0] + "+" + Eq[1] + "+" + Eq[2] + "+" + Eq[3]);      
            bool isz = isZero(v);
            return isz;

        }



        public static bool isZero(double target_v)
        {
            double uplimt = 0.000001;
            double dowmlimt = -0.000001;
            if ( target_v > dowmlimt && 
                target_v < uplimt )
            {
                return true;
            }
            else
                return false;
        }


        // 兩個方向求角度
        public static double getTwoVectorAngle(double[] from,double[] to)
        {

            double[] dot_vector = new double[3];
            dot_vector[0] = from[0] * to[0] ;
            dot_vector[1] = from[1] * to[1] ;
            dot_vector[2] = from[2] * to[2];

            double dot_v = dot_vector[0] + dot_vector[1] + dot_vector[2];

            double from_v = Math.Sqrt (from[0] * from[0] + from[1] * from[1] + from[2] * from[2] );
            double to_v =  Math.Sqrt (to[0] * to[0] + to[1] * to[1] + to[2] * to[2] );

            double value = Math.Acos(dot_v / (from_v * to_v) );

            return value;
        }

        public static double getTwoPoint_Normal_Angle(double[] p1,double[] p2,double[] normal)
        {
            double[] p1p2 = new double[3];
            p1p2[0] = p1[0] - p2[0];
            p1p2[1] = p1[1] - p2[1];
            p1p2[2] = p1[2] - p2[2];

            double angle = getTwoVectorAngle(normal, p1p2);

            return RadianToAngle(angle);
        }



        public static double AngleToRadian(double Angle)
        {
            return Angle * Math.PI / 180;
        }

        public static double RadianToAngle(double Radian)
        {
            return Radian * 180 / Math.PI ;
        }

    }// end class CMath



    public class CSelect
    {
        // class members
        static private Session theSession_ = Session.GetSession();
        static private UI theUI_ = UI.GetUI();
        static private UFSession theUfSession_ = UFSession.GetUFSession();
        static private CLog clog;

        static private List<Tag> preSelectAry;
        static private Tag[] refMemberTags = {};
        static private UFUi.Mask[] mask_triples = { };

        public CSelect()
        {
            try
            {
                
                theSession_ = Session.GetSession();
                theUI_ = UI.GetUI();
                theUfSession_ = UFSession.GetUFSession();
                clog = new CLog();
            }
            catch (NXOpen.NXException ex)
            {
                clog.showlog(ex.Message);

            }
        }

        public static void showlog(String log)
        {
            clog = new CLog(); 
            clog.showlog(log);
        }

        public static void showmsg(String msg)
        {
            clog = new CLog();
            clog.showmsg(msg);
        }

       
        public static Selection.MaskTriple[] setFilter(String filter_str)
        {
            String[] filter = filter_str.Split(',');

            Selection.MaskTriple[] mask_arry = new Selection.MaskTriple[filter.Length];


            for (int i = 0; i < filter.Length; i++)
            {
                String str = filter[i];
                if (str == "feature")
                {
                    mask_arry[i].Type = UFConstants.UF_feature_type;
                    mask_arry[i].Subtype = UFConstants.UF_feature_subtype;
                    mask_arry[i].SolidBodySubtype = 0;
                }
                else if (str == "face")
                {
                    mask_arry[i].Type = UFConstants.UF_face_type;
                    mask_arry[i].Subtype = UFConstants.UF_solid_face_subtype;
                    mask_arry[i].SolidBodySubtype = 0;
                }
                else if (str == "solid")
                {
                    mask_arry[i].Type = UFConstants.UF_solid_type;
                    mask_arry[i].Subtype = UFConstants.UF_solid_body_subtype;
                    mask_arry[i].SolidBodySubtype = 0;
                }
                else if (str == "datum")
                {
                    mask_arry[i].Type = UFConstants.UF_datum_plane_type;
                    mask_arry[i].Subtype = 0;// UFConstants.UF_solid_body_subtype;
                    mask_arry[i].SolidBodySubtype = 0;
                }
                else if (str == "edge")
                {
//                     mask_arry[i].Type = UFConstants.UF_edge_type;
//                     mask_arry[i].Subtype = 0;//UFConstants.UF_solid_edge_subtype;
//                     mask_arry[i].SolidBodySubtype = 0;

                    mask_arry[i].Type = UFConstants.UF_solid_type;
                    mask_arry[i].Subtype = 0;
                    mask_arry[i].SolidBodySubtype = UFConstants.UF_UI_SEL_FEATURE_ANY_EDGE;
                }
                else if (str == "comp")
                {
                    mask_arry[i].SolidBodySubtype = 0;
                    mask_arry[i].Type = UFConstants.UF_component_type;
                    mask_arry[i].Subtype = 0;
                }
                else if (str == "point")
                {
                    mask_arry[i].SolidBodySubtype = 0;
                    mask_arry[i].Type = UFConstants.UF_point_type;
                    mask_arry[i].Subtype = UFConstants.UF_point_subtype;
                }
                else if ( str == "plane")
                {
                    mask_arry[i].SolidBodySubtype = 0;
                    mask_arry[i].Type = UFConstants.UF_MODL_PLANAR_FACE;
                    mask_arry[i].Subtype = 0;
                }

            }

            return mask_arry;
        }

        // for UF
        public static UFUi.Mask[] setFilter_UF(String filter_str)
        {
            String[] filter = filter_str.Split(',');

            int num_triples = filter.Length;
            UFUi.Mask[] mask_arry = new UFUi.Mask[filter.Length];

            for (int i = 0; i < filter.Length; i++)
            {
                String str = filter[i];
                if (str == "feature")
                {
                    mask_arry[i].object_type = UFConstants.UF_feature_type;
                    mask_arry[i].object_subtype = UFConstants.UF_feature_subtype;
                    mask_arry[i].solid_type = 0;
                }
                else if (str == "face")
                {
                    mask_arry[i].object_type = UFConstants.UF_face_type;
                    mask_arry[i].object_subtype = UFConstants.UF_solid_face_subtype;
                    mask_arry[i].solid_type = 0;
                }
                else if (str == "solid")
                {
                    mask_arry[i].object_type = UFConstants.UF_solid_type;
                    mask_arry[i].object_subtype = UFConstants.UF_solid_body_subtype;
                    mask_arry[i].solid_type = 0;
                }
                else if (str == "datum")
                {
                    mask_arry[i].object_type = UFConstants.UF_datum_plane_type;
                    mask_arry[i].object_subtype = 0;// UFConstants.UF_solid_body_subtype;
                    mask_arry[i].solid_type = 0;
                }
                else if (str == "edge")
                {
                    //                     mask_arry[i].Type = UFConstants.UF_edge_type;
                    //                     mask_arry[i].Subtype = 0;//UFConstants.UF_solid_edge_subtype;
                    //                     mask_arry[i].SolidBodySubtype = 0;

                    mask_arry[i].object_type = UFConstants.UF_solid_type;
                    mask_arry[i].object_subtype = 0;
                    mask_arry[i].solid_type = UFConstants.UF_UI_SEL_FEATURE_ANY_EDGE;
                }
                else if (str == "point")
                {
                    mask_arry[i].solid_type = 0;
                    mask_arry[i].object_type = UFConstants.UF_point_type;
                    mask_arry[i].object_subtype = UFConstants.UF_point_subtype;
                }
            }

            return mask_arry;
        }



        public static Selection.SelectionType[] setFilter_single(String filter_str)
        {
            String[] filter = filter_str.Split(',');

            Selection.SelectionType[] mask_arry = new Selection.SelectionType[filter.Length];

            for (int i = 0; i < filter.Length; i++)
            {
                String str = filter[i];
                if (str == "feature")
                {
                    mask_arry[i] = Selection.SelectionType.Features;
                    
                }
                else if (str == "face")
                {
                    mask_arry[i] = Selection.SelectionType.Faces;
                }
                else if (str == "edge")
                {
                    mask_arry[i] = Selection.SelectionType.Edges;
                }
               
               
            }

            return mask_arry;
        }

        //選取多個面
        public static List<Tag> SelectObj(String filterStr, String title_msg)
        {
            String message = "";
            String title = "";
            if (title_msg != "")
            {
                message = title_msg;
                title = title_msg;
            }
            else
            {
                message = "Select " + filterStr;
                title = "Selection " + filterStr;
            }
            TaggedObject[] solidAry = { };
            Selection.MaskTriple[] mask_arry = setFilter(filterStr);

            List<Tag> result = new List<Tag>();
            Selection.Response resp = theUI_.SelectionManager.SelectTaggedObjects(message, title, Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific, false, false, mask_arry, out solidAry);

            if (resp == Selection.Response.Back || resp == Selection.Response.Cancel)
                return result;

            for (int i = 0; i < solidAry.Length; i++)
            {
                result.Add(solidAry[i].Tag);
                //solidAry[i]  .Unhighlight();
            }

            return result;
        }

        //選取單面
        public static TaggedObject SelectObj_single(String filterStr,String title_msg)
        {
            String message = "";
            String title = "";
            if (title_msg != "")
            {
                message = title_msg;
                title = title_msg;
            }
            else
            {
                message = "Select " + filterStr;
                title = "Selection " + filterStr;
            }
            TaggedObject solidAry ;
            Point3d point;

            Selection.SelectionType[] selectType = setFilter_single(filterStr);

            List<Tag> result = new List<Tag>();
            Selection.Response resp = theUI_.SelectionManager.SelectTaggedObject(message, title, Selection.SelectionScope.AnyInAssembly,
                 false, selectType, out solidAry, out point);

            if (resp == Selection.Response.Back || resp == Selection.Response.Cancel)
                return null;


            return solidAry;
        }


        //選取單面 
        public static TaggedObject SelectObj_single_1(String filterStr, String title_msg)
        {
            String message = "";
            String title = "";
            if (title_msg != "")
            {
                message = title_msg;
                title = title_msg;
            }
            else
            {
                message = "Select " + filterStr;
                title = "Selection " + filterStr;
            }
            TaggedObject solidAry;
            Point3d point;

            //Selection.SelectionType[] selectType = setFilter_single(filterStr);
            Selection.MaskTriple[] mask_arry = setFilter(filterStr);

            List<Tag> result = new List<Tag>();
            /*
            Selection.Response resp = theUI_.SelectionManager.SelectTaggedObject(message, title, Selection.SelectionScope.AnyInAssembly,
                 false, selectType, out solidAry, out point);
            */

            Selection.Response resp = theUI_.SelectionManager.SelectTaggedObject(
                message,
                title,
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                true,
                false,
                mask_arry,
                out solidAry,
                out point);

            if (resp == Selection.Response.Back || resp == Selection.Response.Cancel)
                return null;


            return solidAry;
        }



        public static void PreSelection(String filterStr, String title_msg, List<Tag> in_SelAry, ref List<Tag> outSel)
        {

            setPreSelectObjAry(in_SelAry);
            String message = "";
            String title = "";
            if (title_msg != "")
            {
                message = title_msg;
                title = title_msg;
            }
            else
            {
                message = "Select " + filterStr;
                title = "Selection " + filterStr;
            }

            mask_triples = setFilter_UF(filterStr);

            theUfSession_.Ui.LockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);
            
            int response = 0;
            int count = 0;
            Tag[] objects = {};
            int i = 0;
            IntPtr user_data = (IntPtr)i;
            try
            {
                theUfSession_.Ui.SelectWithClassDialog(message, title, UFConstants.UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY,
                     sel_init_proc, System.IntPtr.Zero, out response, out count, out objects);
            }
            finally
            {
                theUfSession_.Ui.UnlockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);
            }

            foreach (Tag obj in objects)
            {
                theUfSession_.Disp.SetHighlight(obj, 0);
            }

            if (response != UFConstants.UF_UI_OK )
            {
                outSel.AddRange(in_SelAry);
            }
          
            //List<Tag> outSel = new List<Tag>();
            for (int j = 0; j < objects.Length;j++ )
            {
                outSel.Add(objects[j]);
            }

           
           
        }

        public static int sel_init_proc(IntPtr select_, IntPtr user_data)
        {
            if (refMemberTags.Length != 0)
            {
                //showlog("sel_init_proc" + refMemberTags.Length);
                theUfSession_.Ui.AddToSelList(select_, refMemberTags.Length, refMemberTags, true);
            }
            int num_triples = mask_triples.Length;
            theUfSession_.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, num_triples, mask_triples);
            return UFConstants.UF_UI_SEL_SUCCESS;
        }

        public static void setPreSelectObjAry(List<Tag> inSelAry)
        {
            if (inSelAry.Count == 0)
            {
                //preSelectAry.Clear();
                refMemberTags = new Tag[]{};//preSelectAry.ToArray();
                return;
            }
            preSelectAry = new List<Tag>();
            preSelectAry.AddRange(inSelAry);
            refMemberTags = preSelectAry.ToArray();

        }

        public static double[] SelectOpint(String title)
        {
            Tag point_tag = NXOpen.Tag.Null;
            int  response = 0;
            UFUi.PointBaseMethod base_method = UFUi.PointBaseMethod.PointOnSurfacePt;
            double[] base_pt  = new double[3];
            theUfSession_.Ui.LockUgAccess(NXOpen.UF.UFConstants.UF_UI_FROM_CUSTOM);

            theUfSession_.Ui.PointConstruct(title, ref base_method, out point_tag, base_pt, out response);
            theUfSession_.Ui.UnlockUgAccess(NXOpen.UF.UFConstants.UF_UI_FROM_CUSTOM);

            return base_pt;
        }


        public static int SelectOpint(String title, double[] base_pt)
        {
            Tag point_tag = NXOpen.Tag.Null;
            int  response = 0;
            UFUi.PointBaseMethod base_method = UFUi.PointBaseMethod.PointOnSurfacePt;
            //double[] base_pt  = new double[3];
            theUfSession_.Ui.LockUgAccess(NXOpen.UF.UFConstants.UF_UI_FROM_CUSTOM);

            theUfSession_.Ui.PointConstruct(title, ref base_method, out point_tag, base_pt, out response);
            theUfSession_.Ui.UnlockUgAccess(NXOpen.UF.UFConstants.UF_UI_FROM_CUSTOM);

            return response;
        }
       

        public static void SelectMultipleOpint(ref List<Point3d> points)
        {

           int res = UFConstants.UF_UI_OK;
           do 
           {
               double[] pt = new double[3];
               res = SelectOpint("請選擇點",pt);
               if (res == UFConstants.UF_UI_OK)
               points.Add(new Point3d(pt[0], pt[1], pt[2]));


           } while (res != UFConstants.UF_UI_CANCEL);
           
        }





        //----------2014 10 08 ----------------------------------------------------------------------
        public static int SelFilterFnT_feature(Tag _object, int[] type, IntPtr user_data, IntPtr select_)
        {
            TaggedObject obj = CTrans.TagtoNXObject(_object);
            //showlog("feature type : " + obj.GetType().ToString());
            if (obj.GetType().ToString() == "NXOpen.Features.Feature")
            {
                String attr_name = "CIM_BOSS";
                String attr_value = "";
                try
                {
                    Feature ff = (Feature)obj;
                    attr_value = ff.GetStringAttribute(attr_name);

                    if (int.Parse(attr_value) != 100)
                        return UFConstants.UF_UI_SEL_ACCEPT;
                }
                catch (System.Exception ex)
                {
                    return UFConstants.UF_UI_SEL_REJECT;
                }

            }

            return UFConstants.UF_UI_SEL_REJECT;
        }

        public static int SelCbFnT_feature( int num_selected, Tag[] selected_objects, 
                                    int num_deselected, Tag[] deselected_objects, 
                                    IntPtr user_data, IntPtr select_
                                    )
        {
            return UFConstants.UF_UI_SEL_ACCEPT;
        }
        
        public static int sel_init_proc_feature(IntPtr select_, IntPtr user_data)
        {
   
            int num_triples = mask_triples.Length;
            theUfSession_.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, num_triples, mask_triples);

            theUfSession_.Ui.SetSelProcs(select_, SelFilterFnT_feature, SelCbFnT_feature,user_data);

            return UFConstants.UF_UI_SEL_SUCCESS;
        }
        
        public static bool SelFeatureByAttr(String filterStr,
                                            String title_msg,
                                            ref List<Tag> outSel
                                            )
        {
            String message = "";
            String title = "";
            if (title_msg != "")
            {
                message = title_msg;
                title = title_msg;
            }
            else
            {
                message = "Select " + filterStr;
                title = "Selection " + filterStr;
            }

            mask_triples = setFilter_UF(filterStr);

            theUfSession_.Ui.LockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);

            int response = 0;
            int count = 0;
            Tag[] objects = { };
            int i = 0;
            IntPtr user_data = (IntPtr)i;
            try
            {
                theUfSession_.Ui.SelectWithClassDialog(message, title, UFConstants.UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY,
                     sel_init_proc_feature, System.IntPtr.Zero, out response, out count, out objects);
            }
            finally
            {
                theUfSession_.Ui.UnlockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);
            }

            foreach (Tag obj in objects)
            {
                theUfSession_.Disp.SetHighlight(obj, 0);
            }

            if (response != UFConstants.UF_UI_OK)
            {
                return false;
            }

            //List<Tag> outSel = new List<Tag>();
            for (int j = 0; j < objects.Length; j++)
            {
                outSel.Add(objects[j]);
            }

            return true;

        }














    }// end class CSelect



    public class CFile
    {
        public CFile()
        {
           
        }

        public static void ReNameFile(string oldFilePath, string newFilePath)
        {
            //string oldFilePath = @"C:\OldFile.txt"; // Full path of old file
            //string newFilePath = @"C:\NewFile.txt"; // Full path of new file

            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }


            File.Move(oldFilePath, newFilePath);
        }

        public static void CopyFile(string oldFilePath, string newFilePath)
        {
            File.Copy(oldFilePath, newFilePath, true);
        }

       
        public static void DeleteFile(String newPath)
        {
            try
            {
                if (File.Exists(newPath))
                {
                    //組立架構無備料零件，實體檔案存在目錄，則刪除實體檔案
                    File.Delete(newPath);
                }
            }
            catch (System.Exception ex)
            {
                //CaxLog.ShowListingWindow(ex.Message);
                return ;
            }
        }



    }// end CFile  









    //-----------------------   
}


/*

 1. 用 body 搜出來的 tag 都是 Prototype 的 tag 
 如果是在組件下 , 用UI選出來的tag 都是 OCC 的 Tag 因此都要做轉換  (  AskPrototypeOfOcc  )
 * 
 2. 反之亦然

*/