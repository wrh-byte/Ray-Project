using System;
using System.IO;
using System.Reflection;
using Aspose.Words;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Blls.ReportBlls.ReportCommonBlls
{
    /// <summary>
    /// 报告图片处理类
    /// </summary>
    public class ReportImageBll
    {
        private static string RunPath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath;

        /// <summary>
        /// 插入图片在文档中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">当前model</param>
        /// <param name="doc">插入的document对象</param>
        /// <param name="bookMarkName">书签名</param>
        /// <param name="imageType"></param>
        /// <param name="orderId"></param>
        /// <param name="imageName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void InsertImgToDocument<T>(T model, Document doc, string bookMarkName ,
            ImageType imageType, string orderId,string imageName,double width,double height)
        {
            try
            {
                string path = GetImagePath(model, orderId, imageName, imageType);
                if (!string.IsNullOrEmpty(path))
                {
                    DocumentBuilder imageBuilder = new DocumentBuilder(doc);
                    imageBuilder.MoveToBookmark(bookMarkName);
                    imageBuilder.InsertImage(path, width, height);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// 获取图片地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="orderId"></param>
        /// <param name="imageName"></param>
        /// <param name="imageType"></param>
        /// <returns></returns>
        public string GetImagePath<T>(T model,string orderId, string imageName, ImageType imageType)
        {
            try
            {
                string ImagePath = null;
                switch (imageType)
                {
                    case ImageType.SamplingSiteImage:
                        ImagePath = $"{RunPath}/SamplingSiteImage";
                        break;
                    case ImageType.SignImage:
                        ImagePath = $"{RunPath}/SignImage";
                        break;
                }

                Type type = typeof(T);
                PropertyInfo orgIdPropertyInfo = type.GetProperty("organizationId");
                string organizationId = orgIdPropertyInfo.GetValue(model).ToString();

                string organizationName = type.GetProperty("organizationName").GetValue(model).ToString();

                string orgPath = $"{ImagePath}/{organizationId}_{organizationName}";
                if (imageType==ImageType.SamplingSiteImage)
                {
                    orgPath = $"{ImagePath}/{organizationId}_{organizationName}/{orderId}";
                }
                string imagePath = $"{orgPath}/{imageName}";
                if (File.Exists(imagePath))
                {
                    return imagePath;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 获取图片文件文件流
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="ImageName"></param>
        /// <returns></returns>
        public  FileStream GetImageStream<T>(T model, string ImageName, ImageType imageType)
        {
            try
            {
                FileStream fileStream = null;
                string ImagePath = null;
                switch (imageType)
                {
                    case ImageType.SamplingSiteImage:
                        ImagePath = RunPath + "/SamplingSiteImage";
                        break;
                    case ImageType.SignImage:
                        ImagePath = RunPath + "/SignImage";
                        break;
                }

                Type type = typeof(T);
                PropertyInfo orgIdPropertyInfo = type.GetProperty("organizationId");
                string organizationId = orgIdPropertyInfo.GetValue(model).ToString();

                string organizationName = type.GetProperty("organizationName").GetValue(model).ToString();

                string orgPath = ImagePath + "/" + organizationId + "_" + organizationName;
                string imagePath = orgPath + "/" + ImageName;
                if (File.Exists(imagePath))
                {
                    fileStream = new FileStream(imagePath, FileMode.Open);
                }
                return fileStream;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public enum ImageType
        {
            /// <summary>
            /// 采样图片
            /// </summary>
            SamplingSiteImage = 0 ,

            /// <summary>
            /// 签名图片
            /// </summary>
            SignImage = 1,
        }
    }
}
