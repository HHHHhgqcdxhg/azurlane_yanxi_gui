using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenCvSharp;

namespace azurlane_yanxi_wpf
{
    public class ResultRankList : IComparable
    {
        public int left { get; set; }
        public int val { get; set; }

        public ResultRankList(int left, int val)
        {
            this.left = left;
            this.val = val;
        }

        public int CompareTo(object obj)
        {
            var resultRankList = (ResultRankList)obj;
            if (this.left - resultRankList.left > 0)
            {
                return 1;
            }

            return 0;
        }
    }

    public class ResultRank
    {
        List<ResultRankList> l;

        public ResultRank()
        {
            l = new List<ResultRankList>();
        }

        public void push(int left, int val)
        {
            l.Add(new ResultRankList(left, val));
        }

        public int getValue()
        {
            int value = 0;
            //            l.Sort();
            var fuckSortedL = fuckSort();
            Console.WriteLine("sorted");
            foreach (var o in fuckSortedL)
            {
                //                Console.WriteLine($"left: {o.left}, val: {o.val}");
                value = value * 10 + o.val;
            }

            return value;
        }

        public List<ResultRankList> fuckSort()
        {
            var list = this.l;
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = 0; j < list.Count - 1 - i; j++)
                {
                    if (list[j].left > list[j + 1].left)
                    {
                        var temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }
            return list;
        }
    }

    public class ImageHanddle
    {
        private static Rect _get1RankRect = new Rect(252, 474, 222, 97);

        Mat[] numberMats = new Mat[10];

        public ImageHanddle()
        {
            // 初始化数字图片模板数组
            for (int i = 0; i <= 9; i++)
            {
                numberMats[i] = new Mat($"./src/img/{i}.png", ImreadModes.Grayscale);
            }
        }


        public int get1Rank(Mat img)
        {
            if (img.Width != 1920 || img.Height != 1080)
            {
                Cv2.Resize(img, img, new Size(1920, 1080));
            }
            var resultRank = new ResultRank();
            //            var raw = new Mat("J:\\test3.png");
            var rankPart = img[_get1RankRect];
            rankPart.Resize(rankPart.Width, Scalar.Black);

            var raw = rankPart;

            var thresholdRankPart = new Mat();
            Cv2.Threshold(rankPart, thresholdRankPart, 155, 255, OpenCvSharp.ThresholdTypes.BinaryInv);

            //            var target = new Mat(new Size(rankPart.Width,rankPart.Height + 10),MatType.CV_8U,Scalar.White);
            //            
            //            
            //            Cv2.AddWeighted(rankPart,1,target,1,0.0,target);

            //            using (new Window("???", thresholdRankPart))
            //            {
            //                Cv2.WaitKey();
            //            }

            Cv2.FindContours(thresholdRankPart, out var contours, out _, RetrievalModes.List,
                ContourApproximationModes.ApproxNone);


            //           Cv2.DrawContours(raw,contours,1,Scalar.Red);
            //           Cv2.DrawContours(raw,contours,0,Scalar.Red);


            // 遍历所有匹配到的轮廓.因为最后一个轮廓是最外层的矩形轮廓,所以舍去
            for (int i = 0; i < contours.Length; ++i)
            {
                Cv2.DrawContours(raw, contours, i, Scalar.Red);
                //                Console.WriteLine("========================================");

                // 找出轮廓的外接矩形
                var left = contours[i][0].X;
                var right = left;
                var top = contours[i][0].Y;
                var bottom = top;
                foreach (var p in contours[i])
                {
                    left = Math.Min(p.X, left);
                    top = Math.Min(p.Y, top);
                    right = Math.Max(p.X, right);
                    bottom = Math.Max(p.Y, bottom);
                }

                // 如果高度太小,判断为内层轮廓,非数字轮廓.如数字6的下部会有一个圆形的轮廓,这一步会将其剔除
                int len = bottom - top;
                if (len < 65 || len > 90)
                {
                    continue;
                }

                // 裁出数字的外接矩形
                var rect = new Rect(left, top, right - left, bottom - top);
                //                Console.WriteLine($"{right-left}  {bottom - top}");
                var thisNumber = thresholdRankPart[rect];

                // 找出最佳匹配数字
                double maxVal = 0;
                int matchedI = 0;
                double maxVal0;

                //                using (new Window("thisNumber", thisNumber))
                //                {
                //                    Cv2.WaitKey();
                //                }
                // 遍历所有数字
                for (int j = 0; j < 10; j++)
                {
                    if (numberMats[j].Width <= thisNumber.Width || numberMats[j].Height <= thisNumber.Height)
                    {
                        continue;
                    }

                    try
                    {
                        var res = thisNumber.MatchTemplate(numberMats[j], TemplateMatchModes.CCoeff);


                        Cv2.MinMaxLoc(res, out var minVal0, out maxVal0);
                        //                    Console.WriteLine($"{j}   : {minVal0},{maxVal0}");
                        if (maxVal0 > maxVal)
                        {
                            maxVal = maxVal0;
                            matchedI = j;
                        }
                    }
                    catch (OpenCVException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }

                //                Console.WriteLine(matchedI);
                //                if (matchedI == 3)
                //                {
                //                    Console.WriteLine($"wid: {right - left}, hei: {bottom - top}");
                //                }

                resultRank.push(left, matchedI);
            }

            //            using (new Window("thresholdRankPart", thresholdRankPart))
            //            using (new Window("raw", raw))
            //            {
            //                Cv2.WaitKey();
            //            }
            return resultRank.getValue();
        }

        //        public void preHanddleNumbers()
        //        {
        //            for (int i = 0; i < 10; i++)
        //            {
        //                var filePath = $"J:\\numberImg\\{i}.png";
        //                var img = new Mat(filePath,ImreadModes.Grayscale);
        //                var thresholdRankPart = new Mat();
        //                Cv2.Threshold(img, thresholdRankPart,155 ,255,OpenCvSharp.ThresholdTypes.BinaryInv);
        //                thresholdRankPart.SaveImage($"J:\\numberImg\\after\\{i}.png");
        //            }
        //        }
    }
}
