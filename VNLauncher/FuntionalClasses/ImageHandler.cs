#pragma warning disable IDE0049

using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

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
        public static Int32 CalculateWhiteIntersections(Mat mat)
        {
            Mat binary1 = Binarize(mat);
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(binary1, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            contours = FilterContours(contours, mat.Size());
            Mat contourImg1 = Mat.Zeros(binary1.Size(), MatType.CV_8UC1);
            Cv2.DrawContours(contourImg1, contours, -1, Scalar.White, 1);

            Int32 height = contourImg1.Height;
            Int32 width = contourImg1.Width;
            Int32[] heights = { height / 10, height / 5, height * 3 / 10, height * 2 / 5, height / 2, 3 * height / 5, 7 * height / 10, 4 * height / 5, 9 * height / 10 };

            Int32 totalIntersections = 0;

            foreach (Int32 y in heights)
            {
                Int32 lineIntersections = 0;
                Boolean inWhite = false;

                for (Int32 x = 0; x < width; x++)
                {
                    Byte color = contourImg1.At<Byte>(y, x);
                    if (color == 255)
                    {
                        if (!inWhite)
                        {
                            inWhite = true;
                            lineIntersections++;
                        }
                    }
                    else
                    {
                        inWhite = false;
                    }
                }

                totalIntersections += lineIntersections;
            }
            return totalIntersections;
        }

        private static Point[][] FilterContours(Point[][] contours, Size size)
        {
            List<Point[]> filteredContours = new List<Point[]>();

            foreach (Point[] contour in contours)
            {
                Double area = Cv2.ContourArea(contour);
                if (area < size.Width * size.Height / 100 && area >= 2 && !Cv2.IsContourConvex(contour))
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
        public static System.Drawing.Bitmap ResizeToFullImage(System.Drawing.Bitmap originalImage)
        {
            System.Drawing.Rectangle contentRect = GetContentRectangle(originalImage);
            System.Drawing.Bitmap resizedImage = new System.Drawing.Bitmap(originalImage.Width, originalImage.Height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(resizedImage))
            {
                g.Clear(System.Drawing.Color.Transparent);
                System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(0, 0, resizedImage.Width, resizedImage.Height);
                g.DrawImage(originalImage, destRect, contentRect, System.Drawing.GraphicsUnit.Pixel);
            }

            return resizedImage;
        }
        public static BitmapSource ConvertBitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource bitmapSource;

            try
            {
                bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return bitmapSource;
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean DeleteObject(IntPtr hObject);
        private static System.Drawing.Rectangle GetContentRectangle(System.Drawing.Bitmap image)
        {
            int minX = image.Width, minY = image.Height, maxX = 0, maxY = 0;

            BitmapData bitmapData = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                                                   ImageLockMode.ReadOnly, image.PixelFormat);
            int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * image.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = bitmapData.Scan0;

            System.Runtime.InteropServices.Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

            for (int y = 0; y < image.Height; y++)
            {
                int yOffset = y * bitmapData.Stride;
                for (int x = 0; x < image.Width; x++)
                {
                    int xOffset = x * bytesPerPixel;
                    byte alpha = pixels[yOffset + xOffset + 3]; // Assuming 32bpp (ARGB)

                    if (alpha != 0) // Non-transparent pixel
                    {
                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            image.UnlockBits(bitmapData);
            return new System.Drawing.Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }
    }
}
