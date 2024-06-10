#pragma warning disable IDE0049

using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace VNLauncher.FuntionalClasses
{
    public class ImageHandler
    {
        private static Mat Binarize(Mat img)
        {
            Mat grayImg = new Mat();
            Cv2.CvtColor(img, grayImg, ColorConversionCodes.BGRA2GRAY);

            Mat binary = new Mat();
            Cv2.Threshold(grayImg, binary, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
//            Mat morphKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
  //          Cv2.MorphologyEx(binary, binary, MorphTypes.Open, morphKernel);
            return binary;
        }
        private static Boolean IsSubset(Mat img1, Mat img2)
        {
            if (img1.Size() != img2.Size())
            {
                return false;
            }
            Mat andImage = new Mat();
            Cv2.BitwiseAnd(img1, img2, andImage);
            Double img1Sum = Cv2.CountNonZero(img1);
            Double andImageSum = Cv2.CountNonZero(andImage);
            return Math.Abs(img1Sum - andImageSum) / img1Sum < 0.01;
        }
        public static Boolean ContainSameText(Mat img1, Mat img2)
        {
            Mat binary1 = Binarize(img1);
            Mat binary2 = Binarize(img2);


            Point[][] contours1, contours2;
            HierarchyIndex[] hierarchy1, hierarchy2;
            Cv2.FindContours(binary1, out contours1, out hierarchy1, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            Cv2.FindContours(binary2, out contours2, out hierarchy2, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            Mat contourImg1 = Mat.Zeros(binary1.Size(), MatType.CV_8UC1);
            Mat contourImg2 = Mat.Zeros(binary2.Size(), MatType.CV_8UC1);
            contours1 = FilterContours(contours1, contourImg1.Size());
            contours2 = FilterContours(contours2, contourImg2.Size());
            Cv2.DrawContours(contourImg1, contours1, -1, Scalar.White, 1);
            Cv2.DrawContours(contourImg2, contours2, -1, Scalar.White, 1);

            return IsSubset(contourImg1, contourImg2);
        }
        public static Int32 GetSimilarity(Mat img1, Mat img2)
        {
            Mat binary1 = Binarize(img1);
            Mat binary2 = Binarize(img2);

            Point[][] contours1, contours2;
            HierarchyIndex[] hierarchy1, hierarchy2;
            Cv2.FindContours(binary1, out contours1, out hierarchy1, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            Cv2.FindContours(binary2, out contours2, out hierarchy2, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            contours1 = FilterContours(contours1, img1.Size());
            contours2 = FilterContours(contours2, img2.Size());
            Mat contourImg1 = Mat.Zeros(binary1.Size(), MatType.CV_8UC1);
            Mat contourImg2 = Mat.Zeros(binary2.Size(), MatType.CV_8UC1);
            Cv2.DrawContours(contourImg1, contours1, -1, Scalar.White, 1);
            Cv2.DrawContours(contourImg2, contours2, -1, Scalar.White, 1);
            if (img1.Size() != img2.Size())
            {
                return Int32.MaxValue;
            }
            Mat andImage = new Mat();
            Cv2.BitwiseAnd(contourImg1, contourImg2, andImage);

            return Cv2.CountNonZero(andImage);

        }
        private static Point[][] FilterContours(Point[][] contours, Size size)
        {
            List<Point[]> filteredContours = new List<Point[]>();

            foreach (Point[] contour in contours)
            {
                Double area = Cv2.ContourArea(contour);
                if (area < size.Width * size.Height / 100 && area >= 2)
                {
                    filteredContours.Add(contour);
                }
            }

            return filteredContours.ToArray();
        }

        public static System.Drawing.Bitmap CropToCaption(System.Drawing.Bitmap originalImage, Game.CaptionLocation captionLocation)
        {
            Int32 imageWidth = originalImage.Width;
            Int32 imageHeight = originalImage.Height;
            if (imageWidth == 1 && imageHeight == 1)
            {
                return new System.Drawing.Bitmap(1, 1);
            }

            Double widthRate = captionLocation.LeftRate + captionLocation.RightRate + captionLocation.CaptionHorizontalRate;
            Double heightRate = captionLocation.UpRate + captionLocation.DownRate + captionLocation.CaptionVerticalRate;

            Int32 left = (Int32)(captionLocation.LeftRate / widthRate * imageWidth);
            Int32 right = (Int32)(captionLocation.RightRate / widthRate * imageWidth);
            Int32 up = (Int32)(captionLocation.UpRate / heightRate * imageHeight);
            Int32 down = (Int32)(captionLocation.DownRate / heightRate * imageHeight);
            Int32 captionWidth = (Int32)(captionLocation.CaptionHorizontalRate / widthRate * imageWidth);
            Int32 captionHeight = (Int32)(captionLocation.CaptionVerticalRate / heightRate * imageHeight);

            System.Drawing.Rectangle captionRect = new System.Drawing.Rectangle(left, up, captionWidth, captionHeight);
            System.Drawing.Bitmap croppedImage = new System.Drawing.Bitmap(captionWidth, captionHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(croppedImage);
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, captionWidth, captionHeight), captionRect, System.Drawing.GraphicsUnit.Pixel);
            return croppedImage;
        }
    }
}
