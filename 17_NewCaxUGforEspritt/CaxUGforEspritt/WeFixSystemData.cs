using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaxUGforEspritt
{
    class WeFixSystemData
    {
    }
    public struct WeFixData
    {
        // for superGrid
        public string comName;
        public string modelID;
        public string modelName;
        public string length;
        public string width;
        public string height;
        // 檔案路徑
        public string filePath;
        public string imagePath;
        // 填入屬性
        public string ATTRIBUTE_FIXTURE_TYPE;
        // for 判斷校正方式
        public string CLP_LENGTH; // 裝夾尺寸
        public string FIX_CLAMP_MODEL; // 裝夾方式
        public string FIX_KG; // 治具重量
        // 2015/11/30 判斷裝夾尺寸新增寬、高
        public string CLP_WIDTH;
        public string CLP_HEIGHT;
        // TYPE
        public string FIXTURE_TYPE;
    }
}
