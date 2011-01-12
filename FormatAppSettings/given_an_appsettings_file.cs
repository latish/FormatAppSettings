using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormatAppSettings
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
            const string inputXml = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><add key=""SaveViewStateToSession"" env=""Staging"" value=""False""/><add key=""SaveViewStateToSession"" env=""Production"" value=""False""/><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx""/><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator""/></MkAppSettings>";
            const string expectedOutputXml = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator"" /><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx"" /><add key=""SaveViewStateToSession"" env=""Production"" value=""False"" /><add key=""SaveViewStateToSession"" env=""Staging"" value=""False"" /></MkAppSettings>";

            var outputXml = new AppSettingsFormatter().Tidy(inputXml);
            Assert.AreEqual(expectedOutputXml,outputXml);
        }

        [TestMethod]
        public void it_should_process_elements_at_multiple_levels_and_maintain_hierarchy()
        {
            const string inputXml = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><add key=""SaveViewStateToSession"" env=""Staging"" value=""False""/><add key=""SaveViewStateToSession"" env=""Production"" value=""False""/><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx""/><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator""/><subsidiary name=""RU""><add key=""DisablePrizeProcessingForOrderTypes"" value=""PrintedMaterials""/><add key=""zzuffixInclusionList"" value=""RU""/><add key=""SuffixInclusionList"" value=""RU""/></subsidiary></MkAppSettings>";
            const string expectedOutputXml = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator"" /><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx"" /><add key=""SaveViewStateToSession"" env=""Production"" value=""False"" /><add key=""SaveViewStateToSession"" env=""Staging"" value=""False"" /><subsidiary name=""RU""><add key=""DisablePrizeProcessingForOrderTypes"" value=""PrintedMaterials"" /><add key=""SuffixInclusionList"" value=""RU"" /><add key=""zzuffixInclusionList"" value=""RU"" /></subsidiary></MkAppSettings>";
            var outputXml = new AppSettingsFormatter().Tidy(inputXml);
            Assert.AreEqual(expectedOutputXml, outputXml);
        }

        [TestMethod]
        public void it_should_preserve_comments_associated_with_elements()
        {
            Assert.Fail("Yet to be implemented.");
        }

        [TestMethod]
        public void it_should_preserve_hard_returns_associated_with_elements()
        {

            Assert.Fail("Yet to be implemented.");
        }

        [TestMethod]
        public void it_should_compress_multiple_hard_returns()
        {
            Assert.Fail("Yet to be implemented.");
        }
    }
}
