using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AcDoNetTool.Cad
{
    public static class AddEntityTool
    {
        /// <summary>
        /// 将图形对象添加到图形文件中
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">图形对象</param>
        /// <returns>图形的ObjectId</returns>
        public static ObjectId AddEntityToModeSpace(this Database db, Entity ent)
        {
            // 声明ObjectId 用于返回
            ObjectId entId = ObjectId.Null;
            // 开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                // 打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                // 添加图形到块表记录
                entId = btr.AppendEntity(ent);
                // 更新数据信息
                trans.AddNewlyCreatedDBObject(ent, true);
                // 提交事务
                trans.Commit();
            }
            return entId;
        }

        /// <summary>
        /// 添加多个图形对象到图形文件中
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="ent">图形对象 可变参数</param>
        /// <returns>图形的ObjectId 数组返回</returns>
        public static ObjectId[] AddEntityToModeSpace(this Database db, params Entity[] ent)
        {
            // 声明ObjectId 用于返回
            ObjectId[] entId = new ObjectId[ent.Length];
            // 开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                // 打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                for (int i = 0; i < ent.Length; i++)
                {
                    // 将图形添加到块表记录
                    entId[i] = btr.AppendEntity(ent[i]);
                    // 更新数据信息
                    trans.AddNewlyCreatedDBObject(ent[i], true);
                }
                // 提交事务
                trans.Commit();
            }
            return entId;
        }

        /// <summary>
        /// 绘制直线
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="startPoint">起点坐标</param>
        /// <param name="endPoint">终点坐标</param>
        /// <returns></returns>
        public static ObjectId AddLineToModeSpace(this Database db, Point3d startPoint, Point3d endPoint)
        {
            return db.AddEntityToModeSpace(new Line(startPoint, endPoint));
        }

        /// <summary>
        /// 起点坐标，角度，长度 绘制直线
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="startPoint">起点</param>
        /// <param name="length">长度</param>
        /// <param name="degree">角度</param>
        /// <returns></returns>
        public static ObjectId AddLineToModeSpace(this Database db, Point3d startPoint, double length, double degree)
        {
            // 利用长度和角度以及起点 计算终点坐标
            double X = startPoint.X + length * Math.Cos(degree.DegreeToAngle());
            double Y = startPoint.Y + length * Math.Sin(degree.DegreeToAngle());
            Point3d endPoint = new Point3d(X, Y, 0);
            return db.AddEntityToModeSpace(new Line(startPoint, endPoint));
        }

        /// <summary>
        /// 绘制圆弧
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">中心</param>
        /// <param name="radius">半径</param>
        /// <param name="startDegree">起始角度</param>
        /// <param name="endDegree">终止角度</param>
        /// <returns></returns>
        public static ObjectId AddArcToModeSpace(this Database db, Point3d center, double radius, double startDegree, double endDegree)
        {
            return db.AddEntityToModeSpace(new Arc(center, radius, startDegree.DegreeToAngle(), endDegree.DegreeToAngle()));
        }

        /// <summary>
        /// 三点画圆弧
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="startPoint">起点</param>
        /// <param name="pointOnArc">圆弧上一点</param>
        /// <param name="endPoint">终点</param>
        /// <returns></returns>
        public static ObjectId AddArcToModeSpace(this Database db, Point3d startPoint, Point3d pointOnArc, Point3d endPoint)
        {
            // 先判断是否在同一条直线上
            if (startPoint.IsOnOneLine(pointOnArc, endPoint))
            {
                return ObjectId.Null;
            }
            // 创建几何对象
            CircularArc3d cArc = new CircularArc3d(startPoint, pointOnArc, endPoint);
            // 创建圆弧对象
            Arc arc = new Arc(cArc.Center, cArc.Radius, cArc.Center.GetAngleToXAxis(startPoint), cArc.Center.GetAngleToXAxis(endPoint));
            // 加入到图形数据库
            return db.AddEntityToModeSpace(arc);
        }

        /// <summary>
        /// 通过圆心、起点、夹角绘制圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">圆心</param>
        /// <param name="startPoint">起点</param>
        /// <param name="degree">夹角</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AddArcToModeSpace(this Database db, Point3d center, Point3d startPoint, double degree)
        {
            //获取半径
            double radius = BaseTool.GetDistanceBetweenTwoPoint(center, startPoint);
            //获取起点角度
            double startAngle = BaseTool.GetAngleToXAxis(center, startPoint);
            //声明圆弧对象
            Arc arc = new Arc(center, radius, startAngle, startAngle + BaseTool.DegreeToAngle(degree));
            return db.AddEntityToModeSpace(arc);
        }

        /// <summary>
        /// 绘制圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">圆心</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public static ObjectId AddCircleModeSpace(this Database db, Point3d center, double radius)
        {
            return db.AddEntityToModeSpace(new Circle((center), new Vector3d(0, 0, 1), radius));
        }

        /// <summary>
        /// 直线为直径绘制圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="point1">第一个电</param>
        /// <param name="point2">第二个点</param>
        /// <returns></returns>
        public static ObjectId AddCircleModeSpace(this Database db, Line line)
        {
            var point1 = line.StartPoint;
            var point2 = line.EndPoint;
            // 获取两点的中心点
            Point3d center = point1.GetCenterPointBetweenTwoPoint(point2);
            // 获取半径
            double radius = point1.GetDistanceBetweenTwoPoint(center);
            return db.AddCircleModeSpace(center, radius);
        }

        /// <summary>
        /// 三点绘制圆
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        public static ObjectId AddCircleModeSpace(this Database db, Point3d point1, Point3d point2, Point3d point3)
        {
            // 先判断三点是否在同一直线上
            if (point1.IsOnOneLine(point2, point3))
            {
                return ObjectId.Null;
            }
            // 声明几何类Circular3d对象
            CircularArc3d cArc = new CircularArc3d(point1, point2, point3);
            return db.AddCircleModeSpace(cArc.Center, cArc.Radius);
        }

        /// <summary>
        /// 绘制折线多段线
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="isClosed">是否闭合</param>
        /// <param name="contantWidth">线宽</param>
        /// <param name="vertices">多线段的顶点 可变参数</param>
        /// <returns></returns>
        public static ObjectId AddPolyLineToModeSpace(this Database db, bool isClosed, double contantWidth, params Point2d[] vertices)
        {
            if (vertices.Length < 2)  // 顶点个数小于2 无法绘制
            {
                return ObjectId.Null;
            }
            // 声明一个多段线对象
            Polyline pline = new Polyline();
            // 添加多段线顶点
            for (int i = 0; i < vertices.Length; i++)
            {
                pline.AddVertexAt(i, vertices[i], 0, 0, 0);
            }
            if (isClosed)
            {
                pline.Closed = true;
            }
            // 设置多段线的线宽
            pline.ConstantWidth = contantWidth;
            return db.AddEntityToModeSpace(pline);
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="point1">第一个点</param>
        /// <param name="point2">对角点</param>
        /// <returns></returns>
        public static ObjectId AddRectToModeSpace(this Database db, Point2d point1, Point2d point2)
        {
            // 声明多段线
            Polyline pLine = new Polyline();
            // 计算矩形的四个顶点坐标
            Point2d p1 = new Point2d(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            Point2d p2 = new Point2d(Math.Max(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            Point2d p3 = new Point2d(Math.Max(point1.X, point2.X), Math.Max(point1.Y, point2.Y));
            Point2d p4 = new Point2d(Math.Min(point1.X, point2.X), Math.Max(point1.Y, point2.Y));
            // 添加多段线顶点
            pLine.AddVertexAt(0, p1, 0, 0, 0); // 参数 索引值 传入点 多段线凸度 起始宽度 终止宽度
            pLine.AddVertexAt(0, p2, 0, 0, 0);
            pLine.AddVertexAt(0, p3, 0, 0, 0);
            pLine.AddVertexAt(0, p4, 0, 0, 0);
            pLine.Closed = true; // 闭合
            return db.AddEntityToModeSpace(pLine);
        }

        /// <summary>
        /// 绘制正多边形
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">多边形所在内接圆圆心</param>
        /// <param name="radius">所在圆半径</param>
        /// <param name="sideNum">边数</param>
        /// <param name="startDegree">起始角度</param>
        /// <returns></returns>
        public static ObjectId AddPolygonToModeSpace(this Database db, Point2d center, double radius, int sideNum, double startDegree)
        {
            // 声明一个多段线对象
            Polyline pLine = new Polyline();
            // 判断边数是否符合要求
            if (sideNum < 3)
            {
                return ObjectId.Null;
            }
            Point2d[] point = new Point2d[sideNum]; // 有几条边就有几个点
            double angle = startDegree.DegreeToAngle();
            // 计算每个顶点坐标
            for (int i = 0; i < sideNum; i++)
            {
                point[i] = new Point2d(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
                pLine.AddVertexAt(i, point[i], 0, 0, 0);
                angle += Math.PI * 2 / sideNum;
            }
            // 闭合多段线
            pLine.Closed = true;
            return db.AddEntityToModeSpace(pLine);
        }

        /// <summary>
        /// 绘制椭圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="center">椭圆中心</param>
        /// <param name="majorRadius">长轴长度</param>
        /// <param name="shortRadius">短轴长度</param>
        /// <param name="degree">长轴与X夹角 角度值</param>
        /// <param name="startDegree">起始角度</param>
        /// <param name="endDegree">终止角度</param>
        /// <returns></returns>
        public static ObjectId AddEllipseToModeSpace(this Database db, Point3d center, double majorRadius, double shortRadius, double degree, double startDegree, double endDegree)
        {
            // 计算相关参数
            double ratio = shortRadius / majorRadius;
            Vector3d majorAxis = new Vector3d(majorRadius * Math.Cos(degree.AngleToDegree()), majorRadius * Math.Sin(degree.DegreeToAngle()), 0);
            Ellipse elli = new Ellipse(center, Vector3d.ZAxis, majorAxis, ratio, startDegree.DegreeToAngle(), endDegree.DegreeToAngle()); // VVector3d.ZAxis 等价于 new Vector3d(0,0,1) 平行于z轴法向量
            return db.AddEntityToModeSpace(elli);
        }

        /// <summary>
        /// 三点绘制椭圆
        /// </summary>
        /// <param name="db">图形数据库</param>
        /// <param name="majorPoint1">长轴端点1</param>
        /// <param name="majorPoint2">长轴端点2</param>
        /// <param name="shortRadius">短轴的长度</param>
        /// <returns>ObjectId</returns>
        public static ObjectId AddEllipseToModeSpace(this Database db, Point3d majorPoint1, Point3d majorPoint2, double shortRadius)
        {
            // 椭圆圆心
            Point3d center = majorPoint1.GetCenterPointBetweenTwoPoint(majorPoint2);
            // 短轴与长轴的比例
            double ratio = 2 * shortRadius / majorPoint1.GetDistanceBetweenTwoPoint(majorPoint2);
            // 长轴的向量
            Vector3d majorAxis = majorPoint2.GetVectorTo(center);
            Ellipse elli = new Ellipse(center, Vector3d.ZAxis, majorAxis, ratio, 0, 2 * Math.PI);
            return db.AddEntityToModeSpace(elli);
        }

        /// <summary>
        /// 绘制椭圆 两点
        /// </summary>
        /// <param name="db"></param>
        /// <param name="point1">所在矩形的顶点</param>
        /// <param name="point2">所在矩形的顶点2</param>
        /// <returns></returns>
        public static ObjectId AddEllipseToModeSpace(this Database db, Point3d point1, Point3d point2)
        {
            // 椭圆圆心
            Point3d center = point1.GetCenterPointBetweenTwoPoint(point2);

            double ratio = Math.Abs((point1.Y - point2.Y) / (point1.X - point2.X));
            Vector3d majorVector = new Vector3d(Math.Abs((point1.X - point2.X)) / 2, 0, 0);
            // 声明椭圆对象
            Ellipse elli = new Ellipse(center, Vector3d.ZAxis, majorVector, ratio, 0, 2 * Math.PI);
            return db.AddEntityToModeSpace(elli);
        }

        /// <summary>
        /// 改变图形颜色
        /// </summary>
        /// <param name="c1Id">图形的ObjectId</param>
        /// <param name="colorIndex">颜色索引</param>
        /// <returns>图形的ObjectId</returns> 图形已经添加图形数据库
        public static ObjectId ChangeEntityColor(this Database db, ObjectId c1Id, short colorIndex)
        {
            // 图形数据库

            // 开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // 打开块表
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                // 打开块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                // 获取图形对象
                Entity ent1 = (Entity)c1Id.GetObject(OpenMode.ForWrite);
                // 设置颜色
                ent1.ColorIndex = colorIndex;
                trans.Commit();
            }
            return c1Id;
        }

        #region 两个圆的交点

        public static double Distance(Point3d A, Point3d B)
        {
            return A.GetDistanceBetweenTwoPoint(B);
        }

        public static bool DoubleEquals(double a, double b)
        {
            return a == b;
        }

        /// <summary>
        /// 两个圆的交点
        /// </summary>
        /// <param name="A">圆A</param>
        /// <param name="B">圆B</param>
        /// <returns></returns>
        public static List<Point3d> CircleInsect(this Circle A, Circle B)
        {
            List<Point3d> points = new List<Point3d>
            {
                new Point3d(0, 0, 0),
                new Point3d(0, 0, 0)
            };
            double x1 = A.Center.X;
            double y1 = A.Center.Y;
            double r1 = A.Radius;
            double x2 = B.Center.X;
            double y2 = B.Center.Y;
            double r2 = B.Radius;
            double Dis = Distance(A.Center, B.Center); // 圆心距离
            if (DoubleEquals(x1, x2) && DoubleEquals(y1, y2) && (DoubleEquals(r1, r2))) // 两圆重合
            {
                Console.WriteLine("The two circles are the same");
            }
            if ((Dis > r1 + r2) || Dis < Math.Abs(r1 - r2))     // 两圆相离或者内含
            {
                Console.WriteLine("The two circles have no intersection");
            }
            if (DoubleEquals(Dis, r1 + r2) || DoubleEquals(Dis, Math.Abs(r1 - r2))) // 两圆有一个交点(即两圆相切[内切和外切])
            {
                if (DoubleEquals(Dis, r1 + r2))// 外切
                {
                    if (DoubleEquals(x1, x2) && !DoubleEquals(y1, y2))
                    {
                        if (y1 > y2)
                        {
                            points[0] = new Point3d(x1, y1 - r1, 0);
                        }
                        else
                        {
                            points[0] = new Point3d(x1, y1 + r1, 0);
                        }
                    }
                    else if (!DoubleEquals(x1, x2) && DoubleEquals(y1, y2))
                    {
                        if (x1 > x2)
                        {
                            points[0] = new Point3d(x1 - r1, y1, 0);
                        }
                        else
                        {
                            points[0] = new Point3d(x1 + r1, y1, 0);
                        }
                    }
                    else if (!DoubleEquals(x1, x2) && !DoubleEquals(y1, y2))
                    {
                        // 外切情况，两圆的交点在圆心AB连线上
                        double k1 = (y2 - y1) / (x2 - x1);
                        double k2 = -1 / k1;
                        points[0] = new Point3d(x1 + (x2 - x1) * r1 / Dis, y1 + (y2 - y1) * r1 / Dis, 0);
                    }
                }
                else if (DoubleEquals(Dis, Math.Abs(r1 - r2))) // 内切 (是否要考虑A包含B 还是B包含A，对结果是否有影响)
                {
                    if (DoubleEquals(x1, x2) && !DoubleEquals(y1, y2))
                    {
                        if (r1 > r2) // A内含B
                        {
                            if (y1 > y2)
                            {
                                points[0] = new Point3d(x1, y1 - r1, 0);
                            }
                            else
                            {
                                points[0] = new Point3d(x1, y1 + r1, 0);
                            }
                        }
                        else // B 内含A
                        {
                            if (y1 > y2)
                            {
                                points[0] = new Point3d(x1, y1 + r1, 0);
                            }
                            else
                            {
                                points[0] = new Point3d(x1, y1 - r1, 0);
                            }
                        }
                    }
                    else if (!DoubleEquals(x1, x2) && DoubleEquals(y1, y2))
                    {
                        if (r1 > r2)
                        {
                            if (x1 > x2)
                            {
                                points[0] = new Point3d(x1 - r1, y2, 0);
                            }
                            else
                            {
                                points[0] = new Point3d(x1 + r1, y2, 0);
                            }
                        }
                        else
                        {
                            if (x1 > x2)
                            {
                                points[0] = new Point3d(x1 + r1, y2, 0);
                            }
                            else
                            {
                                points[0] = new Point3d(x1 - r1, y2, 0);
                            }
                        }
                    }
                    else if (!DoubleEquals(x1, x2) && !DoubleEquals(y1, y2)) // 是否要考虑内含关系(求坐标时是否有影响)
                    {
                        // 内切情况，交点在AB连线的延长线上，要考虑切点的位置
                        double k1 = (y2 - y1) / (x2 - x1);
                        double k2 = -1 / k1;
                        points[0] = new Point3d(x1 + (x1 - x2) * r1 / Dis, y1 + (y1 - y2) * r1 / Dis, 0);
                    }
                }
            }
            if ((Dis < r1 + r2) && Dis > Math.Abs(r1 - r2))    // 两圆有两个交点(内交或者外交) 【内交与外交的情况是否一样？】
            {
                if (DoubleEquals(x1, x2) && !DoubleEquals(y1, y2))  // 圆A和圆B 横坐标相等
                {
                    double x0 = x1 = x2;
                    double y0 = y1 + (y2 - y1) * (r1 * r1 - r2 * r2 + Dis * Dis) / (2 * Dis * Dis);
                    double Dis1 = Math.Sqrt(r1 * r1 - (x0 - x1) * (x0 - x1) - (y0 - y1) * (y0 - y1));
                    points[0] = new Point3d(x0 - Dis1, y0, 0);
                    points[1] = new Point3d(x0 + Dis1, y0, 0);
                }
                else if (!DoubleEquals(x1, x2) && DoubleEquals(y1, y2)) // 圆A和圆B 纵坐标相等
                {
                    double y0 = y1 = y2;
                    double x0 = x1 + (x2 - x1) * (r1 * r1 - r2 * r2 + Dis * Dis) / (2 * Dis * Dis);
                    double Dis1 = Math.Sqrt(r1 * r1 - (x0 - x1) * (x0 - x1) - (y0 - y1) * (y0 - y1));
                    points[0] = new Point3d(x0, y0 - Dis1, 0);
                    points[1] = new Point3d(x0, y0 + Dis1, 0);
                }
                else if (!DoubleEquals(x1, x2) && !DoubleEquals(y1, y2)) // 横纵坐标都不等
                {
                    double k1 = (y2 - y1) / (x2 - x1);//AB的斜率
                    double k2 = -1 / k1;             // CD的斜率
                    double x0 = x1 + (x2 - x1) * (r1 * r1 - r2 * r2 + Dis * Dis) / (2 * Dis * Dis);
                    double y0 = y1 + k1 * (x0 - x1);
                    double Dis1 = r1 * r1 - (x0 - x1) * (x0 - x1) - (y0 - y1) * (y0 - y1); //CE的平方
                    double Dis2 = Math.Sqrt(Dis1 / (1 + k2 * k2));//EF的长，过C作过E点水平直线的垂线，交于F点
                    points[0] = new Point3d(x0 - Dis2, y0 - k2 * Dis2, 0);
                    points[1] = new Point3d(x0 + Dis2, y0 + k2 * Dis2, 0);
                }
            }
            return points;
        }

        #endregion 两个圆的交点

        /// <summary>
        /// 弧转化为多段线
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ent"></param>
        /// <returns></returns>
        public static Polyline ArcToPolyLine(this Database db, Entity ent)
        {
            Polyline polyline = new Polyline();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                if (ent is Arc)
                {
                    Arc arc = ent as Arc;
                    double R = arc.Radius;
                    Point3d startPoint = arc.StartPoint;
                    Point3d endPoint = arc.EndPoint;
                    Point2d p1, p2;
                    p1 = new Point2d(startPoint.X, startPoint.Y);
                    p2 = new Point2d(endPoint.X, endPoint.Y);
                    double L = p1.GetDistanceTo(p2);
                    double H = R - Math.Sqrt(R * R - L * L / 4);

                    polyline.AddVertexAt(0, p1, 2 * H / L, 0, 0);
                    polyline.AddVertexAt(1, p2, 0, 0, 0);
                }
                trans.Commit();
            }
            return polyline;
        }

        /// <summary>
        /// 圆转化为多段线
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ent"></param>
        /// <returns></returns>
        public static Polyline CircleToPolyline(this Database db, Entity ent)
        {
            Polyline polyline = new Polyline();
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                if (ent is Circle)
                {
                    Circle cir = ent as Circle;
                    double r = cir.Radius;
                    Point3d cc = cir.Center;
                    Point2d p1 = new Point2d(cc.X + r, cc.Y);
                    Point2d p2 = new Point2d(cc.X - r, cc.Y);
                    Polyline poly = new Polyline();
                    polyline.AddVertexAt(0, p1, 1, 0, 0);
                    polyline.AddVertexAt(1, p2, 1, 0, 0);
                    polyline.AddVertexAt(2, p1, 1, 0, 0);
                }
                trans.Commit();
            }
            return polyline;
        }

        /// <summary>
        /// 获取父类下面的所有子类
        /// </summary>
        /// <param name="db"></param>
        /// <param name="parentType"></param>
        /// <returns></returns>
        public static List<Type> GetChildTypes(this Type parentType)
        {
            List<Type> lstType = new List<Type>();
            Assembly assem = Assembly.GetAssembly(parentType);
            foreach (Type tChild in assem.GetTypes())
            {
                if (tChild.BaseType == parentType)
                {
                    lstType.Add(tChild);
                }
            }
            return lstType;
        }
    }
}