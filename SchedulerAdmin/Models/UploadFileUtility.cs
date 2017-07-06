using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace SchedulerAdmin.Models
{
    public static class UploadFileUtility
    {
        public static string GetSchedulerImageDirectory()
        {
            string setting = ConfigurationManager.AppSettings["SchedulerImageDirectory"];

            if (string.IsNullOrEmpty(setting))
                throw new Exception("Missing appSetting: SchedulerImageDirectory");

            if (!Directory.Exists(setting))
                Directory.CreateDirectory(setting);

            return setting;
        }

        /// <summary>
        /// Uploads image and save original image and icon image.
        /// </summary>
        public static void UploadImage(HttpPostedFileBase file, string path, string id)
        {
            string fileName = Path.GetFileName(file.FileName);

            if (file.ContentLength == 0)
                throw new Exception("File is empty.");

            if (file.ContentLength >= 5242880)
                throw new Exception("File is too large, max size is 5 MB.");

            string[] allowed = { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };

            if (!allowed.Contains(Path.GetExtension(fileName)))
                throw new Exception(string.Format("Invalid file type. Allowed extensions: {0}", string.Join(", ", allowed)));

            // Check for existing files
            string imagePhysicalPath = GetImagePhysicalPath(path, id);
            string iconPhysicalPath = GetIconPhysicalPath(path, id);
            if (File.Exists(imagePhysicalPath)) File.Delete(imagePhysicalPath);
            if (File.Exists(iconPhysicalPath)) File.Delete(iconPhysicalPath);

            // Save original image
            var img = new Bitmap(file.InputStream);
            img.Save(imagePhysicalPath, ImageFormat.Png);

            // Save icon image
            int iconHeight = 32;
            int iconWidth = Convert.ToInt32(img.Width * iconHeight / img.Height);
            img = new Bitmap(img, iconWidth, iconHeight);
            img.Save(iconPhysicalPath, ImageFormat.Png);

            img.Dispose();
            img = null;
        }

        public static void DisplayImage(System.Web.UI.WebControls.Image img, string path, string id)
        {
            string url = GetImageURL(path, id);

            if (string.IsNullOrEmpty(url))
                img.Visible = false;
            else
            {
                img.ImageUrl = url;
                img.Visible = true;
            }
        }

        public static void DisplayIcon(System.Web.UI.WebControls.Image img, string path, string id)
        {
            string url = GetIconURL(path, id);

            if (string.IsNullOrEmpty(url))
                img.Visible = false;
            else
            {
                img.ImageUrl = url;
                img.Visible = true;
            }
        }

        public static void DeleteImages(string path, string id)
        {
            string imagePhysicalPath = GetImagePhysicalPath(path, id);
            string iconPhysicalPath = GetIconPhysicalPath(path, id);
            if (File.Exists(imagePhysicalPath)) File.Delete(imagePhysicalPath);
            if (File.Exists(iconPhysicalPath)) File.Delete(iconPhysicalPath);
        }

        public static string GetIconPhysicalPath(string path, string id)
        {
            string root = GetSchedulerImageDirectory();
            string dir = Path.Combine(root, path);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string name = string.Format("{0}{1}_icon.png", path, id);

            string result = Path.Combine(dir, name);

            return result;
        }

        public static string GetIconURL(string path, string id)
        { 
            string physicalPath  = GetIconPhysicalPath(path, id);

            if (!File.Exists(physicalPath))
                return string.Empty;
            else
                return VirtualPathUtility.ToAbsolute(string.Format("~/images/{0}/{1}/icon", path, id));
        }

        public static string GetImagePhysicalPath(string path, string id)
        {
            string root = GetSchedulerImageDirectory();
            string dir = Path.Combine(root, path);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string name = string.Format("{0}{1}.png", path, id);

            string result = Path.Combine(dir, name);

            return result;
        }

        public static string GetImageURL(string path, string id)
        {
            string physicalPath = GetImagePhysicalPath(path, id);
            if (!File.Exists(physicalPath))
                return string.Empty;
            else
                return VirtualPathUtility.ToAbsolute(string.Format("~/images/{0}/{1}", path, id));
        }
    }
}