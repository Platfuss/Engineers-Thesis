using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EngineersThesis.General;

namespace EngineersThesis.Tests
{
    [TestClass]
    public class UnitTest
    {
        ContractorEditor contractorEditor;

        [TestInitialize]
        public void TestInitialize()
        {
            contractorEditor = new ContractorEditor(new SqlHandler(), false);
        }

        [TestMethod]
        public void ContractorGoodTaxCodeTest()
        {
            String goodEntry = "7625019373";

            try
            {
                contractorEditor.SetTaxCode(goodEntry);
                Assert.AreEqual(contractorEditor.TaxCode, goodEntry);
            }
            catch (Exception)
            {
                Assert.Fail("Good entry failed");
            }
        }

        [TestMethod]
        public void ContractorBadTaxCodeTest()
        {
            String badEntry = "5237605645";

            try
            {
                contractorEditor.SetTaxCode(badEntry);
                Assert.Fail("Exception wasn't thrown for bad entry");
            }
            catch (ArgumentException e)
            {
                StringAssert.Equals(e.Message, "Niepoprawny NIP");
            }
            catch (Exception)
            {
                Assert.Fail("Unknown exception was thrown");
            }
        }
    }
}
