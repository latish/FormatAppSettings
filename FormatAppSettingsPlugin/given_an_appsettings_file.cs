using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormatAppSettingsPlugin
{
    [TestClass]
    public class given_an_appsettings_file
    {
        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void it_should_throw_an_exception_if_it_is_not_a_valid_xml_file()
        {
            const string inputXml = @"<a>bad xml </b>";
            new AppSettingsFormatter().Tidy(inputXml);
        }

        [TestMethod]
        public void it_should_sort_elements_by_key_value()
        {
            const string inputXml           = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><add key=""SaveViewStateToSession"" env=""Staging"" value=""False""/><add key=""SaveViewStateToSession"" env=""Production"" value=""False""/><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx""/><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator""/></MkAppSettings>";
            const string expectedOutputXml  = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator"" /><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx"" /><add key=""SaveViewStateToSession"" env=""Production"" value=""False"" /><add key=""SaveViewStateToSession"" env=""Staging"" value=""False"" /></MkAppSettings>";

            var outputXml = new AppSettingsFormatter().Tidy(inputXml, SaveOptions.DisableFormatting);
            Assert.AreEqual(expectedOutputXml, outputXml);
        }

        [TestMethod]
        public void it_should_process_elements_at_multiple_levels_and_maintain_hierarchy()
        {
            const string inputXml           = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><add key=""SaveViewStateToSession"" env=""Staging"" value=""False""/><add key=""SaveViewStateToSession"" env=""Production"" value=""False""/><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx""/><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator""/><subsidiary name=""RU""><add key=""DisablePrizeProcessingForOrderTypes"" value=""PrintedMaterials""/><add key=""zzuffixInclusionList"" value=""RU""/><add key=""SuffixInclusionList"" value=""RU""/></subsidiary></MkAppSettings>";
            const string expectedOutputXml  = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator"" /><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx"" /><add key=""SaveViewStateToSession"" env=""Production"" value=""False"" /><add key=""SaveViewStateToSession"" env=""Staging"" value=""False"" /><subsidiary name=""RU""><add key=""DisablePrizeProcessingForOrderTypes"" value=""PrintedMaterials"" /><add key=""SuffixInclusionList"" value=""RU"" /><add key=""zzuffixInclusionList"" value=""RU"" /></subsidiary></MkAppSettings>";
            var outputXml = new AppSettingsFormatter().Tidy(inputXml, SaveOptions.DisableFormatting);
            Assert.AreEqual(expectedOutputXml, outputXml);
        }

        [TestMethod]
        public void it_should_preserve_comments_associated_with_elements()
        {
            const string inputXml           = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><!--Comment1--><add key=""SaveViewStateToSession"" env=""Staging"" value=""False""/><add key=""SaveViewStateToSession"" env=""Production"" value=""False""/><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx""/><!--Comment2--><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator""/></MkAppSettings>";
            const string expectedOutputXml  = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><!--Comment2--><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator"" /><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx"" /><add key=""SaveViewStateToSession"" env=""Production"" value=""False"" /><!--Comment1--><add key=""SaveViewStateToSession"" env=""Staging"" value=""False"" /></MkAppSettings>";

            var outputXml = new AppSettingsFormatter().Tidy(inputXml, SaveOptions.DisableFormatting);
            Assert.AreEqual(expectedOutputXml, outputXml);
        }

        [TestMethod]
        public void it_should_work_on_file()
        {
            var doc =XDocument.Load("AppSettings.config");
            var output = new AppSettingsFormatter().Tidy(doc.ToString());
            using (var writer = new StreamWriter("AppSettings_formatted.config", false,Encoding.UTF8))
            {
                writer.Write(output);
                writer.Close();
            }

        }

    }
}
