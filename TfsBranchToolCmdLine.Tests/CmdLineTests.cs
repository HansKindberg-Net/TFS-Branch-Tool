// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CmdLineTests.cs">
//   Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.
// </copyright>
// <summary>
//   The action execution engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TfsBranchToolCmdLine.Tests
{
    using System;
    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine;
    using Microsoft.ALMRangers.BranchTool.ActionExecutionEngine.BranchPlan;
    using Microsoft.ALMRangers.BranchTool.CmdLineParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class TfsBranchToolCmdLineTests
    {                
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseArguments_NoArguments_ThrowsException()
        {
            // Arrange
            int result;
            CommandBase cmdObj = new CommandBase();

            // Act
            result = cmdObj.ParseArguments(null);

            // Assert            
        }

        [TestMethod]
        public void ParseArguments_CorrectValues_ReturnsZero()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            string[] args = { "/collection:http", "/TeamProject:proj" };

            // Act
            cmdObj.GetValidProperties("Basic");
            result = cmdObj.ParseArguments(args);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ParseArguments_UpperCaseCorrectValues_ReturnsZero()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            string[] args = { "/COLLECTION:http", "/ROOTFOLDER:$/Dummy" };

            // Act
            cmdObj.GetValidProperties("Basic");
            result = cmdObj.ParseArguments(args);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ParseArguments_EmptyArguments_ReturnsExpected12()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            string[] args = { "/ProjectCollectionURL:", "/RootFolder:proj" };

            // Act
            cmdObj.GetValidProperties("Basic");
            result = cmdObj.ParseArguments(args);

            // Assert
            Assert.AreEqual(12, result);
        }

        [TestMethod]
        public void ParseArguments_DoubleArguments_ReturnsExpected14()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            string[] args = { "/ProjectCollectionURL:url", "/RootFolder:proj", "/RootFolder:proj" };

            // Act
            cmdObj.GetValidProperties("Basic");
            result = cmdObj.ParseArguments(args);

            // Assert
            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void ParseArguments_BoolArgWorks()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            string[] args = { "/ProjectCollectionURL:url", @"/RootFolder:$/Dummy", "/BoolTest:y" };

            // Act
            cmdObj.GetValidProperties("BOOLTEST");
            result = cmdObj.ParseArguments(args);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ParseArguments_BollArgs_WrongData()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            string[] args = { "/ProjectCollectionURL:url",  @"/RootFolder:$/Dummy", "/BoolTest:HALLO" };

            // Act
            cmdObj.GetValidProperties("BOOLTEST");
            result = cmdObj.ParseArguments(args);

            // Assert
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void ParseArguments_EmptyArguments_ReturnsExpected13()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            
            string[] args = { "/ProjectCollectionURL:http", "/RootFolder" };

            // Act
            cmdObj.GetValidProperties("Basic");
            result = cmdObj.ParseArguments(args);

            // Assert
            Assert.AreEqual(13, result);
        }

        [TestMethod]
        public void ValidOperation_CorrectValue_ReturnsZero()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            
            // Act
            result = cmdObj.ValidateOperation("basic");

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ValidOperation_CorrectValueDifferentCasing_ReturnsZero()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            
            // Act
            result = cmdObj.ValidateOperation("bAsIc");

            // Assert
            Assert.AreEqual(0, result);
        }        

        [TestMethod]
        public void ValidateArguments_CorrectValue_ReturnsZero()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            
            string[] args = { "/ProjectCollectionURL:http", "/RootFolder:folder" };

            cmdObj.GetValidProperties("basic");
            cmdObj.ParseArguments(args);
            
            // Act            
            result = cmdObj.Validate();
            
            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ValidateArguments_WithoutSlash_ReturnsExpected11()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            
            string[] args = { "/ProjectCollectionURL:http", "RootFolder:folder" };
            cmdObj.GetValidProperties("basic");
            cmdObj.ParseArguments(args);
            
            // Act            
            result = cmdObj.Validate();

            // Assert
            Assert.AreEqual(11, result);
        }
        
        [TestMethod]
        public void ValidateArguments_WrongValue_ReturnsNotZero()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            string[] args = { "/ProjectCollectionUrl:http", "/RootFolder:$/proj", "/AddFeature:Nisse" };
            cmdObj.GetValidProperties("basic");
            cmdObj.ParseArguments(args);
            
            // Act            
            result = cmdObj.Validate();

            // Assert
            Assert.AreNotEqual(0, result);
        }

        [TestMethod]
        public void Main_CorrectSyntxWrongValue_ReturnsError()
        {
            // Arrange
            int result;
            IActionExecutionEngine iae = new MockExecutionEngine();
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmd = new BranchPlanBaseCommand(iae, fakePlan);

            CommandLineParser myparser = new CommandLineParser(cmd);

            string[] args = { "Basic", "/ProjectCollectionUrl:http", "/RootFolder:folder" };

            // Act
            result = myparser.TestableMain(args);

            // Assert
            Assert.AreEqual(-1, result);
        }
        
        [TestMethod]
        public void Main_WrongCommand_ReturnsCorrectCode20()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            CommandLineParser myparser = new CommandLineParser(cmdObj);

            string[] args = { "WrongCommand", "/collection:http", "/TeamProject:proj" };
            
            // Act
            result = myparser.TestableMain(args);
            
            // Assert
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void Main_CommandIsJustSlask_ReturnsCorrectCode20()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            CommandLineParser myparser = new CommandLineParser(cmdObj);
            string[] args = { "/" };

            // Act
            result = myparser.TestableMain(args);

            // Assert
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void Main_WrongParameter_ReturnsCorrectCode10()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            CommandLineParser myparser = new CommandLineParser(cmdObj);
            string[] args = { "basic", "/ProjectCollectionUrl:http", "/RootFolder:proj", "/WrongParameter:wrong" };
            
            // Act
            result = myparser.TestableMain(args);
            
            // Assert
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Main_OperationOnlyNoParameter_ReturnsOne()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            CommandLineParser myparser = new CommandLineParser(cmdObj);
            string[] args = { "basic" };
            
            // Act
            result = myparser.TestableMain(args);
            
            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Main_QuestionMark_ReturnsCorrect1()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            CommandLineParser myparser = new CommandLineParser(cmdObj);
            string[] args = { "/?" };

            // Act
            result = myparser.TestableMain(args);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Main_OperationAndQuestionMark_ReturnsCorrect1()
        {
            // Arrange
            int result;
            IPlanCatalog fakePlan = new MockPlanCatalog();
            BranchPlanBaseCommand cmdObj = new BranchPlanBaseCommand(null, fakePlan);
            CommandLineParser myparser = new CommandLineParser(cmdObj);
            string[] args = { "basic", "/?" };

            // Act
            result = myparser.TestableMain(args);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Main_NullParameter_ExcepctedException()
        {
            // Arrange
            int result;
            CommandLineParser myparser = new CommandLineParser();            

            // Act
            result = myparser.TestableMain(null);

            // Assert           
        } 
    }
}
