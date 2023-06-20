using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;

namespace AcDoNetTool.Cad
{
    public enum AddLayerStatuts
    {
        AddLayerOk,
        IllegalLayerName,
        LayerNameExist
    }

    public struct AddLayerResult
    {
        public AddLayerStatuts statuts;
        public string layerName;
    }

    public enum ChangeLayerPropertyStatus
    {
        ChangeOk,
        layerIsNotExist
    }

    public static partial class LayerTool
    {
        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layername">图层名</param>
        /// <returns>AddLayerResult</returns>
        public static AddLayerResult AddLayer(this Database db, string layername)
        {
            //声明AddLayerResult用于返回
            AddLayerResult res = new AddLayerResult();
            try
            {
                SymbolUtilityServices.ValidateSymbolName(layername, false);
            }
            catch (Exception)
            {
                res.statuts = AddLayerStatuts.IllegalLayerName;
                return res;
            }
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                if (!lt.Has(layername))
                {
                    res.statuts = AddLayerStatuts.AddLayerOk;
                    LayerTableRecord ltr = new LayerTableRecord
                    {
                        Name = layername
                    };
                    //升级层表打开权限
                    lt.UpgradeOpen();
                    lt.Add(ltr);
                    lt.DowngradeOpen();
                    trans.AddNewlyCreatedDBObject(ltr, true);
                    trans.Commit();

                    res.statuts = AddLayerStatuts.AddLayerOk;
                    res.layerName = layername;
                }
                else
                {
                    res.statuts = AddLayerStatuts.LayerNameExist;
                }
            }
            return res;
        }

        /// <summary>
        /// 修改图层颜色
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <param name="colorIndex">图层颜色</param>
        /// <returns></returns>
        public static ChangeLayerPropertyStatus ChangeLayerColor(this Database db, string layerName, short colorIndex)
        {
            ChangeLayerPropertyStatus status;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                //判断图形名是否存在
                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = lt[layerName].GetObject(OpenMode.ForWrite) as LayerTableRecord;
                    ltr.Color = Color.FromColorIndex(ColorMethod.ByAci, colorIndex);
                    trans.Commit();
                    status = ChangeLayerPropertyStatus.ChangeOk;
                }
                else
                {
                    status = ChangeLayerPropertyStatus.layerIsNotExist;
                }
            }
            return status;
        }

        /// <summary>
        /// 锁定图层
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="layerName">图层名</param>
        /// <returns></returns>
        public static bool LockLayer(this Database db, string layerName)
        {
            bool isOK = true;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                //判断图形名是否存在
                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = lt[layerName].GetObject(OpenMode.ForWrite) as LayerTableRecord;
                    ltr.IsLocked = true;
                    trans.Commit();
                }
                else
                {
                    isOK = false;
                }
            }
            return isOK;
        }

        /// <summary>
        /// 解除锁定
        /// </summary>
        /// <param name="db"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static bool UnLockLayer(this Database db, string layerName)
        {
            bool isOk = true;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                //判断图形名是否存在
                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = lt[layerName].GetObject(OpenMode.ForWrite) as LayerTableRecord;
                    ltr.IsLocked = false;
                    trans.Commit();
                }
                else
                {
                    isOk = false;
                }
            }
            return isOk;
        }

        /// <summary>
        /// 修改图层线宽
        /// </summary>
        /// <param name="db"></param>
        /// <param name="layerName"></param>
        /// <param name="lineWeight"></param>
        /// <returns></returns>
        public static bool ChangeLineWeight(this Database db, string layerName, LineWeight lineWeight)
        {
            bool isOk = true;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                //判断图形名是否存在
                if (lt.Has(layerName))
                {
                    LayerTableRecord ltr = lt[layerName].GetObject(OpenMode.ForWrite) as LayerTableRecord;
                    ltr.LineWeight = lineWeight;
                    trans.Commit();
                }
                else
                {
                    isOk = false;
                }
            }
            return isOk;
        }

        /// <summary>
        /// 设置当前图层
        /// </summary>
        /// <param name="db"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static bool SetCurrentLayer(this Database db, string layerName)
        {
            bool isSetOk = false;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开层表
                LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                //判断是否存在
                if (lt.Has(layerName))
                {
                    ObjectId layerId = lt[layerName];
                    if (db.Clayer != layerId)
                    {
                        db.Clayer = layerId;
                    }
                    isSetOk = true;
                }
                trans.Commit();
            }
            return isSetOk;
        }

        /// <summary>
        /// 返回所有层表记录
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<LayerTableRecord> GetAllLayers(this Database db)
        {
            List<LayerTableRecord> layerList = new List<LayerTableRecord>();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                lt.GenerateUsageData();
                foreach (ObjectId item in lt)
                {
                    LayerTableRecord ltr = item.GetObject(OpenMode.ForRead) as LayerTableRecord;
                    layerList.Add(ltr);
                }
            }
            return layerList;
        }

        /// <summary>
        /// 获取所有图层名字
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static List<string> GetAllLayersName(this Database db)
        {
            List<string> layerList = new List<string>();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (ObjectId item in lt)
                {
                    LayerTableRecord ltr = item.GetObject(OpenMode.ForRead) as LayerTableRecord;
                    layerList.Add(ltr.Name);
                }
            }
            return layerList;
        }

        /// <summary>
        /// 加载线型
        /// </summary>
        /// <param name="db"></param>
        /// <param name="lineTypeName"></param>
        /// <returns></returns>
        public static ObjectId LoadLineType(this Database db, string lineTypeName)
        {
            ObjectId lineTypeId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开线型表
                LinetypeTable ltt = trans.GetObject(db.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                //判断图形数据库是否已经存在指定的线型
                if (!ltt.Has(lineTypeName))
                {
                    db.LoadLineTypeFile(lineTypeName, "acad.lin");
                }
                lineTypeId = ltt[lineTypeName];
                trans.Commit();
            }

            return lineTypeId;
        }
    }
}