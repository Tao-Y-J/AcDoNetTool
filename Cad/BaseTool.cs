using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace AcDoNetTool.Cad
{
    public static class BaseTool
    {
        /// <summary>
        /// 角度转化为弧度
        /// </summary>
        /// <param name="degree">角度值</param>
        /// <returns></returns>
        public static double DegreeToAngle(this Double degree)
        {
            return degree * Math.PI / 180;
        }

        /// <summary>
        /// 弧度转换角度
        /// </summary>
        /// <param name="angle">弧度制</param>
        /// <returns></returns>
        public static double AngleToDegree(this Double angle)
        {
            return angle * 180 / Math.PI;
        }

        /// <summary>
        /// 判断三个点是否在同一直线上
        /// </summary>
        /// <param name="firstPoint">第一个点</param>
        /// <param name="secondPoint">第二个点</param>
        /// <param name="thirdPoint">第三个点</param>
        /// <returns></returns>
        public static bool IsOnOneLine(this Point3d firstPoint, Point3d secondPoint, Point3d thirdPoint)
        {
            Vector3d v21 = secondPoint.GetVectorTo(firstPoint);
            Vector3d v23 = secondPoint.GetVectorTo(thirdPoint);
            if (v21.GetAngleTo(v23) == 0 || v21.GetAngleTo(v23) == Math.PI)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取向量
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static double GetAngleToXAxis(this Point3d startPoint, Point3d endPoint)
        {
            // 声明一个与X轴平行的向量
            Vector3d temp = new Vector3d(1, 0, 0);
            // 获取起点到终点的向量
            Vector3d VsToe = startPoint.GetVectorTo(endPoint);
            return VsToe.Y > 0 ? temp.GetAngleTo(VsToe) : -temp.GetAngleTo(VsToe);
        }

        /// <summary>
        /// 两点之间的距离
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double GetDistanceBetweenTwoPoint(this Point3d point1, Point3d point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2) + Math.Pow(point1.Z + point2.Z, 2));
        }

        /// <summary>
        /// 获取两点的中心点
        /// </summary>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">第二个点</param>
        /// <returns></returns>
        public static Point3d GetCenterPointBetweenTwoPoint(this Point3d point1, Point3d point2)
        {
            return new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, (point1.Z + point2.Z) / 2);
        }

        /// <summary>
        /// 移动图形
        /// </summary>
        /// <param name="entId">图形对象的ObjectId</param>
        /// <param name="sourucePoint">参考圆点</param>
        /// <param name="targetPoint">参考目标点</param>
        public static void MoveEntity(this ObjectId entId, Point3d sourucePoint, Point3d targetPoint)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                //打开图形
                Entity ent = (Entity)entId.GetObject(OpenMode.ForWrite);
                //计算变换矩阵
                Vector3d vector = sourucePoint.GetVectorTo(targetPoint);
                Matrix3d mt = Matrix3d.Displacement(vector);
                ent.TransformBy(mt);
                trans.Commit();
            }
        }

        /// <summary>
        /// 移动图形
        /// </summary>
        /// <param name="ent">图形对象</param>
        /// <param name="sourucePoint">参考圆点</param>
        /// <param name="targetPoint">参考目标点</param>
        public static void MoveEntity(this Entity ent, Point3d sourucePoint, Point3d targetPoint)
        {
            //判断图形对象的IsNewlyObject属性
            if (ent.IsNewObject)
            {
                //计算变换矩阵
                Vector3d vector = sourucePoint.GetVectorTo(targetPoint);
                Matrix3d mt = Matrix3d.Displacement(vector);
                ent.TransformBy(mt);
            }
            else
            {
                ent.ObjectId.MoveEntity(sourucePoint, targetPoint);
            }
        }
    }
}