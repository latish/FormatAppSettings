using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormatAppSettings
{
    [TestClass]
    public class given_an_appsettings_file
    {
        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void it_should_throw_an_exception_if_it_is_not_a_valid_XML_file()
        {
            const string inputXml = @"<a>bad xml </b>";
            var formatter = new AppSettingsFormatter();
            var outputXml = formatter.Tidy(inputXml);
        }

        [TestMethod]
        public void it_should_sort_elements_by_key_value()
        {
            const string inputXml = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><add key=""SaveViewStateToSession"" env=""Staging"" value=""False""/><add key=""SaveViewStateToSession"" env=""Production"" value=""False""/><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx""/><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator""/></MkAppSettings>";
            const string expectedOutputXml = @"<?xml version=""1.0"" encoding=""utf-8""?><MkAppSettings><add key=""PaymentAccountCode"" env=""Development"" value=""DevEmulator"" /><add key=""PaymentReturnUrl"" env=""Development"" value=""~/FinishCcPayment.ashx"" /><add key=""SaveViewStateToSession"" env=""Production"" value=""False"" /><add key=""SaveViewStateToSession"" env=""Staging"" value=""False"" /></MkAppSettings>";

            var formatter = new AppSettingsFormatter();
            var outputXml = formatter.Tidy(inputXml);
            Assert.AreEqual(expectedOutputXml,outputXml);
        }

        [TestMethod]
        public void it_should_process_elements_at_multiple_levels_and_maintain_hierarchy()
        {
            Assert.Fail("Yet to be implemented.");
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
