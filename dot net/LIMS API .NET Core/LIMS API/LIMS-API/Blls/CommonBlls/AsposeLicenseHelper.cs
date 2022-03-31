using System;
using System.IO;
using Aspose.BarCode;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LIMS_API.Blls.CommonBlls
{
    /// <summary>
    /// Aspose License Helper
    /// </summary>
    public class AsposeLicenseHelper
    {
        private static byte[] lisenceBytes = Properties.Resources.Aspose_Total;
        private static Stream stream = new MemoryStream(lisenceBytes);
        private string licencefilePath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath + "/licence/Aspose.Total.lic";

        /// <summary>
        /// Set Aspose Words License
        /// </summary>
        public void SetAsposeWordsLicense()
        {
            Aspose.Words.License license = new Aspose.Words.License();
            if (File.Exists(licencefilePath))
            {
                license.SetLicense(licencefilePath);
            }
        }

        /// <summary>
        /// Set Aspose Cells License
        /// </summary>
        public void SetAsposeCellsLicense()
        {
            Aspose.Cells.License license = new Aspose.Cells.License();
            if (File.Exists(licencefilePath))
            {
                license.SetLicense(licencefilePath);
            }
        }

        /// <summary>
        /// Set Aspose Pdf License
        /// </summary>
        public void SetAsposePdfLicense()
        {
            Aspose.Pdf.License license = new Aspose.Pdf.License();
            if (File.Exists(licencefilePath))
            {
                license.SetLicense(licencefilePath);
            }
        }

        /// <summary>
        /// Set Aspose BarCode License
        /// </summary>
        public void SetAsposeBarCodeLicense()
        {
            Aspose.BarCode.License license = new Aspose.BarCode.License();
            if (File.Exists(licencefilePath))
            {
                license.SetLicense(licencefilePath);
            }
        }
    }
}
